using System.Data;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
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
        static uint oldUpdate;
        void updateTarget()
        {
            if (map == null || map.IsDisposed || map.Width != Main.screenWidth || map.Height != Main.screenHeight)
            {
                map?.Dispose();

                map = new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    SurfaceFormat.Vector2,                    
                    //Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
        }
        public void Apply()
        {
            if (oldUpdate != Main.GameUpdateCount)
            {
                updateTarget();
                Main.graphics.GraphicsDevice.SetRenderTarget(map);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                oldUpdate = Main.GameUpdateCount;
                ManagedScreenFilter filter = ShaderManager.GetFilter("Terrapain.Wave");
                filter.SetTexture(map, 1);

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
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
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

        public bool Update()
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