namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PRMainPlugin.Systems.Database;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class NickRemove : ICommand
    {
        public string Command => "nickremove";

        public string[] Aliases => new string[] { };

        public string Description => "Remove your custom nickname.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player is null, contact an admin/developer!";
                return false;
            }

            player.CustomName = null;
            Database.UpdateCustomNick(player);

            response = "Your nick has been removed!";
            return true;
        }
    }
}