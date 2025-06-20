namespace PRMainPlugin.Systems.RGBNuke
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Disco : ICommand
    {
        public string Command => "disco";

        public string[] Aliases => new string[] { "disconuke" };

        public string Description => "Start or stop the RGB nuke effect, does not actually start/stop the nuke.\n Usage:\n.disco start - Start Effect\n.disco stop - Stop Effect";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("NG.RGBNuke"))
            {
                response = "no permission";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = Description;
                return false;
            }

            if (arguments.At(0) == "start")
            {
                RGBNuke.Start();
                // Disabled because of bugs on Modded_Main
                //AudioPlayer.PlayAudio();
                response = "Started the effect";
                return true;
            }

            if (arguments.At(0) == "stop")
            {
                RGBNuke.Stop();
                // Disabled because of bugs on Modded_Main
                //AudioPlayer.RemoveDummy();
                response = "Stopped the effect";
                return true;
            }

            response = Description;
            return false;
        }
    }
}
