using CommandSystem;
using System;
using Exiled.API.Features;
using NGMainPlugin.DB;
using Exiled.Permissions.Extensions;

namespace NGMainPlugin.Systems.RespawnTimer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DataInfoClient : ICommand
    {
        public string Command { get; } = "pt";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "See your playtime.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var info = Database.GetPlayerInfo(Player.Get(sender));

            if (info != null)
            {
                response = $"<color=green>Your playtime: </color> {TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}";
                return true;
            }

            response = "<color=red>You have not agreed to the data collection, your playtime is not stored.</color>";
            return false;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DataInfoServer : ICommand
    {
        public string Command { get; } = "pt";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "See other players' playtime.\n" +
            "No arguments = playtime of every player on the server.\n" +
            "<int> Id of a player on the server = playtime of a certain player currently on the server.\n" +
            "<string> ID or SteamID of a player = playtime of a certain player.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.Get(sender).CheckPermission("NG.pt"))
            {
                response = "You dont have a permission to use that command!";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "<color=green>Playtime of players on server: </color>\n";
                foreach (Player p in Player.List)
                {
                    PlayerInfo info = Database.GetPlayerInfo(Player.Get(sender));
                    if (info == null)
                        continue;

                    response += $"<color=yellow>{p.Nickname}</color> - {TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}\n";
                }

                return true;
            }
            else if (arguments.Count == 1)
            {
                Player p = Player.Get(arguments.At(0));
                PlayerInfo info;
                if (p == null)
                {
                    info = Database.GetPlayerInfo(arguments.At(0)+"@steam");

                    if (info == null)
                    {
                        response = "<color=red>Wrong player ID/SteamID or Player's Data is not stored.</color>";
                        return false;
                    }

                    response = $"<color=green>Playtime of {arguments.At(0)}:</color> {TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}";
                    return true;

                }

                info = Database.GetPlayerInfo(Player.Get(sender));
                if (info == null)
                {
                    response = "<color=red>Player's data is not saved due to them not agreeing to data collection.</color>";
                    return false;
                }

                response = $"<color=green>Playtime of {p.Nickname}: {TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}";
                return true;
            }

            response = "";
            return false;
        }
    }
}