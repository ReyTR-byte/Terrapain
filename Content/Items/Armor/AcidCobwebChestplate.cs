using Terrapain.Common.Player;
using Terrapain.Content.Items.Ingredients;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AcidCobwebChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 4); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 6; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 2;
            player.manaRegen += 2;
            player.GetModPlayer<TerrapainPlayer>().GranithShellChestplateBonus = true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ScorspiderCobweb>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
