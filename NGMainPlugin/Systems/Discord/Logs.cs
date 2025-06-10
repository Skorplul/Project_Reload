using DCWB = PRMainPlugin.API.DiscordWebhookAPI;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using MEC;
using Exiled.API.Features;
using UnityEngine;


namespace PRMainPlugin.Systems.Discord
{
    public static class Loggs
    {
        internal static Config Config;
        public static void Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Player.RemovedHandcuffs += OnRemovedHandcuffes;
            Exiled.Events.Handlers.Player.Spawning += OnSpawned;

            Timing.RunCoroutine(SendLogs());
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;
            Exiled.Events.Handlers.Player.RemovedHandcuffs -= OnRemovedHandcuffes;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawned;

            Timing.KillCoroutines();
        }

        private static string msg = "";

        private static void AddLog(string Log)
        {
            msg += $"Log\n";
        }

        private static IEnumerator<float> SendLogs()
        {
            while (true)
            {
                if (!msg.IsEmpty())
                {
                    DCWB.SendMs(msg);
                    msg = "";
                }
                else
                {
                    yield return Timing.WaitForSeconds(7);
                }

                yield return Timing.WaitForSeconds(3);
            }
        }


        // Start Log funcs

        private static void OnRoundStarted()
        {
            AddLog($"[{DateTime.Now}] \\n \\n \\n**-------------------------------------------------------------------------------------\\nEine neu Runde hat jetzt gestarted!\\n-------------------------------------------------------------------------------------\\n \\n \\n**");
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] \\n \\n \\n**-------------------------------------------------------------------------------------\\nDie Runde hat jetzt geendet!\\n-------------------------------------------------------------------------------------\\n \\n \\n**");
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` ist gejoint.");
        }

        private static void OnLeft(LeftEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` ist gegangen.");
        }

        // fucked sometimes... (NullReference exception)
        private static void OnDied(DiedEventArgs ev)
        {
            try
            {
                if (ev.Attacker == null)
                    return;
                if (ev.DamageHandler.IsSuicide)
                {
                    AddLog($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` hat sich selbst umgebracht.");
                    return;
                }
                AddLog($"[{DateTime.Now}] `{ev.Attacker.Nickname}` `({ev.Attacker.UserId})` hat `{ev.Player.Nickname}` `({ev.Player.UserId})` umgebracht.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Another {ex.GetType()} in PRMainPlugin.Systems.Discord.Logs.OnDied");
            }
        }


        // fucked sometimes... (NullReference exception)
        private static void OnHurt(HurtEventArgs ev)
        {
            try
            {
                if (ev == null || ev.Player == null || ev.Attacker == null)
                {
                    return;
                }
                if (Round.IsEnded || ev.DamageHandler.IsFriendlyFire || ev.Amount == 0)
                    return;
                if (ev.Player == ev.Attacker)
                {
                    AddLog($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` hat sich selbst {ev.Amount} schaden zugefügt.");
                    return;
                }
                AddLog($"[{DateTime.Now}] `{ev.Attacker.Nickname}` `({ev.Attacker.UserId})` hat `{ev.Player.Nickname}` `({ev.Player.UserId})` {ev.Amount} schaden mit {ev.DamageHandler.Type} zugefügt.");
            }
            catch (Exception ex)
            {
                Log.Warn($"Another {ex.GetType()} in PRMainPlugin.Systems.Discord.Logs.OnHurt");
            }
        }

        private static void OnHandcuffing(HandcuffingEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] `{ev.Target.Nickname}` `({ev.Target.UserId})` wurde von `{ev.Player.Nickname}` `({ev.Player.UserId})` cuffed.");
        }

        private static void OnRemovedHandcuffes(RemovedHandcuffsEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] `{ev.Target.Nickname}` `({ev.Target.UserId})` wurde von `{ev.Player.Nickname}` `({ev.Player.UserId})` uncuffed.");
        }

        private static void OnSpawned(SpawningEventArgs ev)
        {
            AddLog($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` wurde als `{ev.NewRole.Name}`gespawned.");
        }
    }
}
