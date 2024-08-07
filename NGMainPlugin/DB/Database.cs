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
        public string Nickname { get; set; }
        public string Rank { get; set; }
        public int Playtime { get; set; }
    }

    internal static class Database
    {
        private static LiteDatabase DB;

        public static void InitDB()
        {
            DB = new LiteDatabase(Path.Combine(Paths.Configs, "Test.db"));
        }

        public static void CloseBD()
        {
            foreach (var p in Player.List)
            {
                UpdatePlayerInfo(p);
            }

            DB = null;
        }

        public static PlayerInfo CreatePlayerInfo(Player player)
        {
            var Result = DB.GetCollection<PlayerInfo>("Players").Query().Where(p => p.Id == player.UserId).ToList();

            if (Result.Count == 1)
                return Result.First();

            Log.Info($"Creating player info! {player.Nickname} {player.UserId}");

            var Player = new PlayerInfo();
            Player.Id = player.UserId;
            Player.Nickname = player.Nickname;
            Player.Rank = player.RankName;

            DB.GetCollection<PlayerInfo>("Players").Insert(Player);

            return Player;
        }

        public static PlayerInfo GetPlayerInfo(Player player)
        {
            Log.Info($"Getting player info! {player.Nickname} {player.UserId}");

            var Result = DB.GetCollection<PlayerInfo>("Players").Query().Where(p => p.Id == player.UserId).ToList();

            if (Result.Count == 1)
                return Result.First();

            return null;
        }

        public static PlayerInfo GetPlayerInfo(string uid)
        {
            Log.Info($"Getting player info! {uid}");

            var Result = DB.GetCollection<PlayerInfo>("Players").Query().Where(p => p.Id == uid).ToList();

            if (Result.Count == 1)
                return Result.First();

            return null;
        }

        public static void UpdatePlayerInfo(Player player)
        {
            Log.Info($"Updating player info! {player.Nickname} {player.UserId}");

            var Info = GetPlayerInfo(player);

            if (Info == null) return;

            Info.Nickname = player.Nickname;
            Info.Rank = player.RankName;

            if (player.SessionVariables.ContainsKey("JoinTime"))
                Info.Playtime += (int)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);

            Log.Info(Info.Playtime);

            DB.GetCollection<PlayerInfo>("Players").Update(Info);
        }


        // Events
        public static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
        }

        public static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdatePlayerInfo(ev.Player);
        }

        public static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.List)
            {
                UpdatePlayerInfo(p);
            }
        }
    }
}
