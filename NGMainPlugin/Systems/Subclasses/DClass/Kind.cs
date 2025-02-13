using Exiled.API.Features;
namespace NGMainPlugin.Systems.Subclasses;

public static class Kind
{
    public static void SetRole(Player ply)
    {
        ply.Role.Set(PlayerRoles.RoleTypeId.ClassD);
        ply.Teleport(Room.Get(Exiled.API.Enums.RoomType.Lcz330));
        ply.Scale = new UnityEngine.Vector3(0.6f, 0.6f, 0.6f);
    }
}
