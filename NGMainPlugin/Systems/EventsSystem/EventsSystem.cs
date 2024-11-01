namespace NGMainPlugin.Systems.EventsSystem
{
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Warhead;
    using System;

    internal static class EventsSystem
    {
        //Event commands params
        private static bool NoRespawn = false;
        private static bool NoAutoNuke = false;
        private static bool NoNuke = false;

        public static void Enable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += Spawning;
            Exiled.Events.Handlers.Warhead.Starting += WarheadActivating;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= Spawning;
            Exiled.Events.Handlers.Warhead.Starting -= WarheadActivating;
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
    }
}
