using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Hints;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            IsLocked = false;
            Round.IsLobbyLocked = true;
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            LobbyTimer = Timing.RunCoroutine(Lobby());
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            if (!Round.IsLobby)
                return;
            ev.Player.Role.Set(RoleTypeId.Tutorial);
            ev.Player.Teleport(SpawnPosition);
        }

        private IEnumerator<float> Lobby()
        {
            double countdown = Main.Instance.Config.LobbyTime;
            int requiredPlayers = Main.Instance.Config.MinimumPlayers;

            while (Round.IsLobby)
            {
                string newValue1;

                if (IsLocked)
                    newValue1 = config.PausedStatus;

                else if (Player.List.Count((p => p.IsTutorial)) < requiredPlayers)
                {
                    countdown = Main.Instance.Config.LobbyTime;
                    newValue1 = config.WaitingStatus;
                }
                else
                {
                    newValue1 = config.StartingStatus.Replace("%countdown%", countdown.ToString());
                    --countdown;
                }

                string str1 = config.TextShown.Replace("%status%", newValue1);

                int num = Player.List.Count((p => p.IsTutorial));
                string newValue2 = num.ToString();
                string str2 = str1.Replace("%playercount%", newValue2);

                num = Server.MaxPlayerCount;
                string maxPlayers = num.ToString();
                string hint = $"<line-height=0%>\n<voffset=17em>{str2.Replace("%maxplayers%", maxPlayers)}\n<voffset=-37em>";

                //Map.Broadcast(1, str2.Replace("%maxplayers%", hint), shouldClearPrevious: true);
                foreach (Player p in Player.List)
                {
                    p.HintDisplay.Show(new TextHint(hint, new HintParameter[] { new StringHintParameter(hint) }, new HintEffect[1] { new AlphaEffect(1f) }, 3f));
                }

                if (countdown == 0.0)
                {
                    foreach (Player player in Player.List.Where((p => p.Role.Type == RoleTypeId.Tutorial)))
                        player.Role.Set(RoleTypeId.Spectator);

                    Timing.CallDelayed(0.1f, (() => Round.Start()));

                    break;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
