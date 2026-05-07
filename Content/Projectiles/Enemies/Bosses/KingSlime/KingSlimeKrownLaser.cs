using Luminance.Common.Utilities;
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

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class KingSlimeKrownLaser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        float lenght
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int KingSlime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        float AngularAcceleration
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        float angularVelocity;
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
            set => Projectile.rotation = value.ToRotation();
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 3;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
        }
        public override bool? CanCutTiles()
        {
            Functions.RayCutTile(Projectile.Center, Projectile.Center + dir * lenght, Main.player[Projectile.owner]);
            return false;
        }
        float maxRotationSpeed;
        public override void OnSpawn(IEntitySource source)
        {
            dir = Projectile.velocity;
            maxRotationSpeed = Projectile.velocity.Length() / 60;
            Projectile.velocity = Vector2.Zero;
        }
        public override void AI()
        {
            Projectile.alpha = 255 - (int)(MathF.Min(Projectile.timeLeft / 50f, 1) * 255);
            float totalLength = 0;
            while (totalLength < lenght)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, TorchID.Red);
                totalLength += 20;
            }
            if (KingSlime > -1 && KingSlime < Main.maxNPCs && Main.npc[KingSlime].active && Main.npc[KingSlime].type == NPCID.KingSlime)
            {
                Vector2 Center = Main.npc[KingSlime].Top;
                switch (Main.npc[KingSlime].frame.Y)
                {
                    case 0:
                        Center = Main.npc[KingSlime].Top - Vector2.UnitY * 15;
                        break;
                    case 120:
                        Center = Main.npc[KingSlime].Top - Vector2.UnitY * 25;
                        break;
                    case 240:
                        Center = Main.npc[KingSlime].Top - Vector2.UnitY * 15;
                        break;
                    case 360:
                        Center = Main.npc[KingSlime].Top - Vector2.UnitY * 5;
                        break;
                }

                Projectile.rotation += angularVelocity;
                angularVelocity += AngularAcceleration;
                angularVelocity = MathF.Min(MathF.Abs(angularVelocity), maxRotationSpeed) * AngularAcceleration.NonZeroSign();
                lenght += 18;
                lenght = Math.Min(lenght, 1000);
                Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                Projectile.Center = Center;
            }
            else
            {
                Projectile.active = false;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            if (Functions.Collision(Projectile.Center, dir, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override void OnHitPlayer(Terraria.Player target, Terraria.Player.HurtInfo info)
        {
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Vector2 collide = Functions.RayColisionInTheWorld(Projectile.Center, Projectile.Center + dir * lenght);
            if (collide != Vector2.Zero)
            {
                lenght = Projectile.Distance(collide);

            }
            return false;
        }
        UnifiedRandom random = new UnifiedRandom();
        int shaderTime;
        public override bool PreDraw(ref Color lightColor)
        {
            float width = random.NextFloat(25, 28);
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.Red * 0.8f * (1 - (Projectile.alpha / 255f)), width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", (Color.Pink * 0.85f * (1 - (Projectile.alpha / 255f))).ToVector4());
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
