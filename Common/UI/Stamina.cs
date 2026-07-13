using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.ChargeStrips;
using Terrapain.Common.UI.Assets.EmptyStrips;
using Terrapain.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Terrapain.Common.UI
{
    public class Stamina : TerrapainUI
    {
        public float Visibility = 0.05f;
        float progress;
        public override void UpdateUI()
        {
            if (Main.gameMenu || Visibility == 0)
            {
                TerrapainUIManager.Close<Stamina>();
            }
            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            progress = pl.Stamina / pl.MaxStamina;
            if (progress == 1)
            {
                Visibility = MathF.Max(0, Visibility - 0.002f);
            }
            else
            {
                Visibility = MathF.Min(1, Visibility + 0.05f);
            }
            Visibility = 0.5f;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            BlendState blendState = new()
            {
                ColorSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
            };

            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null, DepthStencilState.Default, null, null, Main.UIScaleMatrix);
            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            Vector2 pos = pl.Player.Bottom + Vector2.UnitY * 15 - Main.screenPosition;
            StaminaEmptyStrip s = new();
            StaminaChargeStrip c = new();
            s.Draw(spriteBatch, pos, Vector2.One, Visibility);
            c.Draw(spriteBatch, pos - Vector2.UnitX * 28, Vector2.One, progress, Visibility);
            s.DrawOver(spriteBatch, pos, Vector2.One, Visibility);
        }
    }
}
