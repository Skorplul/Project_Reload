using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Exiled.Events.EventArgs.Server;
using NGMainPlugin.API;

namespace NGMainPlugin
{
    public class EventHandlers
    {
        private readonly Main plugin;

        public static int PcCurentLvl;
        public bool friendlyFireDisable = false;
        private bool Banned = false;

        public EventHandlers (Main plugin)
        {
            this.plugin = plugin;
        }

        public void OnRoundStarted()
        {
            PcCurentLvl = 1;
            Commands.SCPSwap.swaped.Clear();
            Commands.Durchsage.spoke.Clear();

            // Disabled because of bugs on Modded_Main
            //foreach (Lift lift in (IEnumerable<Lift>)Lift.List)
            //    Timing.RunCoroutine(Methods.CheckingPlayerLift(lift));
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial)
            {
                if (plugin.Config.NoTesTuts)
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public void OnSCP079GainingLvl(GainingLevelEventArgs ev)
        {
            PcCurentLvl = ev.NewLevel;
        }

        public void OnBan(BannedEventArgs ev)
        {
            Banned = true;
            Map.Broadcast(1, $"<align=center><color=red>A PLAYER HAS BEEN BANNED \n Name: {ev.Target.Nickname} \n UserID: {ev.Target.UserId} \n Reason: {ev.Details.Reason} \n Durration: {ev.Details.IssuanceTime} \n Issuer: {ev.Details.Issuer}", Broadcast.BroadcastFlags.AdminChat);
            Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Target.Nickname} has been banned from the server!");
        }

        public void OnKick(KickedEventArgs ev)
        {
            if (!Banned)
            {
                Map.Broadcast(1, $"<align=\"center\"><color=#ff0000>A PLAYER HAS BEEN KICKED \n Name: {ev.Player.Nickname} \n UserID: {ev.Player.UserId} \n Reason: {ev.Reason}", Broadcast.BroadcastFlags.AdminChat);
                Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Player.Nickname} has been kicked from the server!");
            }
            else if (Banned)
                Banned = false;
        }

        public void OnWaitingForPlayers()
        {
            if (friendlyFireDisable)
            {
                Log.Debug($"{nameof(OnWaitingForPlayers)}: Disabling friendly fire.");
                Server.FriendlyFire = false;
                friendlyFireDisable = false;
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (plugin.Config.RoundEndFF && !Server.FriendlyFire)
            {
                Log.Debug($"{nameof(OnRoundEnded)}: Enabling friendly fire.");
                Server.FriendlyFire = true;
                friendlyFireDisable = true;
            }
        }

        public void OnEndingRound (EndingRoundEventArgs ev)
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

        public void OnPlayerLeaving(LeftEventArgs ev)
        {
            if (ServerEvents.EventRound)
            {
                ev.Player.UnMute();
            }
        }
    }
}
