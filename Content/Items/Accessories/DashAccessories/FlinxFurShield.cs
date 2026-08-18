using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories.DashAccessories
{
	[AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class FlinxFurShield : ModItemActiveAccessory
	{
        public override void  ModSetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.defense = 1;
			Item.GetGlobalItem<TGlobalItem>().dashAccessory = true;
			activeAccessory = new ActiveAccessory(this);
			DashDuration = 15;
            DashPower = 11;
            DashReloadMax = 90;
        }

		public override void ModUpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.10f;
			player.Custom().Dash = new ActiveAccessoryDash(Item) { DashPower = DashPower, DashDuration = DashDuration, damageType = Item.DamageType };
        }
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<WoodenShield>());
			recipe.AddIngredient(ItemID.FlinxFur, 2);
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 3);
			recipe.AddIngredient(ItemID.Silk, 4);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}