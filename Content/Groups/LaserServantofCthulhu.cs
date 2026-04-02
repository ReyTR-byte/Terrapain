using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public class LaserServantofCthulhu : Group
    {
        float startAngle = -1;
        public LaserServantofCthulhu()
        {
            NPCType = [ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>()];
        }
        public override void UpdateGroup()
        {
            List<NPC> healers = AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>());
            foreach (var npc in healers)
            {
                if (!members.Contains(npc.whoAmI))
                {
                    AddMember(npc.whoAmI);
                }
            }
            CheckMembers();
            AfterCheckMembers();
            if (members.Count > 0)
            {
                float angle = MathF.PI * 2 / members.Count;
                for (int i = 0; i < members.Count; i++)
                {
                    NPC npc = Main.npc[members[i]];
                    if (startAngle < 0)
                    {
                        npc.TargetClosest(false);
                        startAngle = npc.AngleFrom(npc.GetT().Target.Center);
                    }
                    npc.ai[1] = startAngle + angle * i;
                }
            }
        }
        void AfterCheckMembers()
        {
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (((NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu)member.ModNPC).AIStyle != 0)
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
