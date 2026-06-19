using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider
{
    public class BigSpiderSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
        }
        public override void AI()
        {
            if (Projectile.velocity.Y < 5)
                Projectile.velocity.Y += 0.3f;
            float angel = (float)Math.Acos(Projectile.velocity.X / Projectile.velocity.Length());
            if (Projectile.velocity.Y < 0)
                angel = 2 * (float)Math.PI - angel;
            Projectile.rotation = angel;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
    }
}
