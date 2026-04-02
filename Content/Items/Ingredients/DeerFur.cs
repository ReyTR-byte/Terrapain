using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
	public class DeerFur : ModItem
	{

		public override void SetDefaults() {
			Item.width = 34;
			Item.height = 40;
			Item.maxStack = 9999;
			Item.value = 300;
			Item.rare = ItemRarityID.Blue; 
		}
	}
}