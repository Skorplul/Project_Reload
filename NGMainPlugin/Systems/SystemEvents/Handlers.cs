using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;

namespace NGMainPlugin.Systems.SystemEvents
{
    public class Handlers
    {
        //Event commands params
        public bool NoRespawn = false;
        public bool NoAutoNuke = false;
        public bool NoNuke = false;

        public void Spawning(RespawningTeamEventArgs ev)
        {
            if (NoRespawn)
                ev.IsAllowed = false;
        }

        public void WarheadActivating(StartingEventArgs ev)
        {
            if (NoNuke)
                ev.IsAllowed = false;
        }
    }
}
