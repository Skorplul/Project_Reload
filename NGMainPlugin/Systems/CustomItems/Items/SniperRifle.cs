namespace NGMainPlugin.Items;

using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using PlayerStatsSystem;
using YamlDotNet.Serialization;

[CustomItem(ItemType.GunE11SR)]
public class SniperRifle : CustomWeapon
{
    [YamlIgnore]
    public override uint Id { get; set; } = 10;

    public override string Name { get; set; } = "SR-119";

    public override string Description { get; set; } = "This modified E-11 Rifle fires high-velocity anti-personnel sniper rounds.";

    public override float Weight { get; set; } = 3.25f;

    public override byte ClipSize { get; set; } = 1;

    public override bool ShouldMessageOnGban { get; } = true;

    [YamlIgnore]
    public override float Damage { get; set; }

    public override SpawnProperties? SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = new List<DynamicSpawnPoint>
        {
            new()
            {
                Chance = 40,
                Location = SpawnLocationType.InsideHid,
            },
            new()
            {
                Chance = 40,
                Location = SpawnLocationType.InsideHczArmory,
            },
        },
    };

    [YamlIgnore]
    public override AttachmentName[] Attachments { get; set; } = new[]
    {
        AttachmentName.ExtendedBarrel,
        AttachmentName.ScopeSight,
    };

    [Description("The amount of extra damage this weapon does, as a multiplier.")]
    public float DamageMultiplier { get; set; } = 7.5f;

    protected override void OnHurting(HurtingEventArgs ev)
    {
        if (ev.Attacker != ev.Player && ev.DamageHandler.Base is FirearmDamageHandler firearmDamageHandler && firearmDamageHandler.WeaponType == ev.Attacker.CurrentItem.Type)
            ev.Amount *= DamageMultiplier;
    }
}