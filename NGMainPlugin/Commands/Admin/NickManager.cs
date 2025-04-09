namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using System.Linq;
    using Exiled.Permissions.Extensions;
    using PRMainPlugin.Systems.Database;
    using UnityEngine.PlayerLoop;

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
                response = "{nickm <PlayerID> <NewNick>} to change the nickname from a user\nor {nickm <PlayerID> remove} to remove the nickname from a user\nor {nickm <PlayerID> remove <reson>} to remove the nickname and tell them the reason\n{<PlayerID> <true/false> to prohibid nickname changes}";
                return false;
            }
            if (player == null)
            {
                response = "Player is null, contact a developer!";
                return false;
            }
            if (!Permissions.CheckPermission(player, "NG.NickManage"))
            {
                response = $"You don't have the permission <color=yellow>NG.NickManage</color>";
                return false;
            }

            Player target = Player.Get(arguments.Array[1]);
            if (target == null)
            {
                response = $"This player does not exist!";
                return false;
            }
            if (arguments.Array[2] == "true")
            {
                Database.UpdateAllowNickChange(target, true);
                response = "Done!";
                return true;
            }
            else if (arguments.Array[2] == "false")
            {
                Database.UpdateAllowNickChange(target, false);
                response = "Done!";
                return true;
            }

            string OldName = target.DisplayNickname;

            if (arguments.Array[2] == "remove")
            {
                if (arguments.Count() >= 4)
                {
                    string reason = string.Join(" ", arguments.Skip(2));
                    target.Broadcast(10, $"Your nickname has been removed by an Admin! \nReason:<color=yellow> {reason} </color>");
                    target.DisplayNickname = null;
                    response = $"Nick of user {target.Nickname} has been removed. (Before: {OldName})";
                    return true;
                }
                else
                {
                    target.DisplayNickname = null;
                    response = $"Nick of user {target.Nickname} has been removed. (Before: {OldName})";
                    return true;
                }
            }

            string NewName = string.Join("_", arguments.Skip(1));
            target.DisplayNickname = NewName;

            response = $"Nick of user {target.Nickname} has been changed to {NewName}. (Before: {OldName})";
            return true;
        }
    }
}