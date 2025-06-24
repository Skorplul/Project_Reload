using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PRMainPlugin.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRMainPlugin.Systems.Database
{
    public static class PlayerDataCache
    {
        internal static readonly ConcurrentDictionary<string, PlayerData> Data = new();

        public static PlayerData Get(string id)
            => Data.TryGetValue(id, out var result) ? result : null;

        public static void Set(string id, PlayerData playerData)
            => Data[id] = playerData;
    }

    // Player data model
    public class PlayerData
    {
        [BsonId]
        public string Id { get; set; }
        public string DiscordId { get; set; } = null;
        public bool Verified { get; set; } = false;
        public string VerificationToken { get; set; }
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

            LoadAllDataFromDatabase();

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            XP.Enable();
        }

        internal static void CloseDB()
        {
            foreach (var p in Player.List)
            {
                UpdateDataAsync(p);
            }

            SaveAllDataToDatabase();

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            XP.Disable();

            _collection = null;
        }

        private static void LoadAllDataFromDatabase()
        {
            Task.Run(async () =>
            {
                var allPlayers = await _collection.Find(_ => true).ToListAsync();
                foreach (var player in allPlayers)
                {
                    PlayerDataCache.Data[player.Id] = player;
                }

                Log.Info($"Loaded {PlayerDataCache.Data.Count} player data entries into memory.");
            }).GetAwaiter().GetResult(); // Block here if you must to ensure data is ready.
        }

        private static void SaveAllDataToDatabase()
        {
            Task.Run(async () =>
            {
                foreach (var playerData in PlayerDataCache.Data.Values)
                {
                    await SavePlayerDataPartialAsync(playerData);
                }

                Log.Info("Saved all in-memory player data to MongoDB.");
            });
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

        public static async Task<PlayerData> GetPlayerDataByIdAsync(string steamId)
        {
            return await _collection.Find(p => p.Id == steamId).FirstOrDefaultAsync();
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
            PlayerDataCache.Data[player.UserId] = data;

            return data;
        }

        public static async Task SavePlayerDataPartialAsync(PlayerData data)
        {
            var filter = Builders<PlayerData>.Filter.Eq(p => p.Id, data.Id);

            var update = Builders<PlayerData>.Update
                .Set(p => p.Nickname, data.Nickname)
                .Set(p => p.CustomNick, data.CustomNick)
                .Set(p => p.NicknameChangable, data.NicknameChangable)
                .Set(p => p.Warns, data.Warns)
                .Set(p => p.Watchlists, data.Watchlists)
                .Set(p => p.XP, data.XP)
                .Set(p => p.Level, data.Level)
                .Set(p => p.RequiredXP, data.RequiredXP)
                .Set(p => p.Playtime, data.Playtime)
                .Set(p => p.Recons, data.Recons)
                .Set(p => p.Deaths, data.Deaths)
                .Set(p => p.Kills, data.Kills)
                .Set(p => p.DiscordId, data.DiscordId);

            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }


        /// <summary>
        /// Add or remove an amount of Recons or XP from a Player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="recons"></param>
        /// <param name="xp"></param>
        public static async void UpdateReconsAndXP(Player player, int recons = 0, int xp = 0, bool ShowHint = true)
        {
            var data = PlayerDataCache.Get(player.UserId);

            if (recons != 0)
                data.Recons = data.Recons + recons;
            if (xp != 0)
                data.XP = data.XP + xp;

            PlayerDataCache.Set(player.UserId, data);

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
            var data = PlayerDataCache.Get(player.UserId);
            data.NicknameChangable = state;

            PlayerDataCache.Set(player.UserId, data);
        }

        /// <summary>
        /// Update the Players Displayname in DB for later use
        /// </summary>
        /// <param name="player"></param>
        public static async void UpdateCustomNick(Player player)
        {
            var data = PlayerDataCache.Get(player.UserId);
            data.CustomNick = player.CustomName;

            PlayerDataCache.Set(player.UserId, data);
        }

        /// <summary>
        /// Adds a Kill to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static async void AddKill(Player player)
        {
            var data = PlayerDataCache.Get(player.UserId);
            data.Kills++;

            PlayerDataCache.Set(player.UserId, data);
        }

        /// <summary>
        /// Adds a Death to the Database entry for this player
        /// </summary>
        /// <param name="player"></param>
        public static async void AddDeath(Player player)
        {
            var data = PlayerDataCache.Get(player.UserId);
            data.Deaths++;

            PlayerDataCache.Set(player.UserId, data);
        }

        /// <summary>
        /// Adds a Warn to the specified player with relevant context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static async void AddWarn(Player target, Player issuer, string reson)
        {
            var data = PlayerDataCache.Get(target.UserId);

            if (data.Warns == null)
                data.Warns = new List<Warn>();

            data.Warns.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            PlayerDataCache.Set(target.UserId, data);
        }

        /// <summary>
        /// Adds a Warn to the specified player with relevant context.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static void AddWarn(PlayerData data, Player issuer, string reson)
        {
            if (data.Warns == null)
                data.Warns = new List<Warn>();

            data.Warns.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            PlayerDataCache.Set(data.Id, data);
        }

        /// <summary>
        /// Remove a warn using the string parsed ObjectId.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="warn"></param>
        /// <returns></returns>
        public static bool RemoveWarn(PlayerData data, ObjectId warn)
        {
            var TestWarn = data.Warns.FirstOrDefault(w => w.Id == warn);
            if (data.Warns.IsEmpty() || data.Warns == null || TestWarn == null)
            {
                return false;
            }

            try
            {
                data.Warns.RemoveAll(w => w.Id == warn);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lsit all warns of the ID
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns>A Task<Lsit<Warn>> with all warns of the specified UserId</returns>
        public async static Task<List<Warn>> ListWarns(string steamID)
        {
            PlayerData data = await _collection.Find(steamID).FirstOrDefaultAsync();

            return data == null ? new List<Warn>() : data.Warns;
        }

        /// <summary>
        /// Adds a Watchlist entry to the specified player with relevant context.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static async void AddWatchlist(Player target, Player issuer, string reson)
        {
            var data = PlayerDataCache.Get(target.UserId);

            if (data.Watchlists == null)
                data.Watchlists = new List<Warn>();

            data.Watchlists.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            PlayerDataCache.Set(target.UserId, data);
        }

        /// <summary>
        /// Adds a Watchlist entry to the specified player with relevant context.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="issuer"></param>
        /// <param name="reson"></param>
        public static void AddWatchlist(PlayerData data, Player issuer, string reson)
        {
            if (data.Watchlists == null)
                data.Watchlists = new List<Warn>();

            data.Watchlists.Add(new Warn
            {
                CreatedAt = DateTime.UtcNow.Date,
                Reason = reson,
                Issuer = issuer.Nickname
            });

            PlayerDataCache.Set(data.Id, data);
        }

        /// <summary>
        /// Remove a watchlists using the string parsed ObjectId.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="watchlist"></param>
        /// <returns>If the Watchlist entry was removed Succesfully</returns>
        public static bool RemoveWatchlist(PlayerData data, ObjectId watchlist)
        {
            var TestWatchlists = data.Watchlists.FirstOrDefault(w => w.Id == watchlist);

            if (data.Watchlists.IsEmpty() || data.Watchlists == null || TestWatchlists == null)
            {
                return false;
            }

            try
            {
                data.Watchlists.RemoveAll(w => w.Id == watchlist);
                PlayerDataCache.Set(data.Id, data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lsit all watchlists of the ID
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns>A Task<Lsit<Warn>> with all watchlists of the specified UserId</returns>
        public async static Task<List<Warn>> ListWatchlist(string steamID)
        {
            PlayerData data = await _collection.Find(steamID).FirstOrDefaultAsync();

            return data == null ? new List<Warn>() : data.Warns;
        }

        internal static async void UpdateDataAsync(Player player)
        {
            var data = PlayerDataCache.Get(player.UserId);

            data.Id = player.UserId; // Just in case
            data.Nickname = player.Nickname;

            if (player.SessionVariables.ContainsKey("JoinTime"))
                data.Playtime += (double)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);

            await SavePlayerDataPartialAsync(data);
        }

        internal static async void UpdateDataAsync(PlayerData data)
        {
            Log.Debug($"Updating {data.Id}:{data.Nickname} data!");

            await SavePlayerDataPartialAsync(data);
        }

        // Events
        internal static async void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();

            var data = await _collection.Find(p => p.Id == ev.Player.UserId).FirstOrDefaultAsync() ?? await CreatePlayerDataAsync(ev.Player);

            PlayerDataCache.Set(ev.Player.UserId, data);
            ev.Player.CustomName = data.CustomNick;
        }

        internal static void OnPlayerLeft(LeftEventArgs ev)
        {
            UpdateDataAsync(ev.Player);
        }

        internal static async void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (var p in Player.List)
            {
                UpdateDataAsync(p);
            }
            SaveAllDataToDatabase();
        }
    }
}
