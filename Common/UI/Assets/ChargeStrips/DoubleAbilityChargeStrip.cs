using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.ChargeStrips
{
    public class DoubleAbilityChargeStrip : ChargeStrip
    {
        public float Division;
        public DoubleAbilityChargeStrip(float division)
        {
            Division = division;
        }
        public override void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float charge)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            int division = (int)(texture.Size().X * Division);
            Rectangle frame = texture.Value.Bounds;
            frame.Height /= 6;
            int Charge = (int)(frame.Width * charge);
            int shift = texture.Value.Width - Charge;
            frame.X = shift;
            if (frame.Width > 0)
            {
                frame.Width = Math.Min(Charge, division + 2);
                spriteBach.Draw(texture.Value, position - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                frame.Width = Math.Min(Charge, division);
                frame.Y = 4;
                spriteBach.Draw(texture.Value, position + Vector2.UnitY * 4 - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                frame.Width = Math.Min(Charge, division - 2);
                frame.Y += 4;
                spriteBach.Draw(texture.Value, position + Vector2.UnitY * 8 - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                if (Charge > division + 2)
                {
                    frame.X = division + 2 + shift;
                    frame.Width = Charge - division - 2;
                    frame.Y = 12;
                    spriteBach.Draw(texture.Value, position + new Vector2(division + 2, 0) - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                }
                if (Charge > division)
                {
                    frame.X = division + shift;
                    frame.Width = Charge - division;
                    frame.Y = 16;
                    spriteBach.Draw(texture.Value, position + new Vector2(division, 4) - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                }
                if (Charge > division - 2)
                {
                    frame.X = division - 2 + shift;
                    frame.Width = Charge - division + 2;
                    frame.Y = 20;
                    spriteBach.Draw(texture.Value, position + new Vector2(division - 2, 8) - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
