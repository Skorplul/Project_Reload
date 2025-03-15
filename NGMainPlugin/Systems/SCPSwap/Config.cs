using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using YamlDotNet.Serialization;

namespace PRMainPlugin.Systems.SCPSwap
{
    public class Config
    {
        [YamlIgnore]
        private static string ConfigFolderPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "SCPSwap");

        [YamlIgnore]
        public static string ConfigPath = Path.Combine(ConfigFolderPath, "config.yml");

        [Description("The time in which players can swap after roundstart.")]
        public static int ScpSwapTimeout { get; set; } = 60;

        [Description("Ihr könnt diese config ändern, aber für den Fall ihr wollt eine Rolle hinzufügen geht das nocht nicht, nur entfernen und wieder hinzufügen der bereits existierenden Rollen!")]
        public static List<RoleTypeId> SwapableScps { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp079,
            RoleTypeId.Scp3114,
            RoleTypeId.Scp096,
            RoleTypeId.Scp173,
            RoleTypeId.Scp106,
            RoleTypeId.Scp939,
        };

        [Description("Should you be able to swap SCPs only one time a round? Default: true")]
        public static bool SingleSwap { get; set; } = true;
    }
}
