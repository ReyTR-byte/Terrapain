using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Ammo.Bullets
{
	public class SomeBullet : ModProjectile
	{
        int timer = 60;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.light = 0.2f;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.Bullet;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();
			return false;
		}
        public override void AI()
        {
            timer--;
            if (timer == 0)
            {
                Projectile.frame += 1;
                Projectile.velocity *= 1.2f;
                Projectile.height -= 2;
                Projectile.width -= 2;
                Projectile.position += Vector2.One;
                Projectile.damage = Convert.ToInt32(Projectile.damage * 0.8);
                AISearchForTarget(out bool foundTarget, out Vector2 targetCenter);
                if (foundTarget)
                {
                    Vector2 VTT = targetCenter - Projectile.Center;
                    VTT.Normalize();
                    Projectile.velocity = VTT * Projectile.velocity.Length();
                }
            }
        }
        private void AISearchForTarget(out bool foundTarget, out Vector2 targetCenter)
        {
            float distanceFromTarget = 2000f;
            targetCenter = Projectile.position;
            foundTarget = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                    bool closeThroughWall = between < 100f;

                    if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
        }
		public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
	}
}