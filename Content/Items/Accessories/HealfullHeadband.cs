using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terrapain.Content.Buffs;
using Terraria.ID;

namespace Terrapain.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class HealfullHeadband : ModItem
    {
        bool regenBuffAdded;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[Item.faceSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 3, 70, 0);
            Item.accessory = true;
            Item.defense = 3;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLifeMax2 > player.statLife && !player.HasBuff<BandageSickness>())
            {
                player.Heal(5);
                player.AddBuff(ModContent.BuffType<BandageSickness>(), 300);
            }
            if (player.statLifeMax2 > 2 * player.statLife)
            {
                if (!regenBuffAdded)
                {
                    player.lifeRegen += 2;
                }
            }
            player.maxRunSpeed += 2;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Headband>());
			recipe.AddIngredient(ItemID.LesserHealingPotion, 8);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 6);
			recipe.AddIngredient(ItemID.IronskinPotion, 6);
            recipe.AddIngredient(ItemID.RegenerationPotion, 6);
            recipe.AddTile(TileID.Bottles);
			recipe.Register();
        }
    }
}