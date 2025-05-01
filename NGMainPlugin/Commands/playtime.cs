namespace PRMainPlugin.Systems.Database.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PRMainPlugin.Systems.Database;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DataInfoClient : ICommand
    {
        public string Command { get; } = "playtime";

        public string[] Aliases { get; } = new string[] { "pt" };

        public string Description { get; } = "See your playtime.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var data = Player.Get(sender).GetPlayerDataAsync().GetAwaiter().GetResult();

            if (data != null)
            {
                response = String.Format("<color=green>Your playtime: </color> {0}", $"{TimeSpan.FromSeconds(data.Playtime):hh\\:mm\\:ss}");
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

        public string Description { get; } = $"See other players' playtime.\\n\" +\r\n            \"No arguments = playtime of every player on the server.\\n\" +\r\n            \"<int> Id of a player on the server = playtime of a certain player currently on the server.\\n\" +\r\n            \"<string> ID or SteamID of a player = playtime of a certain player.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.Get(sender).CheckPermission("NG.PtManage"))
            {
                response = "You dont have a permission to use that command!";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = $"<color=green>Playtime of players on server: </color>\n";
                foreach (Player p in Player.List)
                {
                    PlayerData info = Database.GetPlayerInfoAsync(p.UserId).GetAwaiter().GetResult();
                    if (info == null)
                        continue;

                    response += $"<color=yellow>{p.Nickname}</color> - {TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}\n";
                }

                return true;
            }
            else if (arguments.Count == 1)
            {
                /*if (arguments.At(0) == "all")
                {
                    response = Database.DebugInfo();

                    return true;
                }*/

                Player p = Player.Get(arguments.At(0));
                PlayerData info;
                if (p == null)
                {
                    info = Database.GetPlayerInfoAsync(arguments.At(0)+"@steam").GetAwaiter().GetResult();

                    if (info == null)
                    {
                        response = $"<color=red>Wrong player ID/SteamID or Player's Data is not stored.</color>";
                        return false;
                    }

                    response = String.Format($"<color=green>Playtime of {0}:</color> {1}", arguments.At(0), $"{TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}");
                    return true;

                }

                info = p.GetPlayerDataAsync().GetAwaiter().GetResult();
                if (info == null)
                {
                    response = $"<color=red>Player's data is not saved due to them not agreeing to data collection.</color>";
                    return false;
                }

                response = String.Format($"<color=green>Playtime of {0}:</color> {1}", p.Nickname, $"{TimeSpan.FromSeconds(info.Playtime):hh\\:mm\\:ss}");
                return true;
            }

            response = "";
            return false;
        }
    }
}