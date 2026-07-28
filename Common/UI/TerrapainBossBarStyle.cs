using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Terrapain.Common.UI
{
    public class TerrapainBossBarStyle : ModBossBarStyle
    {
        public static TerrapainBossBar BossBar;
        public override bool PreventDraw => BossBar != null;
        public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info)
        {
            if (BossBar != null)
            {
                BossBar.Draw(spriteBatch);
                if (!Main.npc[BossBar.boss].active || Main.npc[BossBar.boss].type != BossBar.bossType)
                {
                    BossBar = null;
                }
            }
        }
    }
}
