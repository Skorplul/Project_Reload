using Exiled.API.Features;
using PlayerRoles;

namespace NGMainPlugin.Systems.Subclasses;

public static class Blitz
{
    public static void SetRole(Player ply)
    {
        ply.ShowHint("[<color=#180D76>P</color><color=#190D7F>r</color><color=#1A0D88>o</color><color=#1B0D91>j</color><color=#1C0D9A>e</color><color=#1D0DA3>c</color><color=#1E0DAC>t</color> <color=#200DBE>R</color><color=#210DC7>e</color><color=#220DD0>l</color><color=#230DD9>o</color><color=#240DE2>a</color><color=#250DEB>d</color>] Du Bist nun eine Blitz Einheit!");
        ply.Role.Set(RoleTypeId.NtfSergeant);
        ply.AddItem(ItemType.SCP207);
        ply.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost);
    }
}
