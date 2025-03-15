using CommandSystem;
using PRMainPlugin.API.Enums;
using PRMainPlugin.Systems.Subclasses;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace PRMainPlugin.Commands.Admin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ForceSubClass : ICommand
{
    public string Command => "forcesubclass";

    public string[] Aliases => new string[] { "fsc" };

    public string Description => "Force a user into a SubClass.";

    public bool SanitizeResponse => true;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);

        if (arguments.Array[1].ToLower() == "help")
        {
            response = "Usage: fsc <subclass> <user>(optional)";
            return true;
        }
        if (arguments.Array[1].ToLower() == "list")
        {
            response = "valid Subclasses:\n Fettsack\n BugsBunny\n Drogendealer\n Kind\n Blitz\n Kamikaze\n AllSeeing\n Sondereinheit";
            return true;
        }
        if (arguments.Count < 1)
        {
            response = "Usage: fsc <subclass> <user>(optional)";
            return false;
        }

        if (!Permissions.CheckPermission(player, PlayerPermissions.ForceclassWithoutRestrictions))
        {
            response = "No Permission.";
            return false;
        }

        switch (arguments.Array[1].ToLower())
        {
            case "fettsack":
                Fettsack.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "bugsbunny":
                BugBunny.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "drogendealer":
                Drogendealer.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "kind":
                Kind.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "blitz":
                Blitz.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "kamikaze":
                Kamikaze.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "allseeing":
                Allseeing.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2]));
                response = "Set SubClass.";
                return true;
            case "sondereinheit":
                Sondereinheit.SetRole(arguments.Count == 1 ? player : Player.Get(arguments.Array[2])); // ToDo: erst nach der dritten Welle Spawnbar.
                response = "Set SubClass.";
                return true;
            case "spy":
                //Spy.SetRole(); (Skin swap needed for this)
                response = "Set SubClass.";
                return true;
            default:
                response = "Something went wrong, contact a dev!";
                return false;
        }
    }

}

