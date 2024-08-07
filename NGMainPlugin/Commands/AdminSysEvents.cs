using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NGMainPlugin.API;


namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AdminSysEvents : ParentCommand
    {
        public AdminSysEvents() => LoadGeneratedCommands();

        public override string Command { get; } = "AutoEvent";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Manualy Trigger a Server Event.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if(player == null)
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