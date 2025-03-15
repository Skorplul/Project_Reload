using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Map;
using YamlDotNet.Serialization;

namespace PRMainPlugin.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class ImpactNade : CustomGrenade
    {
        [YamlIgnore]
        public override uint Id { get; set; } = 3;
 
        public override string Name { get; set; } = "Impact Grenade";

        public override string Description { get; set; } = "This grenade instantly explodes on impact with a surface!";

        public override float Weight { get; set; } = 1.5f;

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 30,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside914,
                }
            }
        };

        public override float FuseTime { get; set; } = 20f;

        public override bool ExplodeOnCollision { get; set; } = true;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            base.OnExploding(ev);
        }
    }
}