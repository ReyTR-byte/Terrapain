using Luminance.Common.Easings;
using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Common.Global.UseStyles;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terrapain.Content.Projectiles.Friendly;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
	public class SlimeBow : ModItem
	{
		public override void SetStaticDefaults() {

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 48;
			Item.scale = 1f;
			Item.rare = ItemRarityID.Green;

			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.useStyle = TGlobalItem.BowOverride;
			Item.autoReuse = true;

			Item.UseSound = SoundID.Item5;

			Item.DamageType = DamageClass.Ranged;
			Item.damage = 8;
			Item.knockBack = 5f;
			Item.noMelee = true;

			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 16f;
			Item.useAmmo = AmmoID.Arrow;
            Item.value = Item.buyPrice(gold: 6);
        }
		UnifiedRandom rand = new UnifiedRandom();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (Item.GetGlobalItem<BowsOverride>().bowTime >= Item.useAnimation * 5)
			{
				int count = 10;
				float step = MathF.PI * 2 / count;
				float startAngle = velocity.ToRotation() + (count % 2 == 0? step / 2 : 0);
				for (int i = 0; i < count; i++)
				{
					Projectile.NewProjectile(source, position, (startAngle + step * i).ToRotationVector2() * 15, ModContent.ProjectileType<FriendlyCrownGem>(), (int)(damage * 0.75f), knockback, player.whoAmI, velocity.X, velocity.Y, 15);
				}
			}
			for (int i = rand.Next(3, 7); i > 0; i--)
			{
				Dust.NewDust(player.Center + velocity / velocity.Length() + new Vector2(-3, -3), 6, 6, DustID.t_Slime, 0, 0, 0, Color.LightBlue);
			}
			return true;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<SuperDenseGel>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}
	}
}