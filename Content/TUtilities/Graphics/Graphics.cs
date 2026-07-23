using Iced.Intel;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.TUtilities.Graphics
{
    public class Graphics : ILoadable
    {
        #region Fields/Properties
        private static DynamicVertexBuffer VertexBuffer;

        private static DynamicIndexBuffer IndexBuffer;

        //private static IPrimitiveSettings MainSettings;

        private static Vector2[] MainPositions;

        private static VertexPosition2DColorTexture[] MainVertices;

        private static short[] MainIndices;

        private const short MaxTrailPositions = 2000;

        /// <summary>
        /// Must be lower than <see cref="MaxTrailPositions"/>, less than 1/4 of <see cref="MaxVertices"/> and less than 1/6 of <see cref="MaxIndices"/>.
        /// </summary>
        private const short MaxCirclePositions = 1500;

        private const short MaxVertices = 6144;

        private const short MaxIndices = 16384;

        private static short PositionsIndex;

        private static short VerticesIndex;

        private static short IndicesIndex;

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
                    TopLeft = TopRight?? l.start - (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                    BottomLeft = BottomRight?? l.start + (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
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
                        Main.spriteBatch.Draw(texture, l.end - Main.screenPosition, null, lightning.color, rotation +MathF.PI, new Vector2(0.5f, 0.5f), l.endWidth, SpriteEffects.None, 0);
                        Main.spriteBatch.End();
                    }
                }
                Color color = lightning.color;
                short num1 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(TopLeft - Main.screenPosition, color, new Vector2(0, 1), 1f);
                short num2 = VerticesIndex;
                MainVertices[VerticesIndex++] = new((TopRight?? TopRightCandidate) - Main.screenPosition, color, new Vector2(1, 1), 1f);
                short num3 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(l.start - Main.screenPosition, color, new Vector2(0, 0), 1f);
                short num4 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(l.end - Main.screenPosition, color, new Vector2(1, 0), 1f);
                short num5 = VerticesIndex;
                MainVertices[VerticesIndex++] = new(BottomLeft - Main.screenPosition, color, Vector2.UnitY, 1f);
                short num6 = VerticesIndex;
                MainVertices[VerticesIndex++] = new((BottomRight?? BottomRightCandidate) - Main.screenPosition, color, Vector2.One, 1f);

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
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            Vector2 quadArea = texture.Size();
            float maxDimension = MathF.Max(quadArea.X, quadArea.Y);

            var viewMatrix = Main.GameViewMatrix.TransformationMatrix
                * Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -maxDimension - 1f, maxDimension + 1f);
            //shader ??= ShaderManager.GetShader("Luminance.QuadRenderer");
            shader.TrySetParameter("uWorldViewProjection", viewMatrix);
            //shader.SetTexture(texture, 1, SamplerState.PointClamp);
            shader.Apply();

            VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Main.instance.GraphicsDevice.Indices = IndexBuffer;
            Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, IndicesIndex / 3);

            //VerticesIndex = 0;
            //IndicesIndex = 0;
            //foreach (var t in triangles)
            //{
            //    MainIndices[IndicesIndex++] = VerticesIndex;
            //    MainVertices[VerticesIndex++] = new(t.pos1 - Main.screenPosition, lightning.color, new Vector2(0, 0), 1);
            //    MainIndices[IndicesIndex++] = VerticesIndex;
            //    MainVertices[VerticesIndex++] = new(t.pos2 - Main.screenPosition, lightning.color, new Vector2(0, 1), 1);
            //    MainIndices[IndicesIndex++] = VerticesIndex;
            //    MainVertices[VerticesIndex++] = new(t.pos3 - Main.screenPosition, lightning.color, new Vector2(1, 1), 1);
            //}

            //VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            //IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            //Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            //Main.instance.GraphicsDevice.Indices = IndexBuffer;
            //Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, VerticesIndex / 3);
        }
    }
}
