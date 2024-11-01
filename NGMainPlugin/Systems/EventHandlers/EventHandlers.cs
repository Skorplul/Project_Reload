namespace NGMainPlugin.Systems.EventHandlers
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using PlayerRoles;
    using NGMainPlugin.Systems.Liftaudio;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.EventArgs.Server;
    using MEC;

    public static class EventHandlers
    {
        internal static Config Config;

        public static int PcCurentLvl;

        private static bool friendlyFireDisable = false;

        private static bool Banned = false;

        public static void Enable()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel += OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel -= OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        public static void OnRoundStarted()
        {
            PcCurentLvl = 1;
            Commands.Announcement.spoke.Clear();

            /* *Disabled because of bugs on Modded_Main* **not rn**
            foreach (Lift lift in (IEnumerable<Lift>)Lift.List)
                Timing.RunCoroutine(Methods.CheckingPlayerLift(lift));
            */
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
