using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class KingSlimeCrownLaser2 : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        float lenght
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        NPC Crown
        {
            get
            {
                if ((int)Projectile.ai[1] < 0 || (int)Projectile.ai[1] >= Main.maxNPCs)
                {
                    return null;
                }

                return Main.npc[(int)Projectile.ai[1]];
            }
        }
        int TimeToSpin
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
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
            Projectile.alpha = 255;
            Projectile.timeLeft = 3;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = (int)Projectile.ai[0];
            lenght = 0;
        }
        public override bool? CanCutTiles()
        {
            Functions.RayCutTile(Projectile.Center, Projectile.Center + dir * lenght, Main.player[Projectile.owner]);
            return false;
        }
        public override void AI()
        {
            if (Crown == null || Crown.active == false || Crown.type != ModContent.NPCType<KingSlimeCrown>())
            {
                Projectile.active = false;
            }
            if (Projectile.timeLeft > TimeToSpin)
            {
                Projectile.rotation = Crown.rotation - MathF.PI / 2;
                Projectile.alpha = Math.Max(Projectile.alpha - 2, 200);
            }
            else
            {
                float progress = MathF.Min((TimeToSpin - Projectile.timeLeft) / (float)TimeToSpin, 0.85f);
                Projectile.rotation = Crown.rotation - MathF.PI / 2 + MathF.PI * progress * Projectile.spriteDirection;
                Projectile.alpha = Math.Max(Projectile.alpha - 4, 60);
            }
            Vector2 Center = Crown.Center;
            lenght += 18;
            lenght = Math.Min(lenght, 2000);
            Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
            Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
            Projectile.Center = Center;
            float totalLength = 0;
            while (totalLength < lenght)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, TorchID.Red);
                totalLength += 20;
            }
            if (Projectile.timeLeft == TimeToSpin)
            {
                shaderTime /= 2;
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
        public override bool? CanDamage()
        {
            if (Projectile.timeLeft > TimeToSpin)
            {
                return false;
            }
            return null;
        }
        UnifiedRandom random = new UnifiedRandom();
        int shaderTime;
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1 - (Projectile.alpha / 255f);
            float width = random.NextFloat(25, 28);
            if (Projectile.timeLeft > TimeToSpin)
            {
                width *= 0.8f;
            }
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.Red * 0.8f * opacity, width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", (Color.Pink * 0.85f * opacity).ToVector4());
            shader.TrySetParameter("width", width);
            shader.TrySetParameter("height", lenght + width);
            shader.TrySetParameter("rastyajenie", 500 / width);
            shader.TrySetParameter("time", shaderTime);
            float speed = 0.96f;
            if (Projectile.timeLeft > TimeToSpin)
            {
                speed = 0.98f;
            }
            shader.TrySetParameter("speed", speed);
            Texture2D texture = ExtraTextureRegistry.Glow2.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.Pink, width, texture);
            shaderTime++;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public float WidthFunction(float value) => 12f;
        public Color ColorFunction(float value) => Color.LightBlue;
    }
}
