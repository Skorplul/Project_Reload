namespace NGMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using System.Linq;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class NickManager : ICommand
    {
        public string Command => "nickm";

        public string[] Aliases => new string[] { "nm" };

        public string Description => "Manage the nicknames of users.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count() < 2)
            {
                response = "{nick <PlayerID> <NewNick>} to change the nickname from a user\nor {nick <PlayerID> remove} to remove the nickname from a user";
                return false;
            }
            if (player == null)
            {
                response = "Player is null, contact a developer!";
                return false;
            }
            if (!Permissions.CheckPermission(player, "NG.NickManagement"))
            {
                response = $"You don't have the permission <color=yellow>NG.NickManagement</color>";
                return false;
            }

            Player target = Player.Get(arguments.Array[1]);

            if (arguments.Array[2] == "remove")
            {
                target.DisplayNickname = null;
                response = $"Nick of user {target.Nickname} has been removed.";
                return true;
            }

            string NewName = string.Join("_", arguments.Skip(1));
            string OldName = target.DisplayNickname;
            target.DisplayNickname = NewName;

            response = $"Nick of user {target.Nickname} has been changed to {NewName}. (Before: {OldName})";
            return true;
        }
    }
}