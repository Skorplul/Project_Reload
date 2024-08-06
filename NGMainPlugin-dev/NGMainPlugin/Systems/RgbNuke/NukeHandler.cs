using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace NGMainPlugin.Systems.RGBNuke;

public static class NukeHandler
{
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

    private static float WaitTime => Main.Singleton.Config.ColorChangeTime;
    private static CoroutineHandle _handle;

    public static void Start()
    {
        if (_handle.IsValid)
        {
            Timing.KillCoroutines(_handle);
        }

        _handle = Timing.RunCoroutine(Run());
    }

    public static void Stop()
    {
        Timing.KillCoroutines(_handle);

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
            yield return Timing.WaitForSeconds(WaitTime);
        }
    }
}