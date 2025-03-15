namespace PRMainPlugin.Commands;

using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

[CommandHandler(typeof(ClientCommandHandler))]
public class C4Charge : ParentCommand
{
    public C4Charge()
    {
        LoadGeneratedCommands();
    }

    public override string Command { get; } = "detonate";

    public override string[] Aliases { get; } = new string[] { "det" };

    public override string Description { get; } = "Detonate command for detonating C4 charges";

    public override void LoadGeneratedCommands() { }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player ply = Player.Get(sender);

        if (!Items.C4Charge.PlacedCharges.ContainsValue(ply))
        {
            response = "\n<color=red>You've haven't placed any C4 charges!</color>";
            return false;
        }

        if (Items.C4Charge.Instance.RequireDetonator && (ply.CurrentItem is null || ply.CurrentItem.Type != Items.C4Charge.Instance.DetonatorItem))
        {
            response = $"\n<color=red>You need to have a Remote Detonator ({Items.C4Charge.Instance.DetonatorItem}) in your hand to detonate C4!</color>";
            return false;
        }

        int i = 0;

        foreach (var charge in Items.C4Charge.PlacedCharges.ToList())
        {
            if (charge.Value != ply)
                continue;

            float distance = Vector3.Distance(charge.Key.Position, ply.Position);

            if (distance < Items.C4Charge.Instance.MaxDistance)
            {
                Items.C4Charge.Instance.C4Handler(charge.Key);

                i++;
            }
            else
            {
                ply.SendConsoleMessage($"One of your charges is out of range. You need to get closer by {Mathf.Round(distance - Items.C4Charge.Instance.MaxDistance)} meters.", "yellow");
            }
        }

        response = i == 1 ? $"\n<color=green>{i} C4 charge has been detonated!</color>" : $"\n<color=green>{i} C4 charges have been deonated!</color>";

        return true;
    }
}