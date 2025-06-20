using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System;

namespace PRMainPlugin.Systems.CustomSettings
{
    internal static class Handlers
    {
        internal static void Enable()
        {

        }

        internal static void Disable()
        {

        }

        internal static void ButtonPress(Player p, SettingBase s)
        {
            Log.Info($"{p.Nickname} clicked the button!");
        }

        internal static void KeyPress(Player p, SettingBase s)
        {
            Log.Info($"{p.Nickname} used the keybind!");
        }
    }
}
