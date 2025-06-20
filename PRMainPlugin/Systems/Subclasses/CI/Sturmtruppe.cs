using Exiled.API.Features;

namespace PRMainPlugin.Systems.Subclasses
{
    internal class Sturmtruppe
    {
        public static void SetRole(Player ply)
        {
            ply.EnableEffect(Exiled.API.Enums.EffectType.Scp1853);
            ply.DropItems();
            ply.AddItem(ItemType.GunA7);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.GrenadeHE);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.Painkillers, 2);
            ply.AddAmmo(Exiled.API.Enums.AmmoType.Ammo12Gauge, 120);
            ply.AddAmmo(Exiled.API.Enums.AmmoType.Nato9, 50);
        }
    }
}
