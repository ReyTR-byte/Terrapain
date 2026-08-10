using Luminance.Core.Graphics;
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
    internal class Sparcle : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.velocity.Y -= 0.3f - 0.6f * (1 - dust.scale);
            dust.velocity.X *= 0.98f;
            dust.scale -= 0.03f;
            if (dust.scale < 0)
            {
                dust.active = false;
                return false;
            }
            if (dust.customData is List<Vector2>)
            {
                if ((dust.customData as List<Vector2>).Count > 20)
                {
                    (dust.customData as List<Vector2>).RemoveRange(0, 15);
                }
                (dust.customData as List<Vector2>).Add(dust.position);
            }
            else
            {
                dust.customData = new List<Vector2>();
                (dust.customData as List<Vector2>).Add(dust.position);
            }
            dust.velocity = dust.velocity.RotatedByRandom(0.2f);
            dust.position += dust.velocity;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            if ((dust.customData is List<Vector2>) && (dust.customData as List<Vector2>).Count > 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terrapain/Content/Dusts/Sparcle").Value;
                Texture2D trail = ModContent.Request<Texture2D>("Terrapain/Content/Dusts/SparcleTrail").Value;
                Main.spriteBatch.Draw(texture, dust.position - Main.screenPosition, null, Color.White, 0, new Vector2(2, 2), dust.scale, SpriteEffects.None, 0);
                float visibility = 1;
                for (int i = (dust.customData as List<Vector2>).Count - 1; i > 1 && i > (dust.customData as List<Vector2>).Count - 5; i--)
                {
                    Vector2 pos = (dust.customData as List<Vector2>)[i];
                    float rotation = pos.DirectionTo((dust.customData as List<Vector2>)[i - 1]).ToRotation();
                    Vector2 scale = new Vector2(pos.Distance((dust.customData as List<Vector2>)[i - 1]), visibility * dust.scale);
                    Main.spriteBatch.Draw(trail, ((dust.customData as List<Vector2>)[i] - Main.screenPosition), null, Color.White * visibility, rotation, new Vector2(0, 2), scale, SpriteEffects.None, 0);
                    visibility -= 0.15f;
                }
            }
            return false;
        }
    }
}
