using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace Terrapain.Common.System
{
    internal class PlayerPostDraw : ModSystem
    {
        //public override void UpdateUI(GameTime gameTime)
        //{
        //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        //    Main.spriteBatch.End();
        //}
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            foreach (var p in Main.player)
            {
                if (p.active)
                    p.Custom().PostDrawPlayer(Main.spriteBatch);
            }
            base.PostDrawInterface(spriteBatch);
        }
        //Terrapain.LightningDrawInfo lightning;
        //public override void PostUpdateNPCs()
        //{
        //    lightning = Functions.NewLightning(Main.LocalPlayer.Center - Vector2.UnitY * 450, Main.LocalPlayer.Center + Vector2.UnitY * 450, 100);
        //}
        //public override void PostDrawTiles()
        //{
        //    if (lightning.lightningPartInfos != null)
        //    {
        //        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        //        Main.spriteBatch.DrawLightning(lightning);
        //        Main.spriteBatch.End();
        //    }
        //}
    }
}
