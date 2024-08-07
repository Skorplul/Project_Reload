using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace NGMainPlugin.Systems.LobbySystem
{
    public sealed class Handler
    {
        private static readonly Config config = Main.Instance.Config;
        public static bool IsLocked;
        public static CoroutineHandle LobbyTimer;

        private static Vector3 SpawnPosition
        {
            get
            {
                return Main.Instance.Config.SpawnRoom != RoomType.Unknown ? Room.Get(Main.Instance.Config.SpawnRoom).Position + Vector3.up : Main.Instance.Config.SpawnPosition;
            }
        }

        public void OnWaitingForPlayers()
        {
            Handler.IsLocked = false;
            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            Handler.LobbyTimer = Timing.RunCoroutine(this.Lobby());
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (!Round.IsLobby)
                return;
            ev.Player.Role.Set(RoleTypeId.Tutorial);
            ev.Player.Teleport(Handler.SpawnPosition);
        }

        private IEnumerator<float> Lobby()
        {
            double countdown = Main.Instance.Config.LobbyTime;
            int requiredPlayers = Main.Instance.Config.MinimumPlayers;
            while (Round.IsLobby)
            {
                string newValue1;
                if (Handler.IsLocked)
                    newValue1 = Handler.config.PausedStatus;
                else if (Exiled.API.Features.Player.List.Count<Exiled.API.Features.Player>((Func<Exiled.API.Features.Player, bool>)(p => p.IsTutorial)) < requiredPlayers)
                {
                    countdown = Main.Instance.Config.LobbyTime;
                    newValue1 = Handler.config.WaitingStatus;
                }
                else
                {
                    newValue1 = Handler.config.StartingStatus.Replace("%countdown%", countdown.ToString());
                    --countdown;
                }
                string str1 = Handler.config.TextShown.Replace("%status%", newValue1);
                int num = Exiled.API.Features.Player.List.Count<Exiled.API.Features.Player>((Func<Exiled.API.Features.Player, bool>)(p => p.IsTutorial));
                string newValue2 = num.ToString();
                string str2 = str1.Replace("%playercount%", newValue2);
                num = Server.MaxPlayerCount;
                string newValue3 = num.ToString();
                Map.Broadcast((ushort)1, str2.Replace("%maxplayers%", newValue3), shouldClearPrevious: true);
                if (countdown == 0.0)
                {
                    foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List.Where<Exiled.API.Features.Player>((Func<Exiled.API.Features.Player, bool>)(p => p.Role.Type == RoleTypeId.Tutorial)))
                        player.Role.Set(RoleTypeId.Spectator);
                    Timing.CallDelayed(0.1f, (Action)(() => Round.Start()));
                    break;
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
