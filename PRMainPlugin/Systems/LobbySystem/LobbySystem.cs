namespace PRMainPlugin.Systems.LobbySystem
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Hints;
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal static class LobbySystem
    {
        internal static Config Config;

        public static bool IsLocked;

        public static CoroutineHandle LobbyTimer;

        private static Vector3 SpawnPosition
        {
            get
            {
                return Config.SpawnRoom != RoomType.Unknown ? Room.Get(Config.SpawnRoom).Position + Vector3.up : Config.SpawnPosition;
            }
        }

        public static void Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

        private static void OnWaitingForPlayers()
        {
            IsLocked = false;
            Round.IsLobbyLocked = true;

            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            LobbyTimer = Timing.RunCoroutine(Lobby());
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            if (!Round.IsLobby)
                return;

            ev.Player.Role.Set(RoleTypeId.Tutorial);
            ev.Player.Teleport(SpawnPosition);
        }

        private static IEnumerator<float> Lobby()
        {
            double countdown = Config.LobbyTime;
            int requiredPlayers = Config.MinimumPlayers;

            while (Round.IsLobby)
            {
                string newValue1;

                if (IsLocked)
                    newValue1 = Config.PausedStatus;

                else if (Player.List.Count((p => p.IsTutorial)) < requiredPlayers)
                {
                    countdown = Config.LobbyTime;
                    newValue1 = Config.WaitingStatus;
                }
                else
                {
                    newValue1 = Config.StartingStatus.Replace("%countdown%", countdown.ToString());
                    --countdown;
                }

                string str1 = Config.TextShown.Replace("%status%", newValue1);

                int num = Player.List.Count((p => p.IsTutorial));
                string newValue2 = num.ToString();
                string str2 = str1.Replace("%playercount%", newValue2);

                num = Server.MaxPlayerCount;
                string maxPlayers = num.ToString();
                string hint = $"<line-height=0%>\n<voffset=17em>{str2.Replace("%maxplayers%", maxPlayers)}\n<voffset=-37em>";

                foreach (Player p in Player.List)
                {
                    p.HintDisplay.Show(new TextHint(hint, new HintParameter[] { new StringHintParameter(hint) }, new HintEffect[1] { new AlphaEffect(1f) }, 1.25f));
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
