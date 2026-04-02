using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Items.Ingredients;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class StarPowerLegings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.defense = 5;
            Item.value = Item.sellPrice(silver: 65);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<StarPowerAlloy>(23);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
