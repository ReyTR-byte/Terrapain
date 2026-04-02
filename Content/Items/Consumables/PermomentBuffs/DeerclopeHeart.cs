using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Common.Player;

namespace Terrapain.Content.Items.Consumables.PermomentBuffs
{
	// This file showcases how to create an item that increases the player's maximum health on use.
	// Within your ModPlayer, you need to save/load a count of usages. You also need to sync the data to other players.
	// The overlay used to display the custom life fruit can be found in Common/UI/ResourceDisplay/VanillaLifeOverlay.cs
	internal class DeerclopeHeart : ModItem
	{
		public const int MaxExampleLifeFruits = 1;
		public const int LifePerFruit = 20;

		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.LifeFruit);
		}

		public override bool CanUseItem(Player player) {
			// This check prevents this item from being used before vanilla health upgrades are maxed out.

			return player.ConsumedLifeCrystals >= 14;
		}

		public override bool? UseItem(Player player) {
			// Moving the exampleLifeFruits check from CanUseItem to here allows this example fruit to still "be used" like Life Fruit can be
			// when at the max allowed, but it will just play the animation and not affect the player's max life
			if (player.ConsumedLifeCrystals > 14) {
				// Returning null will make the item not be consumed
				return null;
			}

			player.GetModPlayer<MaxLifeCristalCheck>().MaxLifeCristals++;
			player.ConsumedLifeCrystals++;
			
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LifeCrystal);
			recipe.AddIngredient(ModContent.ItemType<DeerFur>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}