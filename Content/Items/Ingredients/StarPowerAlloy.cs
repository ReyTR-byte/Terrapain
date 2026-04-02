using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
    public class StarPowerAlloy : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PlatinumBar);
            Item.value += 500;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(nameof(ItemID.GoldOre), 4);
            recipe.AddRecipeGroup(nameof(ItemID.IronOre), 1);
            recipe.AddRecipeGroup(nameof(ItemID.ViciousPowder), 2);
            recipe.AddIngredient(ItemID.FallenStar);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}
