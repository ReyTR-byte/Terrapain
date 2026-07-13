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
        public virtual Vector2 Offset => Vector2.Zero;
        public Vector2? origin = null;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float charge, float alpha = 1)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Value.Bounds;
            frame.Width = (int)(frame.Width * charge);
            frame.X = texture.Value.Width - frame.Width;
            Vector2 _origin = origin?? new Vector2(0, frame.Height / 2);
            if (frame.Width > 0)
            {
                spriteBach.Draw(texture.Value, position - Offset, frame, Color.White * alpha, 0, _origin, scale, SpriteEffects.None, 0.2f);
            }
        }
    }
}
