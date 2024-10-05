namespace NGMainPlugin
{
    using System;
    using HarmonyLib;
    using Exiled.API.Features;
    using NGMainPlugin.Systems;
    using NGMainPlugin.Systems.EventsSystem;
    using NGMainPlugin.Systems.LobbySystem;
    using NGMainPlugin.Systems.RGBNuke;
    using NGMainPlugin.Systems.RespawnTimer;
    using NGMainPlugin.Systems.EventHandlers;
    using NGMainPlugin.Systems.CustomItems;
    using NGMainPlugin.Systems.Database;
    using NGMainPlugin.Systems.SCPSwap;

    public class NGMainPlguin : Plugin<Config>
    {
        public override string Author => "Skorp 1.0 and LastPenguin";
        public override string Name => "NGMainPlugin";
        public override string Prefix => "NGM";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(8, 11, 0);

        public static NGMainPlguin Instance { get; private set; }

        private Harmony Harmony { get; } = new Harmony($"com.NGMainPlugin-{DateTime.Now.Ticks}");

        public override void OnEnabled()
        {
            Instance = this;

            Log.Info("-----[NGMainPlugin Initialize]-----");
            
            ///throws error; please check @lastpenguin
            /*
            // Load Database
            Log.Info("Loading Database...");
            Database.InitDB();
            */

            // Load all configs
            Log.Info("Loading Configs...");
            Config.LoadConfigs();

            // All patches
            Log.Info("Harmony Patching...");
            Harmony.PatchAll();

            // Enable all the custom systems
            Log.Info("-----[Systems]-----");

            Log.Info("Enabling Custom Items..");
            CustomItems.Enable();

            Log.Info("Enabling Event Handlers...");
            EventHandlers.Enable();

            Log.Info("Enabling SCP Swap..");
            SCPSwap.Enable();

            Log.Info("Enabling Random Painkillers...");
            RandomPainkillers.Enable();

            Log.Info("Enabling Lobby System...");
            LobbySystem.Enable();

            Log.Info("Enabling RGBNuke System...");
            RGBNuke.Enable();

            Log.Info("Enabling Respawn Timer System...");
            RespawnTimer.Enable();

            Log.Info("Discord Logs: Unimplemented");
            DiscordLogs.Enable();

            Log.Info("Events System: Unimplemented");
            EventsSystem.Enable();

            Log.Info("-----[NGMainPlugin Initialized]-----");

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Info("-----[NGMainPlugin Disable]-----");

            // Close the DB
            Log.Info("Closing Database...");
            Database.CloseBD();

            // Unpatch everything
            Log.Info("Harmony Unpatching...");
            Harmony.UnpatchAll();

            // Disable all the custom systems
            Log.Info("-----[Systems]-----");

            Log.Info("Disabling Custom Items..");
            CustomItems.Disable();

            Log.Info("Disabling Event Handlers...");
            EventHandlers.Disable();

            Log.Info("Disabling SCP Swap...");
            SCPSwap.Disable();

            Log.Info("Disabling Random Painkillers...");
            RandomPainkillers.Disable();

            Log.Info("Disabling Lobby System...");
            LobbySystem.Disable();

            Log.Info("Disabling RGBNuke System...");
            RGBNuke.Disable();

            Log.Info("Disabling Respawn Timer System...");
            RespawnTimer.Disable();

            Log.Info("Discord Logs: Unimplemented");
            DiscordLogs.Disable();

            Log.Info("Events System: Unimplemented");
            EventsSystem.Disable();

            Log.Info("-----[NGMainPlugin Disabled]-----");

            base.OnDisabled();
        }
    }
}
