namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Stuck : ICommand
    {
        public string Command => "stuck";

        public string[] Aliases => new string[] { };

        public string Description => "If you are stuck, use this to notify a teammember!";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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

            foreach (Player ply in Player.List)
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