using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Dusts
{
    public class SmokeCloud : ModDust
    {
        static UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(Dust dust)
        {
            int frame = random.Next(151);
            if (frame == 150)
            {
                dust.frame = new Rectangle(0, 160, 28, 36);
            }
            else
            {
                switch (frame % 6)
                {
                    case 0:
                        dust.frame = new Rectangle(0, 0, 24, 24);
                        break;
                    case 1:
                        dust.frame = new Rectangle(0, 24, 44, 24);
                        break;
                    case 2:
                        dust.frame = new Rectangle(0, 48, 40, 28);
                        break;
                    case 3:
                        dust.frame = new Rectangle(0, 76, 28, 28);
                        break;
                    case 4:
                        dust.frame = new Rectangle(0, 104, 52, 32);
                        break;
                    case 5:
                        dust.frame = new Rectangle(0, 136, 40, 24);
                        break;
                }
            }
        }
        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.955f;
            dust.position += dust.velocity;
            dust.customData = (int)dust.customData + 4;
            dust.alpha = (byte)MathHelper.Clamp((int)dust.customData, 0, 255);
            if (dust.alpha >= 255)
            {
                dust.active = false;
            }
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, dust.position - Main.screenPosition, dust.frame, dust.GetAlpha(new Color(dust.color.ToVector4() * Lighting.GetColor(dust.position.ToTileCoordinates()).ToVector4())), 0, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
