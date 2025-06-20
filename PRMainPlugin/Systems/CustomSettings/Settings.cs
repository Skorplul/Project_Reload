using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PRMainPlugin.Systems.CustomSettings
{
    internal class Settings
    {
        private static List<SettingBase> settings;  // store settings so we can unregister later
        private static HeaderSetting header;
        public static void Enable()
        {
            header = new HeaderSetting("[Project Reload]", "Einstellungen für Project Reload.", false);

            var button = new ButtonSetting(
                0,                        // unique ID
                "ClickAction",            // setting label
                "Do It!",                 // text on the button
                0f,                       // hold time (0 for instant)
                "Click to perform action",// hint description
                header,                   // parent header
                (player, setting) => {    // callback when pressed
                    Handlers.ButtonPress(player, setting);
                }
            );

            var keybind = new KeybindSetting(
                1,                       // unique ID (must differ from the button ID)
                "ActivateAction",        // setting label
                KeyCode.K,               // default key
                false,                   // prevent key use in menus
                "Press K to activate!",  // hint description
                header,                  // parent header (same header groups them)
                (player, setting) => {   // callback when key is pressed
                    Handlers.KeyPress(player, setting);
                }
            );

            // Store all settings in a list for registration
            settings = new List<SettingBase> { header, button, keybind };

            // Register settings for all players (predicate can filter players if needed)
            SettingBase.Register(settings.Skip(1), p => true);

            Handlers.Enable();
        }

        public static void Disable()
        {
            // Unregister settings from all players (predicate can filter players if needed)
            SettingBase.Unregister(p => true, settings);

            Handlers.Disable();
        }
    }
}
