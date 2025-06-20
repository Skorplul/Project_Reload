namespace PRMainPlugin.Systems.Database
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    public class Translations : ITranslation
    {
        [Description("Welcome to the server broadcast")]
        public string DBWelcome { get; set; } = "Welcome to the server! Please, check console!";

        [Description("Welcome to the server console message")]
        public string DBWelcomeConsole { get; set; } = "This server tracks some of custom data like your playtime. If you have DNT enabled, it is disabled by default. " +
                    "If you want to toggle custom data tracking On or Off, use .DataToggleTrack command. (or .DTT for short)";

        [Description(".DTT command description")]
        public string DBDTTDescription { get; set; } = "Toggle custom data tracking on and off.";

        [Description("Tracking toggled message")]
        public string DBTrackingToggled { get; set; } = "<color=green>You have toggled custom data tracking.</color>";

        [Description("Tracking toggle off warning message")]
        public string DBToggleWarning { get; set; } = "<color=red>WARNING.</color> <color=yellow>You are about to toggle custom data tracking off. Doing so will wipe all stored custom data. Confirm by using .DTT confirm." +
                    " (You can also skip this message by using .DTT force, if you feel like it ever again.)</color>";

        [Description(".pt client command description")]
        public string DBPTDescriptionClient { get; set; } = "";

        [Description(".pt remote admin command description")]
        public string DBPTDescriptionServer { get; set; } = "";

        [Description("Client .pt command, data is not stored error")]
        public string DBDataNotStored { get; set; } = "";

        [Description("Client .pt command, playtime")]
        public string DBClientPlaytime { get; set; } = "";

        [Description("Server .pt command, players on server playtime")]
        public string DBServerEveryonePlaytime { get; set; } = "";

        [Description("Server .pt command, playtime of one player")]
        public string DBServerPlayerPlaytime { get; set; } = "";

        [Description("Server .pt command, wrong ID error")]
        public string DBServerWrongID { get; set; } = "";

        [Description("Server .pt command, data of player is not stored error")]
        public string DBServerDataNotStored { get; set; } = "";
    }
}