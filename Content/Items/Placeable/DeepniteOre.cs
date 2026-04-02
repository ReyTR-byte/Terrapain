using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Placeable
{
	public class DeepniteOre : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.DeepniteOre>();
			Item.width = 12;
			Item.height = 12;
			Item.value = 3000;
		}
	}
}