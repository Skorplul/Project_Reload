using Exiled.API.Features;

namespace NGMainPlugin.Systems.Subclasses;

public static class BugBunny
{
    public static void SetRole(Player ply)
    {
        ply.Scale = new UnityEngine.Vector3(0.7f, 0.7f, 0.7f);
        ply.TryAddCandy(InventorySystem.Items.Usables.Scp330.CandyKindID.Yellow);
    }
}
