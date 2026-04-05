using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Terrapain.Content.Items.Accessories.DashAccessories
{
	[AutoloadEquip(EquipType.Shield)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class WoodenShield : ActiveAccessory
	{
		public override void ModSetDefaults() {
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 0, 15);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
            Item.DamageType = ModContent.GetInstance<Unarmed>();
			Item.damage = 10;
			Item.knockBack = 2;
            Item.defense = 1;
			Item.GetGlobalItem<TGlobalItem>().dashAccessory = true;
            activeAccessory = new ClasicDashAccessory();
            DashDuration = 10;
			DashPower = 9;
			DashReloadMax = 110;
		}
        public override void ModUpdateAccessory(Player player, bool hideVisual)
        {
            DashReloadMax = 110 - (player.GetModPlayer<TerrapainPlayer>().unarmed ? 20 : 0);
			DashDuration = 10 + (player.GetModPlayer<TerrapainPlayer>().unarmed ? 5 : 0);
            player.Custom().Dash = new ActiveAccessoryDash(Item) { DashPower = DashPower, DashDuration = DashDuration };
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
			{
				player.Custom().Dash = new ActiveAccessoryDash(Item) { priority = 1.5f, DashPower = DashPower + 4, DashDuration = DashDuration + 5, damageType = Item.DamageType, hurtfull = true };
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			foreach (TooltipLine line in tooltips)
			{
				if ((line.Name == "Damage" || line.Name == "Knockback") && !Main.player[Main.myPlayer].GetModPlayer<TerrapainPlayer>().unarmed)
				{
					line.Text = "";
				}
			}
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			DashReload = 50;
        }
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup("Wood", 25);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}