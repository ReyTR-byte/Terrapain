using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    internal class EaterofWorldsBody : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsBody;
        public override void ModSetDefaults(NPC entity)
        {
            t.useVanillaDrawing = false;
            entity.alpha = 0;
            entity.knockBackResist = 0;
            entity.lifeMax = (int)(entity.lifeMax * 2.5f);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.ai[1] < 0)
            {
                npc.alpha = 255;
            }
            npc.position.Y += npc.height / 2;
            if (BrainOfCthulhu.segmentsLifes.Count > 0)
            {
                npc.life = BrainOfCthulhu.segmentsLifes[0];
                BrainOfCthulhu.segmentsLifes.RemoveAt(0);
            }
            base.OnSpawn(npc, source);
        }
        public override bool ModPreAI(NPC npc)
        {
            NPC brain = Main.npc[EaterofWorldsHead.BrainofCthulhu];
            if (npc.ai[1] < 0 && npc.Distance(brain.Center) > npc.width)
            {
                npc.alpha = 0;
                if (npc.ai[1] < -1)
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), brain.Center, NPCID.EaterofWorldsBody, npc.whoAmI, npc.whoAmI, npc.ai[1] + 1);
                }
                else
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), brain.Center, NPCID.EaterofWorldsTail, npc.whoAmI, npc.whoAmI);
                }
                for (int i = 0; i < t.MyGroups.Count; i++)
                {
                    int g = t.MyGroups[i];
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                    }
                }
            }
            NPC head = Main.npc[(int)npc.ai[0]];
            if (head.active == false || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody))
            {
                if (npc.ai[1] >= 0)
                {
                    NPC tail = Main.npc[(int)npc.ai[1]];
                    if (tail.active && (tail.type == NPCID.EaterofWorldsBody))
                    {
                        int newHead = NewNPC(npc.GetSource_FromThis(), npc.Center, NPCID.EaterofWorldsHead, 0, 0, npc.ai[1]);
                        tail.ai[0] = newHead;
                        Main.npc[newHead].rotation = npc.rotation;
                        Main.npc[newHead].velocity = npc.position - npc.oldPosition;
                        npc.active = false;
                        for (int i = 0; i < t.MyGroups.Count; i++)
                        {
                            int g = t.MyGroups[i];
                            if (Terrapain.group[g] is EaterofWorlds)
                            {
                                (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                            }
                        }
                    }
                    else
                    {
                        npc.life = 0;
                        npc.checkDead();
                    }
                }
                else
                {
                    npc.life = 0;
                    npc.checkDead();
                }
                return false;
            }
            else if (npc.ai[1] >= 0)
            {
                NPC tail = Main.npc[(int)npc.ai[1]];
                if (tail.active == false || (tail.type != NPCID.EaterofWorldsTail && tail.type != NPCID.EaterofWorldsBody))
                {
                    int newTail = NewNPC(npc.GetSource_FromThis(), npc.Center, NPCID.EaterofWorldsTail, 0, npc.ai[0]);
                    head.ai[1] = newTail;
                    Main.npc[newTail].rotation = npc.rotation;
                    npc.active = false;
                }
                for (int i = 0; i < t.MyGroups.Count; i++)
                {
                    int g = t.MyGroups[i];
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                    }
                }
            }
            DoFirstPhase(npc);
            
            return false;
        }
        public void DoFirstPhase(NPC npc)
        {
        }
        public void NextAttack(NPC npc, int oldAttack, int newAttack)
        {
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
