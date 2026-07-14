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

namespace Terrapain.Common.UI.Assets.BarFills
{
    public abstract class BarFill
    {
        public virtual Vector2 Offset => Vector2.Zero;
        public Vector2? origin = null;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float charge, float alpha = 1, Color? DrawColor = null)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Value.Bounds;
            frame.Width = (int)(frame.Width * charge);
            frame.X = texture.Value.Width - frame.Width;
            Vector2 _origin = origin?? new Vector2(0, frame.Height / 2);
            Color drawColor = DrawColor?? Color.White;
            if (frame.Width > 0)
            {
                spriteBach.Draw(texture.Value, position - Offset, frame, drawColor * alpha, 0, _origin, scale, SpriteEffects.None, 0.2f);
            }
        }
    }
}
