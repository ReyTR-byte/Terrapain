using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global.Trails;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Content;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.NPCs;
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
        public void TDrawProjectile(Projectile projectile, Texture2D texture, Color lightColor)
        {
            Vector2 DrawCenter = drawCenter;
            if (projectile.spriteDirection == -1)
            {
                DrawCenter.X = texture.Width - DrawCenter.X;
            }
            Color color = projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition + drawOffcet, new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]), color, projectile.rotation, DrawCenter, 1, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
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

                    Color color = Lighting.GetColor(projectile.Center.ToTileCoordinates());
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
                TDrawProjectile(projectile, texture, lightColor);
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
                TDrawProjectile(projectile, texture, lightColor);
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