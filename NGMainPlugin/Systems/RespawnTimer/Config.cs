namespace NGMainPlugin.Systems.RespawnTimer
{
    using Exiled.API.Features;
    using Exiled.Loader;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using YamlDotNet.Serialization;

    internal class Config
    {
        [YamlIgnore]
        private static string ConfigFolderPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "RespawnTimer");

        [YamlIgnore]
        public static string ConfigPath = Path.Combine(ConfigFolderPath, "config.yml");

        [YamlIgnore]
        public static string TimerBeforeRespawnPath = Path.Combine(ConfigFolderPath, "TimerBeforeSpawn.txt");

        [YamlIgnore]
        public static string TimerDuringRespawnPath = Path.Combine(ConfigFolderPath, "TimerDuringSpawn.txt");

        [YamlIgnore]
        public static string HintsPath = Path.Combine(ConfigFolderPath, "Hints.txt");

        [YamlIgnore]
        public string TimerBeforeRespawn = "";

        [YamlIgnore]
        public string TimerDuringRespawn = "";

        [YamlIgnore]
        public List<string> Hints = new List<string>();


        [Description("Whether the timer should be reloaded each round. Useful if you have many different timers designed.")]
        public bool ReloadTimerEachRound { get; private set; } = true;

        [Description("Whether the timer should be hidden for players in overwatch.")]
        public bool HideTimerForOverwatch { get; private set; } = true;

        [Description("Whether the leading zeros should be added in minutes and seconds if number is less than 10.")]
        public bool LeadingZeros { get; private set; } = true;

        [Description("Whether the timer should add time offset depending on MTF/CI spawn.")]
        public bool TimerOffset { get; private set; } = true;

        [Description("How often custom hints should be changed (in seconds).")]
        public int HintInterval { get; private set; } = 10;

        [Description("The Nine-Tailed Fox display name.")]
        public string Ntf { get; private set; } = "<color=blue>Nine-Tailed Fox</color>";

        [Description("The Chaos Insurgency display name.")]
        public string Ci { get; private set; } = "<color=green>Chaos Insurgency</color>";

        [Description("The Serpent's Hand display name.")]
        public string Sh { get; private set; } = "<color=red>Serpent's Hand</color>";

        [Description("The Unusual Incidents Unit  display name.")]
        public string Uiu { get; private set; } = "<color=yellow>Unusual Incidents Unit</color>";

        [Description("The display names for warhead statuses:")]
        public Dictionary<Exiled.API.Enums.WarheadStatus, string> WarheadStatus { get; private set; } = new Dictionary<Exiled.API.Enums.WarheadStatus, string>()
        {
            {
              Exiled.API.Enums.WarheadStatus.NotArmed,
              "<color=green>Unarmed</color>"
            },
            {
              Exiled.API.Enums.WarheadStatus.Armed,
              "<color=orange>Armed</color>"
            },
            {
              Exiled.API.Enums.WarheadStatus.InProgress,
              "<color=red>In Progress - </color> {detonation_time} s"
            },
            {
              Exiled.API.Enums.WarheadStatus.Detonated,
              "<color=#640000>Detonated</color>"
            }
        };

        public void LoadConfig()
        {
            // Fill path with missing directories
            string dir = ConfigFolderPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(TimerBeforeRespawnPath))
                File.WriteAllText(TimerBeforeRespawnPath, "");
            if (!File.Exists(TimerDuringRespawnPath))
                File.WriteAllText(TimerDuringRespawnPath, "");
            if (!File.Exists(HintsPath))
                File.WriteAllText(HintsPath, "");

            TimerBeforeRespawn = File.ReadAllText(TimerBeforeRespawnPath);
            TimerDuringRespawn = File.ReadAllText(TimerDuringRespawnPath);
            Hints.AddRange(File.ReadAllLines(HintsPath));
        }
    }
}
