using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.ChargeStrips
{
    public abstract class ChargeStrip
    {
        public virtual Vector2 Offcet => Vector2.Zero;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float charge)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Value.Bounds;
            frame.Width = (int)(frame.Width * charge);
            frame.X = texture.Value.Width - frame.Width;
            if (frame.Width > 0)
            {
                spriteBach.Draw(texture.Value, position - Offcet, frame, Color.White, 0, new Vector2(-2, 6), scale, SpriteEffects.None, 0);
            }
        }
    }
}
