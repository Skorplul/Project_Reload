namespace NGMainPlugin
{
    using Exiled.API.Interfaces;
    using Exiled.API.Features;
    using Exiled.Loader;
    using System;
    using System.ComponentModel;
    using System.IO;
    using YamlDotNet.Serialization;
    using System.Collections.Generic;
    using PlayerRoles;
    using NGMainPlugin.Systems.RespawnTimer;
    using NGMainPlugin.Systems.LobbySystem;
    using NGMainPlugin.Systems.RGBNuke;
    using NGMainPlugin.Systems.EventHandlers;
    using NGMainPlugin.Systems.CustomItems;
    using System.Linq;
    using HarmonyLib;
    using AudioSystem.Models.SoundConfigs;
    using NGMainPlugin.Systems.SCPSwap;

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [YamlIgnore]
        public string ConfigFolderPath = Path.Combine(Paths.Configs, "NGMainPlugin");

        [YamlIgnore]
        public ItemConfigs ItemConfigs;

        private T GetConfig<T>(string path, object config)
        {
            // Fill path with missing directories
            string dir = ConfigFolderPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            T Result;

            if (!File.Exists(path))
            {
                Result = (T)config;
                File.WriteAllText(path, Loader.Serializer.Serialize(config));
            }
            else
            {
                Result = Loader.Deserializer.Deserialize<T>(File.ReadAllText(path));
            }

            return Result;
        }

        public void LoadConfigs()
        {
            RespawnTimer.Config = GetConfig<Systems.RespawnTimer.Config>(Systems.RespawnTimer.Config.ConfigPath, new Systems.RespawnTimer.Config());
            LobbySystem.Config = GetConfig<Systems.LobbySystem.Config>(Systems.LobbySystem.Config.ConfigPath, new Systems.LobbySystem.Config());
            RGBNuke.Config = GetConfig<Systems.RGBNuke.Config>(Systems.RGBNuke.Config.ConfigPath, new Systems.RGBNuke.Config());
            EventHandlers.Config = GetConfig<Systems.EventHandlers.Config>(Systems.EventHandlers.Config.ConfigPath, new Systems.EventHandlers.Config());
            SCPSwap.Config = GetConfig<Systems.SCPSwap.Config>(Systems.SCPSwap.Config.ConfigPath, new Systems.SCPSwap.Config());
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
