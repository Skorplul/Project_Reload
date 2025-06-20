using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Fettsack
{
    //private static readonly List<InventorySystem.Items.Usables.Scp330.CandyKindID> candies = new List<InventorySystem.Items.Usables.Scp330.CandyKindID>
    //{
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Blue,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Green,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Purple,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Red,
    //    InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow,
    //};

    public static void SetRole(Player ply)
    {
        ply.Role.Set(PlayerRoles.RoleTypeId.ClassD);
        ply.MaxHealth = 120;
        ply.Heal(ply.MaxHealth - ply.Health, false);
        ply.Stamina = ply.Stamina - 0.1f;
        ply.EnableEffect(Exiled.API.Enums.EffectType.Slowness, 7 ,9999, false);
        //ply.TryAddCandy(candies.RandomItem());
        ply.AddItem(ItemType.AntiSCP207);
        ply.Scale = new Vector3(1.2f,1,1.2f);
    }
}
