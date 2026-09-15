using Terrapain.Common.Global;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Ammo.Arrows
{
    public class CopperArrow: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 25;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * TGlobalNPC.random.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<CopperNugget>(), Projectile.damage / 5 + 1, 0.1f, Projectile.owner);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
        public class CopperNugget: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = TGlobalNPC.random.NextFloat(MathF.PI * 2);
            Projectile.scale = TGlobalNPC.random.NextFloat(0.9f, 1.1f);
            Projectile.ai[0] = TGlobalNPC.random.NextFloat(-0.1f, 0.1f);
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation += Projectile.ai[0];
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}