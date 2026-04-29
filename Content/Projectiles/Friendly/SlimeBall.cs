using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Friendly
{
	public class SlimeBall : ModProjectile
	{
		private bool onFire
        {
			get => (int)Projectile.ai[0] == 1;
			set => Projectile.ai[0] = value? 1 : 0;
        }
		int penetrate;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox
			Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
		}
		public override void OnSpawn(IEntitySource source)
		{
			penetrate = Projectile.penetrate + 6;
        }
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (target.HasBuff(BuffID.OnFire))
			{
				onFire = true;
			}
			if (onFire)
            {
				target.AddBuff(BuffID.OnFire, 300);
            }
			penetrate--;
			Vector2 CollidePosition = Vector2.Zero;
			if (Projectile.oldVelocity.Y > 0)
				CollidePosition.X = Math.Abs(target.position.Y - Projectile.Center.Y) * Projectile.oldVelocity.X / Projectile.oldVelocity.Length() + Projectile.Center.X;
			else
			{
				if (Projectile.oldVelocity.Y < 0)
					CollidePosition.X = Math.Abs(target.position.Y + target.height - Projectile.Center.Y) * Projectile.oldVelocity.X / Projectile.oldVelocity.Length() + Projectile.Center.X;
				else
				{
					Projectile.velocity.X *= -1.05f;
					if (penetrate > 0)
					{
						//int newProj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, penetrate - 5);
						//Main.projectile[newProj].penetrate = penetrate - 6;
					}
					return;
				}
			}
			if (CollidePosition.X > target.position.X && CollidePosition.X < target.position.X + target.width)
			{
				Projectile.velocity.Y *= -1.05f;
			}
			else
			{
				Projectile.velocity.X *= -1.05f;
			}
			if (penetrate > 0)
			{
				//int newProj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, penetrate - 5);
				// Main.projectile[newProj].penetrate = penetrate - 6;
			}
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
					Projectile.velocity.X = -oldVelocity.X * 1.05f;
				}

				// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
				{
					Projectile.velocity.Y = -oldVelocity.Y * 1.05f;
				}
			}

			return false;
		}
        public override void AI()
        {
            if (penetrate <= 0)
                Projectile.Kill();
			if (Projectile.velocity.Y < 12)
				Projectile.velocity.Y += 0.075f;
			if (Projectile.wet)
			{
				Projectile.velocity *= 0.99f;
				onFire = false;
				Projectile.velocity.Y -= 0.15f;
				Projectile.position += Projectile.velocity / 2;
			}
			if (Projectile.lavaWet)
			{
				Projectile.velocity *= 0.97f;
				onFire = true;
				Projectile.velocity.Y -= 0.2f;
				Projectile.position += Projectile.velocity / 2;
			}
			if (Projectile.honeyWet)
            {
                Projectile.velocity *= 0.98f;
				Projectile.velocity.Y -= 0.17f;
				Projectile.position += Projectile.velocity / 2;
            }
			if (onFire)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
			}
			else
            {
				foreach (var player in Main.player)
				{
					if (player.active && !player.dead && player.HasBuff(BuffID.Inferno) && player.Distance(Projectile.Center) < 150)
					{
						onFire = true;
					}
				}
				if ((int)Projectile.Center.X >= 0 && (int)Projectile.Center.Y >= 0 && ((int)Projectile.Center.X / 16) < Main.maxTilesX && ((int)Projectile.Center.Y / 16) < Main.maxTilesY && ((Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileType == TileID.Campfire && Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileFrameY < 36) || Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileType == TileID.Torches))
				{
					onFire = true;
				}
            }
        }
		public override void OnKill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}
	}
}