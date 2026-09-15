using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terrapain.Content.Buffs;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.DamageClasses;

namespace Terrapain.Content.Items.Tools
{
	internal class ScorspiderHook : ModItem
	{
		public override void SetStaticDefaults() 
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
        {
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.value = Item.buyPrice(silver: 80);
			Item.shootSpeed = 18f;
			Item.shoot = ModContent.ProjectileType<ScorspiderHookProjectile>();
            Item.damage = 20;
            Item.DamageType = ModContent.GetInstance<Unarmed>();
            Item.width = 32;
			Item.height = 32;
		}
	}

	internal class ScorspiderHookProjectile : ModProjectile
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

		public override bool? CanUseGrapple(Player player) 
        {
			int hooksOut = 0;
			for (int l = 0; l < 1000; l++)
            {
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                {
					hooksOut++;
				}
			}

			return hooksOut <= 3;
		}

		// Return true if it is like: Hook, CandyCaneHook, BatHook, GemHooks
		// public override bool? SingleGrappleHook(Player player)
		// {
		//	return true;
		// }

		// Use this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
		// You can also change the projectile like: Dual Hook, Lunar Hook
		// public override void UseGrapple(Player player, ref int type)
		// {
		//	int hooksOut = 0;
		//	int oldestHookIndex = -1;
		//	int oldestHookTimeLeft = 100000;
		//	for (int i = 0; i < 1000; i++)
		//	{
		//		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
		//		{
		//			hooksOut++;
		//			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
		//			{
		//				oldestHookIndex = i;
		//				oldestHookTimeLeft = Main.projectile[i].timeLeft;
		//			}
		//		}
		//	}
		//	if (hooksOut > 1)
		//	{
		//		Main.projectile[oldestHookIndex].Kill();
		//	}
		// }

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange()
        {
			return 500f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks)
        {
			numHooks = 3;
		}

		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 18f;
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 10;
		}

        int iTarget = -1;
        NPC Target => iTarget >= 0? Main.npc[iTarget] : null;
        Vector2 PositionAboutTarget;
        int messageCount;

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 0 && Target == null && Projectile.Colliding(Projectile.Hitbox, target.Hitbox) && target.GetGlobalNPC<TGlobalNPC>().canBeHooked)
            {
                PositionAboutTarget = Projectile.Center - target.Center;
                PositionAboutTarget = PositionAboutTarget.RotatedBy(-target.rotation);
                PositionAboutTarget.X *= target.spriteDirection;
                iTarget = target.whoAmI;
                return null;
            }
            else
            {
                return false;
            }
        }
        public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
			if (Target != null)
            {
				return Target.knockBackResist == 0; 
			}
			return null;
        }
        public override bool PreAI()
        {
			if (Target != null)
            {
            	if (Target.knockBackResist == 0)
            	{
            		Projectile.ai[0] = 2;
            	}
				else
                {
                    Projectile.ai[0] = 0;
                }
			}	
			return true;
        }
        public override void AI()
        {
            if (Target != null)
            {
				Target.GetGlobalNPC<TGlobalNPC>().hooked = true;
				Target.GetGlobalNPC<TGlobalNPC>().hookProjectile = Projectile;
                Vector2 PAT = PositionAboutTarget;
				PAT.X *= Target.spriteDirection;
                Projectile.Center = PAT.RotatedBy(Target.rotation) + Target.Center;

				if (!Target.friendly)
				{
                	Target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 30);
				}
				float rotation = Functions.AngleFromVector(Target.DirectionTo(Projectile.Center));
				Vector2 RotatatedVelocity = Target.velocity.RotatedBy(-rotation);

				if (RotatatedVelocity.X < 10)
                {
					Target.velocity += Target.DirectionTo(Main.player[Projectile.owner].Center) * Target.knockBackResist * Target.Distance(Main.player[Projectile.owner].Center) / 500;

				}

                if (Target.Distance(Main.player[Projectile.owner].Center) > 750 || !Target.active || KeybindSystem.RealiseHookedNPC.Current)
                {
					Target.GetGlobalNPC<TGlobalNPC>().hooked = false;
					Target.GetGlobalNPC<TGlobalNPC>().hookProjectile = null;
                    iTarget = -1;
					Projectile.ai[0] = 1;
                }
            }
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