namespace PRMainPlugin.Systems.Database.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PRMainPlugin.Systems.Database;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Level : ICommand
    {
        public string Command { get; } = "level";

        public string[] Aliases { get; } = new string[] { "lvl" };

        public string Description { get; } = "Zeigt alle Level Statistiken an.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var data = Player.Get(sender).GetPlayerData();
            Player player = Player.Get(sender);

            if (data != null)
            {
                response = $"<color=white>Deine Stats: \nLevel: {data.Level}\nXp: {data.XP} / {data.RequiredXP}\nRang: <color={player.RankColor}>{player.RankName}\nRecons: {data.Recons}";
                return true;
            }

            response = "<color=red>Es Gab einen Fehler, bitte reporte das in unserem Discord!.</color>";
            return false;
        }
    }
}
