using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs.Potions
{
    public class BrokenHeart : ModBuff
    {
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            if (player.Custom().brokenHeartLevel < 3)
                player.Custom().brokenHeartLevel++;
            return false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.Custom().brokenHeartLevel < 1)
            {
                player.Custom().brokenHeartLevel = 1;
            }
        }
    }
}