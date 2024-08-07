using System;
using System.Reflection;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer.API.Features.ExternalTeams
{
    public class SerpentsHandTeam : ExternalTeamChecker
    {
        public override void Init(Assembly assembly)
        {
            this.PluginEnabled = true;
            Type type = assembly.GetType("SerpentsHand.SerpentsHand");
            this.Singleton = type.GetField("Singleton").GetValue((object)null);
            this.FieldInfo = type.GetField("IsSpawnable");
        }
    }
}
