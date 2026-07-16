using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Consumables.Potions
{
    public class ChaosPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            // Dust that will appear in these colors when the item with ItemUseStyleID.DrinkLiquid is used
            ItemID.Sets.DrinkParticleColors[Type] = [
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 16;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(silver: 15);
            Item.buffType = ModContent.BuffType<Chaos>();
            Item.buffTime = 600;
        }
        public override bool? UseItem(Player player)
        {
            player.Custom().NewChaosBuff();
            return base.UseItem(player);
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff<Chaos>();
        }
    }
}
