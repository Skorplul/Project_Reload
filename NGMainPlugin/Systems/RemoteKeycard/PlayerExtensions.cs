using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
using Exiled.API.Enums;

namespace PRMainPlugin.Systems.RemoteKeycard
{
    public static class PlayerExtensions
    {
        public static bool HasKeycardPermission(this Exiled.API.Features.Player player, KeycardPermissions permissions, bool requiresAllPermissions = false)
        {
            bool flag = RemoteKeycards.Config.AmnesiaMatters && player.IsEffectActive<AmnesiaItems>();
            return !flag && (requiresAllPermissions ? player.Items.Any(delegate (Exiled.API.Features.Items.Item item)
            {
                Keycard keycard = item as Keycard;
                return keycard != null && keycard.Permissions.HasFlag(permissions);
            }) : player.Items.Any(delegate (Exiled.API.Features.Items.Item item)
            {
                Keycard keycard = item as Keycard;
                return keycard != null && (keycard.Permissions & permissions) > KeycardPermissions.None;
            }));
        }

        public static bool HasKeycardPermission(this Exiled.API.Features.Player player, Interactables.Interobjects.DoorUtils.KeycardPermissions permissions, bool requiresAllPermissions = false)
        {
            bool flag = RemoteKeycards.Config.AmnesiaMatters && player.IsEffectActive<AmnesiaItems>();
            return !flag && (requiresAllPermissions ? player.Items.Any(delegate (Exiled.API.Features.Items.Item item)
            {
                Keycard keycard = item as Keycard;
                return keycard != null && keycard.Base.Permissions.HasFlag(permissions);
            }) : player.Items.Any(delegate (Exiled.API.Features.Items.Item item)
            {
                Keycard keycard = item as Keycard;
                return keycard != null && (keycard.Base.Permissions & permissions) > 0;
            }));
        }
    }
}
