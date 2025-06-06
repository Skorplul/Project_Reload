namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using Exiled.API.Features;
    using PRMainPlugin.Systems.Database;
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

            Player target = Player.Get(arguments.Array[2]);

            if (target == null)
            {
                response = "target ist null, bitte sofort mit vollem screenshot vom RA an @Skorp1.0 melden!";
                return false;
            }

            if (arguments.Array[1] == "add")
            {
                Database.AddWatchlist(target, issuer, string.Join(" ", arguments.Skip(2)));
                response = $"Warn wurde zu {target.Nickname} ({target.UserId}) hinzugefügt.";
                return true;
            }
            if (arguments.Array[1] == "remove")
            {
                //Database.AddWarn(target, issuer, string.Join(" ", arguments.Skip(2)));
                response = $"Das geht leider noch nicht, bitte fürs erste an @Skorp1.0 wenden.";
                return true;
            }

            response = "Benutzung: \nwatchlist <add/remove> <PlayerID> <Grund>";
            return false;
        }
    }
}
