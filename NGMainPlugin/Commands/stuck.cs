using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;


namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class stuck : ParentCommand
    {
        public stuck() => LoadGeneratedCommands();

        public override string Command { get; } = "stuck";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "If you are stuck, use this to notify a teammember!";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!Round.IsStarted)
            {
                response = "You can‘t use this here!";
                return false; 
            }
            if (Round.IsEnded)
            {
                response = "You can‘t use this here!";
                return false;
            }

            foreach  (Player ply in Player.List)
            {
                if (ply.CheckPermission(PlayerPermissions.ForceclassSelf))
                {
                    ply.Broadcast(10, $"[STUCK]: A player is stuck and needs help! (Player: {player.Nickname})");
                }
            }

            response = "Online teammembers have been notified!";
            return true;
        }
    }
}