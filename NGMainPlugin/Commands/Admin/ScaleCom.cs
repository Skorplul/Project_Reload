namespace NGMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Scale : ICommand
    {
        public string Command => "scale";

        public string[] Aliases => new string[] {  };

        public string Description => "Edit Scale of a user.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player is NULL, contact a dev!";
                return false;
            }
            if (!player.CheckPermission("NG.Scale"))
            {  
                response = "No Permission.";
                return false;
            }
            if (arguments.Count < 3)
            {
                response = "Usage: scale <x> <y> <z> <User>(optional)";
                return false;
            }
            if (arguments.Count == 3)
            {
                try
                {
                    player.Scale.Set(float.Parse(arguments.Array[0]), float.Parse(arguments.Array[1]), float.Parse(arguments.Array[2]));
                    response = "Set own Scale to the specified ones.";
                }
                catch (Exception ex)
                {
                    response = "Failed to Parse arguments, please only use numbers as xyz input.";
                    Log.Error($"Failed To Set Scale {player.Nickname}\n{ex}");
                    return false;
                }
            }
            if (arguments.Count != 4)
            {
                response = "Usage: scale <x> <y> <z> <User>(optional)";
                return false;
            }

            try
            {
                player.Scale.Set(float.Parse(arguments.Array[0]), float.Parse(arguments.Array[1]), float.Parse(arguments.Array[2]));
                response = "Set own Scale to the specified ones.";
            }
            catch (Exception ex)
            {
                response = "Failed to Parse arguments, please only use numbers as xyz input. (Console for more Info.)";
                Log.Error($"Failed To Set Scale {player.Nickname}\n{ex}");
                return false;
            }
            response = "Set the scale for the user.";
            return true;
        }
    }
}