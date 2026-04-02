using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Dusts
{
    public class MagicParticles : ModDust
    {
        static UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, 10 * random.Next(10), 10, 10);
        }
        public override bool Update(Dust dust)
        {
            if (dust.customData is int)
            {
                if (dust.position.Distance(Main.npc[(int)dust.customData].Center) < 30)
                {
                    dust.active = false;
                }
                dust.velocity += dust.position.DirectionTo(Main.npc[(int)dust.customData].Center);
            }
            return true;
        }
    }
}
