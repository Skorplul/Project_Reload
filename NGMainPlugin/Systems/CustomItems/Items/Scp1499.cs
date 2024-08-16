namespace NGMainPlugin.Items;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;

[CustomItem(ItemType.SCP268)]
public class Scp1499 : CustomItem
{
    private readonly Dictionary<Player, (Vector3, Lift)> scp1499Players = new();

    [YamlIgnore]
    public override uint Id { get; set; } = 8;

    public override string Name { get; set; } = "SCP-1499";

    public override string Description { get; set; } = "The gas mask that temporarily teleports you to another dimension, when you put it on.";

    public override float Weight { get; set; } = 1.5f;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 10,
                Location = SpawnLocationType.InsideHid,
            },
        },
    };

    [Description("How long the SCP-1499 can be wore, before automaticly player takes it off. (set to 0 for no limit)")]
    public float Duration { get; set; } = 15f;

    [Description("The location to teleport when using SCP-1499")]
    public Vector3 TeleportPosition { get; set; } = new(38.464f, 1014.112f, -32.689f);

    protected override void SubscribeEvents()
    {
        Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;
        Exiled.Events.Handlers.Player.Destroying += OnDestroying;
        Exiled.Events.Handlers.Player.Died += OnDied;

        base.SubscribeEvents();
    }

    protected override void UnsubscribeEvents()
    {
        Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;
        Exiled.Events.Handlers.Player.Destroying -= OnDestroying;
        Exiled.Events.Handlers.Player.Died -= OnDied;

        base.UnsubscribeEvents();
    }

    protected override void OnDropping(DroppingItemEventArgs ev)
    {
        if (scp1499Players.ContainsKey(ev.Player) && Check(ev.Item))
        {
            ev.IsAllowed = false;

            SendPlayerBack(ev.Player);
        }
        else
        {
            base.OnDropping(ev);
        }
    }

    protected override void OnWaitingForPlayers()
    {
        scp1499Players.Clear();

        base.OnWaitingForPlayers();
    }

    private void OnDied(DiedEventArgs ev)
    {
        if (scp1499Players.ContainsKey(ev.Player))
            scp1499Players.Remove(ev.Player);
    }

    private void OnDestroying(DestroyingEventArgs ev)
    {
        if (scp1499Players.ContainsKey(ev.Player))
            scp1499Players.Remove(ev.Player);
    }

    private void OnUsedItem(UsedItemEventArgs ev)
    {
        if (!Check(ev.Player.CurrentItem))
            return;

        (Vector3, Lift) info;
        if (ev.Player.Lift != null)
            info = (ev.Player.Position - ev.Player.Lift.Position, ev.Player.Lift);
        else 
            info = (ev.Player.Position, null);

        if (scp1499Players.ContainsKey(ev.Player))
            scp1499Players[ev.Player] = info;
        else
            scp1499Players.Add(ev.Player, info);

        ev.Player.Position = TeleportPosition;
        ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Invisible>();

        if (Duration > 0)
        {
            Timing.CallDelayed(Duration, () =>
            {
                SendPlayerBack(ev.Player);
            });
        }
    }

    private void SendPlayerBack(Player player)
    {
        if (!scp1499Players.ContainsKey(player))
            return;

        if (scp1499Players[player].Item2 != null)
            player.Position = scp1499Players[player].Item1 + scp1499Players[player].Item2.Position;
        else
            player.Position = scp1499Players[player].Item1;

        bool shouldKill = false;
        if (Warhead.IsDetonated)
        {
            if (player.CurrentRoom.Zone != ZoneType.Surface)
            {
                shouldKill = true;
            }
            else
            {
                if (Lift.List.Where(lift => lift.Name.Contains("Gate")).Any(lift => (player.Position - lift.Position).sqrMagnitude <= 10f))
                {
                    shouldKill = true;
                }
            }

            if (shouldKill)
                player.Hurt(new WarheadDamageHandler());
        }
        else if (Map.IsLczDecontaminated)
        {
            if (player.CurrentRoom.Zone == ZoneType.LightContainment)
            {
                shouldKill = true;
            }
            else
            {
                if (Lift.List.Where(lift => lift.Name.Contains("El")).Any(lift => (player.Position - lift.Position).sqrMagnitude <= 10f))
                {
                    shouldKill = true;
                }
            }

            if (shouldKill)
                player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Decontamination));
        }

        scp1499Players.Remove(player);
    }
}