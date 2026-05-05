using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class SapphireSharp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().trailColor = Color.Blue * 0.6f;
            Projectile.GetT().trailLength = 20;
            Projectile.GetT().trailWidth = 15;
            Projectile.GetT().drawTrail = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        List<int> SapphireSharps = new List<int>();
        static UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.rotation = Projectile.velocity.ToRotation();
            //int count = 0;
            //int[] others = new int[6];
            //foreach (int id in SapphireSharps)
            //{
            //    Projectile proj = Main.projectile[id];
            //    if (proj.active && proj.type == Type)
            //    {
            //        if (Projectile.owner == proj.owner)
            //        {
            //            others[count] = id;
            //            proj.ai[0] = count;
            //            count++;
            //            if (count == 6)
            //            {
            //                Main.projectile[others[0]].ai[0] = -1;
            //                for (int i = 0; i < 5; i++)
            //                {
            //                    others[i] = others[i + 1];
            //                    Main.projectile[others[i]].ai[0] = i;
            //                }
            //                Projectile.ai[0] = 5;
            //                return;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        SapphireSharps.Remove(id);
            //    }
            //}
            //Projectile.ai[0] = count;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.02f;
            Lighting.AddLight(Projectile.Center, TorchID.Blue);
            if (Projectile.timeLeft % 3 == 0 && random.NextBool(4))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
            }
            if (Projectile.ai[2] == 0)
            {
                float maxVelocity = 25;
                float maxVelocityMultyplier = 1;
                Vector2 targetPosition = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                //Projectile.Center = targetPosition;
                //Projectile.velocity = Vector2.Zero;
                if (targetPosition != Projectile.Center)
                {
                    Projectile.velocity = Projectile.DirectionTo(targetPosition) * Projectile.velocity.Length();
                    Projectile.velocity += Projectile.DirectionTo(targetPosition) * 1.2f;
                }
                if (Projectile.Distance(targetPosition) < 75)
                {
                    maxVelocityMultyplier = 1 - (75 - Projectile.Distance(targetPosition)) / 75;
                }
                if (Projectile.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                {
                    Projectile.velocity = Projectile.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                }
            }
            Projectile.ai[2] = -1;
        }
        
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            float val = MathF.Max((Projectile.velocity.Length() - 20) / 5, 0);
            if (Projectile.velocity.Length() > 20)
            {
                float trailWidth = Projectile.GetT().trailWidth;
                Color trailColor = Color.LightBlue * val;
                int trailLength = 7;


                var blackTile = ExtraTextureRegistry.BlackPixel;
                ManagedShader trailShader = ShaderManager.GetShader("Terrapain.TrailingShader");
                int w = Projectile.width / 2;
                int h = Projectile.height / 2;
                float top = Projectile.Center.Y - trailWidth;
                float bottom = Projectile.Center.Y + trailWidth;
                float left = Projectile.Center.X - trailWidth;
                float right = Projectile.Center.X + trailWidth;
                int realLength = 1;
                for (int i = 1; i < trailLength; i++)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    realLength++;
                }
                Vector2[] normals = new Vector2[44];
                Vector2[] positions = new Vector2[45];
                float[] radius = new float[45];
                Vector4[] color = new Vector4[45];
                for (int i = 0; i < realLength; i++)
                {
                    positions[i] = Projectile.oldPos[i] + Projectile.Size / 2;
                    if (i < realLength - 1)
                    {
                        if (positions[i] != Projectile.oldPos[i + 1] + Projectile.Size)
                        {
                            normals[i] = positions[i].DirectionTo(Projectile.oldPos[i + 1] + Projectile.Size).RotatedBy(MathF.PI / 2);
                        }
                        else
                        {
                            normals[i] = Vector2.UnitY;
                        }
                    }
                    radius[i] = trailWidth * MathF.Sqrt(i / (float)trailLength * 6 + 1);
                    color[i] = trailColor.ToVector4() - trailColor.ToVector4() * (realLength - 1 - i) / (realLength - 1f);
                    top = MathF.Min(top, Projectile.oldPos[i].Y + h - radius[i]);
                    bottom = MathF.Max(bottom, Projectile.oldPos[i].Y + h + radius[i]);
                    left = MathF.Min(left, Projectile.oldPos[i].X + w - radius[i]);
                    right = MathF.Max(right, Projectile.oldPos[i].X + w + radius[i]);
                }
                Rectangle rectangle = new Rectangle((int)left, (int)top, (int)right - (int)left, (int)bottom - (int)top);
                rectangle.Location -= Main.screenPosition.ToPoint();
                trailShader.TrySetParameter("positions", positions);
                trailShader.TrySetParameter("scales", radius);
                trailShader.TrySetParameter("colors", color);
                trailShader.TrySetParameter("count", realLength);
                trailShader.TrySetParameter("screenPosition", Main.screenPosition + rectangle.Location.ToVector2());
                trailShader.TrySetParameter("screenSize", rectangle.Size());
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, trailShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(blackTile.Value, rectangle, null, Color.Black/*, 0f, blackTile.Value.Size() * 0.5f, 0, 1f*/);
            }
            ManagedShader shader = ShaderManager.GetShader("Terrapain.GlowShader");
            Texture2D texture = ExtraTextureRegistry.Glow1.Value;
            shader.TrySetParameter("color", Color.LightBlue.ToVector4() * (1 - val));
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Blue, Projectile.rotation, texture.Size() / 2, new Vector2(60, 60) / texture.Size(), SpriteEffects.None, 1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
