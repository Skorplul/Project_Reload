using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using SCP079 = Exiled.Events.Handlers.Scp079;
using Warhead = Exiled.Events.Handlers.Warhead;
using Exiled.CustomItems.API.Features;
using HarmonyLib;
using NGMainPlugin.Commands;
using NGMainPlugin.Systems.PainkillerHandlers;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using System.IO;
using NGMainPlugin.Systems.RespawnTimer.API.Features;
using System.Net;
using System;
using NGMainPlugin.DB;

namespace NGMainPlugin
{
    public class Main : Plugin<Config>
    {
        public override string Author { get; } = "Skorp 1.0 and LastPenguin";
        public override string Name { get; } = "NGMainPlugin (Dev-Build)";
        public override string Prefix { get; } = "NGM (Dev)";
        public override Version Version { get; } = new Version(1, 4, 0);

        public override Version RequiredExiledVersion { get; } = new Version(8, 9, 11);

        public EventHandlers EventHandlers;
        public PainkillerHand PainkillerHand;
        public Systems.LobbySystem.Handler LobbySystemHandler;
        public Systems.DiscordLogs.LogHandler DCLogHandler;
        public Systems.SystemEvents.Handlers SysEvHandler;
        public Systems.RGBNuke.EventHandler GayNukeHandler;

        public static Main Instance { get; private set; }
        private Harmony Harmony { get; set; } = new Harmony("LobbySystem");

        public static NGMainPlugin.Main Singleton;

        public static string RespawnTimerDirectoryPath { get; } = Path.Combine(Paths.Configs, nameof(NGMainPlugin.Systems.RespawnTimer));

