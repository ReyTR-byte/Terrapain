using Iced.Intel;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;
using static Terrapain.Content.TUtilities.Graphics.TrailSettings;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Terrapain.Content.TUtilities.Graphics
{
    public class Graphics : ILoadable
    {
        #region Fields/Properties
        private static DynamicVertexBuffer VertexBuffer;

        private static DynamicIndexBuffer IndexBuffer;

        //private static IPrimitiveSettings MainSettings;

        private static Vector2[] MainPositions;

        public static VertexPosition2DColorTexture[] MainVertices;

        public static short[] MainIndices;

        private const short MaxTrailPositions = 2000;

        /// <summary>
        /// Must be lower than <see cref="MaxTrailPositions"/>, less than 1/4 of <see cref="MaxVertices"/> and less than 1/6 of <see cref="MaxIndices"/>.
        /// </summary>
        private const short MaxCirclePositions = 1500;

        private const short MaxVertices = 6144;

        private const short MaxIndices = 16384;

        private static short PositionsIndex;

        public static short VerticesIndex;

        public static short IndicesIndex;

        private static readonly short[] QuadIndices = [0, 1, 2, 2, 3, 0];

        private static Matrix QuadVertexMatrix;
        #endregion

        #region General Methods
        void ILoadable.Load(Mod mod)
        {
            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                MainPositions = new Vector2[MaxTrailPositions];
                MainVertices = new VertexPosition2DColorTexture[MaxVertices];
                MainIndices = new short[MaxIndices];
                VertexBuffer ??= new DynamicVertexBuffer(Main.instance.GraphicsDevice, VertexPosition2DColorTexture.VertexDeclaration2D, MaxVertices, BufferUsage.WriteOnly);
                IndexBuffer ??= new DynamicIndexBuffer(Main.instance.GraphicsDevice, IndexElementSize.SixteenBits, MaxIndices, BufferUsage.WriteOnly);
            });
        }

        void ILoadable.Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                MainPositions = null;
                MainVertices = null;
                MainIndices = null;
                VertexBuffer?.Dispose();
                VertexBuffer = null;
                IndexBuffer?.Dispose();
                IndexBuffer = null;
            });
        }

        //private static void PerformPixelationSafetyChecks(IPrimitiveSettings settings)
        //{
        //    // Don't allow accidental screw ups with these.
        //    if (settings.Pixelate && !PrimitivePixelationSystem.CurrentlyRendering)
        //        throw new Exception("Error: Primitives using pixelation MUST be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method, did you forget to use the interface?");
        //    else if (!settings.Pixelate && PrimitivePixelationSystem.CurrentlyRendering)
        //        throw new Exception("Error: Primitives not using pixelation MUST NOT be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method.");
        //}
        #endregion
        public struct Triangle
        {
            public Vector2 pos1;
            public Vector2 pos2;
            public Vector2 pos3;
            public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
            {
                pos1 = p1;
                pos2 = p2;
                pos3 = p3;
            }
        }
        /// <summary>
        /// end spriteBatch before call it
        /// </summary>
        /// <param name="lightning"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void DrawLightning(Terrapain.LightningDrawInfo lightning, float start = 0, float end = -1)
        {
            if (end == -1)
            {
                end = lightning.TotalLength;
            }
            if (end <= 0 || start >= lightning.TotalLength || start > end)
            {
                return;
            }
            Vector2 TopLeft = Vector2.Zero;
            Vector2 BottomLeft = Vector2.Zero;
            Vector2? TopRight = Vector2.Zero;
            Vector2? BottomRight = Vector2.Zero;

            List<Triangle> triangles = [];

            ManagedShader shader = ShaderManager.GetShader("Terrapain.LightningShader");

            Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;

            VerticesIndex = 0;
            IndicesIndex = 0;

            int starti = 0;
            if (start > 0)
            {
                float progress = 0;
                for (int i = 0; i < lightning.Count; i++)
                {
                    progress += lightning.parts[i].Length;
                    if (progress > start)
                    {
                        starti = i + 1;
                        break;
                    }
                }
            }
            int endi = lightning.Count;
            if (end < lightning.TotalLength)
            {
                float progress = lightning.TotalLength;
                for (int i = lightning.Count - 1; i > 0; i--)
                {
                    progress -= lightning.parts[i].Length;
                    if (progress < end)
                    {
                        endi = i;
                        break;
                    }
                }
            }
            if (starti == endi)
                return;
            for (int i = starti; i < endi; i++)
            {
                var l = lightning.parts[i];
                float length = (l.start - l.end).Length();
                float rotation = (l.end - l.start).ToRotation();
                float biggestWidth = Math.Max(l.startWidth, l.endWidth);

                Vector2 TopRightCandidate = l.end - (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
                Vector2 BottomRightCandidate = l.end + (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
                if (i == starti)
                {
                    ManagedShader HalfCircle = ShaderManager.GetShader("Terrapain.HalfCircle");
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, HalfCircle.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Main.spriteBatch.Draw(texture, l.start - Main.screenPosition, null, lightning.color, rotation, new Vector2(0.5f, 0.5f), l.startWidth, SpriteEffects.None, 0);
                    Main.spriteBatch.End();
                    TopLeft = l.start - (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                    BottomLeft = l.start + (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                }
                else
                {
                    TopLeft = TopRight ?? l.start - (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                    BottomLeft = BottomRight ?? l.start + (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                }
                if (i + 1 < endi)
                {
                    var l1 = lightning.parts[i + 1];
                    float rotation1 = (l1.end - l1.start).ToRotation();
                    Vector2 start2 = l1.start - (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1);
                    Vector2 end2 = l1.end - (Vector2.UnitY * l1.endWidth / 2).RotatedBy(rotation1);

                    TopRight = RayColision(TopLeft, TopRightCandidate, start2, end2);// ?? TopRightCandidate;

                    start2 = l1.start + (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1);
                    end2 = l1.end + (Vector2.UnitY * l1.endWidth / 2).RotatedBy(rotation1);

                    BottomRight = RayColision(BottomLeft, BottomRightCandidate, start2, end2);// ?? BottomRightCandidate;

                    if (TopRight == null)
                    {
                        //triangles.Add(new Triangle(l.end, TopRightCandidate, l1.start - (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1)));
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new(l.end - Main.screenPosition, lightning.color, new Vector2(0, 0), 1);
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new(TopRightCandidate - Main.screenPosition, lightning.color, new Vector2(0, 1), 1);
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new((l1.start - (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1)) - Main.screenPosition, lightning.color, new Vector2(1, 1), 1);
                    }
                    else if (BottomRight == null)
                    {
                        //triangles.Add(new Triangle(l.end, BottomRightCandidate, start2));
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new(l.end - Main.screenPosition, lightning.color, new Vector2(0, 0), 1);
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new(BottomRightCandidate - Main.screenPosition, lightning.color, new Vector2(0, 1), 1);
                        MainIndices[IndicesIndex++] = VerticesIndex;
                        MainVertices[VerticesIndex++] = new(start2 - Main.screenPosition, lightning.color, new Vector2(1, 1), 1);
                    }
                }
                else
                {
                    BottomRight = l.end + (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
                    TopRight = l.end + (Vector2.UnitY * -l.endWidth / 2).RotatedBy(rotation);
                    if (l.endWidth > 0)
                    {
                        ManagedShader HalfCircle = ShaderManager.GetShader("Terrapain.HalfCircle");
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, HalfCircle.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                        Main.spriteBatch.Draw(texture, l.end - Main.screenPosition, null, lightning.color, rotation + MathF.PI, new Vector2(0.5f, 0.5f), l.endWidth, SpriteEffects.None, 0);
                        Main.spriteBatch.End();
                    }
                }
                Color color = lightning.color;
                short num1 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(TopLeft - Main.screenPosition, color, new Vector2(0, 1), 1f);
                short num2 = VerticesIndex;
                MainVertices[VerticesIndex++] = new((TopRight ?? TopRightCandidate) - Main.screenPosition, color, new Vector2(1, 1), 1f);
                short num3 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(l.start - Main.screenPosition, color, new Vector2(0, 0), 1f);
                short num4 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(l.end - Main.screenPosition, color, new Vector2(1, 0), 1f);
                short num5 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(BottomLeft - Main.screenPosition, color, Vector2.UnitY, 1f);
                short num6 = VerticesIndex;
                MainVertices[VerticesIndex++] = new((BottomRight ?? BottomRightCandidate) - Main.screenPosition, color, Vector2.One, 1f);

                MainIndices[IndicesIndex++] = num1;
                MainIndices[IndicesIndex++] = num2;
                MainIndices[IndicesIndex++] = num3;
                MainIndices[IndicesIndex++] = num3;
                MainIndices[IndicesIndex++] = num4;
                MainIndices[IndicesIndex++] = num2;
                MainIndices[IndicesIndex++] = num3;
                MainIndices[IndicesIndex++] = num4;
                MainIndices[IndicesIndex++] = num5;
                MainIndices[IndicesIndex++] = num5;
                MainIndices[IndicesIndex++] = num6;
                MainIndices[IndicesIndex++] = num4;
            }
            DrawPimitives(shader);
            // Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            // Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            // Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            // Vector2 quadArea = texture.Size();
            // float maxDimension = MathF.Max(quadArea.X, quadArea.Y);

            // var viewMatrix = Main.GameViewMatrix.TransformationMatrix
            //     * Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -maxDimension - 1f, maxDimension + 1f);
            // shader.TrySetParameter("uWorldViewProjection", viewMatrix);
            // shader.Apply();

            // VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            // IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            // Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            // Main.instance.GraphicsDevice.Indices = IndexBuffer;
            // Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, IndicesIndex / 3);
        }
        public static void RenderTrail(IEnumerable<Vector2> Points, TrailSettings settings, int segmentsCount = 1, Vector2? dirIn = null, Vector2? dirOut = null, bool DebugPoints = false)
        {
            MakeTrailVertices(Points, settings, dirIn, dirOut, segmentsCount);
            // VerticesIndex = 0;
            // IndicesIndex = 0;
            // float totalLength = 0;
            // for (int i = 1; i < Points.Count(); i++)
            // {
            //     totalLength += Points.ElementAt(i).Distance(Points.ElementAt(i - 1));
            // }
            // float length = 0;
            // float progress = 0;
            // for (int i = 0; i < Points.Count(); i++)
            // {
            //     if (i > 0)
            //     {
            //         MainIndices[IndicesIndex++] = (short)(i * 2 - 2);
            //         MainIndices[IndicesIndex++] = (short)(i * 2 - 1);
            //         MainIndices[IndicesIndex++] = (short)(i * 2);
            //         MainIndices[IndicesIndex++] = (short)(i * 2);
            //         MainIndices[IndicesIndex++] = (short)(i * 2 + 1);
            //         MainIndices[IndicesIndex++] = (short)(i * 2 - 1);
            //         float distance = (Points.ElementAt(i).Distance(Points.ElementAt(i - 1)));
            //         length += distance;
            //         progress = length / totalLength * segmentsCount;
            //     }
            //     Color color = settings.ColorFunction(progress, length, totalLength, Points.ElementAt(i));
            //     float Width = settings.WidthFunction(progress, length, totalLength, Points.ElementAt(i));
            //     Vector2 dir = Vector2.Zero;
            //     if (i == 0)
            //     {
            //         dir = dirIn?? Points.ElementAt(0).DirectionTo(Points.ElementAt(1));
            //         dir = new Vector2(dir.Y, -dir.X);
            //     }
            //     else if (i < Points.Count() - 1)
            //     {
            //         Vector2 dir1 = Points.ElementAt(i).DirectionTo(Points.ElementAt(i + 1));
            //         Vector2 dir2 = Points.ElementAt(i).DirectionTo(Points.ElementAt(i - 1));
            //         float angle = AngleBetweenVectors(dir1, dir2);
            //         if (angle > MathF.PI * 0.99f || angle < -MathF.PI * 0.99f)
            //         {
            //             dir = new Vector2(dir1.Y, -dir1.X);
            //         }
            //         else
            //         {
            //             if (angle > 0)
            //             {
            //                 dir = ((dir1 + dir2) / 2).ToUnit();
            //             }
            //             else
            //             {
            //                 dir = -((dir1 + dir2) / 2).ToUnit();
            //             }
            //         }
            //     }
            //     else
            //     {
            //         dir = dirOut?? Points.ElementAt(i).DirectionFrom(Points.ElementAt(i - 1));
            //         dir = new Vector2(dir.Y, -dir.X);
            //     }

            //     MainVertices[VerticesIndex++] = new(Points.ElementAt(i) + dir * Width - Main.screenPosition, color, new Vector2(progress, 1), 1);
            //     MainVertices[VerticesIndex++] = new(Points.ElementAt(i) - dir * Width - Main.screenPosition, color, new Vector2(progress, 0), 1);
            // }
            // Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
            ManagedShader shader = settings.Shader?? ShaderManager.GetShader("Terrapain.TrailShader");

            // Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            // Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            // Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            // Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            // Vector2 quadArea = texture.Size();
            // float maxDimension = MathF.Max(quadArea.X, quadArea.Y);

            // var viewMatrix = Main.GameViewMatrix.TransformationMatrix
            //     * Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -maxDimension - 1f, maxDimension + 1f);
            // shader.TrySetParameter("uWorldViewProjection", viewMatrix);
            // shader.Apply();

            // VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            // IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            // Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            // Main.instance.GraphicsDevice.Indices = IndexBuffer;
            // Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, IndicesIndex / 3);
            DrawPimitives(shader);
            if (DebugPoints)
            {
                foreach(var point in Points)
                {
                    Main.spriteBatch.Draw(ExtraTextureRegistry.BlackPixel.Value, point - Main.screenPosition, null, Color.Black, 0, new Vector2(0.5f), 4, SpriteEffects.None, 0);
                }
            }
        }
        public static void MakeTrailVertices(IEnumerable<Vector2> Points, TrailSettings settings, Vector2? dirIn = null, Vector2? dirOut = null, int segmentsCount = 1, float? TotalLength = null)
        {
            VerticesIndex = 0;
            IndicesIndex = 0;
            float totalLength = TotalLength?? 0;
            if (!TotalLength.HasValue)
            {
                for (int i = 1; i < Points.Count(); i++)
                {
                    totalLength += Points.ElementAt(i).Distance(Points.ElementAt(i - 1));
                }
            }
            float length = 0;
            float progress = 0;
            for (int i = 0; i < Points.Count(); i++)
            {
                if (i > 0)
                {
                    MainIndices[IndicesIndex++] = (short)(i * 2 - 2);
                    MainIndices[IndicesIndex++] = (short)(i * 2 - 1);
                    MainIndices[IndicesIndex++] = (short)(i * 2);
                    MainIndices[IndicesIndex++] = (short)(i * 2);
                    MainIndices[IndicesIndex++] = (short)(i * 2 + 1);
                    MainIndices[IndicesIndex++] = (short)(i * 2 - 1);
                    float distance = (Points.ElementAt(i).Distance(Points.ElementAt(i - 1)));
                    length += distance;
                    progress = length / totalLength * segmentsCount;
                }
                Color color = settings.ColorFunction(progress, length, totalLength, Points.ElementAt(i));
                float Width = settings.WidthFunction(progress, length, totalLength, Points.ElementAt(i));
                Vector2 dir = Vector2.Zero;
                if (i == 0)
                {
                    dir = dirIn?? Points.ElementAt(0).DirectionTo(Points.ElementAt(1));
                    dir = new Vector2(dir.Y, -dir.X);
                }
                else if (i < Points.Count() - 1)
                {
                    Vector2 dir1 = Points.ElementAt(i).DirectionTo(Points.ElementAt(i + 1));
                    Vector2 dir2 = Points.ElementAt(i).DirectionTo(Points.ElementAt(i - 1));
                    float angle = AngleBetweenVectors(dir1, dir2);
                    if (angle > MathF.PI * 0.99f || angle < -MathF.PI * 0.99f)
                    {
                        dir = new Vector2(dir1.Y, -dir1.X);
                    }
                    else
                    {
                        if (angle > 0)
                        {
                            dir = ((dir1 + dir2) / 2).ToUnit();
                        }
                        else
                        {
                            dir = -((dir1 + dir2) / 2).ToUnit();
                        }
                    }
                }
                else
                {
                    dir = dirOut?? Points.ElementAt(i).DirectionFrom(Points.ElementAt(i - 1));
                    dir = new Vector2(dir.Y, -dir.X);
                }

                MainVertices[VerticesIndex++] = new(Points.ElementAt(i) + dir * Width - Main.screenPosition, color, new Vector2(progress, 1), 1);
                MainVertices[VerticesIndex++] = new(Points.ElementAt(i) - dir * Width - Main.screenPosition, color, new Vector2(progress, 0), 1);
            }
        }
        public static void DrawPimitives(ManagedShader shader, SamplerState samplerState = null, RasterizerState rasterizerState = null)
        {
            //Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
            //ManagedShader shader = settings.Shader?? ShaderManager.GetShader("Terrapain.TrailShader");

            Main.instance.GraphicsDevice.SamplerStates[1] = samplerState?? SamplerState.PointClamp;
            Main.instance.GraphicsDevice.RasterizerState = rasterizerState??RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            //Vector2 quadArea = texture.Size();
            //float maxDimension = MathF.Max(quadArea.X, quadArea.Y);

            var viewMatrix = Main.GameViewMatrix.TransformationMatrix
                * Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -2f, 2f);
            shader.TrySetParameter("uWorldViewProjection", viewMatrix);
            shader.Apply();

            VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Main.instance.GraphicsDevice.Indices = IndexBuffer;
            Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, IndicesIndex / 3);
        }
        public static bool IsPointInBoundsOfScreen(Vector2 point, int expantion)
        {
            point -= Main.screenPosition;
            float diffX = Main.screenWidth - Main.screenWidth / Main.GameZoomTarget;
            float diffY = Main.screenHeight - Main.screenHeight / Main.GameZoomTarget;
            point.X -= diffX / 2;
            point.Y -= diffY / 2;
            expantion = (int)(expantion * Main.GameZoomTarget);
            return point.X > - expantion && point.X < Main.screenWidth / Main.GameZoomTarget + expantion && point.Y > -expantion && point.Y < Main.screenHeight + expantion;
        }
        public static List<Vector2> SmoothTrail(List<Vector2> points, int targetDistance, Vector2? In = null, Vector2? Out = null, int screenBounds = 20)
        {
            Vector2 dir1 = Vector2.Zero;
            Vector2 dir2 = Vector2.Zero;
            List<Vector2> SmoothedPoints = [points[0]];
            float scale = Main.GameZoomTarget;
            if (points.Count < 3)
            {
                if (points.Count == 2 && In.HasValue || Out.HasValue)
                {
                    float _MinX = Main.screenPosition.X - screenBounds;
                    float _MaxX = Main.screenPosition.X + Main.ScreenSize.X + screenBounds;
                    float _MinY = Main.screenPosition.Y - screenBounds;
                    float _MaxY = Main.screenPosition.Y + Main.ScreenSize.Y + screenBounds;
                    dir1 = In?? Vector2.Zero;
                    dir2 = -Out?? Vector2.Zero;
                    if (IsPointInBoundsOfScreen(points[0], screenBounds) || IsPointInBoundsOfScreen(points[1], screenBounds))
                    {
                        float distance = points[0].Distance(points[1]);
                        int target = (int)(distance / targetDistance * scale);
                        for (int j = 1; j < target + 1; j++)
                        {
                            float progress = (float)j / (target + 1);
                            progress = 1 - (MathF.Cos(progress * MathF.PI) + 1) / 2;
                            float k = (progress - 0.5f) * 2;
                            k = MathF.Cos(MathF.Asin(k));
                            Vector2 newPoint = points[0] * (1 - progress) + points[1] * progress;
                            newPoint += (dir1 * MathF.Sqrt(1 - progress) + dir2 * MathF.Sqrt(progress)) * distance / 4 * k;
                            SmoothedPoints.Add(newPoint);
                        }
                    }
                    SmoothedPoints.Add(points[1]);
                    return SmoothedPoints;
                }
                return points;
            }
            float MinX = Main.screenPosition.X - screenBounds;
            float MaxX = Main.screenPosition.X + Main.ScreenSize.X + screenBounds;
            float MinY = Main.screenPosition.Y - screenBounds;
            float MaxY = Main.screenPosition.Y + Main.ScreenSize.Y + screenBounds;
            
            if (In.HasValue)
            {
                dir2 = -In.Value;
            }
            else
            {
                dir2 = points[1].DirectionTo(points[0]) + points[1].DirectionFrom(points[2]);
                if (dir2 == Vector2.Zero)
                {
                    dir2 = points[1].DirectionTo(points[0]).RotatedBy(MathF.PI);
                }
                else
                {
                    dir2.Normalize();
                }
            }
            for (int i = 0; i < points.Count - 2; i++)
            {
                dir1 = -dir2;
                dir2 = points[i + 1].DirectionTo(points[i]) + points[i + 1].DirectionFrom(points[i + 2]);
                if (dir2 == Vector2.Zero)
                {
                    dir2 = points[i + 1].DirectionTo(points[i]).RotatedBy(MathF.PI);
                }
                else
                {
                    dir2.Normalize();
                }
                if (IsPointInBoundsOfScreen(points[i], screenBounds) || IsPointInBoundsOfScreen(points[i + 1], screenBounds))
                {
                    float distance = points[i].Distance(points[i + 1]);
                    int target = (int)(distance / targetDistance * scale);
                    for (int j = 1; j < target + 1; j++)
                    {
                        float progress = (float)j / (target + 1);
                        progress = 1 - (MathF.Cos(progress * MathF.PI) + 1) / 2;
                        float k = (progress - 0.5f) * 2;
                        k = MathF.Cos(MathF.Asin(k));
                        Vector2 newPoint = points[i] * (1 - progress) + points[i + 1] * progress;
                        newPoint += (dir1 * (1 - progress) + dir2 * (progress)) * distance / 4 * k;
                        SmoothedPoints.Add(newPoint);
                    }
                }
                SmoothedPoints.Add(points[i + 1]);
            }
            int c = points.Count - 2;
            if (IsPointInBoundsOfScreen(points[c], screenBounds) || IsPointInBoundsOfScreen(points[c + 1], screenBounds))
            {
                dir1 = -dir2;
                if (Out.HasValue)
                {
                    dir2 = -Out.Value;
                }
                else
                {
                    dir2 = points[c].DirectionFrom(points[c + 1]) + points[c].DirectionFrom(points[c - 1]);
                    if (dir2 == Vector2.Zero)
                    {
                        dir2 = points[c + 1].DirectionTo(points[c]);
                    }
                    else
                    {
                        dir2.Normalize();
                    }
                }
                float distance = points[c].Distance(points[c + 1]);
                int target = (int)(distance / targetDistance * scale);
                for (int j = 1; j < target + 1; j++)
                {
                    float progress = (float)j / (target + 1);
                    progress = 1 - (MathF.Cos(progress * MathF.PI) + 1) / 2;
                    float k = (progress - 0.5f) * 2;
                    k = MathF.Cos(MathF.Asin(k));
                    Vector2 newPoint = points[c] * (1 - progress) + points[c + 1] * progress;
                    newPoint += (dir1 * MathF.Sqrt(1 - progress) + dir2 * MathF.Sqrt(progress)) * distance / 4 * k;
                    SmoothedPoints.Add(newPoint);
                }
            }
            SmoothedPoints.Add(points[c + 1]);

            return SmoothedPoints;
        }
        public static void RenderSnakeBody(List<Vector2> SmoothedPoints, float HalfWidth, int SegmentsCount, Rectangle Frame, Texture2D SegmentTexture, List<Vector4> SegmentsGetAlphaColors = null, List<Vector4> SegmentsGetColorColors = null, Vector4? HeadAlpha = null, Vector4? HeadColor = null, Vector4? TailAlpha = null, Vector4? TailColor = null, ManagedShader Shader = null, Vector2? dirIn = null, Vector2? dirOut = null)
        {
            Main.instance.GraphicsDevice.Textures[1] = SegmentTexture;
            float WidthFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
            {
                return HalfWidth;
            }
            Color AlphaFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
            {
                if (SegmentsCount != SegmentsGetAlphaColors.Count)
                {
                    trailLengthInterpolant /= SegmentsCount;
                    trailLengthInterpolant *= SegmentsGetAlphaColors.Count;
                }
                if (trailLengthInterpolant < 0.5f)
                {
                    if (HeadAlpha.HasValue)
                    {
                        trailLengthInterpolant *= 2f;
                        Vector4 color1 = HeadAlpha.Value;
                        Vector4 color2 = SegmentsGetAlphaColors[0];
                        return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                    }
                    return new Color(SegmentsGetAlphaColors[0]);
                }
                trailLengthInterpolant -= 0.5f;
                int s = (int)trailLengthInterpolant;
                trailLengthInterpolant -= s;
                if (s == SegmentsCount - 1)
                {
                    if (TailAlpha.HasValue)
                    {
                        trailLengthInterpolant *= 2f;
                        Vector4 color1 = SegmentsGetAlphaColors[s];
                        Vector4 color2 = TailAlpha.Value;
                        return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                    }
                    return new Color(SegmentsGetAlphaColors[s]);
                }
                {
                    Vector4 color1 = SegmentsGetAlphaColors[s];
                    Vector4 color2 = SegmentsGetAlphaColors[s + 1];
                    return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                }
            }
            Color NullColor(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
            {
                return Color.White;
            }
            ManagedShader shader = Shader?? ShaderManager.GetShader("Terrapain.SnakeShader");
            Vector2 size = SegmentTexture.Size();
            Vector4 frame = new Vector4(Frame.X / size.X, Frame.Y / size.Y, Frame.Width / size.X, Frame.Height / size.Y);
            shader.TrySetParameter("frame", frame);
            TrailSettings settings = new TrailSettings(WidthFunction, SegmentsGetAlphaColors == null || SegmentsGetAlphaColors.Count == 0? NullColor : AlphaFunction, Shader: shader);
            RenderTrail(SmoothedPoints, settings, SegmentsCount, dirIn, dirOut);
            if (SegmentsGetColorColors != null)
            {
                Color ColorFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
                {
                    if (SegmentsCount != SegmentsGetColorColors.Count)
                    {
                        trailLengthInterpolant /= SegmentsCount;
                        trailLengthInterpolant *= SegmentsGetColorColors.Count;
                    }
                    if (trailLengthInterpolant < 0.5f)
                    {
                        if (HeadColor.HasValue)
                        {
                            trailLengthInterpolant *= 2f;
                            Vector4 color1 = HeadColor.Value;
                            Vector4 color2 = SegmentsGetColorColors[0];
                            return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                        }
                        return new Color(SegmentsGetColorColors[0]);
                    }
                    trailLengthInterpolant -= 0.5f;
                    int s = (int)trailLengthInterpolant;
                    trailLengthInterpolant -= s;
                    if (s == SegmentsCount - 1)
                    {
                        if (TailColor.HasValue)
                        {
                            trailLengthInterpolant *= 2f;
                            Vector4 color1 = SegmentsGetColorColors[s];
                            Vector4 color2 = TailColor.Value;
                            return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                        }
                        return new Color(SegmentsGetColorColors[s]);
                    }
                    {
                        Vector4 color1 = SegmentsGetColorColors[s];
                        Vector4 color2 = SegmentsGetColorColors[s + 1];
                        return new Color(color1 * (1 - trailLengthInterpolant) + color2 * trailLengthInterpolant);
                    }
                }
                settings = new TrailSettings(WidthFunction, ColorFunction, Shader: shader);
                RenderTrail(SmoothedPoints, settings, SegmentsCount, dirIn, dirOut);
            }
        }
    }
    public record TrailSettings(VertexWidthFunction WidthFunction, VertexColorFunction ColorFunction, bool Smoothen = true, bool Pixelate = false,
    ManagedShader Shader = null, int? ProjectionAreaWidth = null, int? ProjectionAreaHeight = null, bool UseUnscaledMatrix = false, (Vector2 Left, Vector2 Right)? InitialVertexPositionsOverride = null)
    {
        /// <summary>
        /// A delegate to dynamically determine the width of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interpolant value.</param>
        /// <returns>The width for the current point.</returns>
        public delegate float VertexWidthFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position);

        /// <summary>
        /// A delegate to dynamically determine the color of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interpolant value.</param>
        /// <returns>The color for the current point.</returns>
        public delegate Color VertexColorFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position);
    }
}
