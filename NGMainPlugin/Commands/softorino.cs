using CommandSystem;
using System;
using Exiled.API.Features;


namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Softorino : ParentCommand
    {
        public Softorino() => LoadGeneratedCommands();

        public override string Command { get; } = "softorino";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Test something :^)";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player.Nickname == "Skorp 1.0")
            {
                ServerConsole.EnterCommand("softrestart");
                response = "done";
                return true;
            }
            response = "nope";
            return false;
        }
    }
}