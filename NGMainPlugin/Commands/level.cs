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
            Player player = Player.Get(sender);
            var data = player.GetPlayerData();

            if (data != null)
            {
                response = $"\n<color=white>[</color><color=#1504B3>P</color><color=#1A09B7>r</color><color=#1F0EBB>o</color><color=#2413BF>j</color><color=#2918C3>e</color><color=#2E1DC7>c</color><color=#3322CB>t</color><color=#3827CF>-</color><color=#3D2CD3>R</color><color=#4231D7>e</color><color=#4736DB>l</color><color=#4C3BDF>o</color><color=#5140E3>a</color><color=#5645E7>d</color><color=white>]</color>\n<color=white>Deine Stats: </color>\n<color=white>Level: {data.Level}</color>\n<color=white>Xp: {data.XP} / {data.RequiredXP}</color>\n<color=white>Rang: </color><color={player.RankColor}>{data.Rank}</color>\n<color=white>Recons: {data.Recons}</color>";
                return true;
            }

            response = "<color=red>Es Gab einen Fehler, bitte reporte das in unserem Discord!.</color>";
            return false;
        }
    }
}
    