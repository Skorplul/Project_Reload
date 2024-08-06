using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using Respawning;
using NGMainPlugin.Systems.RespawnTimer.Configs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer.API.Features
{
    public class TimerView
    {
        public readonly NGMainPlugin.Commands.AdminSysEvents EvCommand;
        public static readonly Dictionary<string, TimerView> CachedTimers = new Dictionary<string, TimerView>();
        private readonly StringBuilder StringBuilder = new StringBuilder(1024);

        public int HintIndex { get; private set; }

        private int HintInterval { get; set; }

        public static void AddTimer(string name)
        {
            if (TimerView.CachedTimers.ContainsKey(name))
                return;
            string str = Path.Combine(Main.RespawnTimerDirectoryPath, name);
            if (!Directory.Exists(str))
            {
                Log.Error(name + " directory does not exist!");
            }
            else
            {
                string path1 = Path.Combine(str, "TimerBeforeSpawn.txt");
                if (!File.Exists(path1))
                {
                    Log.Error(Path.GetFileName(path1) + " file does not exist!");
                }
                else
                {
                    string path2 = Path.Combine(str, "TimerDuringSpawn.txt");
                    if (!File.Exists(path2))
                    {
                        Log.Error(Path.GetFileName(path2) + " file does not exist!");
                    }
                    else
                    {
                        string path3 = Path.Combine(str, "Properties.yml");
                        if (!File.Exists(path3))
                        {
                            Log.Error(Path.GetFileName(path3) + " file does not exist!");
                        }
                        else
                        {
                            string path4 = Path.Combine(str, "Hints.txt");
                            List<string> hints = new List<string>();
                            if (File.Exists(path4))
                                hints.AddRange((IEnumerable<string>)File.ReadAllLines(path4));
                            TimerView timerView = new TimerView(File.ReadAllText(path1), File.ReadAllText(path2), Exiled.Loader.Loader.Deserializer.Deserialize<Properties>(File.ReadAllText(path3)), hints);
                            TimerView.CachedTimers.Add(name, timerView);
                        }
                    }
                }
            }
        }

        public static bool TryGetTimerForPlayer(Player player, out TimerView timerView)
        {
            string key;
            if (player.GroupName != null && Main.Singleton.Config.Timers.TryGetValue(player.GroupName, out key))
            {
                timerView = TimerView.CachedTimers[key];
                return true;
            }
            if (Main.Singleton.Config.Timers.TryGetValue(player.UserId, out key))
            {
                timerView = TimerView.CachedTimers[key];
                return true;
            }
            if (Main.Singleton.Config.Timers.TryGetValue("default", out key))
            {
                timerView = TimerView.CachedTimers[key];
                return true;
            }
            timerView = (TimerView)null;
            return false;
        }

        public string GetText(int? spectatorCount = null)
        {
            this.StringBuilder.Clear();
            this.StringBuilder.Append(!Respawn.IsSpawning ? this.BeforeRespawnString : this.DuringRespawnString);
            this.SetAllProperties(spectatorCount);
            this.StringBuilder.Replace("{RANDOM_COLOR}", string.Format("#{0:X6}", (object)UnityEngine.Random.Range(0, 16777215)));
            this.StringBuilder.Replace('{', '[').Replace('}', ']');
            return this.StringBuilder.ToString();
        }

        internal void IncrementHintInterval()
        {
            ++this.HintInterval;
            if (this.HintInterval != this.Properties.HintInterval)
                return;
            this.HintInterval = 0;
            this.IncrementHintIndex();
        }

        private void IncrementHintIndex()
        {
            ++this.HintIndex;
            if (this.Hints.Count != this.HintIndex)
                return;
            this.HintIndex = 0;
        }

        private TimerView(
          string beforeRespawnString,
          string duringRespawnString,
          Properties properties,
          List<string> hints)
        {
            this.BeforeRespawnString = beforeRespawnString;
            this.DuringRespawnString = duringRespawnString;
            this.Properties = properties;
            this.Hints = hints;
        }

        public string BeforeRespawnString { get; }

        public string DuringRespawnString { get; }

        public Properties Properties { get; }

        public List<string> Hints { get; }

        private void SetAllProperties(int? spectatorCount = null)
        {
            this.SetRoundTime();
            this.SetMinutesAndSeconds();
            this.SetSpawnableTeam();
            this.SetSpectatorCountAndTickets(spectatorCount);
            this.SetWarheadStatus();
            this.SetGeneratorCount();
            this.SetTpsAndTickrate();
            this.SetHint();
        }

        private void SetRoundTime()
        {
            int minutes = Round.ElapsedTime.Minutes;
            this.StringBuilder.Replace("{round_minutes}", string.Format("{0}{1}", !this.Properties.LeadingZeros || minutes >= 10 ? (object)string.Empty : (object)"0", (object)minutes));
            int seconds = Round.ElapsedTime.Seconds;
            this.StringBuilder.Replace("{round_seconds}", string.Format("{0}{1}", !this.Properties.LeadingZeros || seconds >= 10 ? (object)string.Empty : (object)"0", (object)seconds));
        }

        private void SetMinutesAndSeconds()
        {
            TimeSpan timeUntilSpawnWave = Respawn.TimeUntilSpawnWave;
            if (Respawn.IsSpawning || !this.Properties.TimerOffset)
            {
                int num1 = (int)timeUntilSpawnWave.TotalSeconds / 60;
                this.StringBuilder.Replace("{minutes}", string.Format("{0}{1}", !this.Properties.LeadingZeros || num1 >= 10 ? (object)string.Empty : (object)"0", (object)num1));
                int num2 = (int)Math.Round(timeUntilSpawnWave.TotalSeconds % 60.0);
                this.StringBuilder.Replace("{seconds}", string.Format("{0}{1}", !this.Properties.LeadingZeros || num2 >= 10 ? (object)string.Empty : (object)"0", (object)num2));
            }
            else
            {
                int num3 = (double)RespawnTokensManager.GetTeamDominance(SpawnableTeamType.NineTailedFox) == 1.0 ? 18 : 14;
                int num4 = (int)(timeUntilSpawnWave.TotalSeconds + (double)num3) / 60;
                this.StringBuilder.Replace("{minutes}", string.Format("{0}{1}", !this.Properties.LeadingZeros || num4 >= 10 ? (object)string.Empty : (object)"0", (object)num4));
                int num5 = (int)Math.Round((timeUntilSpawnWave.TotalSeconds + (double)num3) % 60.0);
                this.StringBuilder.Replace("{seconds}", string.Format("{0}{1}", !this.Properties.LeadingZeros || num5 >= 10 ? (object)string.Empty : (object)"0", (object)num5));
            }
        }

        private void SetSpawnableTeam()
        {
            switch (Respawn.NextKnownTeam)
            {
                case SpawnableTeamType.ChaosInsurgency:
                    this.StringBuilder.Replace("{team}", !RespawnTimer.API.API.SerpentsHandSpawnable ? this.Properties.Ci : this.Properties.Sh);
                    break;
                case SpawnableTeamType.NineTailedFox:
                    this.StringBuilder.Replace("{team}", !RespawnTimer.API.API.UiuSpawnable ? this.Properties.Ntf : this.Properties.Uiu);
                    break;
            }
        }

        private void SetSpectatorCountAndTickets(int? spectatorCount = null)
        {
            StringBuilder stringBuilder1 = this.StringBuilder;
            int num1;
            string newValue1;
            if (!spectatorCount.HasValue)
            {
                newValue1 = (string)null;
            }
            else
            {
                num1 = spectatorCount.GetValueOrDefault();
                newValue1 = num1.ToString();
            }
            if (newValue1 == null)
            {
                num1 = Player.List.Count<Player>((Func<Player, bool>)(x => x.Role.Team == Team.Dead && !x.IsOverwatchEnabled));
                newValue1 = num1.ToString();
            }
            stringBuilder1.Replace("{spectators_num}", newValue1);
            StringBuilder stringBuilder2 = this.StringBuilder;
            float num2 = Mathf.Round(Respawn.NtfTickets);
            string newValue2 = num2.ToString();
            stringBuilder2.Replace("{ntf_tickets_num}", newValue2);
            StringBuilder stringBuilder3 = this.StringBuilder;
            num2 = Mathf.Round(Respawn.ChaosTickets);
            string newValue3 = num2.ToString();
            stringBuilder3.Replace("{ci_tickets_num}", newValue3);
        }

        private void SetWarheadStatus()
        {
            WarheadStatus status = Warhead.Status;
            this.StringBuilder.Replace("{warhead_status}", this.Properties.WarheadStatus[status]);
            this.StringBuilder.Replace("{detonation_time}", status == WarheadStatus.InProgress ? Mathf.Round(Warhead.DetonationTimer).ToString((IFormatProvider)CultureInfo.InvariantCulture) : string.Empty);
        }

        private void SetGeneratorCount()
        {
            int num1 = 0;
            int num2 = 0;
            foreach (Generator generator in (IEnumerable<Generator>)Generator.List)
            {
                if (generator.State.HasFlag((Enum)GeneratorState.Engaged))
                    ++num1;
                ++num2;
            }
            this.StringBuilder.Replace("{generator_engaged}", num1.ToString());
            this.StringBuilder.Replace("{generator_count}", num2.ToString());
        }

        private void SetTpsAndTickrate()
        {
            this.StringBuilder.Replace("{tps}", Server.Tps.ToString((IFormatProvider)CultureInfo.InvariantCulture));
            this.StringBuilder.Replace("{tickrate}", Application.targetFrameRate.ToString());
        }

        private void SetHint()
        {
            if (!this.Hints.Any<string>())
                return;
            this.StringBuilder.Replace("{hint}", this.Hints[this.HintIndex]);
        }
    }
}
