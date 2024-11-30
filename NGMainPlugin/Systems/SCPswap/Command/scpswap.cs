namespace NGMainPlugin.Systems.SCPSwap.Command
{
    using CommandSystem;
    using System;
    using Exiled.API.Features;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class SCPSwapCommand : ICommand
    {
        public string Command => "scpswap";

        public string[] Aliases => new string[] { "scps" };

        public string Description => "Swap your scp role!";

        public bool SanitizeResponse => true;

        public static List<string> swaped = new List<string>();

        public TimeSpan NoSwapTime = TimeSpan.FromSeconds(SCPSwap.Config.ScpSwapTimeout);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count < 1)
            {
                response = "Usage: .scpswap {scpNumber}";
                return false;
            }
            if (player == null)
            {
                response = "ERROR: The player is null, please contact an admin/developer!";
                return false;
            }
            if (!player.IsScp)
            {
                response = "You can only use this as SCP.";
                return false;
            }
            if (TimeSpan.FromSeconds(SCPSwap.Config.ScpSwapTimeout) == null)
            {
                response = "Timeout time hasn't been set properly, please contact an admin/developer!";
                return false;
            }
            if (Round.ElapsedTime > NoSwapTime)
            {
                response = "You can no longer swap, sorry but you're too late.";
                return false;
            }
            if (arguments.Array[1] == "0492" || arguments.Array[1] == "049-2")
            {
                response = "You can't swap to a Zombie";
                return false;
            }
            RoleTypeId GetSwap(string scpNum)
            {
                switch (scpNum)
                {
                    case "049":
                        return RoleTypeId.Scp049;
                    case "079":
                        return RoleTypeId.Scp079;
                    case "096":
                        return RoleTypeId.Scp096;
                    case "106":
                        return RoleTypeId.Scp106;
                    case "939":
                        return RoleTypeId.Scp939;
                    case "173":
                        return RoleTypeId.Scp173;
                    case "3114":
                        return RoleTypeId.Scp3114;
                    default:
                        return RoleTypeId.None;
                }
            }
            if (GetSwap(arguments.Array[1]) == RoleTypeId.None)
            {
                response = "Not a valid SCP!";
                return false;
            }
            if (!SCPSwap.Config.SwapableScps.Contains(GetSwap(arguments.Array[1])))
            {
                response = "You can not swap to this SCP!";
                return false;
            }
            if (GetSwap(arguments.Array[1]) == RoleTypeId.Scp3114)
            {
                if (Server.PlayerCount < SCPSwap.Config.SkelliCount)
                {
                    response = string.Format("You can only swap to SCP-3114, if the player count is {0} or greater!", SCPSwap.Config.SkelliCount);
                    return false;
                }
            }
            if (player.Role == GetSwap(arguments.Array[1]))
            {
                response = "You can't swap to the same SCP you already are!";
                return false;
            }
            foreach (Player ply in Player.List)
            {
                if (ply.IsScp)
                {
                    if (ply.Role == GetSwap(arguments.Array[1]))
                    {
                        response = "Someone is this SCP already!";
                        return false;
                    }
                }
            }
            if (SCPSwap.Config.SingleSwap)
            {
                if (swaped.Contains(player.UserId))
                {
                    response = "You already swapped in this round!";
                    return false;
                }
            }

            swaped.Add(player.UserId);

            player.Role.Set(GetSwap(arguments.Array[1]));

            response = string.Format("You are now SCP{0}!", arguments.Array[1]);
            return true;
        }
    }
}
