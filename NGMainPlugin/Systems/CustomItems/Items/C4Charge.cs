namespace PRMainPlugin.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;
using YamlDotNet.Serialization;

using PlayerEvent = Exiled.Events.Handlers.Player;

[CustomItem(ItemType.GrenadeHE)]
public class C4Charge : CustomGrenade
{
    public enum C4RemoveMethod
    {
        Remove,
        Detonate,
        Drop,
    }

    public static C4Charge Instance { get; private set; } = null!;

    public static Dictionary<Pickup, Player> PlacedCharges { get; } = new();

    [YamlIgnore]
    public override uint Id { get; set; } = 15;

    public override string Name { get; set; } = "C4-119";

    public override float Weight { get; set; } = 0.75f;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 5,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 10,
                Location = SpawnLocationType.InsideLczArmory,
            },

            new()
            {
                Chance = 25,
                Location = SpawnLocationType.InsideHczArmory,
            },

            new()
            {
                Chance = 50,
                Location = SpawnLocationType.InsideNukeArmory,
            },

            new()
            {
                Chance = 50,
                Location = SpawnLocationType.Inside049Armory,
            },
        },
    };

    public override string Description { get; set; } = "Explosive charge that can be remotly detonated.";

    [Description("Should C4 charge stick to walls / ceiling.")]
    public bool IsSticky { get; set; } = true;

    [Description("Defines how strongly C4 will be thrown")]
    public float ThrowMultiplier { get; set; } = 40f;

    [Description("Should C4 require a specific item to be detonated.")]
    public bool RequireDetonator { get; set; } = true;

    [Description("The Detonator Item that will be used to detonate C4 Charges")]
    public ItemType DetonatorItem { get; set; } = ItemType.Radio;

    [Description("What happens with C4 charges placed by player, when he dies/leaves the game. (Remove / Detonate / Drop)")]
    public C4RemoveMethod MethodOnDeath { get; set; } = C4RemoveMethod.Drop;

    [Description("Should shooting at C4 charges do something.")]
    public bool AllowShoot { get; set; } = true;

    [Description("What happens with C4 charges after they are shot. (Remove / Detonate / Drop)")]
    public C4RemoveMethod ShotMethod { get; set; } = C4RemoveMethod.Remove;

    [Description("Maximum distance between C4 Charge and player to detonate.")]
    public float MaxDistance { get; set; } = 100f;

    [Description("Time after which the C4 charge will automatically detonate.")]
    public override float FuseTime { get; set; } = 9999f;

    [YamlIgnore]
    public override bool ExplodeOnCollision { get; set; } = false;

    [YamlIgnore]
    public override ItemType Type { get; set; } = ItemType.GrenadeHE;

    public void C4Handler(Pickup? charge, C4RemoveMethod removeMethod = C4RemoveMethod.Detonate)
    {
        if (charge?.Position is null)
            return;
        switch (removeMethod)
        {
            case C4RemoveMethod.Remove:
                {
                    break;
                }

            case C4RemoveMethod.Detonate:
                {
                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(charge.Position);
                    break;
                }

            case C4RemoveMethod.Drop:
                {
                    TrySpawn(Id, charge.Position, out _);
                    break;
                }
        }

        PlacedCharges.Remove(charge);
        charge.Destroy();
    }

    protected override void SubscribeEvents()
    {
        Instance = this;

        PlayerEvent.Destroying += OnDestroying;
        PlayerEvent.Died += OnDied;
        PlayerEvent.Shooting += OnShooting;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        PlayerEvent.Destroying -= OnDestroying;
        PlayerEvent.Died -= OnDied;
        PlayerEvent.Shooting -= OnShooting;

        base.UnsubscribeEvents();
    }

    protected override void OnWaitingForPlayers()
    {
        PlacedCharges.Clear();

        base.OnWaitingForPlayers();
    }

    protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        if (!PlacedCharges.ContainsKey(ev.Projectile))
            PlacedCharges.Add(ev.Projectile, ev.Player);
        base.OnThrownProjectile(ev);
    }

    protected override void OnExploding(ExplodingGrenadeEventArgs ev)
    {
        PlacedCharges.Remove(Pickup.Get(ev.Projectile.Base));
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        foreach (var charge in PlacedCharges.ToList())
        {
            if (charge.Value == ev.Player)
            {
                C4Handler(charge.Key, C4RemoveMethod.Remove);
            }
        }
    }

    private void OnDied(DiedEventArgs ev)
    {
        foreach (var charge in PlacedCharges.ToList())
        {
            if (charge.Value == ev.Player)
            {
                C4Handler(charge.Key, MethodOnDeath);
            }
        }
    }

    private void OnShooting(ShootingEventArgs ev)
    {
        if (!AllowShoot)
            return;

        Vector3 forward = ev.Player.CameraTransform.forward;
        if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
        {
            EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
            if (grenade == null)
            {
                return;
            }

            if (PlacedCharges.ContainsKey(Pickup.Get(grenade)))
            {
                C4Handler(Pickup.Get(grenade), ShotMethod);
            }
        }
    }

    private void OnRoundEnded(RoundEndedEventArgs ev)
    {
        PlacedCharges.Clear();
    }
}