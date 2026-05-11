using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class RubySharp : ModProjectile
    {
        public override string Texture => base.Texture + $"_{variant}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prisma Regalia");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().drawTrail = true;
            Projectile.GetT().trailColor = Color.Red * 0.5f;
            Projectile.GetT().trailLength = 10;
            Projectile.GetT().trailWidth = 7;
            Projectile.penetrate = 1;
        }
        static UnifiedRandom random = new UnifiedRandom();
        int variant;
        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            variant = random.Next(4);
            Projectile.rotation = random.NextFloat(MathF.PI * 2);
            angularVelocity = random.NextFloat(0.1f);
            switch (variant)
            {
                case 0:
                    Projectile.GetT().drawCenter = new Vector2(4, 2);
                    break;
                case 1:
                    Projectile.GetT().drawCenter = new Vector2(4, 4);
                    break;
                case 2:
                    Projectile.GetT().drawCenter = new Vector2(5, 3);
                    break;
                case 3:
                    Projectile.GetT().drawCenter = new Vector2(5, 5);
                    break;
            }
        }
        public override void AI()
        {
            Projectile.rotation += angularVelocity;
            Lighting.AddLight(Projectile.Center, 0.7f, 0, 0);
            Projectile.velocity += Vector2.UnitY * 0.05f;
            if (Projectile.timeLeft % 5 == 0 && random.NextBool(6))
            { 
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, Scale: 2); 
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
            }
        }
    }
}
