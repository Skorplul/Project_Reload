namespace NGMainPlugin.Systems.EventsSystem
{
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;

    internal class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "eventsystem.yml");


    }
}
