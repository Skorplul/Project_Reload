namespace NGMainPlugin.Systems.Database
{
    using Exiled.API.Features;
    using Exiled.Loader;
    using HarmonyLib;
    using System.IO;
    using System.Linq;
    using YamlDotNet.Serialization;

    public class Config
    {
        [YamlIgnore]
        private static string ConfigFolderPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "Database");

        [YamlIgnore]
        private static string ConfigPath = Path.Combine(ConfigFolderPath, "config.yml");

        [YamlIgnore]
        private static string TranslationsPath = Path.Combine(ConfigFolderPath, "translations.yml");

        [YamlIgnore]
        public static Translations Translations;


        public static void LoadConfig()
        {
            
            // Fill path with missing directories
            string dir = ConfigFolderPath;
            Log.Info(dir);
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
