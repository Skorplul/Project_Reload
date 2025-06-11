namespace PRMainPlugin.Systems.EventHandlers
{
    using Exiled.API.Features;
    using PlayerRoles;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Scp330;
    using System;
    using Exiled.Events.EventArgs.Scp914;
    using Exiled.API.Enums;
    using Scp914;
    using PRMainPlugin.API;

    public static class EventHandlers
    {
        internal static Config Config;

        public static int PcCurentLvl;

        private static bool friendlyFireDisable = false;

        private static Random random = new Random();

        public static void Enable()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel += OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Player.UsingRadioBattery += OnUsingRadioBattery;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += OnUpgradingPlayer;
            Exiled.Events.Handlers.Player.Left += OnLeft;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel -= OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330;
            Exiled.Events.Handlers.Player.UsingRadioBattery -= OnUsingRadioBattery;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= OnUpgradingPlayer;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
        }

        private static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player.GetData().Subclass == API.Enums.SubclassType.Blitz && ev.DamageHandler.Type is DamageType.Scp207)
                ev.IsAllowed = false;
        }

        private static void OnLeft(LeftEventArgs ev)
        {
            ev.Player.RemoveData();
        }

        private static void OnUpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            // 25% chance to be teleported as an scp
            int tp = random.Next(4);
            if (ev.Player.IsScp && ev.KnobSetting == Scp914KnobSetting.Rough && tp == 2) 
            {
                ev.Player.Teleport(RoomType.LczClassDSpawn);
            }
        }

        private static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            if (!Config.RadioUsingBattery)
            {
                ev.IsAllowed = false;
            }
        }

        private static void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (!Config.PinkIn330)
                return;

            if (random.Next(0, 100) <= Config.PinkCandyChance)
            {
                ev.Candy = InventorySystem.Items.Usables.Scp330.CandyKindID.Pink;
            }
            else
                return;
        }

        public static void OnRoundStarted()
        {
            PcCurentLvl = 1;
            Commands.Announcement.spoke.Clear();
        }

        private static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial)
            {
                if (Config.NoTesTuts)
                {
                    ev.IsAllowed = false;
                }
            }
        }

        private static void OnSCP079GainingLvl(GainingLevelEventArgs ev)
        {
            PcCurentLvl = ev.NewLevel;
        }
        private static void OnWaitingForPlayers()
        {
            if (friendlyFireDisable)
            {
                Log.Debug($"{nameof(OnWaitingForPlayers)}: Disabling friendly fire.");
                Server.FriendlyFire = false;
                friendlyFireDisable = false;
            }
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (Config.RoundEndFF && !Server.FriendlyFire)
            {
                Log.Debug($"{nameof(OnRoundEnded)}: Enabling friendly fire.");
                Server.FriendlyFire = true;
                friendlyFireDisable = true;
            }
        }
    }
}
