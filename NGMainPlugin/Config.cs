using Exiled.API.Interfaces;
using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.ComponentModel;
using System.IO;
using YamlDotNet.Serialization;
using AudioSystem.Models.SoundConfigs;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Enums;
using PlayerRoles;


namespace NGMainPlugin
{
    public class Config : IConfig
    {
        /// <summary>
        /// Gets item Config settings.
        /// </summary>
        [YamlIgnore]
        public Configs.Items ItemConfigs { get; private set; } = null!;

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets if tutorials should trigger teslas.
        /// </summary>
        [Description("Should Tutorials be ignored by teslas? Default: false")]
        public bool NoTesTuts { get; set; } = false;

        /// <summary>
        /// Gets or sets if should be able to only swap one time in one round.
        /// </summary>
        [Description("Should you be able to swap SCPs only one time a round? Default: true")]
        public bool SingleSwap { get; set; } = true;

        /// <summary>
        /// Gets or sets if FF should be enabled at the end of the round.
        /// </summary>
        [Description("Should friendly fire get enabled at the end of the round.")]
        public bool RoundEndFF { get; set; } = true;

        /// <summary>
        /// Gets or sets the time SCPs have to use .scpswap.
        /// </summary>
        [Description("After how many seconds should you not be able to swap scps anymore? Default: 60")]
        public float ScpSwapTimeout { get; set; } = 60f;

        /// <summary>
        /// Gets or sets the valid SCPs for the .scpswap command.
        /// </summary>
        [Description("Ihr könnt diese config ändern, aber für den Fall ihr wollt eine Rolle hinzufügen geht das nocht nicht, nur entfernen und wieder hinzufügen der bereits existierenden Rollen!")]
        public List<RoleTypeId> SwapableScps { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp079,
            RoleTypeId.Scp3114,
            RoleTypeId.Scp096,
            RoleTypeId.Scp173,
            RoleTypeId.Scp106,
            RoleTypeId.Scp939,
        };

        [Description("The player count at which 3114 will be allowed to get swaped to.")]
        public int SkelliCount = 20;

        [Description("Should SCP079 be able to use CASSI only once per round? Default: true")]
        public bool Single079Cassi { get; set; } = true;

        public List<LocalSoundConfig> ListOfPossibeMusics { get; set; } = new List<LocalSoundConfig>()
        {
            new LocalSoundConfig()
            {
                SoundName = "test",
                Volume = 25,
                DummyName = "test",
                Radius = 2f
            },
            new LocalSoundConfig()
            {
                SoundName = "test1",
                Volume = 25,
                DummyName = "test1",
                Radius = 4f
            },
        };

        /// <summary>
        /// Gets or sets a value indicating what folder item configs will be stored in.
        /// </summary>
        public string ItemConfigFolder { get; set; } = Path.Combine(Paths.Configs, "CustomItems");

        /// <summary>
        /// Gets or sets a value indicating what file will be used for item configs.
        /// </summary>
        public string ItemConfigFile { get; set; } = "global.yml";

        /// <summary>
        /// Loads the item configs.
        /// </summary>
        public void LoadItems()
        {
            if (!Directory.Exists(ItemConfigFolder))
                Directory.CreateDirectory(ItemConfigFolder);

            string filePath = Path.Combine(ItemConfigFolder, ItemConfigFile);
            Log.Info($"{filePath}");
            if (!File.Exists(filePath))
            {
                ItemConfigs = new Configs.Items();
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }
            else
            {
                ItemConfigs = Loader.Deserializer.Deserialize<Configs.Items>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }
        }

        [Description("LobbySystem")]
        public double LobbyTime { get; set; } = 30.0;

        public int MinimumPlayers { get; set; } = 2;

        [Description("Edit the text shown")]
        public string TextShown { get; set; } = "<size=27>│ %status% │ <b>SERVER NAME</b>  │ <color=red>%playercount%/%maxplayers%</color> Inmates Waiting │</size>";

        public string PausedStatus { get; set; } = "<color=red>\uD83D\uDFE5</color> Lobby Paused";

