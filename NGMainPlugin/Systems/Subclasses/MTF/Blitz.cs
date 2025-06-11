using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Blitz
{
    public static void SetRole(Player ply)
    {
        int aliveMTF = 0;

        foreach (Player p in Player.List)
        {
            switch ((RoleTypeId)p.Role)
            {
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.NtfSpecialist:
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.NtfSergeant:
                    aliveMTF++;
                    break;
                
            }
        }

        if (aliveMTF! >= 3)
            return;

        ply.ClearInventory();
        ply.Scale = new Vector3(1f, 1f, 1f);
        ply.GrantLoadout(RoleTypeId.NtfSergeant);
        ply.AddItem(ItemType.SCP207, 3);
        ply.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost);
    }
}
