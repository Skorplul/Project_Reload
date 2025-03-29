using DCWB = PRMainPlugin.API.DiscordWebhookAPI;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using System;
using Exiled.API.Features;


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
        }

        // Start Log funcs

        private static void OnRoundStarted()
        {
            DCWB.SendMs($"[{DateTime.Now}] \\n \\n \\n**-------------------------------------------------------------------------------------\\nEine neu Runde hat jetzt gestarted!\\n-------------------------------------------------------------------------------------\\n \\n \\n**");
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] \\n \\n \\n**-------------------------------------------------------------------------------------\\nDie Runde hat jetzt geendet!\\n-------------------------------------------------------------------------------------\\n \\n \\n**");
        }

        private static void OnVerified(VerifiedEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` ist gejoint.");
        }

        private static void OnLeft(LeftEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` ist gegangen.");
        }

        private static void OnDied(DiedEventArgs ev)
        {
            if (ev.DamageHandler.IsSuicide)
            {
                DCWB.SendMs($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` hat sich selbst umgebracht.");
                return;
            }
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Attacker.Nickname}` `({ev.Attacker.UserId})` hat `{ev.Player.Nickname}` `({ev.Player.UserId})` umgebracht.");
        }

        private static void OnHurt(HurtEventArgs ev)
        {
            if (ev == null)
            {
                Log.Error("OnHurt Is NULL??");
            }
            if (Round.IsEnded || ev.DamageHandler.IsFriendlyFire || ev.Amount == 0)
                return;
            if (ev.Player == ev.Attacker)
            {
                DCWB.SendMs($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` hat sich selbst {ev.Amount} schaden zugefügt.");
                return;
            }
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Attacker.Nickname}` `({ev.Attacker.UserId})` hat `{ev.Player.Nickname}` `({ev.Player.UserId})` {ev.Amount} schaden mit {ev.DamageHandler.Type} zugefügt.");
        }

        private static void OnHandcuffing(HandcuffingEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Target.Nickname}` `({ev.Target.UserId})` wurde von `{ev.Player.Nickname}` `({ev.Player.UserId})` cuffed.");
        }

        private static void OnRemovedHandcuffes(RemovedHandcuffsEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Target.Nickname}` `({ev.Target.UserId})` wurde von `{ev.Player.Nickname}` `({ev.Player.UserId})` uncuffed.");
        }

        private static void OnSpawned(SpawningEventArgs ev)
        {
            DCWB.SendMs($"[{DateTime.Now}] `{ev.Player.Nickname}` `({ev.Player.UserId})` wurde als `{ev.NewRole.Name}`gespawned.");
        }
    }
}
