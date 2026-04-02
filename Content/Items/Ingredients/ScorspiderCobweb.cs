using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
	public class ScorspiderCobweb : ModItem
	{

		public override void SetDefaults() {
			Item.width = 30;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.value = 140;
			Item.rare = ItemRarityID.Blue; 
		}
	}
}