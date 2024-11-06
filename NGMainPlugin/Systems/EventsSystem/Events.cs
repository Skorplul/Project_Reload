namespace NGMainPlugin.Systems.EventsSystem
{
    using System;
    using NGMainPlugin.API;
    using Exiled.API.Features;
    using PlayerRoles;
    using Exiled.API.Extensions;
    using System.Collections.Generic;
    using MEC;
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;

    /// <summary>
    /// The functions execute Eventrounds on the Server. Ask Skorp for defenitions.
    /// </summary>
    public static class Events
    {
        static Random random= new Random();
        public static List<Player> scpList = new List<Player>();

        private static IEnumerator<float> DoVirus()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);
            
            foreach (Player ply in Player.List) 
            {
                ply.Role.Set(RoleTypeId.ClassD);
            }

            Player VP = Player.List.GetRandomValue();
            VP.Role.Set(RoleTypeId.Scp0492);
            Respawn.TimeUntilNextPhase = -1;
        }
        private static IEnumerator<float> DoPeanutRun(int timeToNuke)
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);
            
            yield return Timing.WaitForSeconds(timeToNuke);
            Respawn.TimeUntilNextPhase = -1;
            Warhead.DetonationTimer = Config.PeanutRunTimeToExplode;
            Warhead.IsLocked = true;
            Warhead.Start();
        }
        private static IEnumerator<float> DoLightsOut()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            foreach (Player ply in Player.List)
            {
                if (ply.IsScp)
                {
                    scpList.Add(ply);
                }
                else
                {
                    ply.Role.Set(RoleTypeId.ClassD);
                    ply.AddItem(ItemType.Lantern);
                }
            }
            int NutP = random.Next(0, scpList.Count);
            scpList[NutP].Role.Set(RoleTypeId.Scp173);
            
            Map.TurnOffAllLights(3000);
        }
        private static IEnumerator<float> DoCockFight()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            Respawn.TimeUntilNextPhase = -1;
            Warhead.AutoDetonate = false;
            Warhead.IsLocked = true;

            foreach (Door door in Door.List)
            {
                if (door.IsCheckpoint)
                {
                    door.Lock(5000, DoorLockType.AdminCommand);
                }
            }

            int i = Config.JailbirdFightStartTime;
            foreach (Player ply in Player.List)
            {
                ply.Role.Set(RoleTypeId.Scientist);
                ply.AddItem(ItemType.Jailbird, 8);
                ply.EnableEffect(EffectType.Ensnared, i);
            }

            while (i > 0)
            {
                if (i >= 5)
                {
                    Map.Broadcast(1, $"<color=red>Start in {i}</color>");
                    i--;
                }

                //Colors have to be reworked!
                switch (i)
                {
                    case 4:
                        Map.Broadcast(1, $"<color=ff7777>Start in {i}</color>");
                        i--;
                        break;
                    case 3:
                        Map.Broadcast(1, $"<color=ffdddd>Start in {i}</color>");
                        i--;
                        break;
                    case 2:
                        Map.Broadcast(1, $"<color=ddffdd>Start in {i}</color>");
                        i--;
                        break;
                    case 1:
                        Map.Broadcast(1, $"<color=77ff77>Start in {i}</color>");
                        i--;
                        break;
                    case 0:
                        Map.Broadcast(1, $"<color=00ff00>Start!</color>");
                        i--;
                        break;
                    default:
                        break;
                }
                yield return Timing.WaitForSeconds(1);
            }
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

            Timing.RunCoroutine(DoPeanutRun(Config.PeanutRunTimeToNuke));
        }

        public static void LightsOut()
        {
            EventsSystemHandler.eventRoundType = EventsType.LightsOut;
            EventsAPI.EventRound = true;

            Timing.RunCoroutine(DoLightsOut());
        }

        public static void CockFight()
        {
            EventsSystemHandler.eventRoundType = EventsType.CockFight;
            EventsAPI.EventRound = true;

            Timing.RunCoroutine(DoCockFight());
        }
    }
}
