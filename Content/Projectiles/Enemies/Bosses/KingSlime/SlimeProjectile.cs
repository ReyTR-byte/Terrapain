using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class SlimeProjectile : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
            Projectile.alpha = 175;
        }
        int frameTimer;
        int frame;
        public int variant;
        static UnifiedRandom rand = new();
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.tileCollide = false;
            }
            if (Projectile.ai[0] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathF.PI * 0.5f;
            }
            variant = rand.Next(6);
        }
        public override void AI()
        {
            frameTimer++;
            if (frameTimer == 12)
            {
                frame = (frame + 1) % 2;
                frameTimer = 0;
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity.Y += 0.3f;
            }
            if (Projectile.ai[0] == 2 && rand.NextBool(6))
            {
                Color color1 = Color.White;
                switch (variant)
                {
                    case 0:
                        color1 = Color.Blue;
                        break;
                    case 1:
                        color1 = Color.Red;
                        break;
                    case 2:
                        color1 = Color.Black;
                        break;
                    case 3:
                        color1 = Color.Purple;
                        break;
                    case 4:
                        color1 = Color.Yellow;
                        break;
                    case 5:
                        color1 = Color.Green;
                        break;
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, newColor: color1);

            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != 0 && Projectile.velocity.Y == 0)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;
            }
            if (oldVelocity.X != 0 && Projectile.velocity.X == 0)
            {
                Projectile.velocity.X = oldVelocity.X * -1;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadNPC(NPCID.BlueSlime);
            Texture2D slime = TextureAssets.Npc[NPCID.BlueSlime].Value;
            float opacity = (255 - Projectile.alpha) / 255f;
            Color color1 = Color.White;
            Color color2 = Color.White * opacity;
            color2.R = (byte)Math.Max(color2.R - 255 + lightColor.R, 0);
            color2.G = (byte)Math.Max(color2.G - 255 + lightColor.G, 0);
            color2.B = (byte)Math.Max(color2.B - 255 + lightColor.B, 0);
            switch (variant)
            {
                case 0:
                    color1 = Color.Blue;
                    break;
                case 1:
                    color1 = Color.Red;
                    break;
                case 2:
                    color1 = Color.Black;
                    break;
                case 3:
                    color1 = Color.Purple;
                    break;
                case 4:
                    color1 = Color.Yellow;
                    break;
                case 5:
                    color1 = Color.Green;
                    break;
            }
            color1.A = 100;
            color1 = new Color(color1.ToVector4() * lightColor.ToVector4());
            Rectangle Frame = new Rectangle(0, 26 * frame, 32, 26);
            Main.spriteBatch.Draw(slime, Projectile.Center - Main.screenPosition, Frame, color2, Projectile.rotation, Frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(slime, Projectile.Center - Main.screenPosition, Frame, color1, Projectile.rotation, Frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
