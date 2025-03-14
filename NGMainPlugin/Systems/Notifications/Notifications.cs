namespace NGMainPlugin.Systems.Notifications
{
    internal static class Notifications
    {
        internal static Config Config;

        public static void Enable()
        {
            //Exiled.Events.Handlers.Player.Banning += Ban_Kick.OnBanning;
            //Exiled.Events.Handlers.Player.Kicked += Ban_Kick.OnKick;
            Exiled.Events.Handlers.Server.RoundStarted += LastTeamMember.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += LastTeamMember.OnRoundEnded;
        }

        public static void Disable()
        {
            //Exiled.Events.Handlers.Player.Banning -= Ban_Kick.OnBanning;
            //Exiled.Events.Handlers.Player.Kicked -= Ban_Kick.OnKick;
            Exiled.Events.Handlers.Server.RoundStarted -= LastTeamMember.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= LastTeamMember.OnRoundEnded;
        }
    }
}