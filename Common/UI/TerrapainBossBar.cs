using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
using Terraria;

namespace Terrapain.Common.UI
{
    public abstract class TerrapainBossBar
    {
        public string textureAddress => this.GetPath();
        public string PhaseBarAddress => this.GetPath() + "_PhaseBar";
        public string PhaseBarFillAddress => this.GetPath() + "_PhaseBarFill";

        public Texture2D texture;
        public Texture2D PhaseBar;
        public Texture2D PhaseBarFill;
        public Bar BossBar;
        public BarFill BossBarFill;

        public Vector2 Size;

        public Vector2 BarPosition;
        public Vector2 HeadPosition;
        public Vector2 PhaseBarPosition;
        public float PhaseBarWidth;
        public int Phases;
        public int boss;
        public int bossType;
        public struct BossBarInfo
        {
            public int CurentPhase;
            public int Health;
            public int MaxPhaseHealth;
            public int MinPhaseHealth;
            public Texture2D head;
        }

        public BossBarInfo info;

        public virtual void PreDraw(SpriteBatch spriteBatch)
        {

        }
        public virtual void PostDraw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2? offset = null)
        {
            PreDraw(spriteBatch);
            var _offset = offset?? Vector2.Zero;
            Vector2 drawPos = Main.ScreenSize.ToVector2() - Size - Vector2.One * 32 + _offset;
            spriteBatch.Draw(texture, drawPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            BossBar.Draw(spriteBatch, drawPos + BarPosition, Vector2.One);
            float progress = MathHelper.Clamp((info.Health - info.MinPhaseHealth) / (float)(info.MaxPhaseHealth - info.MinPhaseHealth), 0, 1);
            BossBarFill.Draw(spriteBatch, drawPos + BarPosition, Vector2.One, progress);
            BossBar.DrawOver(spriteBatch, drawPos + BarPosition, Vector2.One);
            Vector2 CurentPos = drawPos + PhaseBarPosition;
            Vector2 PhaseBarOrigin = PhaseBar.Size() / 2;
            for (int i = 1; i <= Phases; i++)
            {
                spriteBatch.Draw(PhaseBar, CurentPos, null, Color.White, 0, PhaseBarOrigin, 1, SpriteEffects.None, 0);
                if (i >= info.CurentPhase)
                {
                    spriteBatch.Draw(PhaseBarFill, CurentPos, null, Color.White, 0, PhaseBarOrigin, 1, SpriteEffects.None, 0);
                }
                CurentPos.X -= PhaseBarWidth;
            }
            if (info.head != null)
            {
                Vector2 headorig = info.head.Size() / 2;
                spriteBatch.Draw(info.head, drawPos + HeadPosition, null, Color.White, 0, headorig, 1, SpriteEffects.None, 0);
            }
            PostDraw(spriteBatch);
        }
    }
}
