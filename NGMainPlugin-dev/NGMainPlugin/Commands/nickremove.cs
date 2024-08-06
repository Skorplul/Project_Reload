using CommandSystem;
using System;
using Exiled.API.Features;
using System.Collections.Generic;

namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class NickRemove : ParentCommand
    {
        public static Main Plugin { get; set; }

        public NickRemove()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "nickremove";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Remove your nickname.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player is null, contact an admin/developer!";
                return false;
            }

            player.DisplayNickname = null;

            response = "Your nick has been removed!";
            return true;
        }
    }
}