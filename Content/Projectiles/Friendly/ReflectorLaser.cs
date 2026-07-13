using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class ReflectorLaser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";

        Color[] colors = new Color[] {
            Color.White,
            Color.Blue,
            Color.Red,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Pink,
            Color.Turquoise,
            Color.Black,
        };

        float lenght;
        Color color
        {
            get => colors[(int)Projectile.ai[0]];
        }
        Vector2 Mouse => new Vector2(Projectile.ai[1], Projectile.ai[2]);
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
            set => Projectile.rotation = value.ToRotation();
        }
        public const float MaxTime = 15;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            lenght = 1500;
            //Projectile.GetT().useVanillaDrawing = false;
        }
        public override bool? CanCutTiles()
        {
            if (Projectile.timeLeft == MaxTime)
                Functions.RayCutTile(Projectile.Center, Projectile.Center + dir * lenght, Main.player[Projectile.owner]);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            dir = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        public override void AI()
        {
            float totalLength = 0;
            while (totalLength < lenght && Projectile.timeLeft != MaxTime)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, color.ToVector3() * Projectile.timeLeft / MaxTime * 0.5f);
                totalLength += 20;
            }
            if (Projectile.timeLeft == MaxTime)
            {
                Vector2 Center = Projectile.Center;
                Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                Projectile.Center = Center;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft == 15)
            {
                Vector2 Pos = Vector2.Zero;
                if (Functions.Collision(Projectile.Center, dir, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
                {
                    return true;
                }
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.Center.Distance(Mouse) < 50)
            {
                modifiers.FinalDamage *= 2;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.Center.Distance(Mouse) < 50)
            {
                modifiers.FinalDamage *= 2;
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.timeLeft == MaxTime)
            {
                Vector2 collide = Functions.RayColisionInTheWorld(Projectile.Center, Projectile.Center + dir * lenght);
                if (collide != Vector2.Zero)
                {
                    lenght = Projectile.Distance(collide);

                }
            }
            return false;
        }
        UnifiedRandom random = new UnifiedRandom();
        int shaderTime;
        public override bool PreDraw(ref Color lightColor)
        {
            float visibility = Projectile.timeLeft / MaxTime;
            float width = random.NextFloat(5, 6);
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), color * 0.8f * visibility, width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", color * visibility * 0.7f);
            shader.TrySetParameter("width", width);
            shader.TrySetParameter("height", lenght + width);
            shader.TrySetParameter("rastyajenie", 500 / width);
            shader.TrySetParameter("time", shaderTime);
            shader.TrySetParameter("speed", 0.96f);
            Texture2D texture = ExtraTextureRegistry.Glow2.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.LightBlue, width, texture);
            shaderTime++;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public float WidthFunction(float value) => 12f;
        public Color ColorFunction(float value) => Color.LightBlue;
    }
}
