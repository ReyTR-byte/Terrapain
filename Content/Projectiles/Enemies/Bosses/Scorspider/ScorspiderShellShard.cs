using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider
{
    public class ScorspiderShellShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 120;
        }
        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            UnifiedRandom random = new UnifiedRandom();
            Projectile.rotation = (random.NextFloat() - 0.5f) * 2 * (float)Math.PI;
            angularVelocity = (random.NextFloat() - 0.5f) * 0.2f * (float)Math.PI;
        }

        public override void AI()
        {
            Projectile.rotation += angularVelocity;
            if (Projectile.friendly)
            {
                Projectile.tileCollide = true;
            }
            Projectile.velocity.Y += 0.3f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
    }
}
