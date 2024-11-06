namespace NGMainPlugin.Systems.EventsSystem.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using NGMainPlugin.API;

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
            EventsType eventsT;

            static EventsType? GetEventType(string Event, out EventsType eventsT)
            {
                eventsT = EventsType.None;
                switch (Event)
                {
                    case "Virus":
                        return EventsType.Virus;
                    case "PeanutRun":
                        return EventsType.PeanutRun;
                    case "LightsOut":
                        return EventsType.LightsOut;
                    case "JailbirdFight":
                        return EventsType.CockFight;
                    default:
                        return null;
                }
            }

            GetEventType(arguments.Array[2], out eventsT);

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
                case EventsType.CockFight:
                    Events.CockFight();
                    break;
                default:
                    break;
            }

            response = "Event has been triggered.";
            return true;
        }
    }
}