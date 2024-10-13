namespace NGMainPlugin.Systems.SCPSwap
{
    using Exiled.Loader;
    using HarmonyLib;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using YamlDotNet.Serialization;

    public class Config
    {
        [YamlIgnore]
        private static string ConfigFolderPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "SCPSwap");

        [YamlIgnore]
        public static string ConfigPath = Path.Combine(ConfigFolderPath, "config.yml");

        [YamlIgnore]
        private static string TranslationsPath = Path.Combine(ConfigFolderPath, "translations.yml");

        [YamlIgnore]
        public static Translations Translations;


        [Description("Should you be able to swap SCPs only once a round? Default: true")]
        public bool SingleSwap { get; set; } = true;

        [Description("After how many seconds should you not be able to swap scps anymore? Default: 60")]
        public float ScpSwapTimeout { get; set; } = 60f;

        [Description("You can change this config, but in case you want to add a role you can't do that yet, only remove and re-add the already existing roles!")]
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


        public static void LoadConfig()
        {
            // Fill path with missing directories
            string dir = ConfigFolderPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(TranslationsPath))
            {
                Translations = new();
                File.WriteAllText(TranslationsPath, Loader.Serializer.Serialize(Translations));
            }
            else
            {
                Translations = Loader.Deserializer.Deserialize<Translations>(File.ReadAllText(TranslationsPath));
            }
        }
    }
}
