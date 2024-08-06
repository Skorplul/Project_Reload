using NGMainPlugin.Systems.RespawnTimer.API.Features.ExternalTeams;
using System.Collections.Generic;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer.API
{
    public static class API
    {
        internal static readonly ExternalTeamChecker SerpentsHandTeam = (ExternalTeamChecker)new RespawnTimer.API.Features.ExternalTeams.SerpentsHandTeam();
        internal static readonly ExternalTeamChecker UiuTeam = (ExternalTeamChecker)new RespawnTimer.API.Features.ExternalTeams.UiuTeam();

        public static List<string> TimerHidden { get; } = new List<string>();

        public static bool SerpentsHandSpawnable => RespawnTimer.API.API.SerpentsHandTeam.IsSpawnable;

        public static bool UiuSpawnable => RespawnTimer.API.API.UiuTeam.IsSpawnable;
    }
}
