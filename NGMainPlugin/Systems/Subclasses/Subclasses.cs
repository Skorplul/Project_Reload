using PRMainPlugin.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace PRMainPlugin.Systems.Subclasses;

public static class Subclasses
{
    private readonly static Random Random = new Random();

    public static int WaveCount = 0;

    /// <summary>
    /// D-Class SubProbs
    /// </summary>
    private static Dictionary<SubclassType, double> DProbs = new Dictionary<SubclassType, double>
    {
        { SubclassType.Fettsack, 7.5 },
        { SubclassType.BugBunny, 5.0 },
        { SubclassType.Drogendealer, 10.0 },
    };

    /// <summary>
    /// Guards Probs
    /// </summary>
    private static Dictionary<SubclassType, double> GuardProbs = new Dictionary<SubclassType, double>
    {
        { SubclassType.Blitz, 10.0 },
        { SubclassType.Kamikaze, 7.5 },
        { SubclassType.AllSeeing, 5.0 },
        { SubclassType.Sondereinheit, 5.0 },
    };

    /// <summary>
    /// Chaos Probs
    /// </summary>
    private static Dictionary<SubclassType, double> CIProbs = new Dictionary<SubclassType, double>
    { 
        { SubclassType.Spy, 5.0 },
    };

    public static void Enable()
    {
        Exiled.Events.Handlers.Player.Spawned += DoSubclass;
    }

    public static void Disable()
    {
        Exiled.Events.Handlers.Player.Spawned -= DoSubclass;
    }

    public static void DoSubclass(SpawnedEventArgs ev)
    {
        ev.Player.Scale = new UnityEngine.Vector3(1f, 1f, 1f);
        if (ev.Reason == Exiled.API.Enums.SpawnReason.ForceClass || ev.Reason == Exiled.API.Enums.SpawnReason.None || ev.Reason == Exiled.API.Enums.SpawnReason.Died)
            return;

        if (Random.Next(0, 10) !>= 9)
            return;

        SubclassType SubClass = GetSubclass(ev.Player.Role);

        switch (SubClass)
        {
            case SubclassType.Fettsack:
                Fettsack.SetRole(ev.Player);
                break;
            case SubclassType.BugBunny:
                BugBunny.SetRole(ev.Player);
                break;
            case SubclassType.Drogendealer:
                Drogendealer.SetRole(ev.Player);
                break;
            case SubclassType.Kind:
                Kind.SetRole(ev.Player);
                break;
            case SubclassType.Blitz:
                Blitz.SetRole(ev.Player);
                break;
            case SubclassType.Kamikaze:
                Kamikaze.SetRole(ev.Player);
                break;
            case SubclassType.AllSeeing:
                Allseeing.SetRole(ev.Player);
                break;
            case SubclassType.Sondereinheit:
                Sondereinheit.SetRole(ev.Player); // ToDo: erst nach der dritten Welle Spawnbar.
                break;
            case SubclassType.Spy:
                //Spy.SetRole(); (Skin swap needed for this)
                break;
            default:
                break;
        }
    }

    private static SubclassType GetSubclass(RoleTypeId RoleID)
    {
        switch (RoleID)
        {
            case RoleTypeId.ClassD:
                return SelectRandomAction(DProbs);
            case RoleTypeId.FacilityGuard:
                return SelectRandomAction(GuardProbs);
            case RoleTypeId.ChaosRepressor | RoleTypeId.ChaosMarauder | RoleTypeId.ChaosRifleman | RoleTypeId.ChaosConscript:
                return SelectRandomAction(CIProbs);
            default:
                return SubclassType.None;
        }

        static T SelectRandomAction<T>(Dictionary<T, double> probabilities)
        {
            double totalProbability = 0;
            foreach (var probability in probabilities.Values)
            {
                totalProbability += probability;
            }

            double randomValue = Random.NextDouble() * totalProbability;
            double cumulative = 0;

            foreach (var kvp in probabilities)
            {
                cumulative += kvp.Value;
                if (randomValue <= cumulative)
                {
                    return kvp.Key;
                }
            }

            // Fallback if no action was selected (should not happen if probabilities are set correctly)
            throw new Exception("No action selected; check probability values.");
        }
    }
}
