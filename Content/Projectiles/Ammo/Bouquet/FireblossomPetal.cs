using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Ammo.Bouquet
{
    public class FireblossomPetal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;

            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.09f;
            float angel = (float)Math.Acos(Projectile.velocity.X / Projectile.velocity.Length());
            if (Projectile.velocity.Y < 0)
                angel = 2 * (float)Math.PI - angel;
            Projectile.rotation = angel;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 480);
        }
    }
}
