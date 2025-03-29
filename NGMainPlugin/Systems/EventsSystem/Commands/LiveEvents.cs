namespace PRMainPlugin.Systems.EventsSystem.Commands
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using Exiled.API.Enums;
    using LightContainmentZoneDecontamination;
    using PRMainPlugin.API;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class LiveEvents : ICommand
    {
        public string Command => "LiveEvents";

        public string[] Aliases => new string[] { };

        public string Description => "Used to make events manualy executed from the Team for the communty.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Player is null! Please contact a Developer or Admin!";
                return false;
            }
            if (!player.CheckPermission("NG.Events"))
            {
                response = "You don't have permission to use this command.";
                return false;
            }
            //due to two idiots
            response = "Piss dich entweder beta oder sirius";
            return false;

            Respawn.PauseWaves();
            Warhead.AutoDetonate = false;

            foreach (Player ply in Player.List)
            {
                if (ply.IsMuted)
                {
                    EventsAPI.MutedBeforeEvent.Add(ply);
                }

                ply.Role.Set(RoleTypeId.Tutorial);
                if (!ply.RemoteAdminAccess)
                    ply.Mute();
            }

            response = "The Event Mode Has Been Activated!";
            return true;
        }
    }

}