        public string WaitingStatus { get; set; } = "<color=yellow>\uD83D\uDFE8</color> Waiting for Players";

        public string StartingStatus { get; set; } = "<color=green>\uD83D\uDFE9</color> Starting in %countdown% Seconds";

        // Database
        [Description("[Database] Welcome to the server broadcast")]
        public string DBWelcome { get; set; } = "Welcome to the server! Please, check console!";

        [Description("[Database] Welcome to the server console message")]
        public string DBWelcomeConsole { get; set; } = "This server tracks some of custom data like your playtime. If you have DNT enabled, it is disabled by default. " +
                    "If you want to toggle custom data tracking On or Off, use .DataToggleTrack command. (or .DTT for short)";

        [Description("[Database] .DTT command description")]
        public string DBDTTDescription { get; set; } = "Toggle custom data tracking on and off.";

        [Description("[Database] Tracking toggled message")]
        public string DBTrackingToggled { get; set; } = "<color=green>You have toggled custom data tracking.</color>";

        [Description("[Database] Tracking toggle off warning message")]
        public string DBToggleWarning { get; set; } = "<color=red>WARNING.</color> <color=yellow>You are about to toggle custom data tracking off. Doing so will wipe all stored custom data. Confirm by using .DTT confirm." +
                    " (You can also skip this message by using .DTT force, if you feel like it ever again.)</color>";

        [Description("[Database] .pt client command description")]
        public string DBPTDescriptionClient { get; set; } = "See your playtime.";

        [Description("[Database] .pt remote admin command description")]
        public string DBPTDescriptionServer { get; set; } = "See other players' playtime.\n" +
            "No arguments = playtime of every player on the server.\n" +
            "<int> Id of a player on the server = playtime of a certain player currently on the server.\n" +
            "<string> ID or SteamID of a player = playtime of a certain player.";

        [Description("[Database] Client .pt command, data is not stored error")]
        public string DBDataNotStored { get; set; } = "<color=red>You have not agreed to the data collection, your playtime is not stored.</color>";

        [Description("[Database] Client .pt command, playtime")]
        public string DBClientPlaytime { get; set; } = "<color=green>Your playtime: </color> {0}";

        [Description("[Database] Server .pt command, players on server playtime")]
        public string DBServerEveryonePlaytime { get; set; } = "<color=green>Playtime of players on server: </color>\n";

        [Description("[Database] Server .pt command, playtime of one player")]
        public string DBServerPlayerPlaytime { get; set; } = "<color=green>Playtime of {0}:</color> {1}";

        [Description("[Database] Server .pt command, wrong ID error")]
        public string DBServerWrongID { get; set; } = "<color=red>Wrong player ID/SteamID or Player's Data is not stored.</color>";

        [Description("[Database] Server .pt command, data of player is not stored error")]
        public string DBServerDataNotStored { get; set; } = "<color=red>Player's data is not saved due to them not agreeing to data collection.</color>";

        public RoomType SpawnRoom { get; set; }

        public Vector3 SpawnPosition { get; set; } = new Vector3(0.0f, 995.6f, -8f);

        public Dictionary<string, string> Timers { get; private set; } = new Dictionary<string, string>()
        {
            {
            "default",
            "ExampleTimer"
            }
        };

        [Description("Whether the timer should be reloaded each round. Useful if you have many different timers designed.")]
        public bool ReloadTimerEachRound { get; private set; } = true;

        [Description("Whether the timer should be hidden for players in overwatch.")]
        public bool HideTimerForOverwatch { get; private set; } = true;

        [Description("The delay before the timer will be shown after player death.")]
        public float TimerDelay { get; private set; } = -1f;

        [Description("Gay Nuke Config. :^)")]
        public float ColorChangeTime { get; set; } = 1f;
        public int NukeChance { get; set; } = 20;
        public int Volume { get; set; } = 500;
        public string MusicDirectory { get; set; } = "{global}/Rainbow Nuke/Audio";
        public string DisplayName { get; set; } = "Alpha warhead";
    }
}
