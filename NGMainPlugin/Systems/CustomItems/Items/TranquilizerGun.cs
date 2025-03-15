namespace PRMainPlugin.Items;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Player;

using MEC;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;
using Ragdoll = Exiled.API.Features.Ragdoll;
using Random = UnityEngine.Random;

[CustomItem(ItemType.GunCOM18)]
public class TranquilizerGun : CustomWeapon
{
    private readonly Dictionary<Player, float> tranquilizedPlayers = new();
    private readonly List<Player> activeTranqs = new();

    [YamlIgnore]
    public override uint Id { get; set; } = 11;

    public override string Name { get; set; } = "TG-119";

    public override string Description { get; set; } = "This modifier USP fires non-lethal tranquilizing darts. Those affected will be rendered unconscious for a short duration. Unreliable against SCPs. Repeated tranquilizing of the same person will render them resistant to it's effect.";

    public override float Weight { get; set; } = 1.55f;

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 30,
                Location = SpawnLocationType.InsideGr18,
            },
            new()
            {
                Chance = 70,
                Location = SpawnLocationType.Inside173Armory,
            },
        },
    };

    public override byte ClipSize { get; set; } = 2;

    public override float Damage { get; set; }
   
    [Description("Whether or not SCPs should be resistant to tranquilizers. (Being resistant gives them a chance to not be tranquilized when shot).")]
    public bool ResistantScps { get; set; } = true;

    [Description("The amount of time a successful tranquilization lasts for.")]
    public float Duration { get; set; } = 5f;

    [Description("Everytime a player is tranquilized, they gain a resistance to further tranquilizations, reducing the duration of future effects. This number signifies the exponential modifier used to determine how much time is removed from the effect.")]
    public float ResistanceModifier { get; set; } = 1.2f;

    [Description("How often the plugin should reduce the resistance amount for players, in seconds.")]
    public float ResistanceFalloffDelay { get; set; } = 120f;

    [Description("Whether or not tranquilized targets should drop all of their items.")]
    public bool DropItems { get; set; } = true;

    [Description("The percent chance an SCP will resist being tranquilized. This has no effect if ResistantScps is false.")]
    public int ScpResistChance { get; set; } = 40;

    protected override void UnsubscribeEvents()
    {
        Exiled.Events.Handlers.Player.PickingUpItem -= OnDeniableEvent;
        Exiled.Events.Handlers.Player.ChangingItem -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp049.StartingRecall -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp106.Teleporting -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.Charging -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.Enraging -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.AddingTarget -= OnDeniableEvent;
        Exiled.Events.Handlers.Scp939.PlacingAmnesticCloud -= OnDeniableEvent;
        Exiled.Events.Handlers.Player.VoiceChatting -= OnDeniableEvent;;
        activeTranqs.Clear();
        tranquilizedPlayers.Clear();
        Timing.KillCoroutines($"{nameof(TranquilizerGun)}-{Id}-reducer");
        base.UnsubscribeEvents();
    }

    protected override void SubscribeEvents()
    {
        Timing.RunCoroutine(ReduceResistances(), $"{nameof(TranquilizerGun)}-{Id}-reducer");
        Exiled.Events.Handlers.Player.PickingUpItem += OnDeniableEvent;
        Exiled.Events.Handlers.Player.ChangingItem += OnDeniableEvent;
        Exiled.Events.Handlers.Scp049.StartingRecall += OnDeniableEvent;
        Exiled.Events.Handlers.Scp106.Teleporting += OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.Charging += OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.Enraging += OnDeniableEvent;
        Exiled.Events.Handlers.Scp096.AddingTarget += OnDeniableEvent;
        Exiled.Events.Handlers.Scp939.PlacingAmnesticCloud += OnDeniableEvent;
        Exiled.Events.Handlers.Player.VoiceChatting += OnDeniableEvent;
        Exiled.Events.Handlers.Player.PickingUpItem += OnDeniableEvent;
        base.SubscribeEvents();
    }

    protected override void OnHurting(HurtingEventArgs ev)
    {
        base.OnHurting(ev);

        if (ev.Attacker == ev.Player)
            return;        

        if (ev.Player.Role.Team == Team.SCPs)
        {
            int r = Random.Range(1, 101);
            Log.Debug($"{Name}: SCP roll: {r} (must be greater than {ScpResistChance})");
            if (r <= ScpResistChance)
            {
                Log.Debug($"{Name}: {r} is too low, no tranq.");
                return;
            }
        }

        float duration = Duration;

        if (!tranquilizedPlayers.TryGetValue(ev.Player, out _))
            tranquilizedPlayers.Add(ev.Player, 1);

        tranquilizedPlayers[ev.Player] *= ResistanceModifier;
        Log.Debug($"{Name}: Resistance Duration Mod: {tranquilizedPlayers[ev.Player]}");

        duration -= tranquilizedPlayers[ev.Player];
        Log.Debug($"{Name}: Duration: {duration}");

        if (duration > 0f)
            Timing.RunCoroutine(DoTranquilize(ev.Player, duration));
    }

    private IEnumerator<float> DoTranquilize(Player player, float duration)
    {
        activeTranqs.Add(player);
        Vector3 oldPosition = player.Position;
        Item previousItem = player.CurrentItem;
        Vector3 previousScale = player.Scale;
        float newHealth = player.Health - Damage;
        List<StatusEffectBase> activeEffects = ListPool<StatusEffectBase>.Pool.Get();
        player.CurrentItem = null;

        if (newHealth <= 0)
            yield break;

        activeEffects.AddRange(player.ActiveEffects.Where(effect => effect.IsEnabled));

        try
        {
            if (DropItems)
            {
                if (player.Items.Count < 0)
                {
                    foreach (Item item in player.Items.ToList())
                    {
                        if (TryGet(item, out CustomItem? customItem))
                        {
                            customItem?.Spawn(player.Position, item, player);
                            player.RemoveItem(item);
                        }
                    }

                    player.DropItems();
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(DoTranquilize)}: {e}");
        }

        Ragdoll? ragdoll = null;
        if (player.Role.Type != RoleTypeId.Scp106)
            ragdoll = Ragdoll.CreateAndSpawn(player.Role, player.DisplayNickname, "Tranquilized", player.Position, player.ReferenceHub.PlayerCameraReference.rotation, player);

        if (player.Role is Scp096Role scp)
            scp.RageManager.ServerEndEnrage();

        try
        {
            player.EnableEffect<Invisible>(duration);
            player.Scale = Vector3.one * 0.2f;
            player.Health = newHealth;
            player.IsGodModeEnabled = true;
            player.EnableEffect(EffectType.Ensnared);

            player.EnableEffect<AmnesiaVision>(duration);
            player.EnableEffect<AmnesiaItems>(duration);
            player.EnableEffect<Ensnared>(duration);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        yield return Timing.WaitForSeconds(duration);

        try
        {
            if (ragdoll != null)
                NetworkServer.Destroy(ragdoll.GameObject);

            if (player.GameObject == null)
                yield break;

            newHealth = player.Health;

            player.DisableEffect(EffectType.Ensnared);
            player.IsGodModeEnabled = false;
            player.Scale = previousScale;
            player.Health = newHealth;

            if (!DropItems)
                player.CurrentItem = previousItem;

            foreach (StatusEffectBase effect in activeEffects.Where(effect => (effect.Duration - duration) > 0))
                player.EnableEffect(effect, effect.Duration);

            activeTranqs.Remove(player);
            ListPool<StatusEffectBase>.Pool.Return(activeEffects);
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(DoTranquilize)}: {e}");
        }

        if (Warhead.IsDetonated && player.Position.y < 900)
        {
            player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Warhead));
            yield break;
        }

        player.Position = oldPosition;
    }

    private IEnumerator<float> ReduceResistances()
    {
        for (; ;)
        {
            foreach (Player player in tranquilizedPlayers.Keys)
                tranquilizedPlayers[player] = Mathf.Max(0, tranquilizedPlayers[player] / 2);

            yield return Timing.WaitForSeconds(ResistanceFalloffDelay);
        }
    }

    private void OnDeniableEvent(IDeniableEvent ev)
    {
        if (ev is IPlayerEvent eP)
        {
            if (activeTranqs.Contains(eP.Player))
                ev.IsAllowed = false;
        }
    }

    protected override void OnReloading(ReloadingWeaponEventArgs ev)
    {
        ev.IsAllowed = false;
        base.OnReloading(ev);
    }
}