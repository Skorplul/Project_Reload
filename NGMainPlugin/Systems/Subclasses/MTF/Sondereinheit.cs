using Exiled.API.Features;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Sondereinheit
{
    public static void SetRole(Player ply)
    {  
        if (Subclasses.WaveCount <= 2)
            return;
        ply.ClearInventory();
        ply.Scale = new Vector3(1f, 1f, 1f);
        ply.AddItem(ItemType.ParticleDisruptor);
        ply.AddItem(ItemType.GunE11SR);
        ply.AddItem(ItemType.ArmorHeavy);
        ply.AddItem(ItemType.GrenadeHE, 2);
        ply.AddItem(ItemType.Medkit);
        ply.AddItem(ItemType.Radio);
        ply.AddItem(ItemType.Painkillers);
        ply.AddAmmo(Exiled.API.Enums.AmmoType.Nato762, 200);
        ply.MaxHealth = 150;
    }
}
