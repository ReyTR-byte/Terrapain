using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.TerrapainBossBars.KingSlimeBossBar
{
    public class KingSlimeBossBar : TerrapainBossBar
    {
        public KingSlimeBossBar() 
        {
            BossBar = new KingSlimeBossBarBar();
            BossBarFill = new KingSlimeBossBarBarFill();
            texture = ModContent.Request<Texture2D>(textureAddress).Value;
            Size = texture.Size();
            PhaseBar = ModContent.Request<Texture2D>(PhaseBarAddress).Value;
            PhaseBarFill = ModContent.Request<Texture2D>(PhaseBarFillAddress).Value;
            BarPosition = new Vector2(-8, -8) - ModContent.Request<Texture2D>(textureAddress + "Bar").Value.Size() / 2 + Size;
            PhaseBarPosition = new Vector2(-14, -28) - PhaseBar.Size() / 2 + Size;
            HeadPosition = new Vector2(24, 16);
            PhaseBarWidth = PhaseBar.Width - 2;
        }
        public bool kingSlimeCrownActive;
        public BossBarInfo kingSlimeCrownInfo;
        public bool ninjaKingSlimeActive;
        public BossBarInfo ninjaKingSlimeInfo;
        public bool crownedKingSlimeActive;
        public bool crownAtCrownedKingSlime;
        public BossBarInfo crownedKingSlimeInfo;
        public bool kingSlimeActive;
        public override void DrawSelf(SpriteBatch spriteBatch, Vector2 offset, BossBarInfo info)
        {
            if (kingSlimeActive)
            {
                base.DrawSelf(spriteBatch, offset, info);
            }
            else
            {
                Phases = 1;
                if (ninjaKingSlimeActive)
                {
                    base.DrawSelf(spriteBatch, offset, ninjaKingSlimeInfo);
                }
                if (crownedKingSlimeActive)
                {
                    offset.Y -= Size.Y + 32;
                    if (crownAtCrownedKingSlime)
                    {
                        Phases = 2;
                    }
                    base.DrawSelf(spriteBatch, offset, crownedKingSlimeInfo);
                }
                if (kingSlimeCrownActive)
                {
                    offset.Y -= Size.Y + 32;
                    base.DrawSelf(spriteBatch, offset, kingSlimeCrownInfo);
                }
            }
        }
    }
}
