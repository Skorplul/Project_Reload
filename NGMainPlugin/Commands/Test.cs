using CommandSystem;
using System;
using Exiled.API.Features;


namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Test : ParentCommand
    {
        public Test() => LoadGeneratedCommands();

        public override string Command { get; } = "test";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Test something :^)";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            
            

            response = "No permission";
            return false;
        }
    }
}