using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
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

        // Временный RenderTarget для рендеринга бара
        private RenderTarget2D _renderTarget;
        private bool _needsRebuild = true;

        public override void UpdateUI()
        {
            if (Main.gameMenu || Visibility == 0)
            {
                TerrapainUIManager.Close<Stamina>();
                // Очищаем RenderTarget при закрытии
                _renderTarget?.Dispose();
                _renderTarget = null;
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

            // Помечаем, что нужно перестроить RenderTarget при изменении размера
            if (_renderTarget != null &&
                (_renderTarget.Width != Main.screenWidth ||
                 _renderTarget.Height != Main.screenHeight))
            {
                _needsRebuild = true;
            }
        }

        private void RebuildRenderTarget()
        {
            _renderTarget?.Dispose();
            _renderTarget = new RenderTarget2D(
                Main.graphics.GraphicsDevice,
                Main.screenWidth,
                Main.screenHeight,
                false,
                Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents
            );
            _needsRebuild = false;
        }

        private void DrawBarToRenderTarget(SpriteBatch spriteBatch)
        {
            if (_renderTarget == null || _needsRebuild)
            {
                RebuildRenderTarget();
            }

            // Устанавливаем RenderTarget
            Main.graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            // Рисуем бар с прозрачностью 1 (полностью непрозрачный)
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
            Vector2 pos = pl.Player.Bottom + Vector2.UnitY * 15 + UIConfig.Instance.Offset - Main.screenPosition;
            pos /= Main.UIScale;
            pos.ToInt();

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
                // Рисуем все слои с alpha = 1
                s.Draw(spriteBatch, pos, Vector2.One, 1f);
                c.Draw(spriteBatch, pos - Vector2.UnitX * 28, Vector2.One, progress, 1f, DrawColor);
                s.DrawOver(spriteBatch, pos, Vector2.One, 1f);
            }

            spriteBatch.End();

            // Возвращаем render target обратно
            Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            Main.graphics.GraphicsDevice.SetRenderTarget(null);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Завершаем текущий SpriteBatch
            spriteBatch.End();

            // Если RenderTarget не создан или требует обновления - создаём/обновляем
            if (_renderTarget == null || _needsRebuild || progress != _lastProgress)
            {
                DrawBarToRenderTarget(spriteBatch);
                _lastProgress = progress;
            }

            // Рисуем готовый RenderTarget с прозрачностью Visibility
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));

            if (_renderTarget != null)
            {
                spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White * Visibility);
            }
        }

        private float _lastProgress = -1f;

        public override void OnOpen()
        {
            TerrapainPlayer pl = Main.LocalPlayer.Custom();
            progress = pl.Stamina / pl.MaxStamina;
            _lastProgress = -1f;
            Visibility = 0.05f;
            _needsRebuild = true;
        }
    }
}