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
                0,
                "ClickAction",
                "Do It!",
                0f,
                "Click to perform action",
                header,
                (player, setting) => { Handlers.ButtonPress(player, setting); }
            );

            var keybind = new KeybindSetting(
                1,
                "Fähigkeit benutzen",
                KeyCode.B,
                false,
                "Taste zum nutzen der Rollen speziefischen Fähigkeit.",
                header,
                (player, setting) => {  Handlers.UseAbility(player, setting); }
            );

            var respawntimer = new TwoButtonsSetting(
                2,
                "Spectatoroverlay Toggle",
                "An",
                "Aus",
                false,
                "Ob der das Spectatoroverlay gezeigt werden soll.",
                header,
                (player, setting) => { Handlers.ToggleRespawnTimer(player, setting); }
            );

            // Store all settings in a list for registration
            settings = new List<SettingBase> { header, button, keybind, respawntimer };

            // Register settings for all players (predicate can filter players if needed)
            SettingBase.Register(settings.Skip(1), p => true);

            //Handlers.Enable();
        }

        public static void Disable()
        {
            // Unregister settings from all players (predicate can filter players if needed)
            SettingBase.Unregister(p => true, settings);

            //Handlers.Disable();
        }
    }
}
