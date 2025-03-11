using Exiled.API.Features;
using UnityEngine;

namespace NGMainPlugin.Systems.Subclasses;

public static class Sondereinheit
{
    public static void SetRole(Player ply)
    {  
        if (Subclasses.WaveCount <= 2)
            return;
        ply.ShowHint("[<color=#180D76>P</color><color=#190D7F>r</color><color=#1A0D88>o</color><color=#1B0D91>j</color><color=#1C0D9A>e</color><color=#1D0DA3>c</color><color=#1E0DAC>t</color> <color=#200DBE>R</color><color=#210DC7>e</color><color=#220DD0>l</color><color=#230DD9>o</color><color=#240DE2>a</color><color=#250DEB>d</color>] Du Bist nun eine Sondereinheit!");
        ply.ClearInventory();
        ply.Scale = new Vector3(1f, 1f, 1f);
        ply.AddItem(ItemType.ParticleDisruptor);
        ply.AddItem(ItemType.GunE11SR);
        ply.AddItem(ItemType.ArmorHeavy);
        ply.AddItem(ItemType.GrenadeHE, 2);
        ply.AddItem(ItemType.Medkit);
        ply.AddItem(ItemType.Radio);
        ply.AddAmmo(Exiled.API.Enums.AmmoType.Nato762, 200);
        ply.MaxHealth = 150;
    }
}
