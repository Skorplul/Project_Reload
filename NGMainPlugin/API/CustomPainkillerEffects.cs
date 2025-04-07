using Random = UnityEngine.Random;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups.Projectiles;
using PlayerRoles.FirstPersonControl;

namespace PRMainPlugin.API
{
    public static class CustomPainkillerEffects
    {
        public static void DoKYS(Player ply)
        {
            ply.DisableAllEffects();
            ply.Explode(ProjectileType.FragGrenade, ply);
        }

        public static void DoKYS_Balls(Player ply)
        {
            int amnt = 10;

            while (amnt >= 0)
            { 
                Scp018Projectile scp018 = Projectile.CreateAndSpawn(ProjectileType.Scp018, ply.Position, ply.Transform.rotation).As<Scp018Projectile>();
                scp018.Rigidbody.velocity = ply.ReferenceHub.GetVelocity() + Random.onUnitSphere * 20;

                amnt--;
            }
        }
    }
}
