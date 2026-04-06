using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;

namespace Terrapain.Content.Dusts
{
    public class PinkHeart : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 10, 10);
        }
        public override bool Update(Dust dust)
        {
            if (dust.alpha > 255)
            {
                dust.active = false;
                return false;
            }
            dust.position += dust.velocity;
            dust.alpha += 4;
            if (!dust.noGravity)
            {
                dust.velocity.Y += 0.3f;
            }    
            return false;
        }
    }
}
