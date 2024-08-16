namespace NGMainPlugin.Systems.CustomItems
{
    using Exiled.Loader;
    using NGMainPlugin.Items;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ItemConfigs
    {
        private static string ConfigFolderPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "CustomItems");

        public List<object> CustomItems = new();

        private static List<Type> ItemList = new()
        {
            typeof(C4Charge),
            typeof(GrenadeLauncher),
            typeof(DoorRemote),
            typeof(ImpactNade),
            typeof(Knife),
            typeof(RPG),
            typeof(Scp1499),
            typeof(SniperRifle),
            typeof(TranquilizerGun),
        };

        public void LoadConfig()
        {
            if (!Directory.Exists(ConfigFolderPath))
                Directory.CreateDirectory(ConfigFolderPath);

            foreach (var item in ItemList)
            {
                string configPath = Path.Combine(ConfigFolderPath, $"{item.Name}.yml");
                if (!File.Exists(configPath))
                {
                    var itemConfig = Activator.CreateInstance(item);
                    CustomItems.Add(itemConfig);
                    File.WriteAllText(configPath, Loader.Serializer.Serialize(itemConfig));
                }
                else
                {
                    var itemConfig = Loader.Deserializer.Deserialize(File.ReadAllText(configPath), item);
                    CustomItems.Add(itemConfig);
                    File.WriteAllText(configPath, Loader.Serializer.Serialize(itemConfig));
                }
            }
        }
    }
}
