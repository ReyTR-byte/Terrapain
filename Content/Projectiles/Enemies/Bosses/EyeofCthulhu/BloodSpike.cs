using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu
{
    public class BloodSpike : ModProjectile
    {
        public override string Texture => base.Texture + $"_{variant}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.GetT().NonPremultiplied = true;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().drawTrail = true;
            Projectile.GetT().trailLength = 25;
            Projectile.GetT().trailWidth = 25;
            Projectile.GetT().trailColor = Color.Red * 25;
        }
        static UnifiedRandom random = new UnifiedRandom();
        int variant;
        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            variant = random.Next(4);
            if (variant < 2)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
            {
                Projectile.rotation = random.NextFloat(MathF.PI * 2);
                angularVelocity = random.NextFloat(0.1f);
            }
            switch (variant)
            {
                case 0:
                    Projectile.GetT().drawCenter = Vector2.One * 15;
                    break;
                case 1:
                    Projectile.GetT().drawCenter = new Vector2(20, 15);
                    break;
                case 2:
                    Projectile.GetT().drawCenter = new Vector2(13, 17);
                    break;
                case 3:
                    Projectile.GetT().drawCenter = new Vector2(15, 13);
                    break;
            }
        }
        public override void AI()
        {
            switch (variant)
            {
                case 0:
                case 1:
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    break;
                case 2:
                    Projectile.rotation += angularVelocity;
                    break;
                case 3:
                    Projectile.rotation += angularVelocity * 1.5f;
                    break;
            }
            Lighting.AddLight(Projectile.Center, 1f, 0, 0);
            Projectile.velocity += Vector2.UnitY * 0.3f;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: 2);
        }
    }
}
