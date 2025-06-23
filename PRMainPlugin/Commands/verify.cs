namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PRMainPlugin.API;
    using System.Linq;
    using CedMod.Commands;
    using PRMainPlugin.Systems.Database;
    using AudioSystem.Patches;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class verify : ICommand
    {
        public string Command => "verify";

        public string[] Aliases => new string[] { };

        public string Description => "Verbinde deinen Discord account mit Server.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player Null! Contact a Developer!";
                return false;
            }
            if (string.IsNullOrEmpty(arguments.Array[1]))
            {
                response = "Bitte gib nach dem Command deinen Verifizierungs Token ein. {.verify <Token>}";
                return false;
            }

            var token = arguments.Array[1];
            var data = PlayerDataCache.Get(player.UserId);

            if (!string.IsNullOrEmpty(data.VerificationToken) && string.IsNullOrEmpty(data.DiscordId) && !string.IsNullOrEmpty(data.VerifyingID))
            {
                data.DiscordId = data.VerifyingID;
                response = "Du wurdest erfolgreich verifiziert!";
                return true;
            }

            response = "Du bist entweder schon verifiziert oder du hast keinen Tocken hinterlegt. Solltest du noch nicht verifiziert sein, bitte warter eine zwei Runden und versuche es dann nocheinmal.";
            return false;
        }
    }
}
