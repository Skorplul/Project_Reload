using System.Reflection;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer.API.Features.ExternalTeams
{
    public class UiuTeam : ExternalTeamChecker
    {
        public override void Init(Assembly assembly)
        {
            this.PluginEnabled = true;
            this.Singleton = (object)null;
            this.FieldInfo = assembly.GetType("UIURescueSquad.EventHandlers").GetField("IsSpawnable");
        }
    }
}
