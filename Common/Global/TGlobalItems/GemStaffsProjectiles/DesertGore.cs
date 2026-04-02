using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class DesertGore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prisma Regalia");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.GetT().afterimage = true;
            Projectile.GetT().afterimagesCount = 5;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().drawTrail = true;
            Projectile.GetT().trailColor = Color.Orange * 0.5f;
            Projectile.GetT().trailLength = 10;
            Projectile.GetT().trailWidth = 17;
            Projectile.GetT().drawCenter = new Vector2(8.5f, 8);
            Projectile.penetrate = 1;
        }
        static UnifiedRandom random = new UnifiedRandom();
        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = random.Next(2);
            Projectile.rotation = random.NextFloat(MathF.PI * 2);
            angularVelocity = random.NextFloat(-0.1f, 0.1f);
        }
        public override void AI()
        {
            Main.projFrames[Projectile.type] = 2;
            Projectile.rotation += angularVelocity;
            Projectile.velocity += Vector2.UnitY * 0.1f;
            if (Projectile.timeLeft % 5 == 0 && random.NextBool(6))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
            }
        }
    }
}
