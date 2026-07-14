using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Config;
using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
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
        }
        public override void Update(GameTime gameTime)
        {
            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            float targetProgress = pl.Stamina / pl.MaxStamina;
            if (progress > targetProgress)
            {
                progress = MathF.Max(targetProgress, progress - 0.008f);
            }
            else if (progress < targetProgress)
            {
                progress = MathF.Min(targetProgress, progress + 0.008f);
            }
            if (progress == 1)
            {
                Visibility = MathF.Max(0, Visibility - 0.02f);
            }
            else
            {
                Visibility = MathF.Min(1, Visibility + 0.02f);
            }
            
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);

            if (UIConfig.Instance.FullColor == Color.Transparent)
            {
                UIConfig.Instance.FullColor = Color.LimeGreen;
            }
            if (UIConfig.Instance.EmptyColor == Color.Transparent)
            {
                UIConfig.Instance.EmptyColor = Color.Red;
            }
            Vector3 FullColor = UIConfig.Instance.FullColor.ToVector3();
            Vector3 EmptyColor = UIConfig.Instance.EmptyColor.ToVector3();
            Color DrawColor = new Color(FullColor * progress + EmptyColor * (1 - progress));

            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            Vector2 pos = pl.Player.Bottom + Vector2.UnitY * 15 - Main.screenPosition + UIConfig.Instance.Offset;
            Bar s = null;
            BarFill c = null;
            switch (UIConfig.Instance.staminaBarType)
            {
                case UIConfig.StaminaBarType.Thin:
                    s = new ThinStaminaBar();
                    c = new ThinStaminaBarFill();
                    break;
                case UIConfig.StaminaBarType.Width:
                    s = new WidthStaminaBar();
                    c = new WidthStaminaBarFill();
                    break;
            }
            if (s != null && c != null)
            {
                s.Draw(spriteBatch, pos, Vector2.One, Visibility);
                c.Draw(spriteBatch, pos - Vector2.UnitX * 28, Vector2.One, progress, Visibility, DrawColor);
                s.DrawOver(spriteBatch, pos, Vector2.One, Visibility);
            }
        }
        public override void OnOpen()
        {
            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            progress = pl.Stamina / pl.MaxStamina;
            Visibility = 0.05f;
        }
    }
}
