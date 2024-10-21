namespace NGMainPlugin.Systems.RespawnTimer.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Timer : ICommand
    { 
        public string Command => "timer";

        public string[] Aliases => new string[] { };

        public string Description => "Hide or show the respawn timer.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "<color=green>Respawn Timer has been shown!</color>";

            if (RespawnTimer.ToggleTimerForPlayer(Player.Get(sender)))
                response = "<color=red>Respawn Timer has been hidden!</color>";

            return true;
        }
    }
}