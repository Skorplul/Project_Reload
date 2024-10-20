namespace NGMainPlugin.Systems.EventHandlers
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using PlayerRoles;
    using NGMainPlugin.API;
    using NGMainPlugin.Systems.Liftaudio;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.EventArgs.Server;
    using MEC;

    public static class EventHandlers
    {
        internal static Config Config;

        public static int PcCurentLvl;

        private static bool friendlyFireDisable = false;

        private static bool Banned = false;

        public static void Enable()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel += OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Banned += OnBan;
            Exiled.Events.Handlers.Player.Kicked += OnKick;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeaving;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
            Exiled.Events.Handlers.Scp079.GainingLevel -= OnSCP079GainingLvl;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Player.Banned -= OnBan;
            Exiled.Events.Handlers.Player.Kicked -= OnKick;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeaving;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
        }

        public static void OnRoundStarted()
        {
            PcCurentLvl = 1;
            Commands.Announcement.spoke.Clear();

            /* *Disabled because of bugs on Modded_Main* **not rn**
            foreach (Lift lift in (IEnumerable<Lift>)Lift.List)
                Timing.RunCoroutine(Methods.CheckingPlayerLift(lift));
            */
        }

        private static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial)
            {
                if (Config.NoTesTuts)
                {
                    ev.IsAllowed = false;
                }
            }
        }

        private static void OnSCP079GainingLvl(GainingLevelEventArgs ev)
        {
            PcCurentLvl = ev.NewLevel;
        }
        /*
        private static void OnBan(BannedEventArgs ev)
        {
            Banned = true;
            Map.Broadcast(1, $"<align=center><color=red>A PLAYER HAS BEEN BANNED \n Name: {ev.Target.Nickname} \n UserID: {ev.Target.UserId} \n Reason: {ev.Details.Reason} \n Durration: {ev.Details.IssuanceTime} \n Issuer: {ev.Details.Issuer}", global::Broadcast.BroadcastFlags.AdminChat);
            Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Target.Nickname} has been banned from the server!");
        }

        private static void OnKick(KickedEventArgs ev)
        {
            if (!Banned)
            {
                Map.Broadcast(1, $"<align=\"center\"><color=#ff0000>A PLAYER HAS BEEN KICKED \n Name: {ev.Player.Nickname} \n UserID: {ev.Player.UserId} \n Reason: {ev.Reason}", global::Broadcast.BroadcastFlags.AdminChat);
                Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Player.Nickname} has been kicked from the server!");
            }
            else if (Banned)
                Banned = false;
        }
        */
        private static void OnWaitingForPlayers()
        {
            if (friendlyFireDisable)
            {
                Log.Debug($"{nameof(OnWaitingForPlayers)}: Disabling friendly fire.");
                Server.FriendlyFire = false;
                friendlyFireDisable = false;
            }
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (Config.RoundEndFF && !Server.FriendlyFire)
            {
                Log.Debug($"{nameof(OnRoundEnded)}: Enabling friendly fire.");
                Server.FriendlyFire = true;
                friendlyFireDisable = true;
            }
        }

        private static void OnEndingRound (EndingRoundEventArgs ev)
        {
            if (ServerEvents.EventRound)
            {
                ServerEvents.EventRound = false;

                foreach (Player ply in Player.List)
                {
                    ply.UnMute();
                }
            }
        }

        private static void OnPlayerLeaving(LeftEventArgs ev)
        {
            if (ServerEvents.EventRound)
            {
                ev.Player.UnMute();
            }
        }
    }
}