        public override void OnEnabled()
        {
            this.Harmony.PatchAll();
            Config.LoadItems(); 

            Log.Debug("Registering items..");
            CustomItem.RegisterItems(overrideClass: Config.ItemConfigs);

            Instance = this;
            EventHandlers = new EventHandlers(this);
            PainkillerHand = new PainkillerHand(this);
            LobbySystemHandler = new Systems.LobbySystem.Handler();
            DCLogHandler = new Systems.DiscordLogs.LogHandler();
            SysEvHandler = new Systems.SystemEvents.Handlers();
            GayNukeHandler = new Systems.RGBNuke.EventHandler();


            Log.Debug("Loading Database...");
            Database.InitDB();

            // Events for the DataBase
            Player.Verified += Database.OnVerified;
            Player.Left += Database.OnPlayerLeft;
            Server.RoundEnded += Database.OnRoundEnded;


            Player.UsingItemCompleted += PainkillerHand.OnTakingPainkiller;
            Player.TriggeringTesla += EventHandlers.OnTriggeringTesla;
            SCP079.GainingLevel += EventHandlers.OnSCP079GainingLvl;
            Server.RoundStarted += EventHandlers.OnRoundStarted;
            Server.WaitingForPlayers += LobbySystemHandler.OnWaitingForPlayers;
            Player.Verified += LobbySystemHandler.OnVerified;
            Player.Banned += EventHandlers.OnBan;
            Player.Kicked += EventHandlers.OnKick;
            Server.RoundEnded += EventHandlers.OnRoundEnded;
            Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Server.RespawningTeam += SysEvHandler.Spawning;
            Warhead.Starting += SysEvHandler.WarheadActivating;
            Player.Left += EventHandlers.OnPlayerLeaving;
            Server.EndingRound += EventHandlers.OnEndingRound;

            Server.RoundStarted += GayNukeHandler.OnRoundStart;
            Server.RoundEnded += GayNukeHandler.OnRoundEnd;
            Warhead.Starting += GayNukeHandler.OnWarheadStart;
            Warhead.Detonated += GayNukeHandler.OnWarheadDetonation;
            Warhead.Stopping += GayNukeHandler.OnWarheadStop;

            SCPSwap.Plugin = this;
            Durchsage.Plugin = this;

            /// only for RespawnTimer
            Main.Singleton = this;
            if (!Directory.Exists(Main.RespawnTimerDirectoryPath))
            {
                Log.Warn("RespawnTimer directory does not exist. Creating...");
                Directory.CreateDirectory(Main.RespawnTimerDirectoryPath);
            }
            string str = Path.Combine(Main.RespawnTimerDirectoryPath, "ExampleTimer");
            if (!Directory.Exists(str))
                Log.Error("RespawnTimer file not existing!");
            Exiled.Events.Handlers.Map.Generated += new CustomEventHandler(Systems.RespawnTimer.EventHandler.OnGenerated);
            Exiled.Events.Handlers.Server.RoundStarted += new CustomEventHandler(Systems.RespawnTimer.EventHandler.OnRoundStart);
            Exiled.Events.Handlers.Player.Dying += new CustomEventHandler<DyingEventArgs>(Systems.RespawnTimer.EventHandler.OnDying);
            foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
            {
                switch (plugin.Name)
                {
                    case "Serpents Hand":
                        if (plugin.Config.IsEnabled)
                        {
                            NGMainPlugin.Systems.RespawnTimer.API.API.SerpentsHandTeam.Init(plugin.Assembly);
                            Log.Debug("Serpents Hand plugin detected!");
                            break;
                        }
                        break;
                    case "UIURescueSquad":
                        if (plugin.Config.IsEnabled)
                        {   
                            NGMainPlugin.Systems.RespawnTimer.API.API.UiuTeam.Init(plugin.Assembly);
                            Log.Debug("UIURescueSquad plugin detected!");
                            break;
                        }
                        break;
                }
            }
            if (!this.Config.ReloadTimerEachRound)
                this.OnReloaded();
            /// end


            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();
            this.Harmony.UnpatchAll();


            Player.Verified -= Database.OnVerified;
            Player.Left -= Database.OnPlayerLeft;
            Server.RoundEnded -= Database.OnRoundEnded;

            Database.CloseBD();


            Player.TriggeringTesla -= EventHandlers.OnTriggeringTesla;
            SCP079.GainingLevel -= EventHandlers.OnSCP079GainingLvl;
            Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Player.UsingItemCompleted -= PainkillerHand.OnTakingPainkiller;
            Server.WaitingForPlayers -= LobbySystemHandler.OnWaitingForPlayers;
            Player.Verified -= LobbySystemHandler.OnVerified;
            Player.Banned -= EventHandlers.OnBan;
            Player.Kicked -= EventHandlers.OnKick;
            Server.RoundEnded -= EventHandlers.OnRoundEnded;
            Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            Server.RespawningTeam -= SysEvHandler.Spawning;
            Warhead.Starting -= SysEvHandler.WarheadActivating;
            Player.Left -= EventHandlers.OnPlayerLeaving;
            Server.EndingRound -= EventHandlers.OnEndingRound;

            Server.RoundStarted -= GayNukeHandler.OnRoundStart;
            Server.RoundEnded -= GayNukeHandler.OnRoundEnd;
            Warhead.Starting -= GayNukeHandler.OnWarheadStart;
            Warhead.Detonated -= GayNukeHandler.OnWarheadDetonation;
            Warhead.Stopping -= GayNukeHandler.OnWarheadStop;

            PainkillerHand = null;
            EventHandlers = null;
            Instance = null;
            LobbySystemHandler = null;
            DCLogHandler = null;
            SysEvHandler = null;
            GayNukeHandler = null;
            SCPSwap.Plugin = null;
            Durchsage.Plugin = null;

            /// only for RespawnTimer
            Exiled.Events.Handlers.Map.Generated -= new CustomEventHandler(Systems.RespawnTimer.EventHandler.OnGenerated);
            Exiled.Events.Handlers.Server.RoundStarted -= new CustomEventHandler(Systems.RespawnTimer.EventHandler.OnRoundStart);
            Exiled.Events.Handlers.Player.Dying -= new CustomEventHandler<DyingEventArgs>(Systems.RespawnTimer.EventHandler.OnDying);
            Main.Singleton = null;
            ///end
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            /// only for RespawnTimer
            if (this.Config.Timers.IsEmpty<string, string>())
            {
                Log.Error("Timer list is empty!");
            }
            else
            {
                TimerView.CachedTimers.Clear();
                foreach (string name in this.Config.Timers.Values)
                    TimerView.AddTimer(name);
            }
        }
        ///end
    }
}
