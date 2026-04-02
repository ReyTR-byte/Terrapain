using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Dusts
{
    public class Shine : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.velocity = Vector2.Zero;
            dust.scale -= 0.07f;
            if (dust.scale <= 0)
            {
                dust.active = false;
            }
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, dust.position - Main.screenPosition, dust.frame, dust.color, 0, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
