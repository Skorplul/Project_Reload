using Exiled.API.Features;
using PlayerRoles;

namespace NGMainPlugin.Systems.Subclasses;

public static class Kamikaze
{
    public static void SetRole(Player ply)
    {  
        if (Round.SurvivingSCPs <= 2)
            return;

        ply.Role.Set(RoleTypeId.NtfSergeant);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Pink);
    }
}
