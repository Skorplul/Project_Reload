namespace NGMainPlugin.Systems.RGBNuke
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.EventArgs.Warhead;
    using MEC;
    using System.Collections.Generic;
    using UnityEngine;

    public class RGBNuke
    {
        internal static Config Config;

        private static bool isEnabled = false;

        private static CoroutineHandle NukeCoroutine;

        private static readonly Color[] NukeColors = new Color[]
        {
            Color.red,
            new Color(1, 0.65f, 0, 1),
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            Color.magenta
        };

        public static void Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
            Exiled.Events.Handlers.Warhead.Starting += OnWarheadStart;
            Exiled.Events.Handlers.Warhead.Detonated += OnWarheadDetonation;
            Exiled.Events.Handlers.Warhead.Stopping += OnWarheadStop;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
            Exiled.Events.Handlers.Warhead.Starting -= OnWarheadStart;
            Exiled.Events.Handlers.Warhead.Detonated -= OnWarheadDetonation;
            Exiled.Events.Handlers.Warhead.Stopping -= OnWarheadStop;
        }

        private static void OnRoundStart()
        {
            var random = new System.Random(Map.Seed);
            isEnabled = random.Next(0, 100) < Config.NukeChance;

            // Make sure leftovers from previous round are gone
            Stop();
            AudioPlayer.RemoveDummy();

            if (isEnabled)
            {
                Log.Debug("Rgb nuke is active this round!");
            }
        }

        private static void OnRoundEnd(RoundEndedEventArgs ev)
        {
            Stop();
            AudioPlayer.RemoveDummy();
        }

        private static void OnWarheadStart(StartingEventArgs ev)
        {
            if (!isEnabled) return;

            Start();
            AudioPlayer.PlayAudio();
        }

        private static void OnWarheadDetonation()
        {
            Stop();
            AudioPlayer.StopAudio();
        }

        private static void OnWarheadStop(StoppingEventArgs ev)
        {
            Stop();
            AudioPlayer.StopAudio();
        }


        internal static void Start()
        {
            if (NukeCoroutine.IsValid)
            {
                Timing.KillCoroutines(NukeCoroutine);
            }

            NukeCoroutine = Timing.RunCoroutine(Run());
        }

        internal static void Stop()
        {
            Timing.KillCoroutines(NukeCoroutine);

            foreach (var instance in RoomLightController.Instances)
            {
                instance.NetworkOverrideColor = Color.clear;
            }
        }

        private static IEnumerator<float> Run()
        {
            var i = 0;
            while (true)
            {
                foreach (var instance in RoomLightController.Instances)
                {
                    instance.NetworkOverrideColor = NukeColors[i];
                }

                i = (i + 1) % NukeColors.Length;
                yield return Timing.WaitForSeconds(Config.ColorChangeTime);
            }
        }
    }
}