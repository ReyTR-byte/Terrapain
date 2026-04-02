using Microsoft.Xna.Framework;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies
{
	public class DemonicEyeLazer : ModProjectile
	{
        Microsoft.Xna.Framework.Color[] colors = new Microsoft.Xna.Framework.Color[] { Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Color.Blue, Microsoft.Xna.Framework.Color.Red, Microsoft.Xna.Framework.Color.Green, Microsoft.Xna.Framework.Color.Yellow, Microsoft.Xna.Framework.Color.Purple, Microsoft.Xna.Framework.Color.Orange, Microsoft.Xna.Framework.Color.Pink, Microsoft.Xna.Framework.Color.Turquoise };
        private float FocusX
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float FocusY
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private int color
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        int unbuffedDamage;
        public override void SetDefaults()
        {
            Projectile.width = 4; // The width of projectile hitbox
            Projectile.height = 4; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 400; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 60; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (FocusX + FocusY != 0)
            {
                Projectile.DamageType = DamageClass.Ranged;
            }
            unbuffedDamage = Projectile.damage;
            if (color < 0 || color >= colors.Length)
            {
                UnifiedRandom random = new UnifiedRandom();
                color = random.Next(colors.Length);
            }
        }

        public override bool PreDraw(ref Microsoft.Xna.Framework.Color lightColor)
        {
            lightColor = colors[color];
            return true;
        }
        private static UnifiedRandom rand = new UnifiedRandom();
        public override bool PreAI()
        {
            if (color < 0 || color >= colors.Length)
            {
                UnifiedRandom random = new UnifiedRandom();
                color = random.Next(colors.Length);
            }
            return true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, colors[color].ToVector3() / 255 * 3);
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Shine>(), 0, 0, 0, colors[color]);
            if (FocusX + FocusY != 0)
            {
                if ((Projectile.position - new Vector2(FocusX, FocusY)).Length() < 40)
                    Projectile.damage = unbuffedDamage * 2;
                else
                    Projectile.damage = unbuffedDamage;
            }

            float angel = (float)Math.Acos(Projectile.velocity.X / Projectile.velocity.Length());

            if (Projectile.velocity.Y < 0)
                angel = 2 * (float)Math.PI - angel;

            Projectile.rotation = angel;
        }
		public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
	}
}