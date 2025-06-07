namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using Exiled.API.Features;
    using MongoDB.Bson;
    using PRMainPlugin.Systems.Database;
    using Exiled.Permissions.Extensions;
    using System;
    using System.Linq;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Watchlists : ICommand
    {
        public string Command => "watchlist";

        public string[] Aliases => new string[] { };

        public string Description => "Watchlist Einträge verwalten.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player issuer = Player.Get(sender);

            if (issuer == null)
            {
                response = "issuer ist null, bitte sofort mit vollem screenshot vom RA an @Skorp1.0 melden!";
                return false;
            }

            Player target;
            PlayerData data = null;

            if (arguments.Array[2].Contains("@steam"))
            {
                try
                {
                    data = Database.GetPlayerDataByIdAsync(arguments.Array[2]).Result;
                }
                catch
                {
                }
            }
            else if (data == null)
            {
                target = Player.Get(arguments.Array[2]);
                data = target.GetPlayerDataAsync().Result;
            }

            if (arguments.Array[1] == "add")
            {
                Database.AddWatchlist(data, issuer, string.Join(" ", arguments.Skip(2)));
                response = $"Wachlist wurde zu {data.Nickname} ({data.Id}) hinzugefügt.";
                return true;
            }
            if (arguments.Array[1] == "remove")
            {
                if (!Permissions.CheckPermission(issuer, "NG.RemoveMod"))
                {
                    response = $"Du hast nicht die Permission <color=yellow>NG.RemoveMod</color>";
                    return false;
                }

                Database.RemoveWatchlist(data, ObjectId.Parse(arguments.Array[3]));
                response = $"Wachlist Eintrag wurde von {data.Nickname} ({data.Id}) entfernt.";
                return true;
            }

            response = "Benutzung: \nwatchlist <add/remove> <PlayerID> <Grund/EintragsID>";
            return false;
        }
    }
}
