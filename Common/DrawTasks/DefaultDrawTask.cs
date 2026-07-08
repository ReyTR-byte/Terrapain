using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Common.DrawTasks
{
    internal class DefaultDrawTask : DrawTask
    {
        Texture2D texture;
        Vector2 position;
        Vector2 scale;
        Color color;
        Rectangle? frame;
        float rotation;
        Vector2 origin;
        SpriteEffects effect;
        public DefaultDrawTask(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect)
        {
            this.texture = texture;
            this.position = position;
            this.frame = frame;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.scale = scale;
            this.effect = effect;
        }
        public DefaultDrawTask(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect)
        {
            this.texture = texture;
            this.position = position;
            this.frame = frame;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.scale = new Vector2(scale);
            this.effect = effect;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, frame, color, rotation, origin, scale, effect, 0);
        }
    }
}
