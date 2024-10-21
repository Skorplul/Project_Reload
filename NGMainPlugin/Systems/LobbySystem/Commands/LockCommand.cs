namespace NGMainPlugin.Systems.LobbySystem.Commands
{
    using CommandSystem;
    using System;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LockCommand : ParentCommand
    {
        public LockCommand()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "locklobby";

        public override string Description { get; } = "Locks/unlocks the lobby!";

        public override string[] Aliases { get; } = new string[] { "llobby" };

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You do not have <color=yellow>RoundEvents</color> permission!";
                return false;
            }
            LobbySystem.IsLocked = !LobbySystem.IsLocked;
            response = LobbySystem.IsLocked ? "Lobby has been <color=red>locked</color>!" : "Lobby has been <color=green>unlocked</color>!";
            return true;
        }
    }
}