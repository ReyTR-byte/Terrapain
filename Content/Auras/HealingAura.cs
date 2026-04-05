using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Utilities.Terraria.Utilities;

namespace Terrapain.Content.Auras
{
    public class HealingAura : Aura
    {
        public bool HealPlayer;
        public List<int> WhiteList;
        public int RegenPower;
        public HealingAura(bool healPlayer, List<int> whiteList, int regenPower, float radius)
        {
            HealPlayer = healPlayer;
            WhiteList = whiteList;
            RegenPower = regenPower;
            AuraColor = Color.Green * 0.5f;
            internalColor = Color.Green * 0.1f;
            this.Radius = radius;
            InternalRadius = radius * 0.8f;
            dust = DustID.HealingPlus;
            dustCountMin = (int)(radius / 200);
            dustCountMax = (int)(radius / 90);
            if (whiteList.Count != 0)
            {
                checkNPC = true;
            }
            checkPlayer = healPlayer;
        }
        public override void PostUpdate()
        {
            Radius = 300;
        }
        public override void OnNPCInAura(NPC npc)
        {
            if(WhiteList.Contains(npc.type))
            {
                npc.lifeRegenCount += RegenPower;
                if (random.NextBool(10))
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.HealingPlus);
            }
        }
        public override void OnPlayerInAura(Player player)
        {
            player.lifeRegen += RegenPower;
            if (random.NextBool(10))
                Dust.NewDust(player.position, player.width, player.height, DustID.HealingPlus);
        }
        public override void PostDraw(SpriteBatch sprite)
        {
        }
    }
}
