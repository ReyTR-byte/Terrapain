using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Auras;

namespace Terrapain.Common.DrawTasks
{
    public class AuraDrawTask : DrawTask
    {
        public Aura aura;
        public AuraDrawTask(Aura aura)
        {
            this.aura = aura;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            aura.Draw(spriteBatch);
        }
    }
}
