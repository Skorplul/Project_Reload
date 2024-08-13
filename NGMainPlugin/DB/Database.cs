using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using LiteDB;
using System;
using System.IO;
using System.Linq;

namespace NGMainPlugin.DB
{
    internal class PlayerInfo
    {
        public string Id { get; set; }
        public bool Track { get; set; }
        public bool FirstJoin { get; set; } = true;
        public string Nickname { get; set; }
        public string Rank { get; set; }
        public int Playtime { get; set; }
    }

    internal static class DBPlayerExtension
    {
        public static bool CanStoreCustomData(this Player plr)
        {
            return (plr.DoNotTrack && Database.GetPlayerInfo(plr).Track) || (!plr.DoNotTrack && Database.GetPlayerInfo(plr).Track);
        }
    }

    internal static class Database
    {
        private static LiteDatabase DB;

        internal static void InitDB()
        {
            DB = new LiteDatabase(Path.Combine(Paths.Configs, "Database.db"));
        }

        internal static void CloseBD()
        {
            foreach (var p in Player.List)
            {
                UpdatePlayerInfo(p);
            }

            DB = null;
        }

        internal static PlayerInfo CreatePlayerInfo(Player player)
        {
            Log.Debug($"Creating player info! {player.Nickname} {player.UserId}");

            var Player = new PlayerInfo();
            Player.Id = player.UserId;
            Player.Track = !player.DoNotTrack;
            if (Player.Track)
            {
                Player.Nickname = player.Nickname;
                Player.Rank = player.RankName;
            }

            DB.GetCollection<PlayerInfo>("Players").Insert(Player);

            return Player;
        }

        internal static PlayerInfo GetPlayerInfo(Player player)
        {
            Log.Debug($"Getting player info! {player.Nickname} {player.UserId}");

            var Result = DB.GetCollection<PlayerInfo>("Players").Query().Where(p => p.Id == player.UserId).ToList();

            if (Result.Count > 0)
                return Result.First();

            return CreatePlayerInfo(player);
        }

        internal static PlayerInfo GetPlayerInfo(string uid)
        {
            Log.Debug($"Getting player info! {uid}");

            var Result = DB.GetCollection<PlayerInfo>("Players").Query().Where(p => p.Id == uid).ToList();

            if (Result.Count > 0)
                return Result.First();

            return null;
        }

        internal static void UpdatePlayerInfo(Player player)
        {
            Log.Debug($"Updating player info! {player.Nickname} {player.UserId}");

            var Info = GetPlayerInfo(player);

            if (player.CanStoreCustomData())
            {
                Info.Nickname = player.Nickname;
                Info.Rank = player.RankName;

                if (player.SessionVariables.ContainsKey("JoinTime"))
                    Info.Playtime += (int)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);
            }

            Info.FirstJoin = false;

            DB.GetCollection<PlayerInfo>("Players").Update(Info);
        }

        internal static void PlayerToggleTrack(Player player)
        {
            Log.Debug($"Player toggles track! {player.Nickname} {player.UserId}");

            var Info = GetPlayerInfo(player);

            if (player.CanStoreCustomData())
            {
                Info.Nickname = "";
                Info.Rank = "";
                Info.Playtime = 0;
            }
            else
            {
                Info.Nickname = player.Nickname;
                Info.Rank = player.RankName;
            }

            Info.Track = !Info.Track;

            DB.GetCollection<PlayerInfo>("Players").Update(Info);
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
            if (GetPlayerInfo(ev.Player).FirstJoin)
            {
                ev.Player.Broadcast(new Exiled.API.Features.Broadcast(Main.Instance.Config.DBWelcome, 7));

                ev.Player.SendConsoleMessage(Main.Instance.Config.DBWelcomeConsole, "yellow");
            }

            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
        }

        internal static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdatePlayerInfo(ev.Player);
        }

        internal static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.List)
            {
                UpdatePlayerInfo(p);
            }
        }
    }
}
