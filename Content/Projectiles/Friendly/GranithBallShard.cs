
using Microsoft.Xna.Framework;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class GranithBallShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
        }
        float angularVelocity;
        UnifiedRandom ur = new UnifiedRandom();
        public override void OnSpawn(IEntitySource source)
        {
            angularVelocity = ur.NextFloat() * 0.6f - 0.3f;
        }
        public override void AI()
        {
            Projectile.rotation += angularVelocity;
            Projectile.velocity.Y += 0.3f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 60);
        }
    }
}
