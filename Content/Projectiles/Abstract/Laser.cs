using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Abstract
{
    public abstract class Laser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public abstract float lenght { get; set; }
        public abstract Vector2 direction { get; set; }
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
        }
        public override bool? CanCutTiles()
        {
            Functions.RayCutTile(Projectile.Center, Projectile.Center + direction * lenght, Main.player[Projectile.owner]);
            return false;
        }
        public void RebuildLaserHitBox()
        {
            int W = (int)(direction.X * 2 * lenght);
            int H = (int)(direction.Y * 2 * lenght);
            Vector2 Center = Projectile.Center;
            Projectile.width = W;
            Projectile.height = H;
            Projectile.Center = Center;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            if (Functions.Collision(Projectile.Center, direction, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
            {
                return true;
            }
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Vector2 collide = Functions.RayColisionInTheWorld(Projectile.Center, Projectile.Center + direction * lenght);
            if (collide != Vector2.Zero)
            {
                lenght = Projectile.Distance(collide);
            }
            return false;
        }
        UnifiedRandom random = new UnifiedRandom();
        public int shaderTime;
        public float shaderSpeed = 0.99f;
        public virtual Color MainColor => Color.White;
        public virtual Color ShineColor => Color.White;
        public virtual float LaserWidth => random.NextFloat(18, 22);
        public virtual float ShineWidth(float width) => width * 3;
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1 - (Projectile.alpha / 255f);
            float width = LaserWidth;
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - direction * width / 2, Projectile.Center + direction * (lenght + width / 2), MainColor * opacity, width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", ShineColor);
            shader.TrySetParameter("width", width);
            shader.TrySetParameter("height", lenght + width);
            shader.TrySetParameter("rastyajenie", 500 / width);
            shader.TrySetParameter("time", shaderTime);
            shader.TrySetParameter("speed", shaderSpeed);
            Texture2D texture = ExtraTextureRegistry.Glow2.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.DrawLine(Projectile.Center - direction * width / 2, Projectile.Center + direction * (lenght + width / 2), ShineColor, width, texture);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
