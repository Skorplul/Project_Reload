using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace NGMainPlugin.Systems.Subclasses;

public static class Fettsack
{
    private static readonly List<InventorySystem.Items.Usables.Scp330.CandyKindID> candies = new List<InventorySystem.Items.Usables.Scp330.CandyKindID>
    {
        InventorySystem.Items.Usables.Scp330.CandyKindID.Blue,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Green,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Purple,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Red,
        InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow,
    };
    public static void SetRole(Player ply)
    {
        ply.MaxHealth = 150;
        ply.Stamina = ply.Stamina - 0.1f;
        ply.EnableEffect(Exiled.API.Enums.EffectType.Slowness);
        ply.TryAddCandy(candies.RandomItem());
        ply.Scale = new Vector3(1.2f,1,1.2f);
    }
}
