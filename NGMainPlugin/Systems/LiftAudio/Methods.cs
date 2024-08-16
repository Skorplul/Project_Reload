using AudioSystem.Models.SoundConfigs;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace NGMainPlugin.Systems.Liftaudio
{
    internal class Methods
    {
        /*public static IEnumerator<float> CheckingPlayerLift(Lift lift)
        {
            yield return Timing.WaitForSeconds(0.5f);
            short idlift = -2;
            while (Round.InProgress)
            {
                yield return Timing.WaitForSeconds(2f);
                if (!lift.Players.Any<Player>())
                    AudioSystem.Methods.StopAudio(idlift);
                else if (!AudioSystem.Methods.IsPlaying(idlift))
                    idlift = NGMainPlguin.Instance.Config.ListOfPossibeMusics.RandomItem().PlayPreset(lift.Transform, true);
            }
        }*/
    }
}
