using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using Terrapain.Common.Global;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Ammo
{
	public class SlimeBullet : ModProjectile
	{
		float penetrate;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of projectile hitbox
			Projectile.height = 8; // The height of projectile hitbox
			Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.light = 0.2f; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
			penetrate = Projectile.ai[0] + 5;
			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
			Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 4;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.damage = (int)(Projectile.damage * 0.8);
			penetrate--;
			Vector2 CollidePosition = Vector2.Zero;
			if (Projectile.velocity.Y > 0)
				CollidePosition.X = Math.Abs(target.position.Y - Projectile.Center.Y) * Projectile.velocity.X / Projectile.velocity.Length() + Projectile.Center.X;
			else
			{
				if (Projectile.velocity.Y < 0)
					CollidePosition.X = Math.Abs(target.position.Y + target.height - Projectile.Center.Y) * Projectile.velocity.X / Projectile.velocity.Length() + Projectile.Center.X;
				else
				{
					if (Projectile.velocity.X > 0)
						Projectile.position.X = target.position.X - 4.05f;
					else
						Projectile.position.X = target.position.X + target.width - 4.05f;
					Projectile.velocity.X *= -0.95f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, penetrate - 5);
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightBlue);
					return;
				}
			}
			if (CollidePosition.X > target.position.X && CollidePosition.X < target.position.X + target.width)
			{
				if (Projectile.velocity.Y > 0)
					Projectile.position.Y = target.position.Y - 4.05f;
				else
					Projectile.position.Y = target.position.Y + target.height - 4.05f;
				Projectile.velocity.Y *= -0.95f;
			}
			else
			{
				if (Projectile.velocity.X > 0)
					Projectile.position.X = target.position.X - 4.05f;
				else
					Projectile.position.X = target.position.X + target.width - 4.05f;
				Projectile.velocity.X *= -0.95f;
			}
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, penetrate - 5);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightBlue);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			penetrate--;
			if (penetrate <= 0)
			{
				Projectile.Kill();
			}
			else
			{
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

				// If the projectile hits the left or right side of the tile, reverse the X velocity
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
				{
					Projectile.velocity.X = -oldVelocity.X * 0.95f;
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightBlue);
				}

				// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
				{
					Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightBlue);
				}
			}

			return false;
		}
		public override void AI()
		{
			if (penetrate <= 0)
				Projectile.Kill();
        }
		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}
	}
}