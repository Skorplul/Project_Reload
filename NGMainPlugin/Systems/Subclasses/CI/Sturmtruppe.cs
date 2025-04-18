using Exiled.API.Features;

namespace PRMainPlugin.Systems.Subclasses
{
    internal class Sturmtruppe
    {
        public static void SetRole(Player ply)
        {
            ply.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost);
            ply.ClearInventory();
            ply.AddItem(ItemType.GunA7);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.GrenadeHE);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.ArmorHeavy);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.Painkillers, 2);
        }
    }
}
