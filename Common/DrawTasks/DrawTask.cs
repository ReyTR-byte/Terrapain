using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Common.DrawTasks
{
    public interface DrawTask
    {
        public void Draw(SpriteBatch spriteBatch);
    }
}
