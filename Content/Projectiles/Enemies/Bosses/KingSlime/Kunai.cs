using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class Kunai : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prisma Regalia");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            AIType = ProjectileID.Bullet;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().drawCenter = new Vector2(44, 13);
            Projectile.GetT().trail = new()
            {
                startWidth = 19,
                endWidth = 12,
                length = 16,
                startColor = Color.Blue * 0.65f,
                smooth = false,
                Offset = new Vector2(40, 0)
            };
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
        }
        public override void AI() 
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
