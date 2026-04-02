using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.System
{
    public class UnarmedContract : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<TerrapainPlayer>().unarmed = !player.GetModPlayer<TerrapainPlayer>().unarmed;
            Functions.Chatic(player.GetModPlayer<TerrapainPlayer>().unarmed);
            return true;
        }
    }
}
