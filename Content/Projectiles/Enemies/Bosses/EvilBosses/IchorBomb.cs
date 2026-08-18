using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terrapain.Common.Global.Trails;
using Terrapain.Common.System;
using Terrapain.Common.System.Filters;
using static Terrapain.Content.TUtilities.AIHelper;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class IchorBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.GetT().trail = new ProjectileTrail()
            {
                startWidth = 75,
                endWidth = 0,
                startColor = Color.Green * 0.8f,
                endColor = Color.Red * 0.8f,
                length = 12
            };
        }
        public override void OnSpawn(IEntitySource source)
        {
            rotationSpeed = random.NextFloat(-0.2f, 0.2f);
            Projectile.rotation = random.NextFloat(MathF.PI * 2);
        }
        UnifiedRandom random = new();
        float rotationSpeed;
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            CommonTerrapainFlyingMovement(Projectile, player.Center, 0.025f, Projectile.velocity.Length(), 0, 0);
            float speed = Projectile.velocity.Length();
            if (speed > 0.2f)
            {
                speed -= 0.2f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= speed;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (Projectile.timeLeft == 1)
            {
                EffectsSystem.AddFilter(new StrikeWaveFilter() {speed1 = 10, speed2 = 10, WaveCenter = Projectile.Center, WavePower = 5, WaveRadius2 = -75, disposingSpeed = 0.05f});
                Vector2 dir = Projectile.DirectionTo(player.Center);
                int count = (int)Projectile.ai[2];
                speed = 18;
                float startAngle = MathF.PI / 2;
                if (count % 2 == 0)
                {
                    startAngle += MathF.PI / count;
                }
                float angleBetween = MathF.PI * 2 / count;
                for (int i = 0; i < count; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (startAngle + angleBetween * i).ToRotationVector2() * speed, BrainOfCthulhu.ichorSpike, BrainOfCthulhu.ichoreSpikeDamage, BrainOfCthulhu.ichoreBombKnockback, -1, Projectile.ai[1], dir.X, dir.Y);
                }
            }
            Projectile.rotation += rotationSpeed;
            rotationSpeed *= 0.995f;
        }
    }
}