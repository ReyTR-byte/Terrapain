using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ingredients
{
	public class MassiveLensSharp : ModItem
	{

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 34;
			Item.maxStack = 9999;
			Item.value = 120;
			Item.rare = ItemRarityID.Blue; 
		}
	}
}