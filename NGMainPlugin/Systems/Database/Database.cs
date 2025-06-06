using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PRMainPlugin.Systems.Database
{
    internal static class DBPlayerExtension
    {
        internal static readonly ConcurrentDictionary<string, PlayerData> CachedPlayerData = new();

        public static async Task<PlayerData> GetPlayerDataAsync(this Player player)
        {
            if (CachedPlayerData.TryGetValue(player.UserId, out var cachedData))
                return cachedData;

            var data = await Database.GetPlayerDataAsync(player);
            CachedPlayerData[player.UserId] = data;
            return data;
        }
    }


    // Player data model
    public class PlayerData
    {
        [BsonId]
        public string Id { get; set; }
        public string Nickname { get; set; }
        public string CustomNick { get; set; }
        public string Rank { get; set; }
        public bool NicknameChangable { get; set; } = true;
        public List<Warn> Warns { get; set; } = new List<Warn>();
        public List<Warn> Watchlists { get; set; } = new List<Warn>();
        public double Playtime { get; set; }
        public int XP { get; set; }
        public int RequiredXP { get; set; } = 230;
        public int Level { get; set; } = 1;
        public int Recons { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public void Update()
        {
            Database.UpdateDataAsync(this);
        }
    }

    //Warn data structure
    public class Warn
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
        public string Issuer { get; set; }
    }

    internal static class Database
    {
        private static IMongoCollection<PlayerData> _collection;

        internal static void InitDB()
        {
            Config.LoadConfig();

            string connectionString = $"mongodb://SL_MAIN:DuFetterNegger1+2@127.0.0.1:27017/?authSource=SL_MAIN";
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("SL_MAIN");
            _collection = db.GetCollection<PlayerData>("players");

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            XP.Enable();
        }

        internal static void CloseDB()
        {
            var updateTasks = new List<Task>();

            foreach (var p in Player.List)
            {
                UpdateDataAsync(p);
            }

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            XP.Disable();

            _collection = null;
        }

        internal static async Task<PlayerData> GetPlayerInfoAsync(string uid)
        {
            var player = Player.UserIdsCache.FirstOrDefault(p => p.Key == uid).Value;

            if (player == null)
            {
                Log.Warn($"Player with UID {uid} not found in UserIdsCache.");
                return null;
            }

            return await GetPlayerDataAsync(player);
        }

        internal static async Task<PlayerData> GetPlayerDataAsync(Player player)
        {
            Log.Debug($"Getting {player.UserId}:{player.Nickname} data!");

            var result = await _collection.Find(p => p.Id == player.UserId).FirstOrDefaultAsync();

            return result ?? await CreatePlayerDataAsync(player);
        }

        internal static async Task<PlayerData> CreatePlayerDataAsync(Player player)
        {
            Log.Debug($"Creating {player.UserId}:{player.Nickname} data!");

            var data = new PlayerData
            {
                Id = player.UserId,
                Nickname = player.Nickname,
                Rank = player.RankName
            };

            await _collection.InsertOneAsync(data);

            return data;
        }

        /// <summary>
        /// Add or remove an amount of Recons or XP from a Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="recons"></param>
        /// <param name="xp"></param>
        public static async void UpdateReconsAndXP(Player player, int recons = 0, int xp = 0, bool ShowHint = true)
        {
            var data = await player.GetPlayerDataAsync();

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
        }

        /// <summary>
        /// Updates if a player can change thier nick
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        public static async void UpdateAllowNickChange(Player player, bool state)
        {
            var data = await player.GetPlayerDataAsync();
            data.NicknameChangable = state;

            data.Update();
        }

        /// <summary>
        /// Update the Players Displayname in DB for later use
        /// </summary>
        /// <param name="player"></param>
        public static async void UpdateCustomNick(Player player)
        {
            var data = await player.GetPlayerDataAsync();
            data.CustomNick = player.CustomName;

            data.Update();
        }

        /// <summary>
        /// Adds a Kill to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static async void AddKill(Player player)
        {
            var data = await player.GetPlayerDataAsync();
            data.Kills++;

            data.Update();
        }

        /// <summary>
        /// Adds a Death to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static async void AddDeath(Player player)
        {
            var data = await player.GetPlayerDataAsync();
            data.Deaths++;

            data.Update();
        }

        /// <summary>
        /// Adds a Warn to the specified player with relevant context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static async void AddWarn(Player target, Player issuer, string reson)
        {
            var data = await target.GetPlayerDataAsync();

            if (data.Warns == null)
                data.Warns = new List<Warn>();

            data.Warns.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            data.Update();
        }

        /// <summary>
        /// Adds a Watchlist entry to the specified player with relevant context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static async void AddWatchlist(Player target, Player issuer, string reson)
        {
            var data = await target.GetPlayerDataAsync();

            if (data.Watchlists == null)
                data.Watchlists = new List<Warn>();

            data.Watchlists.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            data.Update();
        }

        internal static async void UpdateDataAsync(Player player)
        {
            var data = await player.GetPlayerDataAsync();

            data.Id = player.UserId; // Just in case
            data.Nickname = player.Nickname;
            data.Rank = player.RankName;

            if (player.SessionVariables.ContainsKey("JoinTime"))
                data.Playtime += (double)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);

            await _collection.ReplaceOneAsync(p => p.Id == data.Id, data);
        }

        internal static async void UpdateDataAsync(PlayerData data)
        {
            Log.Debug($"Updating {data.Id}:{data.Nickname} data!");

            await _collection.ReplaceOneAsync(p => p.Id == data.Id, data);
        }

        // Events
        internal static async void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
            DBPlayerExtension.CachedPlayerData.TryRemove(ev.Player.UserId, out _);
            var data = await ev.Player.GetPlayerDataAsync();
            ev.Player.CustomName = data.CustomNick;
        }

        internal static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdateDataAsync(ev.Player);
        }

        internal static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.List)
            {
                UpdateDataAsync(p);
                DBPlayerExtension.CachedPlayerData.Clear(); // Clear cache to ensure fresh data next round
            }
        }
    }
}
