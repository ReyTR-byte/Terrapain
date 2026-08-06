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
            if (head.active == false || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody))
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
            Vector2 targetPosition = head.Center + Vector2.UnitY.RotatedBy(head.rotation) * (head.width / 2 - 2);
            npc.rotation = npc.DirectionTo(targetPosition).ToRotation();
            npc.rotation -= head.rotation - MathF.PI / 2;
            npc.rotation = NormalizeRotation(npc.rotation, false);
            if (npc.rotation > MathF.PI * 0.7f)
            {
                npc.rotation = MathF.PI * 0.7f;
            }
            else if (npc.rotation < -MathF.PI * 0.7f)
            {
                npc.rotation = -MathF.PI * 0.7f;
            }
            npc.rotation += head.rotation - MathF.PI / 2;
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