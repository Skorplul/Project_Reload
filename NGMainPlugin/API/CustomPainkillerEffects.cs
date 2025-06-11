using Random = UnityEngine.Random;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups.Projectiles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles;

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

        public static void DoTeamSwap(Player ply)
        {
            if (ply.Role == RoleTypeId.ClassD)
            {
                ply.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                return;
            }
            if (ply.Role == RoleTypeId.Scientist)
            {
                ply.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                return;
            }

            switch ((RoleTypeId)ply.Role)
            {
                case RoleTypeId.NtfCaptain:
                    ply.Role.Set(RoleTypeId.ChaosMarauder, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.NtfSpecialist:
                    ply.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.NtfPrivate:
                    ply.Role.Set(RoleTypeId.ChaosRifleman, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.NtfSergeant:
                    ply.Role.Set(RoleTypeId.ChaosRepressor, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.FacilityGuard:
                    ply.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    return;
            }

            switch ((RoleTypeId)ply.Role)
            {
                case RoleTypeId.ChaosMarauder:
                    ply.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.ChaosConscript:
                    ply.Role.Set(RoleTypeId.NtfSpecialist, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.ChaosRifleman:
                    ply.Role.Set(RoleTypeId.NtfPrivate, RoleSpawnFlags.None);
                    return;
                case RoleTypeId.ChaosRepressor:
                    ply.Role.Set(RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    return;
            }

        }
    }
}
