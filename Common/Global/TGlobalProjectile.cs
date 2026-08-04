using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global.Trails;
using Terrapain.Common.Player;
using Terrapain.Content;
using Terrapain.Content.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class TGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool useModDrawingInPreDraw;
        public bool useModDrawingInPostDraw;
        public bool useVanillaDrawing = true;
        public bool drawExtras = true;
        public Vector2 drawOffcet;
        public Vector2 drawCenter;
        public bool NonPremultiplied;
        public bool afterimage;
        public int afterimagesCount;
        public int[] oldFrame = new int[60];
        public bool fullLight;
        public ProjectileTrail trail = null;


        public override void Load()
        {
            On_Projectile.UpdatePosition += On_Projectile_UpdatePosition;
        }
        public override void Unload()
        {
            On_Projectile.UpdatePosition -= On_Projectile_UpdatePosition;
        }

        private void On_Projectile_UpdatePosition(On_Projectile.orig_UpdatePosition orig, Projectile self, Vector2 wetVelocity)
        {
            orig(self, wetVelocity);
            if (self.GetT().trail != null)
            {
                self.GetT().trail.projectile = self.whoAmI;
                self.GetT().trail.Update();
            }
        }

        public override void SetDefaults(Projectile entity)
        {
            if (TextureAssets.Projectile[entity.type] != null)
            {
                Main.instance.LoadProjectile(entity.type);
                Texture2D texture = TextureAssets.Projectile[entity.type].Value;
                drawCenter = texture.Size() / 2;
            }
            oldFrame = new int[ProjectileID.Sets.TrailCacheLength[entity.type]];
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (Main.projHook[projectile.type])
            {
                if (projectile.owner == -1)
                    return;
                Terraria.Player player = Main.player[projectile.owner];
                if (player.Custom().unarmed)
                {
                    projectile.velocity = player.GetUnarmedDirection() * projectile.velocity.Length();
                }
            }
        }
        int afterimageTimer;
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (NonPremultiplied)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (fullLight)
            {
                lightColor = Color.White;
            }
            Texture2D texture;
            if (projectile.type < ProjectileID.Count)
            {
                texture = TextureAssets.Projectile[projectile.type].Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>(projectile.ModProjectile.Texture).Value;
            }
            if (trail != null)
            {
                trail.Draw(Main.spriteBatch);
                if (NonPremultiplied)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                //int realLength = 0;
                //for (int i = 1; i < length; i++)
                //{
                //    if (projectile.oldPos[i] == Vector2.Zero)
                //    {
                //        break;
                //    }
                //    realLength++;
                //}
                //Vector2[] positions = new Vector2[45];
                //float[] radius = new float[45];
                //Vector4[] color = new Vector4[45];
                //float totatlLength = 0;
                //for (int i = 0; i < realLength; i++)
                //{
                //    positions[i] = projectile.oldPos[i] + projectile.Size / 2;
                //    if (i > 0)
                //    {
                //        totatlLength += positions[i].Distance(positions[i - 1]);
                //    }
                //    radius[i] = trailWidth * MathHelper.Lerp(1, 0, (float)i / length);
                //    color[i] = trailColor.ToVector4();
                //}

                    //float WidthFunction(float progress)
                    //{
                    //    return (1 - progress) * trailWidth / 2;
                    //}
                    //Color ColorFunction(float progress)
                    //{
                    //    return trailColor;
                    //}
                    //ManagedShader shader = ShaderManager.GetShader("Terrapain.TrailShader");
                    //TrailSettings primitiveSettings = new TrailSettings(WidthFunction, ColorFunction, Shader: shader);
                    //PrimitiveRenderer.RenderTrail(positions, primitiveSettings);
                    //ManagedShader startShader = ShaderManager.GetShader("Terrapain.TrailStart");
                    //var blackTile = ExtraTextureRegistry.BlackPixel;
                    //float rotation = positions[1].DirectionTo(positions[0]).ToRotation();
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, startShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    //Main.spriteBatch.Draw(blackTile.Value, positions[0] - Main.screenPosition, null, trailColor, rotation, new Vector2(0, 0.5f), new Vector2(trailWidth / 2, trailWidth), SpriteEffects.None, 0);
                    //if (NonPremultiplied)
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
                    //else
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
                    //var blackTile = ExtraTextureRegistry.BlackPixel;
                    //ManagedShader trailShader = ShaderManager.GetShader("Terrapain.TrailingShader");
                    //int w = projectile.width / 2;
                    //int h = projectile.height / 2;
                    //float top = projectile.Center.Y - trailWidth;
                    //float bottom = projectile.Center.Y + trailWidth;
                    //float left = projectile.Center.X - trailWidth;
                    //float right = projectile.Center.X + trailWidth;
                    //int realLength = 0;
                    //for (int i = 1; i < length; i++)
                    //{
                    //    if (projectile.oldPos[i] == Vector2.Zero)
                    //    {
                    //        break;
                    //    }
                    //    realLength++;
                    //    float rad = trailWidth * MathHelper.Lerp(1, 0, (float)i / length);
                    //    top = MathF.Min(top, projectile.oldPos[i].Y + h - rad);
                    //    bottom = MathF.Max(bottom, projectile.oldPos[i].Y + h + rad);
                    //    left = MathF.Min(left, projectile.oldPos[i].X + w - rad);
                    //    right = MathF.Max(right, projectile.oldPos[i].X + w + rad);
                    //}
                    //Vector2[] normals = new Vector2[44];
                    //Vector2[] positions = new Vector2[45];
                    //float[] radius = new float[45];
                    //Vector4[] color = new Vector4[45];
                    //for (int i = 0; i < realLength; i++)
                    //{
                    //    positions[i] = projectile.oldPos[i] + projectile.Size / 2;
                    //    if (i < realLength - 1)
                    //    {
                    //        if (positions[i] != projectile.oldPos[i + 1] + projectile.Size)
                    //        {
                    //            normals[i] = positions[i].DirectionTo(projectile.oldPos[i + 1] + projectile.Size).RotatedBy(MathF.PI / 2);
                    //        }
                    //        else
                    //        {
                    //            normals[i] = Vector2.UnitY;
                    //        }
                    //    }
                    //    radius[i] = trailWidth * MathHelper.Lerp(1, 0, (float)i / length);
                    //    color[i] = trailColor.ToVector4();
                    //}
                    //Rectangle rectangle = new Rectangle((int)left, (int)top, (int)right - (int)left, (int)bottom - (int)top);
                    //rectangle.Location -= Main.screenPosition.ToPoint();
                    //trailShader.TrySetParameter("positions", positions);
                    //trailShader.TrySetParameter("scales", radius);
                    //trailShader.TrySetParameter("colors", color);
                    //trailShader.TrySetParameter("count", realLength);
                    //trailShader.TrySetParameter("normals", normals);
                    //trailShader.TrySetParameter("screenPosition", Main.screenPosition + rectangle.Location.ToVector2());
                    //trailShader.TrySetParameter("screenSize", rectangle.Size());
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, trailShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    //Main.spriteBatch.Draw(blackTile.Value, rectangle, null, Color.Black/*, 0f, blackTile.Value.Size() * 0.5f, 0, 1f*/);
                    //if (NonPremultiplied)
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
                    //else
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
            }
            if (afterimage || afterimageTimer > 0)
            {
                if (afterimageTimer < afterimagesCount && afterimage && !Main.gamePaused)
                {
                    afterimageTimer++;
                }
                for (int i = 0; i < (afterimageTimer > afterimagesCount? afterimagesCount : afterimageTimer); i++)
                {
                    Vector2 DrawCenter = drawCenter;
                    if (projectile.oldSpriteDirection[i] == -1)
                    {
                        DrawCenter.X = texture.Width - DrawCenter.X;
                    }
                    float rotation = projectile.rotation;
                    int spriteDir = projectile.spriteDirection;
                    if(ProjectileID.Sets.TrailingMode[projectile.type] != 0)
                    {
                        rotation = projectile.oldRot[i];
                        spriteDir = projectile.oldSpriteDirection[i];
                    }

                    Color color = Lighting.GetColor((int)(projectile.position.X + DrawCenter.X) / 16, (int)(projectile.position.Y + DrawCenter.Y) / 16);
                    color *= (afterimagesCount - i) / (float)afterimagesCount * 0.5f;
                    Main.EntitySpriteDraw(texture, projectile.oldPos[i] - Main.screenPosition + projectile.Size / 2 + drawOffcet, new Rectangle(0, oldFrame[i] * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, rotation, DrawCenter, 1, spriteDir == 1? SpriteEffects.None : SpriteEffects.FlipHorizontally);  
                }
            }
            if (!afterimage && afterimageTimer > 0 && !Main.gamePaused)
            {
                afterimageTimer--;
            }
            if (useModDrawingInPreDraw)
            {
                Vector2 DrawCenter = drawCenter;
                if (projectile.spriteDirection == -1)
                {
                    DrawCenter.X = texture.Width - DrawCenter.X;
                }
                Color color = lightColor;
                color.A = (byte)(255 - projectile.alpha);
                Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + drawOffcet, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, projectile.rotation, DrawCenter, 1, projectile.spriteDirection == 1? SpriteEffects.None : SpriteEffects.FlipHorizontally);  
            }
            return useVanillaDrawing;
        }
        public override bool PreDrawExtras(Projectile projectile)
        {
            return drawExtras;
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (fullLight)
            {
                lightColor = Color.White;
            }
            Texture2D texture;
            if (projectile.type < ProjectileID.Count)
            {
                texture = TextureAssets.Projectile[projectile.type].Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>(ModContent.GetModProjectile(projectile.type).Texture).Value;
            }

            if (useModDrawingInPostDraw)
            {
                Vector2 DrawCenter = drawCenter;
                if (projectile.spriteDirection == -1)
                {
                    DrawCenter.X = texture.Width - DrawCenter.X;
                }
                Color color = lightColor;
                if (NonPremultiplied){
                    color.A = (byte)(255 - projectile.alpha);
                }
                else
                {
                    color *= (255 - projectile.alpha) / 255f;
                }
                Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + drawOffcet, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, projectile.rotation, DrawCenter, 1, projectile.spriteDirection == 1? SpriteEffects.None : SpriteEffects.FlipHorizontally);  
            }
            if (NonPremultiplied)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public override void PostAI(Projectile projectile)
        {
            //for (int i = oldPositions.Length - 1; i > 0; i--)
            //{
            //    oldPositions[i] = oldPositions[i - 1];
            //}
            //for (int i = oldRotation.Length - 1; i > 0; i--)
            //{
            //    oldRotation[i] = oldRotation[i - 1];
            //}
            for (int i = oldFrame.Length - 1; i > 0; i--)
            {
                oldFrame[i] = oldFrame[i - 1];
            }
            //for (int i = oldDirections.Length - 1; i > 0; i--)
            //{
            //    oldDirections[i] = oldDirections[i - 1];
            //}

            //oldPositions[0] = projectile.position;
            //oldRotation[0] = projectile.rotation;
            oldFrame[0] = projectile.frame;
            //oldDirections[0] = projectile.spriteDirection;
        }
    }
}