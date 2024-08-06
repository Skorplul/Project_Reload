using CommandSystem;
using System;
using Exiled.API.Features;
using PlayerRoles;
using System.Linq;
using System.Collections.Generic;

namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Durchsage : ParentCommand
    {
        public static List<string> spoke = new List<string>();

        public static Main Plugin { get; set; }

        public Durchsage()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "durchsage";

        public override string[] Aliases { get; } = new string[] { "durch" };

        public override string Description { get; } = "Make an announcement as 079! (Keep in mind, CASSI can only speak so many words!)";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count() < 1)
            {
                response = "Usage: .durchsage <Your Message Here>";
                return false;
            }
            if (player == null)
            {
                response = "Player is null, contact an admin/developer!";
                return false;
            }
            if (player.Role != RoleTypeId.Scp079)
            {
                response = "You need to be SCP079 to use this!";
                return false;
            }
            if (EventHandlers.PcCurentLvl < 3)
            {
                response = "You need a higher access Tier! (3)";
                return false;
            }
            if (Plugin.Config.Single079Cassi)
            {
                if (spoke.Contains(player.UserId))
                {
                    response = "You already used CASSI this round!";
                    return false;
                }
            }

            spoke.Add(player.UserId);
            string msg = string.Join(" ", arguments.Skip(0));
            Cassie.Message("MESSAGE FROM SCP 0 7 9 . . " + msg, false, true, true);

            response = "Your message has been sent!";
            return true;
        }
    }
}