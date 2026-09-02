using System.Data;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terraria;

namespace Terrapain.Common.System.Filters
{
    public class StrikeWaveFilter : ITerrapainFilter
    {
        public bool necessary;
        public float WaveRadius1;
        public float WaveRadius2;
        public float WavePower;
        public float disposingSpeed;
        public float speed1;
        public float speed2;
        public bool multiplySpeedByWavePower;
        public Vector2 WaveCenter;
        public static RenderTarget2D map;
        static GraphicsDevice mapDevice;
        static uint oldUpdate;
        public void updateTarget()
        {
            Point resolution = Point.Zero;
            switch (GraphicsConfig.Instance.filters)
            {
                case GraphicsConfig.GraphicsLevel.Potato:
                case GraphicsConfig.GraphicsLevel.Low:
                    resolution.X = Main.screenWidth / 2;
                    resolution.Y = Main.screenHeight / 2;
                    break;
                case GraphicsConfig.GraphicsLevel.Medium:
                    resolution.X = (int)(Main.screenWidth / 1.5f);
                    resolution.Y = (int)(Main.screenHeight / 1.5f);
                    break;
                case GraphicsConfig.GraphicsLevel.High:
                case GraphicsConfig.GraphicsLevel.Ultra:
                    resolution.X = Main.screenWidth;
                    resolution.Y = Main.screenHeight;
                    break;
            }

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            if (map == null || map.IsDisposed || mapDevice != graphicsDevice || map.Width != resolution.X || map.Height != resolution.Y)
            {
                map?.Dispose();

                map = new RenderTarget2D(
                    graphicsDevice,
                    resolution.X,
                    resolution.Y,
                    false,
                    SurfaceFormat.Vector2,                    
                    //Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
                mapDevice = graphicsDevice;
            }
        }
        public void Apply(int i)
        {
            bool isFirst = true;
            for (int j = 0; j < i; j++)
            {
                if (EffectsSystem.filters[i] is StrikeWaveFilter)
                {
                    isFirst = false;
                    break;
                }
            }
            if (isFirst)//oldUpdate != Main.GameUpdateCount)
            {
                updateTarget();
                Main.graphics.GraphicsDevice.SetRenderTarget(map);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                oldUpdate = Main.GameUpdateCount;
                ManagedScreenFilter filter = ShaderManager.GetFilter("Terrapain.Wave");
                filter.SetTexture(map, 1, SamplerState.LinearClamp);

                filter.Activate();
            }
            else
            {
                Main.graphics.GraphicsDevice.SetRenderTarget(map);
            }
            ManagedShader shader = ShaderManager.GetShader("Terrapain.Wave");

            shader.TrySetParameter("w", Main.screenWidth);
            shader.TrySetParameter("h", Main.screenHeight);
            shader.TrySetParameter("center", WaveCenter - Main.screenPosition);
            shader.TrySetParameter("radius1", WaveRadius1);
            shader.TrySetParameter("radius2", WaveRadius2);
            shader.TrySetParameter("power", WavePower);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(map.Width / 2, map.Height / 2, map.Width, map.Height);
            Main.spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rekt, null, Color.White, 0f, ExtraTextureRegistry.WhitePixel.Value.Size() * 0.5f, 0, 1f);
            Main.spriteBatch.End();


            Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            Main.graphics.GraphicsDevice.SetRenderTarget(null);
        }

        public bool Necessary()
        {
            return necessary;
        }

        public void OnDispose()
        {
            foreach (var filter in EffectsSystem.filters)
            {
                if (filter is StrikeWaveFilter)
                {
                    return;
                }
            }
            map.Dispose();
        }

        public bool Update(int i)
        {
            if (multiplySpeedByWavePower)
            {
                WaveRadius1 += speed1 * WavePower;
                WaveRadius2 += speed2 * WavePower;
            }
            else
            {
                WaveRadius1 += speed1;
                WaveRadius2 += speed2;
            }

            WavePower -= disposingSpeed;
            return WavePower <= 0;
        }

        public float Weight()
        {
            return 2.5f;
        }
    }
}