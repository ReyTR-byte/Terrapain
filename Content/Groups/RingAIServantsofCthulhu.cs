using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terraria;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public class RingAIServantsofCthulhu : Group
    {
        public int timer;
        public int target = -1;
        public float rotation;
        public RingAIServantsofCthulhu()
        {
            NPCType = [5];
        }

        public override void UpdateGroup()
        {
            CheckMembers();
            AfterCheckMembers();
            for (int i = 0; i < members.Count; i++)
            {
                Main.npc[members[i]].target = target;
            }
            if (timer == 100)
            {
                Vector2 CommonCenter = Vector2.Zero;
                foreach (var mem in members)
                {
                    NPC Mem = Main.npc[mem];
                    CommonCenter += Mem.Center;
                }
                CommonCenter /= members.Count;
                float CommonRotation = 0;
                for (int i = 0; i < members.Count; i++)
                {
                    NPC Mem = Main.npc[members[i]];
                    CommonRotation = (Mem.Center - CommonCenter).ToRotation();
                    sort[i] = (Mem.Center - CommonCenter).ToRotation();
                }
                CommonRotation /= members.Count;
                for (int i = 0; i < members.Count; i++)
                {
                    sort[i] -= CommonRotation;
                    sort[i] = NormalizeRotation(sort[i], false);
                }
                Sort();
            }
            if (timer > 35)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    NPC npc = Main.npc[members[i]];
                    npc.ai[0] = (MathF.PI * 2 / members.Count) * i + EasingInOut(80, 100 - timer) + rotation;
                    npc.ai[1] = 200 + (timer < 50? EasingIn(15, 50 - timer) * 80 : 0);
                    Main.npc[members[i]].ai[2] = 0;
                }
            }
            else if (timer > 20)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Main.npc[members[i]].ai[2] = 0.5f;
                }
            }
            else if (timer == 20)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Main.npc[members[i]].ai[2] = 1;
                }
            }
            else if (timer > 0)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Main.npc[members[i]].ai[2] = 0.5f;
                }
            }
            else
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Main.npc[members[i]].ai[2] = 0;
                }
            }
            timer--;
        }
        void AfterCheckMembers()
        {
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (member.GetGlobalNPC<ServantofCthulhu>().AIStyle != 1)
                {
                    DelMember(i);
                    i--;
                }
            }
            if (members.Count == 0)
            {
                Terrapain.group[whoAmI] = null;
            }
        }
    }
}
