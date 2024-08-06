using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.DamageHandlers;
using Exiled.CustomItems;
using PlayerRoles;

namespace NGMainPlugin.Items
{
    /// <inheritdoc />
    [CustomItem(ItemType.Adrenaline)]
    public class Knife : CustomItem
    {
        private const int PlayerLayerMask = 1208246273;

        /// <inheritdoc/>
        public override uint Id { get; set; } = 6;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Stichwaffe";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Stab them! :D";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.1f;

        /// <inheritdoc/>
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

        /// <summary>
        /// Gets or sets how much damage is done when hit with a rock in melee.
        /// </summary>
        [Description("How much damage is done when hit with a rock in melee.")]
        public float HitDamage { get; set; } = 10f;

        /// <summary>
        /// Gets or sets a value indicating whether or not rocks will deal damage to friendly targets.
        /// </summary>
        [Description("Whether or not rocks will deal damage to friendly targets.")]
        public bool FriendlyFire { get; set; } = false;

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsing;
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsing;
            base.UnsubscribeEvents();
        }

        public readonly DamageHandlerBase.CassieAnnouncement d = new DamageHandlerBase.CassieAnnouncement("") // Probably remove?
        {
            Announcement = "",
            SubtitleParts = null,
        };

        /// <summary>
        /// Called befor the player uses the item
        /// </summary>
        // Not quiet shure if it only happens on the custom item or every item (To be tested!)
        protected void OnUsing(Exiled.Events.EventArgs.Player.UsingItemEventArgs ev)
        {
            if (this.Check(ev.Item))
            {
                Log.Debug("OnUsing in Knife.cs has been used for this item!");
                ev.IsAllowed = false;
                Timing.CallDelayed(1.25f, () =>
                {
                    foreach (Item item in ev.Player.Items)
                    {
                        if (Check(item))
                        {
                            ev.Player.CurrentItem = item;
                            break;
                        }
                    }

                    Vector3 forward = ev.Player.CameraTransform.forward;

                    if (!Physics.Linecast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.position + (forward * 1.5f), out RaycastHit hit, PlayerLayerMask))
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

                    if (!FriendlyFire)
                    {
                        if (ev.Player.Role == target.Role)
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

                    ev.Player.ShowHitMarker();
                    target.Hurt(ev.Player, HitDamage, DamageType.Bleeding, null, "You died to a knife!");
                });
            }

        }

    }
}