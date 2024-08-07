using Exiled.API.Features;
using HarmonyLib;
using PlayerRoles;
using System;
using System.Linq;

#nullable disable
namespace NGMainPlugin.Systems.LobbySystem
{
    [HarmonyPatch(typeof(CharacterClassManager), "ForceRoundStart")]
    internal static class RoundStartPatch
    {
        private static void Prefix()
        {
            foreach (Player player in Player.List.Where<Player>((Func<Player, bool>)(p => p.Role == RoleTypeId.Tutorial)))
                player.Role.Set(RoleTypeId.Spectator);
        }
    }
}
