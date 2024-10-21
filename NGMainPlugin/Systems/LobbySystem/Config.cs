namespace NGMainPlugin.Systems.LobbySystem
{
    using Exiled.API.Enums;
    using System.ComponentModel;
    using System.IO;
    using UnityEngine;
    using YamlDotNet.Serialization;

    internal class Config
    {
        [YamlIgnore]
        public static string ConfigPath = Path.Combine(NGMainPlguin.Instance.Config.ConfigFolderPath, "lobbySystem.yml");


        [Description("Time in lobby before round start.")]
        public double LobbyTime { get; set; } = 30.0;

        [Description("Minimum players for the countdown to start.")]
        public int MinimumPlayers { get; set; } = 2;

        [Description("Info text for the lobby.")]
        public string TextShown { get; set; } = "<size=27>│ %status% │ <b>SERVER NAME</b>  │ <color=red>%playercount%/%maxplayers%</color> Inmates Waiting │</size>";

        public string PausedStatus { get; set; } = "<color=red>\uD83D\uDFE5</color> Lobby Paused";

        public string WaitingStatus { get; set; } = "<color=yellow>\uD83D\uDFE8</color> Waiting for Players";

        public string StartingStatus { get; set; } = "<color=green>\uD83D\uDFE9</color> Starting in %countdown% Seconds";

        [Description("Room where the Lobby will be.")]
        public RoomType SpawnRoom { get; set; }

        [Description("Position of the spawn in lobby. Works only if SpawnRoom is Unknown.")]
        public Vector3 SpawnPosition { get; set; } = new Vector3(0.0f, 995.6f, -8f);
    }
}
