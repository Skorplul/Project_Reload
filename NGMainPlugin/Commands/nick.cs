namespace PRMainPlugin.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using System.Linq;
    using PRMainPlugin.Systems.Database;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Nick : ICommand
    {
        public string Command => "nick";

        public string[] Aliases => new string[] { };

        public string Description => "Set yourself a custom nickname.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count() < 1)
            {
                response = "Usage: .nick <Your New Nickname Here> (<.nick remove> zum entfernen)";
                return false;
            }
            if (player == null)
            {
                response = "Player is null, contact an admin/developer!";
                return false;
            }
            if (arguments.Array[1] == "remove")
            {
                player.CustomName = null;
                Database.UpdateCustomNick(player);

                response = "Your nick has been removed!";
                return true;
            }
            if (!player.GetPlayerDataAsync().Result.NicknameChangable)
            {
                response = "Du darfst deinen Name nicht ändern. Solltest du nicht wissen warum, frag bitte ein Teammitglied.";
                return false;
            }
            
            string NewName = string.Join("_", arguments.Skip(0));
            player.CustomName = NewName;
            Database.UpdateCustomNick(player);

            response = "Your new nick has been set!";
            return true;
        }
    }
}