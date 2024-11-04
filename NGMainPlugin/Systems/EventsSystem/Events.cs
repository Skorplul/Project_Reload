namespace NGMainPlugin.Systems.EventsSystem
{
    using System;
    using NGMainPlugin.API;
    using NGMainPlugin.Systems.EventsSystem;
    using Exiled.API.Features;
    using PlayerRoles;
    using Exiled.API.Extensions;

    /// <summary>
    /// The functions execute Eventrounds on the Server. Ask Skorp for defenitions.
    /// </summary>
    public static class Events
    {
        static Random random= new Random();

        public static void Virus()
        {
            EventsSystemHandler.eventRoundType = EventsType.Virus;
            EventsAPI.EventRound = true;
            Respawn.TimeUntilNextPhase = -1;
            Warhead.AutoDetonate = false;
            
            foreach (Player ply in Player.List) 
            {
                ply.Role.Set(RoleTypeId.ClassD);
            }

            Player VP = Player.List.GetRandomValue();
            VP.Role.Set(RoleTypeId.Scp0492);

        }
    }
}
