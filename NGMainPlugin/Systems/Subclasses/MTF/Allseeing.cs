using PlayerRoles;
using Exiled.API.Features;

namespace NGMainPlugin.Systems.Subclasses;

public static class Allseeing
{
    public static void SetRole(Player ply)
    {
        ply.Role.Set(RoleTypeId.NtfCaptain);
        ply.EnableEffect(Exiled.API.Enums.EffectType.Scp1344);
    }
}
