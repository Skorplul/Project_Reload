using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Kamikaze
{
    public static void SetRole(Player ply)
    {  
        if (Round.SurvivingSCPs < 2)
            return;
        ply.ClearInventory();
        ply.Scale = new Vector3(1f, 1f, 1f);
        ply.GrantLoadout(RoleTypeId.NtfSergeant);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
    }
}
