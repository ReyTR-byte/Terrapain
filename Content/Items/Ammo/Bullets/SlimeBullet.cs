using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;
using Terrapain.Content.Items.Ingredients;

namespace Terrapain.Content.Items.Ammo.Bullets
{
	public class SlimeBullet : ModItem
	{
		public override void SetStaticDefaults() {

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults() {
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 8;
			Item.height = 8;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = 10;
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<Projectiles.Ammo.Bullets.SlimeBullet>();
			Item.shootSpeed = 5f;
			Item.ammo = AmmoID.Bullet;
            Item.value = Item.buyPrice(0, 0, 0, 2);
        }
		
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(100);
			recipe.AddIngredient(ModContent.ItemType<SuperDenseGel>());
			recipe.AddRecipeGroup(nameof(ItemID.TungstenBullet), 100);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}