using System.ComponentModel;
using System.IO;
using YamlDotNet.Serialization;

namespace PRMainPlugin.Systems.RemoteKeycard
{
    public class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "remote_keaycard.yml");


        [Description("Whether  Amnesia affects the usage of keycards.")]
        public bool AmnesiaMatters { get; set; } = false;

        [Description("Whether this plugin works on generators.")]
        public bool AffectGenerators { get; set; } = true;

        [Description("Whether this plugin works on Warhead's panel.")]
        public bool AffectWarheadPanel { get; set; } = true;

        [Description("Whether this plugin works on SCP lockers.")]
        public bool AffectScpLockers { get; set; } = true;

        [Description("Whether this plugin works on doors.")]
        public bool AffectDoors { get; set; } = true;
    }
}
