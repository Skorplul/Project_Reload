namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PRMainPlugin.Systems.Database;
    using LiteDB;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class FixDB : ICommand
    {
        public string Command => "fixdb";

        public string[] Aliases => new string[] { };

        public string Description => "test something (@Skorp 1.0 weiﬂ was)";

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
                foreach (PlayerData ply in Database.DB.GetCollection<PlayerData>("Players").Query().ToList())
                {
                    ply.RequiredXP = 230;
                    ply.Level = 1;
                    ply.Update();
                }

                response = "done!";
                return true;
            }
            response = "something went wrong!";
            return false;
        }
    }
}
