using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories.DashAccessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class PlatinumShield : ActiveAccessory
	{
        public override void ModSetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;

            Item.defense = 3;
            Item.GetGlobalItem<TGlobalItem>().dashAccessory = true;
            activeAccessory = new ClasicDashAccessory();
        }

		public override void ModUpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            player.Custom().Dash = new ActiveAccessoryDash(Item) { DashPower = DashPower, DashDuration = DashDuration, damageType = Item.DamageType };
        }

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<WoodenShield>());
			recipe.AddIngredient(ItemID.PlatinumBar, 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}