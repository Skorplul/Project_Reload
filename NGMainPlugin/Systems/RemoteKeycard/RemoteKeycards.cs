using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;

namespace NGMainPlugin.Systems.RemoteKeycard
{
    public class RemoteKeycards
    {
        internal static Config Config;

        public void Start()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += new CustomEventHandler<InteractingDoorEventArgs>(this.OnDoorInteract);
            Exiled.Events.Handlers.Player.UnlockingGenerator += new CustomEventHandler<UnlockingGeneratorEventArgs>(this.OnGeneratorUnlock);
            Exiled.Events.Handlers.Player.InteractingLocker += new CustomEventHandler<InteractingLockerEventArgs>(this.OnLockerInteract);
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += new CustomEventHandler<ActivatingWarheadPanelEventArgs>(this.OnWarheadUnlock);
        }

        public void Stop()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= new CustomEventHandler<InteractingDoorEventArgs>(this.OnDoorInteract);
            Exiled.Events.Handlers.Player.UnlockingGenerator -= new CustomEventHandler<UnlockingGeneratorEventArgs>(this.OnGeneratorUnlock);
            Exiled.Events.Handlers.Player.InteractingLocker -= new CustomEventHandler<InteractingLockerEventArgs>(this.OnLockerInteract);
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= new CustomEventHandler<ActivatingWarheadPanelEventArgs>(this.OnWarheadUnlock);
        }

        private void OnDoorInteract(InteractingDoorEventArgs ev)
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

        private void OnWarheadUnlock(ActivatingWarheadPanelEventArgs ev)
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

        private void OnGeneratorUnlock(UnlockingGeneratorEventArgs ev)
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

        private void OnLockerInteract(InteractingLockerEventArgs ev)
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
