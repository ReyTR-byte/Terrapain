using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class Shuriken : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().useModDrawingInPreDraw = true;
        }
        public override void AI()
        {
            Projectile.rotation += 0.075f * MathF.PI * Projectile.ai[0];
            Projectile.velocity.RotateBy(0.0055f * MathF.PI * Projectile.ai[0]);
        }
    }
}
