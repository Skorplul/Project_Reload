namespace NGMainPlugin.Systems.EventsSystem
{
    using System;
    using NGMainPlugin.API;
    using Exiled.API.Features;
    using PlayerRoles;
    using Exiled.API.Extensions;
    using System.Collections.Generic;
    using MEC;

    /// <summary>
    /// The functions execute Eventrounds on the Server. Ask Skorp for defenitions.
    /// </summary>
    public static class Events
    {
        static Random random= new Random();
        public static List<Player> scpList = new List<Player>();

        private static IEnumerator<float> DoVirus()
        {
            if (!Round.IsStarted)
                yield return Timing.WaitForSeconds(1);
            
            foreach (Player ply in Player.List) 
            {
                ply.Role.Set(RoleTypeId.ClassD);
            }

            Player VP = Player.List.GetRandomValue();
            VP.Role.Set(RoleTypeId.Scp0492);
            Respawn.TimeUntilNextPhase = -1;
            yield return Timing.KillCoroutines();
        }
        private static IEnumerator<float> DoPeanutRun(int timeToNuke)
        {
            if (!Round.IsStarted)
                yield return Timing.WaitForSeconds(1);
            
            yield return Timing.WaitForSeconds(timeToNuke);
            Respawn.TimeUntilNextPhase = -1;
            Warhead.DetonationTimer = 90;
            Warhead.IsLocked = true;
            Warhead.Start();
            yield return Timing.KillCoroutines();
        }
        private static IEnumerator<float> DoLightsOut()
        {
            if (!Round.IsStarted)
                yield return Timing.WaitForSeconds(1);

            foreach (Player ply in Player.List)
            {
                if (ply.IsScp)
                {
                    scpList.Add(ply);
                }
                else
                {
                    ply.AddItem(ItemType.Lantern);
                }
            }
            int NutP = random.Next(0, scpList.Count);
            scpList[NutP].Role.Set(RoleTypeId.Scp173);
            
            Map.TurnOffAllLights(3000);
            yield return Timing.KillCoroutines();
        }

        public static void Virus()
        {
            EventsSystemHandler.eventRoundType = EventsType.Virus;
            EventsAPI.EventRound = true;
            Warhead.AutoDetonate = false;
            
            Timing.RunCoroutine(DoVirus());
        }

        public static void PeanutRun()
        {
            EventsSystemHandler.eventRoundType = EventsType.PeanutRun;
            EventsAPI.EventRound = true;
            Warhead.AutoDetonate = false;

            Timing.RunCoroutine(DoPeanutRun(10));
        }

        public static void LightsOut()
        {
            EventsSystemHandler.eventRoundType = EventsType.LightsOut;
            EventsAPI.EventRound = true;

            Timing.RunCoroutine(DoLightsOut());
        }
    }
}
