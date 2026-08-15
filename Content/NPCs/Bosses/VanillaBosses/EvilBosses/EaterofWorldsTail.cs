using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Content.Groups;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    public class EaterofWorldsTail : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsTail;
        public override void ModSetDefaults(NPC entity)
        {
            t.useVanillaDrawing = false;
            entity.knockBackResist = 0;
            entity.alpha = 0;
            entity.lifeMax = (int)(entity.lifeMax * 2.5f);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            npc.position.Y += npc.height / 2;
            if (BrainOfCthulhu.segmentsLifes.Count > 0)
            {
                npc.life = BrainOfCthulhu.segmentsLifes[0];
                BrainOfCthulhu.segmentsLifes.RemoveAt(0);
            }
        }
        public override bool ModPreAI(NPC npc)
        {
            NPC head = Main.npc[(int)npc.ai[0]];
            if (head.active == false || (head.type != NPCID.EaterofWorldsBody))
            {
                npc.life = 0;
                npc.checkDead();
                for (int i = 0; i < t.MyGroups.Count; i++)
                {
                    int g = t.MyGroups[i];
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                    }
                }
                return false;
            }
            return false;
        }
        public void NextAttack(NPC npc, int oldAttack, int newAttack)
        {
            if (newAttack == 4)
            {
                npc.ai[2] = 0;
            }
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0;
        }
        public override bool CheckActive(NPC npc)
        {
            return false;
        }
    }
}