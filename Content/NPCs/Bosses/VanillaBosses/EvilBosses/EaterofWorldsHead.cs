using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terraria;
using Terraria.ID;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    public class EaterofWorldsHead : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsHead;
        public override bool ModPreAI(NPC npc)
        {
            
            return false;
        }
    }
}
