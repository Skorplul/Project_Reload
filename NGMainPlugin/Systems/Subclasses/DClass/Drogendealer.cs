using Exiled.API.Features;

namespace NGMainPlugin;

public static class Drogendealer
{
    public static void SetRole(Player ply)
    {
        ply.AddItem(ItemType.Painkillers, 2);
    }
}
