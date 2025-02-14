using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;

namespace NGMainPlugin;

public static class LastTeamMember
{
    public static void OnRoundStarted()
    {
        Timing.RunCoroutine(LastOfTeam());
    }

    public static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        Timing.KillCoroutines();
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

    private static IEnumerator<float> LastOfTeam()
    {
        while (true)
        {
            Ds.RemoveAll(p => true);
            Nerds.RemoveAll(p => true);
            MTF.RemoveAll(p => true);
            Chaos.RemoveAll(p => true);

            foreach (Player ply in Player.List)
            {
                if (ply.IsCHI)
                    Chaos.Add(ply);
                else if (ply.Role == RoleTypeId.Scientist)
                    Nerds.Add(ply);
                else if (ply.Role == RoleTypeId.ClassD)
                    Ds.Add(ply);
                else if ((RoleTypeId)ply.Role is RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfSpecialist or RoleTypeId.FacilityGuard)
                    MTF.Add(ply);
                // if (ply.Role == RoleTypeId.Spectator || ply.Role == RoleTypeId.Overwatch || ply.Role == RoleTypeId.Filmmaker || ply.IsTutorial || ply.IsScp || ply.Role == RoleTypeId.None)
            }

            if (Chaos.Count == 1)
                Chaos[0].ShowHint("Du bist der letzte Chaos!", 1);
            if (Nerds.Count == 1)
                Nerds[0].ShowHint("Du bist der letzte Scientist!", 1);
            if (Ds.Count == 1)
                Ds[0].ShowHint("Du bist die letzte D-Klasse!", 1);
            if (MTF.Count == 1)
                MTF[0].ShowHint("Du bist der letzte MTF / Guard!", 1);
            

            yield return Timing.WaitForSeconds(1);
        }
    }
}
