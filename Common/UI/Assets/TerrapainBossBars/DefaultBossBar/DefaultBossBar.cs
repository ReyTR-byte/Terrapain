using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.TerrapainBossBars.DefaultBossBar
{
    public class DefaultBossBar : TerrapainBossBar
    {
        public DefaultBossBar() 
        {
            BossBar = new DefaultBossBarBar();
            BossBarFill = new DefaultBossBarBarFill();
            texture = ModContent.Request<Texture2D>(textureAddress).Value;
            Size = texture.Size();
            PhaseBar = ModContent.Request<Texture2D>(PhaseBarAddress).Value;
            PhaseBarFill = ModContent.Request<Texture2D>(PhaseBarFillAddress).Value;
            BarPosition = new Vector2(-8, -8) - BossBar.size / 2 + Size;
            PhaseBarPosition = new Vector2(-14, -28) - PhaseBar.Size() / 2 + Size;
            HeadPosition = new Vector2(24, 16);
            PhaseBarWidth = PhaseBar.Width - 2;
        }
    }
}
