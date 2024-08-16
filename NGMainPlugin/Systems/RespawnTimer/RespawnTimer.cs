namespace NGMainPlugin.Systems.RespawnTimer
{
    using Exiled.API.Features;
    using MEC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Respawning;
    using UnityEngine;
    using Exiled.API.Enums;
    using System.Globalization;

    public static class RespawnTimer
    {
        internal static Config Config;

        private static readonly StringBuilder stringBuilder = new StringBuilder(2048);

        private static CoroutineHandle _timerCoroutine;

        private static CoroutineHandle _hintsCoroutine;

        private static int CurrentHint = 0;

        private static List<Player> TimerHidden = new List<Player>();

        public static void Enable()
        {
            Exiled.Events.Handlers.Map.Generated += OnGenerated;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Map.Generated -= OnGenerated;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
        }

        internal static void OnGenerated()
        {
            if (Config.ReloadTimerEachRound)
                Config.LoadConfig();

            if (_timerCoroutine.IsRunning)
                Timing.KillCoroutines(_timerCoroutine);

            if (_hintsCoroutine.IsRunning)
                Timing.KillCoroutines(_hintsCoroutine);
        }

        internal static void OnRoundStart()
        {
            try
            {
                _timerCoroutine = Timing.RunCoroutine(TimerCoroutine());
                _hintsCoroutine = Timing.RunCoroutine(HintsCoroutine());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            Log.Debug("RespawnTimer coroutine started successfully!");
        }

        public static bool ToggleTimerForPlayer(Player player)
        {
            if (TimerHidden.Contains(player))
                TimerHidden.Remove(player);
            else 
                TimerHidden.Add(player);

            return TimerHidden.Contains(player);
        }

        private static string BuildHintString()
        {
            stringBuilder.Clear();
            stringBuilder.Append(!Respawn.IsSpawning ? Config.TimerBeforeRespawn : Config.TimerDuringRespawn);

            // Round Time
            int minutes = Round.ElapsedTime.Minutes;
            int seconds = Round.ElapsedTime.Seconds;

            stringBuilder.Replace("{round_minutes}", string.Format("{0}{1}", !Config.LeadingZeros || minutes >= 10 ? string.Empty : "0", minutes));
            stringBuilder.Replace("{round_seconds}", string.Format("{0}{1}", !Config.LeadingZeros || seconds >= 10 ? string.Empty : "0", seconds));

            // Respawn Time
            TimeSpan timeUntilSpawnWave = Respawn.TimeUntilSpawnWave;

            var offset = Config.TimerOffset ? (RespawnTokensManager.GetTeamDominance(SpawnableTeamType.NineTailedFox) == 1.0 ? 18 : 14) : 0;
            minutes = (int)(timeUntilSpawnWave.TotalSeconds + offset) / 60;
            seconds = (int)(timeUntilSpawnWave.TotalSeconds + offset) % 60;

            stringBuilder.Replace("{minutes}", string.Format("{0}{1}", !Config.LeadingZeros || minutes >= 10 ? string.Empty : "0", minutes));
            stringBuilder.Replace("{seconds}", string.Format("{0}{1}", !Config.LeadingZeros || seconds >= 10 ? string.Empty : "0", seconds));

            // Next Known Team
            stringBuilder.Replace("{team}", Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency ? Config.Ci : Config.Ntf);

            // Spectators and Tickets
            stringBuilder.Replace("{spectators_num}", Player.List.Where(p => p.IsDead && !p.IsOverwatchEnabled).Count().ToString());
            stringBuilder.Replace("{ntf_tickets_num}", Mathf.Round(Respawn.NtfTickets).ToString());
            stringBuilder.Replace("{ci_tickets_num}", Mathf.Round(Respawn.ChaosTickets).ToString());

            // Warhead Status
            stringBuilder.Replace("{warhead_status}", Config.WarheadStatus[Warhead.Status]);
            stringBuilder.Replace("{detonation_time}", Warhead.Status == WarheadStatus.InProgress ? Mathf.Round(Warhead.DetonationTimer).ToString((IFormatProvider)CultureInfo.InvariantCulture) : string.Empty);

            // Generator Count
            int active = 0;
            int total = 0;
            foreach (Generator generator in (IEnumerable<Generator>)Generator.List)
            {
                if (generator.State.HasFlag((Enum)GeneratorState.Engaged))
                    active++;
                total++;
            }
            stringBuilder.Replace("{generator_engaged}", active.ToString());
            stringBuilder.Replace("{generator_count}", total.ToString());

            // TPS and Tickrate
            stringBuilder.Replace("{tps}", Server.Tps.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace("{tickrate}", Application.targetFrameRate.ToString());

            // Hint
            if (Config.Hints.Count > 0)
                stringBuilder.Replace("{hint}", Config.Hints[CurrentHint]);

            // Random Color
            stringBuilder.Replace("{RANDOM_COLOR}", string.Format("#{0:X6}", UnityEngine.Random.Range(0, 16777215)));
            stringBuilder.Replace('{', '[').Replace('}', ']');

            return stringBuilder.ToString();
        }

        private static IEnumerator<float> TimerCoroutine()
        {

            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(1f);
                
                try
                {
                    foreach (var player in Player.List.Where(p => p.IsDead && (!p.IsOverwatchEnabled || !Config.HideTimerForOverwatch) && !TimerHidden.Contains(p))) {
                        player.ShowHint(BuildHintString(), 2f);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private static IEnumerator<float> HintsCoroutine()
        {
            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(Config.HintInterval);

                CurrentHint++;
                if (CurrentHint >= Config.Hints.Count)
                    CurrentHint = 0;
            }
        }
    }
}
