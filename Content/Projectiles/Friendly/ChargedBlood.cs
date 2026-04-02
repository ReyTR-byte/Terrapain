using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Friendly
{
	public class ChargedBlood : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of projectile hitbox
			Projectile.height = 8; // The height of projectile hitbox
			Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
            Projectile.light = 0.2f; 
			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.alpha += 5;
            if (Projectile.alpha > 250)
                Projectile.Kill();
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 4 == 0 && Projectile.velocity.Length() > 1)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y); 
            }
		}
	}
}