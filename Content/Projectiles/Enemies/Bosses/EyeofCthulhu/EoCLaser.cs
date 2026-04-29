using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terraria;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu
{
    public class EoCLaser : ModProjectile
    {

        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
        public override bool CanHitPlayer(Player target)
        {
            if (TriangleCollision(target, Projectile.Center, Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.ai[1] + Projectile.ai[2]) * Projectile.ai[0], Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.ai[1] - Projectile.ai[2]) * Projectile.ai[0]) || TriangleCollision(target, Projectile.Center, Projectile.Center - Vector2.UnitX.RotatedBy(Projectile.ai[1] + Projectile.ai[2]) * Projectile.ai[0], Projectile.Center - Vector2.UnitX.RotatedBy(Projectile.ai[1] - Projectile.ai[2]) * Projectile.ai[0]))
            {
                return true;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
