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
using Terraria;
using Terraria.GameContent;

namespace Terrapain.Common.UI.Assets.BarFills
{
    public abstract class BarFill
    {
        public virtual Vector2 Offset => Vector2.Zero;
        public Vector2? origin = null;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float charge, float alpha = 1, Color? DrawColor = null)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Bounds;
            frame.Width = (int)(frame.Width * charge);
            frame.X = texture.Width - frame.Width;
            Vector2 _origin = origin?? texture.Size() / 2;
            Color drawColor = DrawColor?? Color.White;
            if (frame.Width > 0)
            {
                spriteBach.Draw(texture, position - Offset, frame, drawColor * alpha, 0, _origin, scale, SpriteEffects.None, 0.2f);
            }
        }
    }
}
