namespace PRMainPlugin
{
    using System;
    using HarmonyLib;
    using Exiled.API.Features;
    using PRMainPlugin.Systems;
    using PRMainPlugin.Systems.LobbySystem;
    using PRMainPlugin.Systems.RGBNuke;
    using PRMainPlugin.Systems.RespawnTimer;
    using PRMainPlugin.Systems.EventHandlers;
    using PRMainPlugin.Systems.EventsSystem;
    using PRMainPlugin.Systems.CustomItems;
    using PRMainPlugin.Systems.Notifications;
    using PRMainPlugin.Systems.Subclasses;
    using PRMainPlugin.Systems.SCPHud;
    using PRMainPlugin.Systems.RemoteKeycard;

    public class NGMainPlguin : Plugin<Config>
    {
        public override string Author => "Skorp 1.0 and LastPenguin";
        public override string Name => "PRMainPlugin";
        public override string Prefix => "NGM";
        public override Version Version => new Version(2, 4, 0);
        public override Version RequiredExiledVersion => new Version(8, 9, 11);

        public static NGMainPlguin Instance { get; private set; }

        private Harmony Harmony { get; } = new Harmony($"com.PRMainPlugin-{DateTime.Now.Ticks}");

        public override void OnEnabled()
        {
            Instance = this;

            Log.Info("\n        ▄███████▄    ▄████████  ▄██████▄       ▄█    ▄████████  ▄████████     ███             ▄████████    ▄████████  ▄█        ▄██████▄     ▄████████ ████████▄       \r\n       ███    ███   ███    ███ ███    ███     ███   ███    ███ ███    ███ ▀█████████▄        ███    ███   ███    ███ ███       ███    ███   ███    ███ ███   ▀███      \r\n       ███    ███   ███    ███ ███    ███     ███   ███    █▀  ███    █▀     ▀███▀▀██        ███    ███   ███    █▀  ███       ███    ███   ███    ███ ███    ███      \r\n       ███    ███  ▄███▄▄▄▄██▀ ███    ███     ███  ▄███▄▄▄     ███            ███   ▀       ▄███▄▄▄▄██▀  ▄███▄▄▄     ███       ███    ███   ███    ███ ███    ███      \r\n     ▀█████████▀  ▀▀███▀▀▀▀▀   ███    ███     ███ ▀▀███▀▀▀     ███            ███          ▀▀███▀▀▀▀▀   ▀▀███▀▀▀     ███       ███    ███ ▀███████████ ███    ███      \r\n       ███        ▀███████████ ███    ███     ███   ███    █▄  ███    █▄      ███          ▀███████████   ███    █▄  ███       ███    ███   ███    ███ ███    ███      \r\n       ███          ███    ███ ███    ███     ███   ███    ███ ███    ███     ███            ███    ███   ███    ███ ███▌    ▄ ███    ███   ███    ███ ███   ▄███      \r\n      ▄████▀        ███    ███  ▀██████▀  █▄ ▄███   ██████████ ████████▀     ▄████▀          ███    ███   ██████████ █████▄▄██  ▀██████▀    ███    █▀  ████████▀       \r\n                    ███    ███            ▀▀▀▀▀▀                                             ███    ███              ▀                                                 ");

            // Load all configs
            Log.Info("Loading Configs...");
            Config.LoadConfigs();

            // All patches
            Log.Info("Harmony Patching...");
            Harmony.PatchAll();

            // Enable all the custom systems
            Log.Info("-----[Systems]-----");

            //Log.Info("Enabling Custom Items..");
            //CustomItems.Enable();

            Log.Info("Enabling Event Handlers...");
            EventHandlers.Enable();

            Log.Info("Enabling Random Painkillers...");
            RandomPainkillers.Enable();

            Log.Info("Enabling Lobby System...");
            LobbySystem.Enable();

            Log.Info("Enabling RGBNuke System...");
            RGBNuke.Enable();

            Log.Info("Enabling Respawn Timer System...");
            RespawnTimer.Enable();

            Log.Info("Enabling Notification System...");
            Notifications.Enable();

            Log.Info("Events System...");
            EventsSystemHandler.Enable();

            Log.Info("Enabling Subclasses...");
            Subclasses.Enable();

            Log.Info("Enabling SCPHud...");
            SCPHud.Enable();

            Log.Info("Enabling RemoteKeycards...");
            RemoteKeycards.Enable();

            Log.Info("-----[PRMainPlugin Initialized]-----");
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Info("-----[PRMainPlugin Disable]-----");

            // Unpatch everything
            Log.Info("Harmony Unpatching...");
            Harmony.UnpatchAll();

            // Disable all the custom systems
            Log.Info("-----[Systems]-----");

            //Log.Info("Disabling Custom Items..");
            //CustomItems.Disable();

            Log.Info("Disabling Event Handlers...");
            EventHandlers.Disable();

            Log.Info("Disabling Random Painkillers...");
            RandomPainkillers.Disable();

            Log.Info("Disabling Lobby System...");
            LobbySystem.Disable();

            Log.Info("Disabling RGBNuke System...");
            RGBNuke.Disable();

            Log.Info("Disabling Respawn Timer System...");
            RespawnTimer.Disable();

            Log.Info("Disabling Notification System...");
            Notifications.Disable();

            Log.Info("Disableing Events System...");
            EventsSystemHandler.Disable();

            Log.Info("Disabling Subclasses...");
            Subclasses.Disable();

            Log.Info("Disableing SCPHud...");
            SCPHud.Disable();

            Log.Info("Disableing RemoteKeycards...");
            RemoteKeycards.Disable();

            Log.Info("-----[PRMainPlugin Disabled]-----");
            
            base.OnDisabled();
        }
    }
}
