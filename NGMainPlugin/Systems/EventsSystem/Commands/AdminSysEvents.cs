namespace PRMainPlugin.Systems.EventsSystem.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PRMainPlugin.API;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AutoEvent : ICommand
    {
        public string Command => "AutoEvent";

        public string[] Aliases => new string[] { };

        public string Description => "Manualy Trigger a Server Event.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player is null, please contact a Dev!";
                return false;
            }
            if (!player.CheckPermission("NG.Events"))
            {
                response = "You don't have the permission for that!";
                return false;
            }
            //due to two idiots
            response = "Piss dich entweder beta oder sirius";
            return false;
            if (arguments.Count < 1)
            {
                response = "Usage: AutoEvent <Event>\nFor a list of all possible events: AutoEvent list";
                return false;
            }
            if (arguments.Array[1] == "list")
            {
                response = "Possible events: Virus, PeanutRun, LightsOut and ParticleFight\nThe events have to be spelled like they are here in the list!";
                return true;
            }
            if (Round.IsStarted)
            {
                response = "You need to use this in the Lobby!";
                return false;
            }
            if (EventsAPI.EventRound)
            {
                response = "An event is already running!";
                return false;
            }

            static EventsType GetEventType(string Event)
            {
                switch (Event)
                {
                    case "Virus":
                        return EventsType.Virus;
                    case "PeanutRun":
                        return EventsType.PeanutRun;
                    case "LightsOut":
                        return EventsType.LightsOut;
                    case "ParticleFight":
                        return EventsType.ParticleFight;
                    default:
                        return EventsType.None;
                }
            }

            EventsType eventsT = GetEventType(arguments.Array[1]);

            if (eventsT == EventsType.None)
            {
                response = "Not a valid event! <AutoEvent list> for all events.";
                return false;
            }

            // I know this could be in GetEventType(), but I just forgor at the time
            // and I am too layze to fix it :P
            switch (eventsT)
            {
                case EventsType.Virus:
                    Events.Virus();
                    break;
                case EventsType.PeanutRun:
                    Events.PeanutRun();
                    break;
                case EventsType.LightsOut:
                    Events.LightsOut();
                    break;
                case EventsType.ParticleFight:
                    Events.ParticleFight();
                    break;
                default:
                    Log.Warn("[EventSystem] Error while selecting the correct event.");
                    response = "Something went wrong. Contact a Developer!";
                    return false;
            }

            response = "Event has been triggered.";
            return true;
        }
    }
}