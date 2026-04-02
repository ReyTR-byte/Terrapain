using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs.Potions
{
    public class Chaos : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.Custom().UpdateChaosBuff();
        }
    }
}
