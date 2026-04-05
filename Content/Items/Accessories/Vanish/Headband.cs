using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terrapain.Content.Buffs;
using Terraria.ID;

namespace Terrapain.Content.Items.Accessories.Vanish
{
    [AutoloadEquip(EquipType.Face)]
    public class Headband : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[Item.faceSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cobweb, 36);
            recipe.AddTile(TileID.Loom);
			recipe.Register();
        }
    }
}