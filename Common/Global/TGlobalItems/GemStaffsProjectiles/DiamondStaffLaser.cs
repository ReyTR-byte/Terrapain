using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class DiamondStaffLaser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        float lenght
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int cooldown
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
            set => Projectile.rotation = value.ToRotation();
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 2;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            //Projectile.GetT().useVanillaDrawing = false;
        }
        public override bool? CanCutTiles()
        {
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
            while (totalLength < lenght)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, TorchID.White);
                totalLength += 20;
            }
            Terraria.Player own = Main.player[Projectile.owner];
            if (!own.controlUseItem || own.dead || !own.active)
            {
                Projectile.active = false;
                return;
            }
            else if (own.HeldItem.active && own.manaCost * own.HeldItem.mana > own.statMana)
            {
                if (own.itemAnimation == 1)
                {
                    Projectile.active = false;
                    return;
                }
            }
            else
            {
                Projectile.timeLeft = 2;
                Vector2 Center = Main.player[Projectile.owner].MountedCenter + TGlobalItem.GetHandOffset(Main.player[Projectile.owner]);
                Projectile.rotation = own.itemRotation + (own.direction == -1? MathF.PI: 0) - 0.25f * MathF.PI * own.direction;
                Center += dir * 65;
                cooldown -= 1;
                lenght += 18;
                lenght = Math.Min(lenght, 800);
                Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                Projectile.Center = Center;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            if (Functions.Collision(Projectile.Center, dir, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
            {
                lenght = Pos.Distance(Projectile.Center);
                return cooldown <= 0;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            cooldown = 20;
            Projectile.penetrate++;
        }
        public override void OnHitPlayer(Terraria.Player target, Terraria.Player.HurtInfo info)
        {
            cooldown = 20;
            Projectile.penetrate++;
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
            float width = random.NextFloat(18, 22);
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.LightBlue * 0.8f, width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", Color.LightBlue * 0.7f);
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
