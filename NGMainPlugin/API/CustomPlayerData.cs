using PRMainPlugin.API.Enums;
using PRMainPlugin.Systems.Subclasses;
using System.Collections.Generic;
using Exiled.API.Features;

namespace PRMainPlugin.API;

public class CustomPlayerData
{
    public SubclassType Subclass { get; set; } = SubclassType.None;
}

public static class PlayerExtensions
{
    private static readonly Dictionary<Player, CustomPlayerData> Data = new();

    public static CustomPlayerData GetData(this Player player)
    {
        if (!Data.ContainsKey(player))
            Data[player] = new CustomPlayerData();

        return Data[player];
    }

    public static void SetSubclass(this Player player, SubclassType subclass)
    {
        var data = player.GetData();
        data.Subclass = subclass;

        ApplySubclassEffects(player, subclass);
    }


    public static string GetDisplayName(this SubclassType subclass)
    {
        return subclass switch
        {
            SubclassType.None => "None",
            // D-Class
            SubclassType.BugBunny => "Bugs Bunny",
            SubclassType.Drogendealer => "Drogendealer",
            SubclassType.Fettsack => "Fettsack",
            SubclassType.Kind => "Kind",
            // MTF
            SubclassType.AllSeeing => "All sehende",
            SubclassType.Blitz => "Blitz",
            SubclassType.Kamikaze => "Kamikaze",
            SubclassType.Sondereinheit => "Sondereinheit",
            // CI
            SubclassType.Spy => "Spion",
            SubclassType.Sturmtruppe => "Sturmtruppler",
            // Zombie
            _ => "Unknown"
        };
    }

    public static void RemoveData(this Player player)
    {
        Data.Remove(player);
    }

    private static void ApplySubclassEffects(Player player, SubclassType subclass)
    {
        switch (subclass)
        {
            case SubclassType.None:
                return;
            case SubclassType.Fettsack:
                Fettsack.SetRole(player);
                break;
            case SubclassType.BugBunny:
                BugBunny.SetRole(player);
                break;
            case SubclassType.Drogendealer:
                Drogendealer.SetRole(player);
                break;
            case SubclassType.Kind:
                Kind.SetRole(player);
                break;
            case SubclassType.Blitz:
                Blitz.SetRole(player);
                break;
            case SubclassType.Kamikaze:
                Kamikaze.SetRole(player);
                break;
            case SubclassType.AllSeeing:
                Allseeing.SetRole(player);
                break;
            case SubclassType.Sondereinheit:
                Sondereinheit.SetRole(player);
                break;
            case SubclassType.Spy:
                //Spy.SetRole(); (Skin swap needed for this)
                break;
            case SubclassType.Sturmtruppe:
                Sturmtruppe.SetRole(player);
                break;
            default:
                break;
        }

        string Name = subclass.GetDisplayName();
        if (Name == "Unknown")
            return;
        player.ShowHint($"[<color=#180D76>P</color><color=#190D7F>r</color><color=#1A0D88>o</color><color=#1B0D91>j</color><color=#1C0D9A>e</color><color=#1D0DA3>c</color><color=#1E0DAC>t</color> <color=#200DBE>R</color><color=#210DC7>e</color><color=#220DD0>l</color><color=#230DD9>o</color><color=#240DE2>a</color><color=#250DEB>d</color>] Du Bist nun <b>{Name}</b>!");
    }

}