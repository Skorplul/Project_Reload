namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PRMainPlugin.Systems.Database;
    using System.Linq;
    using Exiled.Permissions.Extensions;
    using MapEditorReborn.API.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class XpAndRecons : ICommand
    {
        public string Command => "xp-recons";

        public string[] Aliases => new string[] { "xur" };

        public string Description => "XP und Recons kontrolle.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player Null! Contact a Developer!";
                return false;
            }
            if (!player.CheckPermission("NG.DB"))
            {
                response = $"You dont have permission!";
                return false;
            }
            if (arguments.Array.Count() < 3)
            {
                response = "{xur <player> <xp> <recons>}";
                return false;
            }

            float xp;
            arguments.Array[2].TryParseToFloat(out xp);

            float recons;
            arguments.Array[3].TryParseToFloat(out recons);

            Player target = Player.Get(arguments.Array[1]);

            if (target == null)
            {
                response = "Target is null, contact a Dev";
                return false;
            }

            if (!Database.UpdateReconsAndXP(target, (int)recons, (int)xp))
            {
                response = "something went wrong!";
                return false;
            }

            response = "Done!";
            return true;
        }
    }
}
