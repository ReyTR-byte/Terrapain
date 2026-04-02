using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terrapain.Content.Buffs;
using Humanizer;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.DamageClasses;

namespace Terrapain.Content.Items.Tools
{
	internal class ScorspiderHook : ModItem
	{
		public override void SetStaticDefaults() 
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // Amount of this item needed to research and become available in Journey mode's duplication menu. Amount based on vanilla hooks' amount needed
		}

		public override void SetDefaults() 
        {
			// Copy values from the Amethyst Hook
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.value = Item.buyPrice(silver: 80);
			Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<ScorspiderHookProjectile>(); // Makes the item shoot the hook's projectile when used.
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
        { // This is called once on mod (re)load when this piece of content is being loaded.
			// This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
			chainTexture = ModContent.Request<Texture2D>("Terrapain/Content/Items/Tools/ScorspiderHookChain");
		}

		public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
			// It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
			chainTexture = null;
		}

		public override void SetDefaults()
        {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes of the Amethyst hook's projectile.
            Projectile.friendly = true;
			Projectile.width = 20;
			Projectile.height = 20;
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
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
			numHooks = 3; // The amount of hooks that can be shot out
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) 
        {
			speed = 18f; // How fast the grapple returns to you after meeting its max shoot distance
		}

		public override void GrapplePullSpeed(Player player, ref float speed) 
        {
			speed = 10; // How fast you get pulled to the grappling hook projectile's landing position
		}

		// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
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

		// Draws the grappling hook's chain.
		public override bool PreDrawExtras() 
        {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer; // get unit vector
				directionToPlayer *= chainTexture.Height(); // multiply by chain link length

				center += directionToPlayer; // update draw position
				directionToPlayer = playerCenter - center; // update distance
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				// Draw chain
				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			// Stop vanilla from drawing the default chain.
			return false;
		}
	}
}