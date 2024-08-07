using CommandSystem;
using System;
using Exiled.API.Features;

namespace NGMainPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class MapThing : ParentCommand
    {
        public static Main Plugin { get; set; }

        public MapThing()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "map";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Show a map.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            Room CurrentRoom = player.CurrentRoom;


            response = "Command not reggistered! (pls ignore)";
            return false;
        }
    }
}
