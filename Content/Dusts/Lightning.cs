using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Dusts
{
    public class Lightning : ModDust
    {
        static UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, 13 * random.Next(3), 9, 13);
        }
        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, 0.5f, 0.5f, 0.5f);
            return base.Update(dust);
        }
        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, dust.position - Main.screenPosition, dust.frame, Color.White, 0, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
