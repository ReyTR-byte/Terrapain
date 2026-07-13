using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;

namespace Terrapain.Common.UI.Assets.EmptyStrips
{
    public abstract class EmptyStrip
    {
        public virtual Vector2 Offset => Vector2.Zero;
        public Vector2? origin = null;
        public virtual string Texture => this.GetPath();
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float alpha = 1)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Value.Bounds;
            frame.Height /= 2;
            Vector2 _origin = origin?? frame.Size() / 2;
            spriteBach.Draw(texture.Value, position - Offset, frame, Color.White * alpha, 0, _origin, scale, SpriteEffects.None, 0.5f);
        }
        public virtual void DrawOver(SpriteBatch spriteBach, Vector2 position, Vector2 scale, float alpha = 1)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = texture.Value.Bounds;
            frame.Height /= 2;
            frame.Y = frame.Height;
            Vector2 _origin = origin ?? frame.Size() / 2;
            spriteBach.Draw(texture.Value, position - Offset, frame, Color.White * alpha, 0, _origin, scale, SpriteEffects.None, 1);
        }
    }
}
