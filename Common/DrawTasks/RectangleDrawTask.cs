using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Common.DrawTasks
{
    public class RectangleDrawTask : DrawTask
    {
        Texture2D texture;
        Rectangle rectangle;
        Color color;
        public RectangleDrawTask(Texture2D texture, Rectangle rectangle, Color color)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.color = color;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }
    }
}
