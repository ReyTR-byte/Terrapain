using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    public class EaterofWorldsTail : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsTail;
        public override void ModSetDefaults(NPC entity)
        {
            t.useModDrawingInPreDraw = true;
            entity.knockBackResist = 0;
            entity.alpha = 0;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
        }
        public override bool ModPreAI(NPC npc)
        {
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
