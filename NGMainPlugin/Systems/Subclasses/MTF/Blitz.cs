using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;

namespace PRMainPlugin.Systems.Subclasses;

public static class Blitz
{
    public static void SetRole(Player ply)
    {
        ply.ClearInventory();
        ply.Scale = new Vector3(1f, 1f, 1f);
        ply.GrantLoadout(RoleTypeId.NtfSergeant);
        foreach (var item in ply.Items)
        {
            if (item.Type == ItemType.KeycardMTFOperative)
            {
                ply.RemoveItem(item);
                break;
            }
        }
        ply.AddItem(ItemType.KeycardMTFPrivate);
        ply.AddItem(ItemType.SCP207);
        ply.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost);
    }
}
