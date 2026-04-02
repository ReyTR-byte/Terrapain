using Terrapain.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using Terraria.Utilities;
using Humanizer;

namespace Terrapain.Content.Projectiles.Friendly
{
	public class GlassBouble : ModProjectile
	{
		bool explode = false;
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.alpha = 60;
		}
		public override void OnSpawn(IEntitySource source)
		{
			//float ProjectileSped = Convert.ToSingle(Math.Sqrt(Math.Pow(Projectile.velocity.X, 2) + Math.Pow(Projectile.velocity.Y, 2)));
			//Vector2 dir = Projectile.velocity / ProjectileSped;
			//double angel = Math.Acos(dir.X);
			//if (dir.Y >= 0)
			//	angel = Math.PI * 2 - angel;
			//UnifiedRandom rand = new UnifiedRandom();
			//angel += Convert.ToDouble(rand.Next(-15, 15)) * ((2 * Math.PI) / 180);
			//dir = new Vector2(Convert.ToSingle(Math.Cos(angel))*-1, Convert.ToSingle(Math.Sin(angel)));
			//Projectile.velocity = dir * ProjectileSped * -1;
			Projectile.velocity = Functions.RandSpread(Projectile.velocity, 15, -15);
		}

		public override void AI()
		{
			Projectile.velocity.Y += Projectile.ai[0];
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlassBoubleExlolode>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			}
			Projectile.velocity *= 0.99f;
			if (Projectile.timeLeft == 5 && !explode && Projectile.height == 16)
			{
				Projectile.position.X -= 64;
				Projectile.position.Y -= 64;
				explode = true;
				Projectile.height *= 8;
				Projectile.width *= 8;
				Projectile.scale *= 8;
				Projectile.alpha = 255;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0 && Projectile.timeLeft > 6)
			{
				Projectile.timeLeft = 6;
			}
			return false;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
			Projectile.timeLeft = 6;
			target.AddBuff(BuffID.CursedInferno, 300, true);
			
        }

		public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlassBoubleExlolode>(), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
			}
			SoundEngine.PlaySound(SoundID.Item25, Projectile.position);
		}
	}
}