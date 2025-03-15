using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using System;

namespace PRMainPlugin.Systems.Notifications
{
    internal static class Ban_Kick
    {
        internal static bool Banned = false;

        internal static void OnBanning(BanningEventArgs ev)
        {
            if (ev.Player == null || ev.Player.Nickname == "")
            {
                return;
            }
            Banned = true;
            Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Target.Nickname} has been banned from the server!");
            foreach (Player ply in Player.List)
            {
                if (ply.AdminChatAccess)
                {
                    ply.ClearBroadcasts();
                }
            }
            Map.Broadcast(1, $"<align=center><color=red>A PLAYER HAS BEEN BANNED \n Name: {ev.Target.Nickname} \n UserID: {ev.Target.UserId} \n Reason: {ev.Reason} \n Durration: {DateTime.Now.AddSeconds((double)ev.Duration).Ticks} \n Issuer: {ev.Player}", Broadcast.BroadcastFlags.AdminChat);
        }

        internal static void OnKick(KickedEventArgs ev)
        {
            if (ev.Player == null || ev.Player.Nickname == "")
            {
                return;
            }
            if (!Banned)
            {
                Map.Broadcast(10, $"[<color=#f67979>N</color><color=#e86e6c>e</color><color=#d96260>x</color><color=#cb5754>u</color><color=#bd4c48>s</color><color=#af413c>G</color><color=#a13631>a</color><color=#932a26>m</color><color=#851f1b>i</color><color=#771211>n</color><color=#6a0303>g</color>]: {ev.Player.Nickname} has been kicked from the server!");
                foreach (Player ply in Player.List)
                {
                    if (ply.AdminChatAccess)
                    {
                        ply.ClearBroadcasts();
                    }
                }
                Map.Broadcast(1, $"<align=\"center\"><color=#ff0000>A PLAYER HAS BEEN KICKED \n Name: {ev.Player.Nickname} \n UserID: {ev.Player.UserId} \n Reason: {ev.Reason}", global::Broadcast.BroadcastFlags.AdminChat);
            }
            else if (Banned)
                Banned = false;
        }
    }
}
