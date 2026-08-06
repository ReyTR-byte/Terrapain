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
			Item.width = 44;
			Item.height = 18;
			Item.rare = ItemRarityID.Green;

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item36;

			Item.DamageType = DamageClass.Ranged;
            Item.damage = 25;
            Item.knockBack = 6f;
			Item.noMelee = true;

			Item.shoot = ModContent.ProjectileType<ReflectorLaser>();
			Item.shootSpeed = 15f;
            Item.value = Item.buyPrice(gold: 7);
        }

		UnifiedRandom rand = new UnifiedRandom();

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Item.damage = 25;
			const int NumProjectiles = 4;

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
			return false;
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