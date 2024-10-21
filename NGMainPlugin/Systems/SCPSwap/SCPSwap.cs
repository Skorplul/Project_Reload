namespace NGMainPlugin.Systems.SCPSwap
{
    internal static class SCPSwap
    {
        internal static Config Config;

        public static void Enable()
        {
            Config.LoadConfig();

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        }

        private static void OnRoundStarted()
        {
            Commands.SCPSwapCommand.swaped.Clear();
        }
    }
}
