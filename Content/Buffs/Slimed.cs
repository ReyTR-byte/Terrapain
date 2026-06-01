using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Buffs
{
    public class Slimed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        UnifiedRandom random = new();
        public override void Update(Player player, ref int buffIndex)
        {
            random = new();
            if (player.controlDown)
            {
                player.gravity *= 2;
                player.maxFallSpeed *= 1.5f;
                player.GetModPlayer<PlayerMovement>().ShouldFallThroughtPlatforms = true;
            }
            if (random.NextBool(6))
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.t_Slime, newColor: Color.LightBlue);
            }
        }
    }
}
