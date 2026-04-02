using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
	public class SuperDenseGel : ModItem
	{

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 26;
			Item.maxStack = 9999;
			Item.value = 75;
			Item.rare = ItemRarityID.Blue; 
		}
	}
}