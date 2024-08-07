using CommandSystem;
using System;
using Exiled.API.Features;
using NGMainPlugin.Systems.RespawnTimer.API;

namespace NGMainPlugin.Systems.RespawnTimer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Timer : ParentCommand
    { 
        public static Main Plugin { get; set; }

        public Timer()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "timer";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Hide or show the respawn timer.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string userId = Player.Get(sender).UserId;
            if (!NGMainPlugin.Systems.RespawnTimer.API.API.TimerHidden.Remove(userId))
            {
                RespawnTimer.API.API.TimerHidden.Add(userId);
                response = "<color=red>Respawn Timer has been hidden!</color>";
                return true;
            }
            response = "<color=green>Respawn Timer has been shown!</color>";
            return true;
        }
    }
}