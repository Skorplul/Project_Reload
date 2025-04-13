namespace PRMainPlugin.Systems.Database
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using LiteDB;
    using PRMainPlugin.API;
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
        public string CustomNick { get; set; }
        public string Rank { get; set; }
        public bool NicknameChangable { get; set; } = true;
        public int Playtime { get; set; }
        public int XP { get; set; }
        public int RequiredXP { get; set; } = 230;
        public int Level { get; set; } = 1;
        public int Recons { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public void Update()
        {
            Database.UpdateData(this);
        }
    }

    internal static class Database
    {

        internal static LiteDatabase DB;

        internal static void InitDB()
        {
            Config.LoadConfig();

            DB = new LiteDatabase(Path.Combine(Paths.Configs, "PRMainDB.db"));

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            XP.Enable();
        }

        internal static void CloseBD()
        {
            foreach (var p in Player.List)
            {
                UpdateData(p);
            }

            DB = null;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            XP.Disable();
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

        /// <summary>
        /// Add or remove an amount of Recons or XP from a Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="recons"></param>
        /// <param name="xp"></param>
        /// <returns>A bool, that indecates the success of the opperation</returns>
        public static bool UpdateReconsAndXP(Player player, int recons = 0, int xp = 0, bool ShowHint = true)
        {
            var data = player.GetPlayerData();

            if (recons != 0)
                data.Recons = data.Recons + recons;
            if (xp != 0)
                data.XP = data.XP + xp;

            data.Update();

            Levels.CheckLevel(data); 

            if (recons > 0 && xp == 0 && ShowHint)
               player.ShowHint($"[<color=#1504B3>P</color><color=#1A09B7>r</color><color=#1F0EBB>o</color><color=#2413BF>j</color><color=#2918C3>e</color><color=#2E1DC7>c</color><color=#3322CB>t</color><color=#3827CF>-</color><color=#3D2CD3>R</color><color=#4231D7>e</color><color=#4736DB>l</color><color=#4C3BDF>o</color><color=#5140E3>a</color><color=#5645E7>d</color>]: Du hast {recons} Recons erhalten.");
            else if (xp > 0 && recons == 0 && ShowHint)
                player.ShowHint($"[<color=#1504B3>P</color><color=#1A09B7>r</color><color=#1F0EBB>o</color><color=#2413BF>j</color><color=#2918C3>e</color><color=#2E1DC7>c</color><color=#3322CB>t</color><color=#3827CF>-</color><color=#3D2CD3>R</color><color=#4231D7>e</color><color=#4736DB>l</color><color=#4C3BDF>o</color><color=#5140E3>a</color><color=#5645E7>d</color>]: Du hast {xp} XP erhalten.");
            else if (ShowHint)
                player.ShowHint($"[<color=#1504B3>P</color><color=#1A09B7>r</color><color=#1F0EBB>o</color><color=#2413BF>j</color><color=#2918C3>e</color><color=#2E1DC7>c</color><color=#3322CB>t</color><color=#3827CF>-</color><color=#3D2CD3>R</color><color=#4231D7>e</color><color=#4736DB>l</color><color=#4C3BDF>o</color><color=#5140E3>a</color><color=#5645E7>d</color>]: Du hast {recons} Recons und {xp} XP erhalten.");

            return true;
        }

        /// <summary>
        /// Updates if a player can change thier nick
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns>The updated state as bool</returns>
        public static bool UpdateAllowNickChange(Player player, bool state)
        {
            var data = player.GetPlayerData();
            data.NicknameChangable = state;

            data.Update();

            return state;
        }

        /// <summary>
        /// Update the Players Displayname in DB for later use
        /// </summary>
        /// <param name="player"></param>
        public static void UpdateCustomNick(Player player)
        {
            var data = player.GetPlayerData();
            data.CustomNick = player.CustomName;

            data.Update();
        }

        /// <summary>
        /// Adds a Kill to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static void AddKill(Player player)
        {
            var data = player.GetPlayerData();
            data.Kills++;

            data.Update();
        }

        /// <summary>
        /// Adds a Death to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static void AddDeath(Player player)
        {
            var data = player.GetPlayerData();
            data.Deaths++;

            data.Update();
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

        // Events
        internal static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
            ev.Player.CustomName = ev.Player.GetPlayerData().CustomNick;
        }

        internal static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdateCustomNick(ev.Player);
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