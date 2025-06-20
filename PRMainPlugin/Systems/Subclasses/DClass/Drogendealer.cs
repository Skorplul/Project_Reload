using Exiled.API.Features;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Drogendealer
{
    public static void SetRole(Player ply)
    {
        ply.Role.Set(PlayerRoles.RoleTypeId.ClassD);
        ply.AddItem(ItemType.Painkillers, 2);
        ply.Scale = new Vector3(1f, 1f, 1f);
    }
}
