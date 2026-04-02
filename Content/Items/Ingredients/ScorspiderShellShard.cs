using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
	public class ScorspiderShellShard : ModItem
	{

		public override void SetDefaults() {
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.value = 140;
			Item.rare = ItemRarityID.Blue; 
		}
	}
}