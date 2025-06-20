using CustomPlayerEffects;
using Exiled.API.Features.Items;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace PRMainPlugin.Systems.RemoteKeycard
{
    public static class Extensions
    {
        /// <summary>
        ///     Checks whether the player has a keycard of a specific permission.
        /// </summary>
        /// <param name="player"><see cref="Player" /> trying to interact.</param>
        /// <param name="permissions">The permission that's gonna be searched for.</param>
        /// <param name="requiresAllPermissions">Whether all permissions are required.</param>
        /// <returns>Whether the player has the required keycard.</returns>
        public static bool HasKeycardPermission(this Player player, KeycardPermissions permissions, bool requiresAllPermissions = false)
        {
            if (RemoteKeycards._config.AmnesiaMatters && player.IsEffectActive<AmnesiaVision>())
                return false;

            return requiresAllPermissions
                ? player.Items.Any(item => item is Keycard keycard && keycard.Permissions.HasFlag(permissions))
                : player.Items.Any(item => item is Keycard keycard && (keycard.Permissions & permissions) != 0);
        }
    }
}
