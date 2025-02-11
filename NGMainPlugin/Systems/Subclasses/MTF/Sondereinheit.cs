using Exiled.API.Features;

namespace NGMainPlugin.Systems.Subclasses;

public static class Sondereinheit
{
    public static void SetRole(Player ply)
    {  
        if (Subclasses.WaveCount <= 2)
            return;

        ply.ClearInventory();
        ply.AddItem(ItemType.ParticleDisruptor);
        ply.AddItem(ItemType.GunE11SR);
        ply.AddItem(ItemType.ArmorHeavy);
        ply.AddItem(ItemType.GrenadeHE, 2);
        ply.AddItem(ItemType.Medkit);
        ply.AddItem(ItemType.Radio);
        ply.MaxHealth = 150;
    }
}
