using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;

namespace NGMainPlugin.Systems.RemoteKeycard
{
    public static class RemoteKeycards
    {
        internal static Config Config;

        public static void Enable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnDoorInteract;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnGeneratorUnlock;
            Exiled.Events.Handlers.Player.InteractingLocker += OnLockerInteract;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnWarheadUnlock;
        }

        public static void Disable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnDoorInteract;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnGeneratorUnlock;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnLockerInteract;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnWarheadUnlock;
        }

        private static void OnDoorInteract(InteractingDoorEventArgs ev)
        {
            bool flag = !Config.AffectDoors || ev.Door.IsLocked;
            if (!flag)
            {
                bool flag2 = !ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Door.RequiredPermissions.RequiredPermissions, false);
                if (flag2)
                {
                    ev.IsAllowed = true;
                }
            }
        }

        private static void OnWarheadUnlock(ActivatingWarheadPanelEventArgs ev)
        {
            bool flag = !Config.AffectWarheadPanel;
            if (!flag)
            {
                bool flag2 = !ev.IsAllowed && ev.Player.HasKeycardPermission(KeycardPermissions.AlphaWarhead, false);
                if (flag2)
                {
                    ev.IsAllowed = true;
                }
            }
        }

        private static void OnGeneratorUnlock(UnlockingGeneratorEventArgs ev)
        {
            bool flag = !Config.AffectGenerators;
            if (!flag)
            {
                bool flag2 = !ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Generator.KeycardPermissions, false);
                if (flag2)
                {
                    ev.IsAllowed = true;
                }
            }
        }

        private static void OnLockerInteract(InteractingLockerEventArgs ev)
        {
            bool flag = !Config.AffectScpLockers;
            if (!flag)
            {
                bool flag2 = !ev.IsAllowed && ev.InteractingChamber != null && ev.Player.HasKeycardPermission(ev.InteractingChamber.RequiredPermissions, true);
                if (flag2)
                {
                    ev.IsAllowed = true;
                }
            }
        }
    }
}
