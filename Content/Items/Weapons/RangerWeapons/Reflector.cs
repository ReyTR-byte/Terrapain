using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terrapain.Common.Global;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles.Enemies;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static AssGen.Assets;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
	public class Reflector : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 21));
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// Common Properties
			Item.width = 44; // Hitbox width of the item.
			Item.height = 18; // Hitbox height of the item.
			Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.

			// Use Properties
			Item.useTime = 20; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 20; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
			Item.UseSound = SoundID.Item36; // The sound that this item plays when used.

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
			Item.damage = 44; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 6f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<ReflectorLaser>(); // For some reason, all the guns in the vanilla source have this.
			Item.shootSpeed = 15f; // The speed of the projectile (measured in pixels per frame.)
            Item.value = Item.buyPrice(gold: 7);
        }

		UnifiedRandom rand = new UnifiedRandom();

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			const int NumProjectiles = 4; // The humber of projectiles that this gun will shoot.

			int color = rand.Next(10);

			Vector2 pos = player.MountedCenter + TGlobalItem.GetHandOffset(player);
			Vector2 dir = pos.DirectionTo(Main.MouseWorld);
            float rot = dir.ToRotation();
			pos += dir * 60;
			float width = 4.5f;
            for (int i = 0; i < NumProjectiles; i++)
			{
				position = pos;
				position += (width * 1.5f - width * i) * dir.RotatedBy(MathF.PI / 2);
				Vector2 newVelocity;
				if ((Main.MouseWorld - player.MountedCenter - TGlobalItem.GetHandOffset(player)).Length() > 75)
					newVelocity = (Main.MouseWorld - position);
				else
					newVelocity = (player.MountedCenter + TGlobalItem.GetHandOffset(player) + dir * 75 - position);
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI, color, Main.MouseWorld.X, Main.MouseWorld.Y);
			}
			return false; // Return false because we don't want tModLoader to shoot projectile
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<MassiveLensSharp>(), 5);
			recipe.AddIngredient(ItemID.IllegalGunParts);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(-16, 4);
        }
	}
}