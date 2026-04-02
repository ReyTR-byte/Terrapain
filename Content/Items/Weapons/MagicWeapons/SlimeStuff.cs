using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles.Friendly;

namespace Terrapain.Content.Items.Weapons.MagicWeapons
{
	public class SlimeStuff : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Magic; // Makes the damage register as magic. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type.
			Item.width = 24;
			Item.height = 58;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot; // Makes the player use a 'Shoot' use style for the Item.
			Item.noMelee = true; // Makes the item not do damage with it's melee hitbox.
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SlimeBall>(); // Shoot a black bolt, also known as the projectile shot from the onyx blaster.
			Item.shootSpeed = 9; // How fast the item shoots the projectile.
			Item.crit = 32; // The percent chance at hitting an enemy with a crit, plus the default amount of 4.
			Item.mana = 3; // This is how much mana the item uses.
            Item.value = Item.buyPrice(gold: 6);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (player.HasBuff(BuffID.Inferno) || player.HasBuff(BuffID.OnFire)) ? 1 : 0);
			return false;
        }
        public override Vector2? HoldoutOrigin()
        {
			return new Vector2(-6, -5);
        }
		
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 9);
            recipe.AddIngredient(ModContent.ItemType<SuperDenseGel>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}