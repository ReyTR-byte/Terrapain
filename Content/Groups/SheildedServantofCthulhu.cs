using Terrapain.Content.NPCs.Servants.EyeofCthulhu;
using Terraria;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public class SheildedServantofCthulhu : Group
    {
        public SheildedServantofCthulhu()
        {
            NPCType = [ModContent.NPCType<NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu>()];
        }
        public override void UpdateGroup()
        {
            List<NPC> defenders = AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu>());
            foreach (var npc in defenders)
            {
                if (!members.Contains(npc.whoAmI))
                {
                    AddMember(npc.whoAmI);
                }
            }
            CheckMembers();
            AfterCheckMembers();
            List<NPC> patients = AllNPCByType(5);
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>()));
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>()));
            bool flag = patients.Count == 0;
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>()));
            //List<int> criticals = new List<int>();
            //foreach (var patient in patients)
            //{
            //    if (patient.type == 5 && (float)patient.life / patient.lifeMax <= 0.3f)
            //    {
            //        criticals.Add(patient.whoAmI);
            //    }
            //    if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.5f)
            //    {
            //        criticals.Add(patient.whoAmI);
            //    }
            //    if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.35f)
            //    {
            //        criticals.Add(patient.whoAmI);
            //    }
            //    if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.35f)
            //    {
            //        criticals.Add(patient.whoAmI);
            //    }
            //}
            //List<NPC> free = new List<NPC>();
            //foreach (int i in members)
            //{
            //    free.Add(Main.npc[i]);
            //}
            //for (int i = 0; i < criticals.Count; i++)
            //{
            //    NPC npc = Main.npc[criticals[i]];
            //    bool findDoctor = false;
            //    while (!findDoctor && free.Count > 0)
            //    {
            //        NPC doctor = FindClosestNPCinCollection(npc.Center, free);
            //        if (criticals.Contains((int)doctor.ai[1]))
            //        {
            //            free.Remove(doctor);
            //            npc.ai[3] = -1;
            //        }
            //        else
            //        {
            //            doctor.ai[1] = criticals[i];
            //            npc.ai[3] = doctor.whoAmI;
            //            free.Remove(doctor);
            //            findDoctor = true;
            //        }
            //    }
            //}
            if (!flag)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    NPC npc = Main.npc[members[i]];
                    float maxValue = 0;
                    int maxValueOwner = -1;
                    foreach (var patient in patients)
                    {
                        float value = (patient.lifeMax - patient.life + 1) / patient.Distance(npc.Center);
                        if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>())
                        {
                            if (patient.life > patient.lifeMax * 0.9)
                            {
                                value = 0;
                            }
                            value *= 1.5f;
                        }
                        if (patient.whoAmI == npc.ai[1])
                        {
                            value *= 1.2f;
                        }
                        if (value > maxValue)
                        {
                            maxValue = value;
                            maxValueOwner = patient.whoAmI;
                        }
                    }
                    npc.ai[1] = maxValueOwner;
                }
            }
            else
            {
                foreach(int i in members)
                {
                    Main.npc[i].ai[1] = -1;
                }
            }
        }
        void AfterCheckMembers()
        {
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (((NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu)member.ModNPC).AIStyle != 0)
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
