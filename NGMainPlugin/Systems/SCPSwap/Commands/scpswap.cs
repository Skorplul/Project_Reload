namespace NGMainPlugin.Systems.SCPSwap.Commands
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

        public string Description => Config.Translations.CommandDescription;

        public bool SanitizeResponse => true;


        public static List<string> swaped = new List<string>();

        public TimeSpan NoSwapTime = TimeSpan.FromSeconds(SCPSwap.Config.ScpSwapTimeout);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (arguments.Count < 1)
            {
                response = Config.Translations.CommandUsage;
                return false;
            }
            if (player == null)
            {
                response = Config.Translations.PlayerNullError;
                return false;
            }
            if (!player.IsScp)
            {
                response = Config.Translations.NotAnSCP;
                return false;
            }
            if (TimeSpan.FromSeconds(SCPSwap.Config.ScpSwapTimeout) == null)
            {
                response = Config.Translations.TimeoutError;
                return false;
            }
            if (Round.ElapsedTime > NoSwapTime)
            {
                response = Config.Translations.Timeout;
                return false;
            }
            if (arguments.Array[1] == "0492" || arguments.Array[1] == "049-2")
            {
                response = Config.Translations.Zombie;
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
                response = Config.Translations.NotValidSCP;
                return false;
            }
            if (!SCPSwap.Config.SwapableScps.Contains(GetSwap(arguments.Array[1])))
            {
                response = Config.Translations.CantSwap;
                return false;
            }
            if (GetSwap(arguments.Array[1]) == RoleTypeId.Scp3114)
            {
                if (Server.PlayerCount < SCPSwap.Config.SkelliCount)
                {
                    response = string.Format(Config.Translations.CantSwapSkeli, SCPSwap.Config.SkelliCount);
                    return false;
                }
            }
            if (player.Role == GetSwap(arguments.Array[1]))
            {
                response = Config.Translations.CantSwapSame;
                return false;
            }
            foreach (Player ply in Player.List)
            {
                if (ply.IsScp)
                {
                    if (ply.Role == GetSwap(arguments.Array[1]))
                    {
                        response = Config.Translations.SCPAlreadyUsed;
                        return false;
                    }
                }
            }
            if (SCPSwap.Config.SingleSwap)
            {
                if (swaped.Contains(player.UserId))
                {
                    response = Config.Translations.AlreadySwapped;
                    return false;
                }
            }

            swaped.Add(player.UserId);

            player.Role.Set(GetSwap(arguments.Array[1]));

            response = string.Format(Config.Translations.SwapSuccess, arguments.Array[1]);
            return true;
        }
    }
}
