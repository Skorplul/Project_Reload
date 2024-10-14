namespace NGMainPlugin.Commands
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
            if (ServerEvents.EventRound)
            {
                response = "An event is already running!";
                return false;
            }


            ServerEvents.EventRound = true;

            response = "Event has been triggered.";
            return true;
        }
    }
}