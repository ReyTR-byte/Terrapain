using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.Auras;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class CursedCloud : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 800;
            Projectile.tileCollide = false;
            aura = new CursedCloudAura();
        }
        CursedCloudAura aura;
        public override void AI()
        {
            aura.Center = Projectile.Center;
            aura.Update();
            if (!aura.CatchPlayer)
            {
                if (!aura.recharge)
                    aura.Charge = MathF.Max(0, aura.Charge - 0.006f);
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }
            aura.CatchPlayer = false;
            if (Aura.random.NextFloat() < aura.Charge && !aura.recharge)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<Sparcle>());
            }
            Projectile.scale = 1 + aura.Charge;
            Projectile.ai[0] += 0.02f;
            Projectile.velocity = Projectile.velocity.Normalized() * MathF.Min(25, Projectile.velocity.Length());
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Projectile.Distance(player.Center);
                distance -= 400;
                if (distance > 0)
                {
                    distance *= 0.5f;
                }
                else
                {
                    distance *= 1.5f;
                }
                distance /= 400;
                distance = distance * distance * distance.NonZeroSign();
                Projectile.velocity += Projectile.DirectionTo(player.Center) * distance * (MathF.Sin(Projectile.ai[0]) + 1) * 2;
            }
        }
        public override bool PreDrawExtras()
        {
            aura.Draw(Main.spriteBatch);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            float prog = (MathF.Sin(Projectile.ai[0]) + 1) / 2;
            float prog2 = prog * 0.2f;
            Color newColor = new Color(Color.Yellow.ToVector4() * prog + Color.White.ToVector4() * (1 - prog));
            return new Color((lightColor.ToVector4() * (1 - prog) * newColor.ToVector4()) * (1 - prog2) + newColor.ToVector4() * prog2);
        }
    }
}
