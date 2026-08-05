using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    internal class EaterofWorldsBody : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsBody;
        public override void ModSetDefaults(NPC entity)
        {
            t.useModDrawingInPreDraw = true;
            entity.knockBackResist = 0;
            entity.alpha = 0;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            npc.ai[1] *= -1;
            npc.ai[1] += -1;
            npc.ai[2] = npc.Center.X;
            npc.ai[3] = npc.Center.Y;
            base.OnSpawn(npc, source);
        }
        public override bool ModPreAI(NPC npc)
        {
            if (npc.ai[1] <= -1 && npc.Distance(new Vector2(npc.ai[2], npc.ai[3])) > npc.width)
            {
                npc.ai[1]++;
                npc.ai[1] *= -1;
                if (npc.ai[1] < 48)
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), new Vector2(npc.ai[2], npc.ai[3]), NPCID.EaterofWorldsBody, npc.whoAmI, npc.whoAmI, npc.ai[1] + 1);
                }
                else
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), new Vector2(npc.ai[2], npc.ai[3]), NPCID.EaterofWorldsTail, npc.whoAmI, npc.whoAmI);
                }
            }
            NPC head = Main.npc[(int)npc.ai[0]];
            Vector2 targetPosition = head.Center + Vector2.UnitY.RotatedBy(head.rotation) * (head.width / 2 - 2);
            npc.rotation = npc.DirectionTo(targetPosition).ToRotation();
            npc.velocity = Vector2.Zero;
            npc.Center = targetPosition - Vector2.UnitX.RotatedBy(npc.rotation) * npc.width;
            npc.rotation += MathF.PI / 2;
            return false;
        }
        public override bool CheckActive(NPC npc)
        {
            return false;
        }
    }
}
