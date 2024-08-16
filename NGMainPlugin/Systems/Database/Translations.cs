namespace NGMainPlugin.Systems.Database
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
        public string DBPTDescriptionClient { get; set; } = "See your playtime.";

        [Description(".pt remote admin command description")]
        public string DBPTDescriptionServer { get; set; } = "See other players' playtime.\n" +
            "No arguments = playtime of every player on the server.\n" +
            "<int> Id of a player on the server = playtime of a certain player currently on the server.\n" +
            "<string> ID or SteamID of a player = playtime of a certain player.";

        [Description("Client .pt command, data is not stored error")]
        public string DBDataNotStored { get; set; } = "<color=red>You have not agreed to the data collection, your playtime is not stored.</color>";

        [Description("Client .pt command, playtime")]
        public string DBClientPlaytime { get; set; } = "<color=green>Your playtime: </color> {0}";

        [Description("Server .pt command, players on server playtime")]
        public string DBServerEveryonePlaytime { get; set; } = "<color=green>Playtime of players on server: </color>\n";

        [Description("Server .pt command, playtime of one player")]
        public string DBServerPlayerPlaytime { get; set; } = "<color=green>Playtime of {0}:</color> {1}";

        [Description("Server .pt command, wrong ID error")]
        public string DBServerWrongID { get; set; } = "<color=red>Wrong player ID/SteamID or Player's Data is not stored.</color>";

        [Description("Server .pt command, data of player is not stored error")]
        public string DBServerDataNotStored { get; set; } = "<color=red>Player's data is not saved due to them not agreeing to data collection.</color>";
    }
}
