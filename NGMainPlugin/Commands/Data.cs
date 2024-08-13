using CommandSystem;
using System;
using Exiled.API.Features;
using NGMainPlugin.DB;

namespace NGMainPlugin.Systems.RespawnTimer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Data : ICommand
    {
        public string Command { get; } = "DataToggleTrack";

        public string[] Aliases { get; } = new string[] { "DTT" };

        public string Description { get; } = Main.Instance.Config.DBDTTDescription;

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player plr = Player.Get(sender);

            if (arguments.Count == 1 && (arguments.At(0) == "confirm" || arguments.At(0) == "force"))
            {
                Database.PlayerToggleTrack(plr);

                response = Main.Instance.Config.DBTrackingToggled;
                return true;
            }

            if (plr.CanStoreCustomData())
            {
                response = Main.Instance.Config.DBToggleWarning;
                return true;
            }

            Database.PlayerToggleTrack(plr);

            response = Main.Instance.Config.DBTrackingToggled;
            return true;
        }
    }
}