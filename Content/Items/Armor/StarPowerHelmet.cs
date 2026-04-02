using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
using Terrapain.Content.Items.Ingredients;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StarPowerHelmet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.defense = 5;
            Item.value = Item.sellPrice(silver: 50);
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StarPowerChestplate>() && legs.type == ModContent.ItemType<StarPowerLegings>();
        }
        public override void UpdateArmorSet(Player player)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                player.GetModPlayer<TerrapainPlayer>().StarPowerSet = true;
                player.setBonus = Language.GetTextValue("Mods.Terrapain.SetBonus.StarPowerSet");
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<StarPowerAlloy>(19);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
