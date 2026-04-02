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

namespace Terrapain.Common.UI.Assets.ItemFrames
{
    public abstract class ItemFrame
    {
        public virtual string Texture => this.GetPath();
        public virtual Vector2 DrawCenter => Vector2.Zero;
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            spriteBach.Draw(texture.Value, position, null, Color.White, 0, DrawCenter, scale, SpriteEffects.None, 0);
        }
    }
}
