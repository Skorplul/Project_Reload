using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using NGMainPlugin.Systems.RespawnTimer.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace NGMainPlugin.Systems.RespawnTimer
{
    public static class EventHandler
    {
        private static CoroutineHandle _timerCoroutine;
        private static CoroutineHandle _hintsCoroutine;
        private static readonly Dictionary<Exiled.API.Features.Player, CoroutineHandle> PlayerDeathDictionary = new Dictionary<Exiled.API.Features.Player, CoroutineHandle>(25);

        internal static void OnGenerated()
        {
            if (Main.Singleton.Config.ReloadTimerEachRound)
                Main.Singleton.OnReloaded();
            if (EventHandler._timerCoroutine.IsRunning)
                Timing.KillCoroutines(EventHandler._timerCoroutine);
            if (!EventHandler._hintsCoroutine.IsRunning)
                return;
            Timing.KillCoroutines(EventHandler._hintsCoroutine);
        }

        internal static void OnRoundStart()
        {
            try
            {
                EventHandler._timerCoroutine = Timing.RunCoroutine(EventHandler.TimerCoroutine());
                EventHandler._hintsCoroutine = Timing.RunCoroutine(EventHandler.HintsCoroutine());
            }
            catch (Exception ex)
            {
                Log.Error((object)ex);
            }
            Log.Debug("RespawnTimer coroutine started successfully!");
        }

        internal static void OnDying(DyingEventArgs ev)
        {
            if ((double)Main.Singleton.Config.TimerDelay < 0.0)
                return;
            if (EventHandler.PlayerDeathDictionary.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(EventHandler.PlayerDeathDictionary[ev.Player]);
                EventHandler.PlayerDeathDictionary.Remove(ev.Player);
            }
            EventHandler.PlayerDeathDictionary.Add(ev.Player, Timing.CallDelayed(Main.Singleton.Config.TimerDelay, (Action)(() => EventHandler.PlayerDeathDictionary.Remove(ev.Player))));
        }

        private static IEnumerator<float> TimerCoroutine()
        {
            yield return Timing.WaitForSeconds(1f);
            Log.Debug("Start");
            do
            {
                yield return Timing.WaitForSeconds(1f);
                Log.Debug("Tick");
                int specNum = Exiled.API.Features.Player.List.Count<Exiled.API.Features.Player>((Func<Exiled.API.Features.Player, bool>)(x => !x.IsAlive || x.SessionVariables.ContainsKey("IsGhost")));
                foreach (Exiled.API.Features.Player player in (IEnumerable<Exiled.API.Features.Player>)Exiled.API.Features.Player.List)
                {
                    try
                    {
                        if (!player.IsAlive || player.SessionVariables.ContainsKey("IsGhost"))
                        {
                            if (!player.IsOverwatchEnabled || !Main.Singleton.Config.HideTimerForOverwatch)
                            {
                                if (!RespawnTimer.API.API.TimerHidden.Contains(player.UserId))
                                {
                                    if (!EventHandler.PlayerDeathDictionary.ContainsKey(player))
                                    {
                                        TimerView timerView;
                                        if (TimerView.TryGetTimerForPlayer(player, out timerView))
                                        {
                                            string text = timerView.GetText(new int?(specNum));
                                            player.ShowHint(text, 1.25f);
                                            timerView = (TimerView)null;
                                            text = (string)null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error((object)ex);
                    }
                }
            }
            while (!Round.IsEnded);
        }

        private static IEnumerator<float> HintsCoroutine()
        {
            do
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (TimerView timerView in TimerView.CachedTimers.Values)
                    timerView.IncrementHintInterval();
            }
            while (!Round.IsEnded);
        }
    }
}
