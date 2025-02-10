using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace NGMainPlugin.Systems.Subclasses;

public class Subclasses
{
    public static void Enable()
    {
        Exiled.Events.Handlers.Server.RespawnedTeam += CheckForRole;
    }

    public static void Disable()
    {
        Exiled.Events.Handlers.Server.RespawnedTeam -= CheckForRole;
    }

    public static void CheckForRole(RespawnedTeamEventArgs ev)
    {
        foreach (Player ply in ev.Players)
        {
            switch (ply.Role)
            {
                case RoleTypeId.ClassD:
                    break;
            }
        }
    }

    public static void GetSubclass()
    {

    }
}
