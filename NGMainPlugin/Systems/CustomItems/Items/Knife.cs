using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.DamageHandlers;
using PlayerRoles;
using MEC;
using YamlDotNet.Serialization;

namespace NGMainPlugin.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class Knife : CustomItem
    {
        private const int PlayerLayerMask = ~(1 << 13);

        [YamlIgnore]
        public override uint Id { get; set; } = 6;

        public override string Name { get; set; } = "Stichwaffe";

        public override string Description { get; set; } = "Stab them! :D";

        public override float Weight { get; set; } = 0.1f;

        public override SpawnProperties? SpawnProperties { get; set; } = new SpawnProperties
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
                {
                    new DynamicSpawnPoint
                    {
                        Chance = 100,
                        Location = SpawnLocationType.InsideLocker,
                    },
                },
        };

        [Description("How much damage is done when hit with a rock in melee.")]
        public float HitDamage { get; set; } = 10f;

        [Description("Cooldown between each hit.")]
        public float Cooldown { get; set; } = 1f;


        private bool OnCooldown = false;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsing;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsing;
            base.UnsubscribeEvents();
        }

        protected void OnUsing(Exiled.Events.EventArgs.Player.UsingItemEventArgs ev)
        {
            if (Check(ev.Item))
            {
                Log.Debug("OnUsing in Knife.cs has been used for this item!");

                ev.IsAllowed = false;

                if (OnCooldown)
                    return;

                Vector3 forward = ev.Player.CameraTransform.forward;

                if (!Physics.Raycast(ev.Player.CameraTransform.position, forward, out RaycastHit hit, 1.5f, PlayerLayerMask))
                    return;

                Log.Debug($"{ev.Player.Nickname} linecast is true!");
                if (hit.collider == null)
                {
                    Log.Debug($"{ev.Player.Nickname} collider is null?");
                    return;
                }

                Player target = Player.Get(hit.collider.GetComponentInParent<ReferenceHub>());
                if (target == null)
                {
                    Log.Debug($"{ev.Player.Nickname} target null");
                    return;
                }

                if (target == ev.Player)
                    return;

                if (!Server.FriendlyFire)
                {
                    if (ev.Player.Role.Type == target.Role.Type)
                        return;
                    if (ev.Player.IsNTF && target.IsNTF)
                        return;
                    if (ev.Player.IsCHI && target.IsCHI)
                        return;
                    if (ev.Player.IsCHI && target.Role == RoleTypeId.ClassD)
                        return;
                    if (ev.Player.IsNTF && target.Role == RoleTypeId.Scientist)
                        return;
                }

                Log.Debug($"{ev.Player.Nickname} hit {target.Nickname}");

                OnCooldown = true;
                Timing.CallDelayed(Cooldown, () => OnCooldown = false);

                target.EnableEffect(EffectType.Blinded, .15f);

                ev.Player.ShowHitMarker();

                target.Hurt(HitDamage, DamageType.Bleeding);
            }

        }

    }
}