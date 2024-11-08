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
    using Exiled.API.Features.Roles;
    using PlayerRoles.PlayableScps.Scp3114;

    /// <summary>
    /// The functions execute Eventrounds on the Server. Ask Skorp for defenitions.
    /// </summary>
    public static class Events
    {
        static Random random= new Random();
        public static List<Player> TempList = new List<Player>();
        private static Room NutSpawn;
        private static UnityEngine.Vector3 NutSpwanVector = UnityEngine.Vector3.up*6;

        private static IEnumerator<float> DoVirus()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);
            
            foreach (Player ply in Player.List) 
            {
                ply.Role.Set(RoleTypeId.ClassD);
            }

            Player.List.GetRandomValue().Role.Set(RoleTypeId.Scp0492);
            Respawn.TimeUntilNextPhase = -1;
        }
        private static IEnumerator<float> DoPeanutRun()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);
            
            foreach (Room room in Room.List)
            {
                if (room.Type == RoomType.Lcz173)
                {
                    NutSpawn = room;
                }
            }

            Door.LockAll(Config.PeanutRunTimeToNuke, DoorLockType.AdminCommand);

            foreach (Player ply in Player.List)
            {
                ply.Role.Set(RoleTypeId.Scp173);
                ply.Teleport(NutSpawn, NutSpwanVector);
            }
            
            yield return Timing.WaitForSeconds(Config.PeanutRunTimeToNuke);
            foreach (Door door in Door.List)
            {
                door.IsOpen = true;
            }
            Respawn.TimeUntilNextPhase = -1;
            Warhead.DetonationTimer = Config.PeanutRunTimeToExplode;
            Warhead.Start();
            Warhead.IsLocked = true;
        }
        private static IEnumerator<float> DoLightsOut()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            foreach (Player ply in Player.List)
            {
                if (ply.IsScp)
                {
                    TempList.Add(ply);
                }
                else
                {
                    ply.Role.Set(RoleTypeId.ClassD);
                    ply.AddItem(ItemType.Lantern);
                }
            }
            int NutP = random.Next(0, TempList.Count);
            TempList[NutP].Role.Set(RoleTypeId.Scp173);
            
            Map.TurnOffAllLights(3000);
        }
        private static IEnumerator<float> DoParticleFight()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);

            Round.IsLocked = true;
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
                ply.AddItem(ItemType.ParticleDisruptor, 8);
                ply.EnableEffect(EffectType.Ensnared, i);
            }

            while (i > -3)
            {
                if (i >= 5)
                {
                    Map.Broadcast(1, $"<color=red>Start in {i}</color>");
                    i--;
                }

                switch (i)
                {
                    case 4:
                        Map.Broadcast(1, $"<color=#ff4800>Start in {i}</color>");
                        i--;
                        break;
                    case 3:
                        Map.Broadcast(1, $"<color=#ff8000>Start in {i}</color>");
                        i--;
                        break;
                    case 2:
                        Map.Broadcast(1, $"<color=#ffb700>Start in {i}</color>");
                        i--;
                        break;
                    case 1:
                        Map.Broadcast(1, $"<color=#ffff00>Start in {i}</color>");
                        i--;
                        break;
                    case 0:
                        Map.Broadcast(1, $"<color=#62ff00>Start!</color>");
                        i--;
                        break;
                    default:
                        i = -10;
                        break;
                }
                yield return Timing.WaitForOneFrame;
            }
            foreach (Player ply in Player.List)
            {
                    TempList.Add(ply);
            }
            Server.FriendlyFire = true;
            yield return Timing.WaitForSeconds(1);

            while (Round.IsStarted)
            {
                foreach (Player ply in Player.List)
                {
                    if (ply.IsAlive)
                    {
                        TempList.Remove(ply);
                    }
                }
                if (TempList.Count <= 1)
                {
                    Round.IsLocked = false;
                }
                yield return Timing.WaitForOneFrame;
            }
        }
        private static IEnumerator<float> DoSkelett()
        {
            yield return Timing.WaitUntilTrue(() => Round.IsStarted);
            
            foreach (Player ply in Player.List) 
            {
                ply.Role.Set(RoleTypeId.ClassD);
                ply.EnableEffect(EffectType.Ensnared, 1, 5);
            }
            foreach (Door door in Door.List)
            {
                if (door.IsCheckpoint)
                {
                    door.Lock(3000, DoorLockType.AdminCommand);
                }
            }

            Player VP = Player.List.GetRandomValue();
            VP.Role.Set(RoleTypeId.Scp3114, RoleSpawnFlags.None);
            VP.DisableAllEffects();
            Ragdoll.CreateAndSpawn(RoleTypeId.ClassD, VP.Nickname, "Use This for the MiniGame", VP.Position + UnityEngine.Vector3.forward, VP.Rotation, VP);
            Respawn.TimeUntilNextPhase = -1;
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

            Timing.RunCoroutine(DoPeanutRun());
        }

        public static void LightsOut()
        {
            EventsSystemHandler.eventRoundType = EventsType.LightsOut;
            EventsAPI.EventRound = true;

            Timing.RunCoroutine(DoLightsOut());
        }

        public static void ParticleFight()
        {
            EventsSystemHandler.eventRoundType = EventsType.ParticleFight;
            EventsAPI.EventRound = true;

            Timing.RunCoroutine(DoParticleFight());
        }
    }
}
