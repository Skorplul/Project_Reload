using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;


namespace PRMainPlugin.Systems.Database;

public class XP
{
    public static void Enable()
    {
        Exiled.Events.Handlers.Player.UsedItem += OnUsingItem;
        Exiled.Events.Handlers.Player.Died += OnDied;
    }
    
    public static void Disable()
    {
        Exiled.Events.Handlers.Player.UsedItem -= OnUsingItem;
        Exiled.Events.Handlers.Player.Died -= OnDied;
    }

    private static void OnUsingItem(UsedItemEventArgs ev)
    {
        switch (ev.Item.Type)
        {
            case ItemType.Medkit:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            case ItemType.Adrenaline:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            case ItemType.SCP207:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            case ItemType.Painkillers:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            case ItemType.AntiSCP207:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            case ItemType.SCP330:
            Database.UpdateXP(ev.Player, 10);
            Database.UpdateShopPoints(ev.Player, 10);
            break;
            default:
            break;
        }
    }

    private static void OnDied(DiedEventArgs ev)
    {
        Database.UpdateShopPoints(ev.Attacker, 50);
        Database.UpdateXP(ev.Attacker, 60);
    }
}
