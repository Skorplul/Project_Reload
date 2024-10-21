namespace NGMainPlugin.Systems.Database.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using NGMainPlugin.Systems.Database;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Data : ICommand
    {
        public string Command { get; } = "DataToggleTrack";

        public string[] Aliases { get; } = new string[] { "DTT" };

        public string Description { get; } = Config.Translations.DBDTTDescription;

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player plr = Player.Get(sender);

            if (arguments.Count == 1 && (arguments.At(0) == "confirm" || arguments.At(0) == "force"))
            {
                Database.PlayerToggleTrack(plr);

                response = Config.Translations.DBTrackingToggled;
                return true;
            }

            if (plr.CanStoreCustomData())
            {
                response = Config.Translations.DBToggleWarning;
                return true;
            }

            Database.PlayerToggleTrack(plr);

            response = Config.Translations.DBTrackingToggled;
            return true;
        }
    }
}