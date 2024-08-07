using Exiled.API.Features;
using Random = System.Random;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;

namespace NGMainPlugin.Systems.RGBNuke;

public class EventHandler
{
    private static int Chance => Main.Singleton.Config.NukeChance;
    
    public bool _isEnabled = true;

    public void OnRoundStart()
    {
        var random = new Random(Map.Seed);
        _isEnabled = random.Next(0, 100) < Chance;

        // Make sure leftovers from previous round are gone
        NukeHandler.Stop();
        AudioPlayer.RemoveDummy();

        if (_isEnabled)
        {
            Log.Debug("Rgb nuke is active this round!");
        }
    }

    public void OnRoundEnd(RoundEndedEventArgs ev)
    {
        NukeHandler.Stop();
        AudioPlayer.RemoveDummy();
    }

    public void OnWarheadStart(StartingEventArgs ev)
    {
        if (!_isEnabled) return;

        NukeHandler.Start();
        AudioPlayer.PlayAudio();
    }

    public void OnWarheadDetonation()
    {
        NukeHandler.Stop();
        AudioPlayer.StopAudio();
    }

    public void OnWarheadStop(StoppingEventArgs ev)
    {
        NukeHandler.Stop();
        AudioPlayer.StopAudio();
    }
}