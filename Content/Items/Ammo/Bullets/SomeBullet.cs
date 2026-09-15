using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ammo.Bullets
{
	public class SomeBullet : ModItem
	{
		public override void SetStaticDefaults() {

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults() {
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 10;
			Item.height = 10;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = 10;
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<Projectiles.Ammo.Bullets.SomeBullet>();
			Item.shootSpeed = 10f;
			Item.ammo = AmmoID.Bullet;
		}
	}
}