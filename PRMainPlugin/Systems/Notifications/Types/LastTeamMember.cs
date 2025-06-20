using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;

namespace PRMainPlugin.Systems.Notifications;

public class LastTeamMember
{
    static CoroutineHandle _lastOfTeamHandle;

    public static void OnRoundStarted()
    {
        Done.RemoveAll(p => true);
        _lastOfTeamHandle = Timing.RunCoroutine(LastOfTeam());
    }

    public static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        Timing.KillCoroutines(_lastOfTeamHandle);
    }

    /// <summary>
    /// All D-Class in the round.
    /// </summary>
    private static List<Player> Ds = new List<Player>();

    /// <summary>
    /// All MTF and Guards in the round.
    /// </summary>
    private static List<Player> MTF = new List<Player>();

    /// <summary>
    /// All Chaos in the round.
    /// </summary>
    private static List<Player> Chaos = new List<Player>();

    /// <summary>
    /// All Scientists in the round.
    /// </summary>
    private static List<Player> Nerds = new List<Player>();

    /// <summary>
    /// All players that have already seen this message
    /// </summary>
    private static List<Player> Done = new List<Player>();

    private static IEnumerator<float> LastOfTeam()
    {
        while (Round.InProgress)
        {
            Ds.RemoveAll(p => true);
            Nerds.RemoveAll(p => true);
            MTF.RemoveAll(p => true);
            Chaos.RemoveAll(p => true);

            foreach (Player ply in Player.List)
            {
                if (ply.IsCHI)
                {
                    Chaos.Add(ply);
                }
                else if (ply.Role == RoleTypeId.Scientist)
                {
                    Nerds.Add(ply);
                }
                else if (ply.Role == RoleTypeId.ClassD)
                {
                    Ds.Add(ply);
                }
                else if ((RoleTypeId)ply.Role is RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist or RoleTypeId.FacilityGuard)
                {
                    MTF.Add(ply);
                }
                // if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.Overwatch || ply.Role == RoleTypeId.Filmmaker || ply.IsTutorial || ply.IsScp || ply.Role == RoleTypeId.None)
            }

            if (Chaos.Count == 1 && !Done.Contains(Chaos[0]))
                Chaos[0].ShowHint("Du bist der letzte Chaos!", 7); Done.Add(Chaos[0]); Chaos.Remove(Chaos[0]);
            if (Nerds.Count == 1 && !Done.Contains(Nerds[0]))
                Nerds[0].ShowHint("Du bist der letzte Scientist!", 7); Done.Add(Nerds[0]); Nerds.Remove(Nerds[0]);
            if (Ds.Count == 1 && !Done.Contains(Ds[0]))
                Ds[0].ShowHint("Du bist die letzte D-Klasse!", 7); Done.Add(Ds[0]); Ds.Remove(Ds[0]);
            if (MTF.Count == 1 && !Done.Contains(MTF[0]))
                MTF[0].ShowHint("Du bist der letzte MTF / Guard!", 7); Done.Add(MTF[0]); MTF.Remove(MTF[0]);

            yield return Timing.WaitForSeconds(1);
        }
    }
}
