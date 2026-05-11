using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terrapain.Content.Projectiles.Friendly;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
	public class InfernalLazerShotgun : ModItem
	{
		public int ScreenX;
		public int ScreenY;
		public float DistFromCent;
		public override void SetStaticDefaults()
		{

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// Common Properties
			Item.width = 44; // Hitbox width of the item.
			Item.height = 18; // Hitbox height of the item.
			Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.

			// Use Properties
			Item.useTime = 55; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 55; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
			Item.UseSound = SoundID.Item36; // The sound that this item plays when used.

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
			Item.damage = 10; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 6f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<Lazer>(); // For some reason, all the guns in the vanilla source have this.
			Item.shootSpeed = 10f; // The speed of the projectile (measured in pixels per frame.)
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			const int NumProjectiles = 4; // The humber of projectiles that this gun will shoot.
			for (int i = 0; i < NumProjectiles; i++)
			{
				Vector2 newVelocity;
				if (player.Distance(Main.MouseWorld) <= 50)
					newVelocity = Functions.Rotate(velocity, 7.5f - 5 * i);
				else if (player.Distance(Main.MouseWorld) >= 500)
					newVelocity = Functions.Rotate(velocity, (7.5f - 5 * i)*0.2f);
				else
					newVelocity = Functions.Rotate(velocity, (7.5f - 5 * i) * ((float)(500 - player.Distance(Main.MouseWorld)) / 500f / 0.9f * 0.8f + 0.2f));
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
				
			}

			return false; // Return false because we don't want tModLoader to shoot projectile
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOrigin()
		{
			return new Vector2(-2f, -2f);
		}
	}
}