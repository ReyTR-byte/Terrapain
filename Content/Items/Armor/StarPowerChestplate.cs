using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Items.Ingredients;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class StarPowerChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 20;
            Item.width = 30;
            Item.defense = 6;
            Item.value = Item.sellPrice(silver: 80);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<Unarmed>() += 0.1f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<StarPowerAlloy>(28);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
