namespace NGMainPlugin.Systems.EventsSystem
{
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;

    internal class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "eventsystem.yml");

        [Description("The time from the beginning of the Round to when the nuke gets activated.")]
        public static int PeanutRunTimeToNuke { get; set; } = 10;

        [Description("The time between nuke activation and Explotion.")]
        public static int PeanutRunTimeToExplode { get; set; } = 90;

    }
}
