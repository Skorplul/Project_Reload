// EF Core + SQLite basic starter template
// Assumes .NET 6 or later (you can adapt if on .NET Framework with workarounds)

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace PRMainPlugin.Systems.Database
{
    internal static class DBPlayerExtension
    {
        private static readonly ConcurrentDictionary<Player, PlayerData> CachedPlayerData = new();
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);

        public static async Task<PlayerData> GetPlayerDataAsync(this Player player)
        {
            if (CachedPlayerData.TryGetValue(player, out var data))
                return data;

            await _cacheLock.WaitAsync();
            try
            {
                // Double-check in case another thread already added it while waiting
                if (!CachedPlayerData.TryGetValue(player, out data))
                {
                    data = await Database.GetPlayerDataAsync(player);
                    CachedPlayerData.TryAdd(player, data);
                }
            }
            finally
            {
                _cacheLock.Release();
            }

            return data;
        }
    }

    // Player data model
    public class PlayerData
    {
        [Key]
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

    // EF Core DB context
    public class PRDbContext : DbContext
    {
        public DbSet<PlayerData> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(Path.Combine(Paths.Configs, "PRMainDB.sqlite"));
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    internal static class Database
    {
        private static PRDbContext DB;
        internal static void InitDB()
        {
            Config.LoadConfig();

            using var DB = new PRDbContext();
            DB.Database.EnsureCreated();

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
                UpdateData(p);
            }

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            XP.Disable();

            DB = null;
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

            var result = await DB.Players.FirstOrDefaultAsync(p => p.Id == player.UserId);

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

            await DB.Players.AddAsync(data);
            await DB.SaveChangesAsync();

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

        internal static async void UpdateData(Player player)
        {
            var data = await player.GetPlayerDataAsync();

            data.Id = player.UserId; // Just in case
            data.Nickname = player.Nickname;
            data.Rank = player.RankName;

            if (player.SessionVariables.ContainsKey("JoinTime"))
                data.Playtime += (int)((DateTime.Now - DateTime.FromBinary((long)player.SessionVariables["JoinTime"])).TotalSeconds);

            DB.Players.Update(data);
            await DB.SaveChangesAsync();
        }

        internal static async void UpdateData(PlayerData data)
        {
            Log.Debug($"Updating {data.Id}:{data.Nickname} data!");

            DB.Players.Update(data);
            await DB.SaveChangesAsync();
        }

        // Events
        internal static async void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.SessionVariables["JoinTime"] = DateTime.Now.ToBinary();
            ev.Player.CustomName = (await ev.Player.GetPlayerDataAsync()).CustomNick;
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
