namespace PRMainPlugin
{
    using Exiled.API.Interfaces;
    using Exiled.API.Features;
    using Exiled.Loader;
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;
    using System.Collections.Generic;
    using PRMainPlugin.Systems.RespawnTimer;
    using PRMainPlugin.Systems.LobbySystem;
    using PRMainPlugin.Systems.RGBNuke;
    using PRMainPlugin.Systems.EventHandlers;
    using PRMainPlugin.Systems.CustomItems;
    using PRMainPlugin.Systems.SCPSwap;
    using AudioSystem.Models.SoundConfigs;
    using PRMainPlugin.Systems.SCPHud;
    using PRMainPlugin.Systems.Notifications;
    using PRMainPlugin.Systems.RemoteKeycard;
    using System;
    using System.Linq;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [YamlIgnore]
        public string ConfigFolderPath = Path.Combine(Paths.Configs, "PRMainPlugin");

        [YamlIgnore]
        public ItemConfigs ItemConfigs;

        private T GetConfig<T>(string path, T defaultConfig)
        {       
            string dir = ConfigFolderPath;
            Log.Debug($"Checking config directory: {dir}");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(path))
            {
                Log.Info($"Config file not found. Creating new one at: {path}");
                File.WriteAllText(path, Loader.Serializer.Serialize(defaultConfig));
                return defaultConfig;
            }

            try
            {
                string fileContent = File.ReadAllText(path);
                T existingConfig = Loader.Deserializer.Deserialize<T>(fileContent);

                if (IsConfigOutdated(existingConfig, defaultConfig))
                {
                    Log.Warn($"Config file at {path} is outdated. Merging new changes while keeping existing values.");
                    existingConfig = MergeConfigs(existingConfig, defaultConfig);
                    File.WriteAllText(path, Loader.Serializer.Serialize(existingConfig));
                }

                return existingConfig;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read or parse config file at {path}. Using default config. Exception: {ex.Message}");
                File.WriteAllText(path, Loader.Serializer.Serialize(defaultConfig)); // Overwrite with a valid default config
                return defaultConfig;
            }
        }

        private T MergeConfigs<T>(T existingConfig, T newConfig)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var oldValue = prop.GetValue(existingConfig);
                var newValue = prop.GetValue(newConfig);

                if (oldValue == null) // If old config is missing a property, update it
                {
                    prop.SetValue(existingConfig, newValue);
                }
                else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
                {
                    // Recursively merge nested objects
                    var mergedValue = MergeConfigs(oldValue, newValue);
                    prop.SetValue(existingConfig, mergedValue);
                }
            }

            return existingConfig;
        }


        private bool IsConfigOutdated<T>(T existingConfig, object newConfig)
        {
            if (existingConfig == null)
                return true; // If the existing file is empty or couldn't be deserialized, it's outdated

            // Convert both to dictionaries (key-value pairs) to compare structures
            var existingProps = existingConfig.GetType().GetProperties().Select(p => p.Name).ToHashSet();
            var newProps = newConfig.GetType().GetProperties().Select(p => p.Name).ToHashSet();

            // If the new config has extra properties that are missing in the old one, it's outdated
            return !existingProps.SetEquals(newProps);
        }


        public void LoadConfigs()
        {
            RespawnTimer.Config = GetConfig<Systems.RespawnTimer.Config>(Systems.RespawnTimer.Config.ConfigPath, new Systems.RespawnTimer.Config());
            LobbySystem.Config = GetConfig<Systems.LobbySystem.Config>(Systems.LobbySystem.Config.ConfigPath, new Systems.LobbySystem.Config());
            RGBNuke.Config = GetConfig<Systems.RGBNuke.Config>(Systems.RGBNuke.Config.ConfigPath, new Systems.RGBNuke.Config());
            EventHandlers.Config = GetConfig<Systems.EventHandlers.Config>(Systems.EventHandlers.Config.ConfigPath, new Systems.EventHandlers.Config());
            SCPSwap.Config = GetConfig<Systems.SCPSwap.Config>(Systems.SCPSwap.Config.ConfigPath, new Systems.SCPSwap.Config());
            Notifications.Config = GetConfig<Systems.Notifications.Config>(Systems.Notifications.Config.ConfigPath, new Systems.Notifications.Config());
            SCPHud.Config = GetConfig<Systems.SCPHud.Config>(Systems.SCPHud.Config.ConfigPath, new Systems.SCPHud.Config());
            RemoteKeycards.Config = GetConfig<Systems.RemoteKeycard.Config>(Systems.RemoteKeycard.Config.ConfigPath, new Systems.RemoteKeycard.Config());
        }

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
    }
}
