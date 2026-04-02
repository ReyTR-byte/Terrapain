using Terraria;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public class FireShooterServantofCthulhu : Group
    {
        public FireShooterServantofCthulhu()
        {
            NPCType = [ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>()];
        }
        public override void UpdateGroup()
        {
            List<NPC> healers = AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>());
            foreach (var npc in healers)
            {
                if (!members.Contains(npc.whoAmI))
                {
                    AddMember(npc.whoAmI);
                }
            }
            CheckMembers();
            AfterCheckMembers();
            for (int i = 0; i < members.Count; i++)
            {
                NPC npc = Main.npc[members[i]];
                npc.ai[1] = 1 * (i % 2 == 0? 1 : -1);
            }
        }
        void AfterCheckMembers()
        {
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (((NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu)member.ModNPC).AIStyle != 0)
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
