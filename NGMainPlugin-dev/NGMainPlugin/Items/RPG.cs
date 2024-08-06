using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NGMainPlugin.Items
{
    [CustomItem(ItemType.GunLogicer)]
    public class RPG : CustomWeapon
    {
        public override uint Id { get; set; } = 4;

        public override string Name { get; set; } = "RPG";

        public override string Description { get; set; } = "This weapon launches Rockets at your enemie!";

        public override float Weight { get; set; } = 2.4f;

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 35,
                    Location = SpawnLocationType.Inside079Secondary,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideHczArmory,
                }
            },
        };

        public override float Damage { get; set; } = 0f;

        public override byte ClipSize { get; set; } = 1;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            ev.CanHurt = false;

            Vector3 forward = ev.Player.Position + ev.Player.Transform.forward * 2f;



            base.OnShot(ev);
        }
    }
}
