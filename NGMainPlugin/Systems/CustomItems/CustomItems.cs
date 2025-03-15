namespace PRMainPlugin.Systems.CustomItems
{
    using Exiled.API.Features.Attributes;
    using Exiled.CustomItems.API.Features;
    using System.Reflection;

    internal static class CustomItems
    {
        private static ItemConfigs Config;

        public static void Enable()
        {
            Config = new ItemConfigs();
            Config.LoadConfig();
            
            foreach (var item in Config.CustomItems)
            {
                CustomItem customItem = item as CustomItem;
                if (customItem.Type == ItemType.None)
                    customItem.Type = item.GetType().GetCustomAttribute<CustomItemAttribute>(true).ItemType;

                typeof(CustomItem).GetMethod("TryRegister", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(customItem, new object[] { });
            }
        }

        public static void Disable()
        {
            foreach (CustomItem item in CustomItem.Registered)
            {
                typeof(CustomItem).GetMethod("TryUnregister", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(item, new object[] { });
            }
        }
    }
}
