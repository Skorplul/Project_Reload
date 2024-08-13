using CommandSystem;
using System;

namespace NGMainPlugin.Systems.LobbySystem
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LockCommand : ParentCommand
    {
        public static Main Plugin { get; set; }

        public LockCommand()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "locklobby";

        public override string Description { get; } = "Locks/unlocks the lobby!";

        public override string[] Aliases { get; } = new string[] { "llock" };

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You do not have <color=yellow>RoundEvents</color> permission!";
                return false;
            }
            Handler.IsLocked = !Handler.IsLocked;
            response = Handler.IsLocked ? "Lobby has been <color=red>locked</color>!" : "Lobby has been <color=green>unlocked</color>!";
            return true;
        }
    }
}