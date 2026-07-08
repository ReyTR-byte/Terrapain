using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terraria;

namespace Terrapain.Common.DrawTasks
{
    public class ShaderFullscreenDrawTask : DrawTask
    {
        ManagedShader shader;
        SpriteSortMode sortMode;
        BlendState blendState;
        SamplerState samplerState;
        DepthStencilState depthStencilState;
        RasterizerState rasterizerState;
        Matrix transformMatrix;

        public ShaderFullscreenDrawTask(ManagedShader shader)
        {
            this.shader = shader;
            sortMode = SpriteSortMode.Immediate;
            blendState = BlendState.AlphaBlend;
            samplerState = SamplerState.LinearWrap;
            depthStencilState = DepthStencilState.None;
            rasterizerState = Main.Rasterizer;
            transformMatrix = Main.GameViewMatrix.TransformationMatrix;
        }
        public ShaderFullscreenDrawTask(ManagedShader shader, SpriteSortMode? sortMode = null, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Matrix? transformMatrix = null)
        {
            this.shader = shader;
            this.sortMode = sortMode?? SpriteSortMode.Immediate;
            this.blendState = blendState?? BlendState.AlphaBlend;
            this.samplerState = samplerState?? SamplerState.LinearWrap;
            this.depthStencilState = depthStencilState?? DepthStencilState.None;
            this.rasterizerState = rasterizerState?? Main.Rasterizer;
            this.transformMatrix = transformMatrix?? Main.GameViewMatrix.TransformationMatrix;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, shader.WrappedEffect, transformMatrix);
            var blackTile = ExtraTextureRegistry.BlackPixel;
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(blackTile.Value, rekt, null, Color.Black, 0f, blackTile.Value.Size() * 0.5f, 0, 1f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
