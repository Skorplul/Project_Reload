namespace NGMainPlugin.Systems.Database
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class DBPlayerExtension
    {
        private static Dictionary<Player, PlayerData> CachedPlayerData = new Dictionary<Player, PlayerData>();

        public static PlayerData GetPlayerData(this Player player) 
        {
            return CachedPlayerData.GetOrAdd(player, () => Database.GetPlayerData(player));
        }
    }

    internal class PlayerData
    {
        public string Id { get; set; }

        public string Nickname { get; set; }

        public string Rank { get; set; }

        public bool NicknameChangable { get; set; } = true;


        public int Playtime { get; set; }

        public int XP { get; set; }

        public void Update()
        {
            Database.UpdateData(this);
        }
    }

    internal static class Database
    {

        private static LiteDatabase DB;

        internal static void InitDB()
        {
            Config.LoadConfig();

            DB = new LiteDatabase(Path.Combine(Paths.Configs, "Database.db"));

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        internal static void CloseBD()
        {
            foreach (var p in Player.List)
            {
                UpdateData(p);
            }

            DB = null;
        }

        internal static PlayerData GetPlayerInfo(string uid)
        {
            return GetPlayerData(Player.UserIdsCache.Select(p => p.Key == uid ? p.Value : null).FirstOrDefault());
        }

        internal static PlayerData GetPlayerData(Player player)
        {
            Log.Debug($"Getting {player.UserId}:{player.Nickname} data!");

            var Result = DB.GetCollection<PlayerData>("Players").Query().Where(p => p.Id == player.UserId).ToList();

            return Result.Count > 0 ? Result.First() : CreatePlayerData(player);
        }

        internal static PlayerData CreatePlayerData(Player player)
        {
            Log.Debug($"Creating {player.UserId}:{player.Nickname} data!");

            var data = new PlayerData
            {
                Id = player.UserId,
                Nickname = player.Nickname,
                Rank = player.RankName
            };

            DB.GetCollection<PlayerData>("Players").Insert(data);

            return data;
        }

        internal static void UpdateData(Player player)
        {
            var data = player.GetPlayerData();

            data.Id = player.UserId; // Just in case
            data.Nickname = data.Nickname;
            data.Rank = player.RankName;

            if (player.SessionVariables.ContainsKey("JoinTime"))
                data.Playtime += (int)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);

            UpdateData(data);
        }

        internal static void UpdateData(PlayerData data)
        {
            Log.Debug($"Updating {data.Id}:{data.Nickname} data!");

            DB.GetCollection<PlayerData>("Players").Update(data);
        }

        /*internal static string DebugInfo()
        {
            string text = "";

            foreach (var i in DB.GetCollection<PlayerInfo>("Players").FindAll())
            {
                text += $"[{i.Track}] {i.Nickname} - {i.Id} - {TimeSpan.FromSeconds(i.Playtime):hh\\:mm\\:ss}\n";
            }

            return text;
        }*/


        // Events
        internal static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
        }

        internal static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdateData(ev.Player);
        }

        internal static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.List)
            {
                UpdateData(p);
            }
        }
    }
}
