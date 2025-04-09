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
                Database.UpdateReconsAndXP(ev.Player, 10, 10);
                break;

            case ItemType.Adrenaline:
                Database.UpdateReconsAndXP(ev.Player, 10, 10);
                break;

            case ItemType.Painkillers:
                Database.UpdateReconsAndXP(ev.Player, 10, 10);
                break;

            case ItemType.SCP207:
                Database.UpdateReconsAndXP(ev.Player, 10, 15);
                break;

            case ItemType.AntiSCP207:
                Database.UpdateReconsAndXP(ev.Player, 15, 15);
                break;

            case ItemType.SCP330:
                Database.UpdateReconsAndXP(ev.Player, 10, 20);
                break;

            case ItemType.SCP018:
                Database.UpdateReconsAndXP(ev.Player, 20, 20);
                break;

            case ItemType.SCP1853:
                Database.UpdateReconsAndXP(ev.Player, 25, 30);
                break;

            case ItemType.SCP244a:
            case ItemType.SCP244b:
                Database.UpdateReconsAndXP(ev.Player, 20, 50);
                break;

            case ItemType.SCP268:
                Database.UpdateReconsAndXP(ev.Player, 25, 20);
                break;

            case ItemType.SCP500:
                Database.UpdateReconsAndXP(ev.Player, 20, 20);
                break;

            case ItemType.SCP2176:
                Database.UpdateReconsAndXP(ev.Player, 20, 30);
                break;
            
            case ItemType.SCP1344:
                Handle1344Use(ev.Player);
                break;

            default:
            break;
        }
    }

    // todo (kann man spammen und damit punkte farmen)
    private static void Handle1344Use(Player ply)
    {
        return;
    }

    private static void OnDied(DiedEventArgs ev)
    {
        Database.AddDeath(ev.Player);
        Database.AddKill(ev.Attacker);
        Database.UpdateReconsAndXP(ev.Player, 60, 50);
    }
}
