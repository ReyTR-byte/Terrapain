using Humanizer;
using log4net.Plugin;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terrapain.Assets.Extratextures;
using Terrapain.Content;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.ModLoader.IO;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.Trails
{
    public class ProjectileTrail
    {
        public List<Vector2> trailCache = new List<Vector2>();
        public List<Vector2> hui = new List<Vector2>();
        public List<int> trailCacheIndexes = new List<int>();
        public float startWidth;
        public float endWidth;
        public Color startColor;
        public Color endColor;
        public int length;
        public bool smooth = true;
        public TrailSettings trailSettings = null;
        public int projectile;
        public Vector2 Offset;
        public List<Vector2> Smooth(List<Vector2> points, float targetDistance, float scale)
        {

            if (points.Count <= 2)
            {
                return points.ToList();
            }
            for (int i = 0; i < trailCache.Count; i++)
            {
                if (trailCacheIndexes[i] >= length)
                {
                    trailCache.RemoveAt(i);
                    trailCacheIndexes.RemoveAt(i);
                    i--;
                }
                else
                {
                    trailCacheIndexes[i]++;
                }
            }
            if (trailCache.Count == 0)
            {
                trailCache.Add(points[2]);
                trailCacheIndexes.Add(0);
            }
            int start = trailCache.Count - 1;
            trailCache.Add(points[1]);
            trailCacheIndexes.Add(0);
            trailCache.Add(points[0]);
            float distance = points[2].Distance(points[1]);
            int target = (int)(distance / targetDistance * scale);
            Vector2 dir1 = Vector2.Zero;
            if (points.Count > 3 && points[2] != points[3])
            {
                dir1 = points[2].DirectionTo(points[1]) + points[2].DirectionFrom(points[3]);
                if (dir1 == Vector2.Zero)
                {
                    dir1 = points[2].DirectionTo(points[1]).RotatedBy(MathF.PI);
                }
                else
                {
                    dir1.Normalize();
                }
            }
            Vector2 dir2 = Vector2.Zero;
            if (points[1] != points[0])
            {
                dir2 = points[1].DirectionTo(points[2]) + points[1].DirectionFrom(points[0]);
                if (dir2 == Vector2.Zero)
                {
                    dir2 = points[1].DirectionTo(points[0]).RotatedBy(MathF.PI);
                }
                else
                {
                    dir2.Normalize();
                }
            }
            for (int i = 1; i < target + 1; i++)
            {
                float progress = (float)i / (target + 1);
                progress = 1 - (MathF.Cos(progress * MathF.PI) + 1) / 2;
                float k = (progress - 0.5f) * 2;
                k = MathF.Cos(MathF.Asin(k));
                Vector2 newPoint = points[2] * (1 - progress) + points[1] * progress;
                newPoint += (dir1 * (1 - progress) + dir2 * (progress)) * distance / 4 * k;
                trailCache.Insert(i + start, newPoint);
                trailCacheIndexes.Add(0);
            }
            trailCache.RemoveAt(trailCache.Count - 1);
            List<Vector2> PointsToReturn = new List<Vector2>();
            start = 1;
            PointsToReturn.Add(trailCache[trailCache.Count - 2]);
            PointsToReturn.Add(trailCache[trailCache.Count - 1]);
            PointsToReturn.Add(points[0]);
            distance = points[0].Distance(points[1]);
            dir1 = -dir2;
            dir2 = Vector2.Zero;
            target = (int)(distance / targetDistance * scale);
            for (int i = 1; i < target + 1; i++)
            {
                float progress = (float)i / (target + 1);
                progress = 1 - (MathF.Cos(progress * MathF.PI) + 1) / 2;
                float k = (progress - 0.5f) * 2;
                k = MathF.Cos(MathF.Asin(k));
                Vector2 newPoint = points[1] * (1 - progress) + points[0] * progress;
                newPoint += (dir1 * (1 - progress) + dir2 * (progress)) * distance / 4 * k;
                PointsToReturn.Insert(i + start, newPoint);
            }
            PointsToReturn.RemoveAt(0);
            PointsToReturn.RemoveAt(0);
            return PointsToReturn;
        }
        Vector2 oldOffset;
        public void Update()
        {
            var proj = Main.projectile[projectile];
            if (smooth)
            {
                Vector2 offset = new Vector2(Offset.X * proj.spriteDirection, Offset.Y).RotatedBy(proj.rotation);
                List<Vector2> positions = new List<Vector2>();
                positions.Add(proj.Center + offset);
                proj.oldPos[0] += oldOffset;
                for (int i = 0; i < 3; i++)
                {
                    if (proj.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    positions.Add(proj.oldPos[i] + proj.Size / 2);
                }
                if (positions.Count > 0)
                {
                    hui = Smooth(positions, 16, Main.GameZoomTarget);
                }
                oldOffset = offset;
            }
            else
            {
                if (proj.oldPos[0] != Vector2.Zero)
                { 
                    Vector2 offset = new Vector2(Offset.X * proj.spriteDirection, Offset.Y).RotatedBy(proj.rotation);
                    proj.oldPos[0] += offset;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            List<Vector2> points = new List<Vector2>();
            var proj = Main.projectile[projectile];
            if (smooth)
            {
                points = new (trailCache);
                points.AddRange(hui);
                if (points.Count == 0)
                {
                    return;
                }
            }
            else
            {
                Vector2 offset = new Vector2(Offset.X * proj.spriteDirection, Offset.Y).RotatedBy(proj.rotation);
                points.Add(proj.Center + offset);
                for (int i = 1; i < length; i++)
                {
                    if (proj.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    points.Add(proj.oldPos[i] + proj.Size / 2);
                }
            }
            float WidthFunction(float progress, float length, float totalLength, Vector2 position)
            {
                if (smooth)
                {
                    progress = 1 - progress;
                }
                return MathHelper.Lerp(startWidth, endWidth, progress) / 2;
            }
            Color ColorFunction(float progress, float length, float totalLength, Vector2 position)
            {
                if (smooth)
                {
                    progress = 1 - progress;
                }

                return new Color(startColor.ToVector4() * (1 - progress) + endColor.ToVector4() * progress);
            }
            if (points.Count > 1)
            {
                //proj.oldPos[2] = proj.position - Vector2.One * 400;
                //proj.oldPos[1] = proj.position - Vector2.UnitX * 200;
                //spriteBatch.DrawLine(proj.Center, proj.oldPos[1] + proj.Size / 2, Color.White, 5);
                //spriteBatch.DrawLine(proj.oldPos[1] + proj.Size / 2, proj.oldPos[2] + proj.Size / 2, Color.Red, 5);
                //spriteBatch.DrawLine(proj.Center, pos2, Color.White, 5);
                //spriteBatch.DrawLine(proj.oldPos[1] + proj.Size / 2, pos2, Color.Red, 5);
                //spriteBatch.DrawLine(proj.oldPos[1] + proj.Size / 2, pos1, Color.Blue, 5);
                //spriteBatch.DrawLine(proj.oldPos[1] + proj.Size / 2, proj.oldPos[1] + proj.Size / 2 + d * 40, Color.Black, 5);
                //spriteBatch.DrawLine(proj.oldPos[2] + proj.Size / 2, pos1, Color.Green, 5);
                ManagedShader shader = ShaderManager.GetShader("Terrapain.TrailShader");
                TrailSettings ts = trailSettings?? new TrailSettings(WidthFunction, ColorFunction, Shader: shader);
                Graphics.RenderTrail(points, ts);
                ManagedShader startShader = ShaderManager.GetShader("Terrapain.TrailStart");
                var blackTile = ExtraTextureRegistry.BlackPixel;
                float rotation = points[1].DirectionTo(points[0]).ToRotation();
                if (smooth)
                {
                    rotation = points[points.Count - 2].DirectionTo(points[points.Count - 1]).ToRotation();
                }
                //foreach (var point in points)
                //{
                //    spriteBatch.Draw(blackTile.Value, point - Main.screenPosition, null, startColor, 0, new Vector2(0.5f, 0.5f), 2, SpriteEffects.None, 0);
                //}
                int num = smooth ? points.Count - 1 : 0;
                Vector2 pos = points[num];
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, startShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(blackTile.Value, pos - Main.screenPosition, null, startColor, rotation, new Vector2(0, 0.5f), new Vector2(startWidth / 2, startWidth), SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
