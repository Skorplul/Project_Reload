using CommandSystem;
using System;
using Exiled.API.Features;
using NGMainPlugin.DB;

namespace NGMainPlugin.Systems.RespawnTimer.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DataAgree : ICommand
    {
        public string Command { get; } = "DataAgree";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Agree to the collection of your data by the server.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Log.Info($"Player agreed to data collection!");

            Database.CreatePlayerInfo(Player.Get(sender));

            response = "<color=yellow>You have agreed to the data collection.</color>";
            return true;
        }
    }
}