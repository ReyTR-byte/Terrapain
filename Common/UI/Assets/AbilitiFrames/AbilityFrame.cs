using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.AbilitiFrames
{
    public abstract class AbilityFrame
    {
        public virtual Vector2 Offcet => Vector2.Zero;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            spriteBach.Draw(texture.Value, position - Offcet, null, Color.White, 0, new Vector2 (0, 40), scale, SpriteEffects.None, 0);
            
        }
    }
}
