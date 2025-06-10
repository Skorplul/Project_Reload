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
    public class Warns : ICommand
    {
        public string Command => "pwarn";

        public string[] Aliases => new string[] { };

        public string Description => "Warns verwalten.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player issuer = Player.Get(sender);

            if (issuer == null)
            {
                response = "issuer ist null, bitte sofort mit vollem screenshot vom RA an @Skorp1.0 melden!";
                return false;
            }

            string targetID;
            PlayerData data;

            if (arguments.Array[2].Contains("@steam"))
            {
                targetID = arguments.Array[2];
            }
            else
            {
                targetID = Player.Get(arguments.Array[2]).UserId;
            }

            try
            {
                data = PlayerDataCache.Get(targetID);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    response = "Diese ID gibt es nicht, bitte überprüfe nochmal die Eingabe.";
                    return false;
                }

                response = "Es gab einen Allgemeinen Fehler, bitte versuche es später noch einmal.";
                return false;
            }

            if (arguments.Array[1] == "add")
            {
                Database.AddWarn(data, issuer, string.Join(" ", arguments.Skip(2)));
                response = $"Warn wurde zu {data.Nickname} ({data.Id}) hinzugefügt.";
                return true;
            }
            if (arguments.Array[1] == "remove")
            {
                if (!Permissions.CheckPermission(issuer, "NG.RemoveMod"))
                {
                    response = $"Du hast nicht die Permission <color=yellow>NG.RemoveMod</color>";
                    return false;
                }

                if (!Database.RemoveWarn(data, ObjectId.Parse(arguments.Array[3])))
                {
                    response = "Diesen Eintrag gibt es nicht! Bitte überprüfe nocheinmal genau die IDs.";
                    return false;
                }

                response = $"Warn wurde von {data.Nickname} ({data.Id}) entfernt.";
                return true;
            }
            if (arguments.Array[1] == "list")
            {
                if (data.Warns.IsEmpty())
                {
                    response = $"{data.Nickname} hat keine Warns.";
                    return true;
                }
                response = $"<color=red>Warns</color>\n";
                foreach (Warn w in data.Warns)
                {
                    response += $"Grund: {w.Reason.ToString()}\nID: <color=orange>{w.Id.ToString()}</color>\n\n";
                }
                return true;
            }

            response = "Benutzung: \npwarn <add/remove/list> <PlayerID> <Grund/EintragsID>";
            return false;
        }
    }
}
