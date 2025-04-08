using Exiled.API.Features;

namespace PRMainPlugin.Systems.Subclasses;

public static class BugBunny
{
    public static void SetRole(Player ply)
    {
        ply.Role.Set(PlayerRoles.RoleTypeId.ClassD);
        ply.Scale = new UnityEngine.Vector3(0.7f, 0.7f, 0.7f);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow);
    }
}
