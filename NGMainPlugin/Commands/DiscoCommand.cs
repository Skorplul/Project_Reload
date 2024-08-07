using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace NGMainPlugin.Systems.RGBNuke;

public class DiscoCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DiscoNuke : ParentCommand
    {
        public DiscoNuke() => LoadGeneratedCommands();

        public override string Command { get; } = "rgbnuke";

        public override string[] Aliases => new string[] {"disco", "disconuke"};

        public override string Description { get; } = "Start or stop the RGB nuke effect, does not actually start/stop the nuke.";

        private const string Usage = "You can start or stop the RGB nuke effect:\n" +
                                     "Start the effect - disco start\n" +
                                     "Stop the effect - disco stop";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("NG.RGBNuke"))
            {
                response = "no permission";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = Usage;
                return false;
            }

            if (arguments.At(0) == "start")
            {
                NukeHandler.Start();
                // Disabled because of bugs on Modded_Main
                //AudioPlayer.PlayAudio();
                response = "Started the effect";
                return true;
            }

            if (arguments.At(0) == "stop")
            {
                NukeHandler.Stop();
                // Disabled because of bugs on Modded_Main
                //AudioPlayer.RemoveDummy();
                response = "Stopped the effect";
                return true;
            }

            response = Usage;
            return false;
        }
    }
}