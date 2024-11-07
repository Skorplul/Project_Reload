namespace NGMainPlugin.Systems.EventsSystem
{
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Warhead;
    using Exiled.API.Features;
    using PlayerRoles;
    using NGMainPlugin.API;
    using MEC;

    public static class EventsSystemHandler
    {
        //Event commands params
        private static bool NoRespawn = false;
        private static bool NoAutoNuke = false;
        private static bool NoNuke = false;
        public static EventsType eventRoundType;

        public static void Enable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += Spawning;
            Exiled.Events.Handlers.Warhead.Starting += WarheadActivating;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeaving;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.IssuingMute += OnMuted;
            Exiled.Events.Handlers.Player.RevokingMute += OnUnMuted;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
            Exiled.Events.Handlers.Player.Died += OnDied;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= Spawning;
            Exiled.Events.Handlers.Warhead.Starting -= WarheadActivating;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeaving;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.IssuingMute -= OnMuted;
            Exiled.Events.Handlers.Player.RevokingMute -= OnUnMuted;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            Exiled.Events.Handlers.Player.Died -= OnDied;
        }

        private static void Spawning(RespawningTeamEventArgs ev)
        {
            if (NoRespawn)
                ev.IsAllowed = false;
        }

        private static void WarheadActivating(StartingEventArgs ev)
        {
            if (NoNuke)
                ev.IsAllowed = false;
            if (NoAutoNuke && ev.IsAuto)
                ev.IsAllowed = false;
        }

        private static void OnPlayerLeaving(LeftEventArgs ev)
        {
            if (EventsAPI.EventRound)
            {
                if (!EventsAPI.MutedBeforeEvent.Contains(ev.Player))
                {
                    ev.Player.UnMute();
                }
            }
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            if (EventsAPI.EventRound && ev.Player.IsMuted)
            {
                if (!EventsAPI.MutedBeforeEvent.Contains(ev.Player))
                {
                    EventsAPI.MutedBeforeEvent.Add(ev.Player);
                }
            }
        }

        private static void OnMuted(IssuingMuteEventArgs ev)
        {
            if (EventsAPI.EventRound)
            {
                if (!EventsAPI.MutedBeforeEvent.Contains(ev.Player))
                {
                    EventsAPI.MutedBeforeEvent.Add(ev.Player);
                }
            }
        }

        private static void OnUnMuted(RevokingMuteEventArgs ev)
        {
            if (EventsAPI.EventRound)
            {
                if (EventsAPI.MutedBeforeEvent.Contains(ev.Player))
                {
                    EventsAPI.MutedBeforeEvent.Remove(ev.Player);
                }
            }
        }

        private static void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (EventsAPI.EventRound)
            {
                switch (eventRoundType)
                {
                    case EventsType.PeanutRun:
                        Timing.KillCoroutines();
                        break;
                    case EventsType.ParticleFight:
                        Server.FriendlyFire = false;
                        break;
                    default:
                        break;
                }
                
                Events.TempList.Clear();
                eventRoundType = EventsType.None;
                EventsAPI.EventRound = false;
                EventsAPI.MutedBeforeEvent.Clear();

                foreach (Player ply in Player.List)
                {
                    if (!EventsAPI.MutedBeforeEvent.Contains(ply))
                    {
                        ply.UnMute();
                    }
                }
            }
        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (EventsAPI.EventRound && eventRoundType == EventsType.Virus)
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.None);
            }
        }
    }
}
