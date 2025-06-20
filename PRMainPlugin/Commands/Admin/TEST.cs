namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PRMainPlugin.API;
    using System.Linq;
    using CedMod.Commands;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        public string Command => "test";

        public string[] Aliases => new string[] { };

        public string Description => "test something (@Skorp 1.0 weiß was)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player Null! Contact a Developer!";
                return false;
            }
            if (player.UserId != "76561199114184249@steam")
            {
                response = $"You dont have permission!";
                return false;
            }
            if (arguments.Count >= 0)
            {
                foreach (Player ply in Player.List)
                {
                    ply.UnMute();
                }

                response = "done!";
                return true;
            }
            response = "something went wrong!";
            return false;
        }
    }
}
