using System.Reflection;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer.API.Features.ExternalTeams
{
    public abstract class ExternalTeamChecker
    {
        public abstract void Init(Assembly assembly);

        public bool IsSpawnable => this.PluginEnabled && (bool)this.FieldInfo.GetValue(this.Singleton);

        protected bool PluginEnabled { get; set; }

        protected FieldInfo FieldInfo { get; set; }

        protected object Singleton { get; set; }
    }
}
