using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Tools
{
	internal class StarterHook : ModItem
	{
		public override void SetStaticDefaults() 
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
        {
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.value = Item.buyPrice(silver: 80);
			Item.shootSpeed = 10f;
			Item.shoot = ModContent.ProjectileType<StarterHookProjectile>();
            Item.width = 32;
			Item.height = 32;
		}
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity += player.velocity;
        }
	}

	internal class StarterHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public override void Load()
        {
			chainTexture = ModContent.Request<Texture2D>("Terrapain/Content/Items/Tools/ScorspiderHookChain");
		}

		public override void Unload()
        {
			chainTexture = null;
		}

		public override void SetDefaults()
        {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
            Projectile.friendly = true;
			Projectile.width = 20;
			Projectile.height = 20;
		}

		public override void UseGrapple(Player player, ref int type)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == Projectile.type)
				{
                    Main.projectile[i].Kill();
				}
			}
		}
		public override float GrappleRange()
        {
			return 250f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks)
        {
			numHooks = 1;
		}

		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 10f;
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 8;
		}
		public override bool PreDrawExtras() 
        {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer;
				directionToPlayer *= chainTexture.Height();

				center += directionToPlayer;
				directionToPlayer = playerCenter - center;
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}