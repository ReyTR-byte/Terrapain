using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Common.Global.Trails;
using Terrapain.Content.TUtilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class IchorSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 45;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.GetT().trail = new ProjectileTrail()
            {
                startWidth = 24,
                endWidth = 0,
                startColor = Color.Yellow,
                endColor = Color.Green,
                length = 45
            };
        }
        public override void OnSpawn(IEntitySource source)
        {
            MaxSpeed = Projectile.velocity.Length() * 1.5f;
        }
        float MaxSpeed;
        public override void AI()
        {
            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[0]--;
            }
            else
            {
                AIHelper.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + new Vector2(Projectile.ai[1], Projectile.ai[2]), 0.1f, MaxSpeed, 0.2f, 0);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
