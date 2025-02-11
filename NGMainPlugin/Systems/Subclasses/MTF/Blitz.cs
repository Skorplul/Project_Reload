using Exiled.API.Features;
using PlayerRoles;

namespace NGMainPlugin.Systems.Subclasses;

public static class Blitz
{
    public static void SetRole(Player ply)
    {
        ply.Role.Set(RoleTypeId.NtfSergeant);
        ply.AddItem(ItemType.SCP207);
        ply.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost);
    }
}
