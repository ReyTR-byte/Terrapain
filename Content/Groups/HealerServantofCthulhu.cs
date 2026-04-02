using Terrapain.Content.NPCs.Servants.EyeofCthulhu;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public class HealerServantofCthulhu : Group
    {
        public HealerServantofCthulhu()
        {
            NPCType = [ ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>() ];
        }
        public override void UpdateGroup()
        {
            List<NPC> healers = AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>());
            foreach(var npc in healers)
            {
                if(!members.Contains(npc.whoAmI))
                {
                    AddMember(npc.whoAmI);
                }
            }
            CheckMembers();
            List<NPC> patients = AllNPCByType(5);
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu>()));
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>()));
            patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>()));
            bool flag = patients.Count == 0;
            List<int> criticals = new List<int>();
            foreach (var patient in patients)
            {
                if (patient.type == 5 && (float)patient.life / patient.lifeMax <= 0.3f)
                {
                    criticals.Add(patient.whoAmI);
                }
                if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.25f)
                {
                    criticals.Add(patient.whoAmI);
                }
                if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.35f)
                {
                    criticals.Add(patient.whoAmI);
                }
                if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.FireShooterServantofCthulhu>() && (float)patient.life / patient.lifeMax <= 0.35f)
                {
                    criticals.Add(patient.whoAmI);
                }
            }
            List<NPC> free = new List<NPC>();
            foreach (int i in members)
            {
                free.Add(Main.npc[i]);
            }
            for (int i = 0; i < criticals.Count; i++)
            {
                NPC npc = Main.npc[criticals[i]];
                bool findDoctor = false;
                while (!findDoctor && free.Count > 0)
                {
                     NPC doctor = FindClosestNPCinCollection(npc.Center, free);
                    if (criticals.Contains((int)doctor.ai[1]))
                    {
                        free.Remove(doctor);
                        npc.ai[3] = -1;
                    }
                    else
                    {
                        doctor.ai[1] = criticals[i];
                        npc.ai[3] = doctor.whoAmI;
                        free.Remove(doctor);
                        findDoctor = true;
                    }
                }
            }
            if (!flag)
            {
                patients.AddRange(AllNPCByType(ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>()));
                for (int i = 0; i < free.Count; i++)
                {
                    NPC npc = free[i];
                    float maxValue = 0;
                    int maxValueOwner = -1;
                    foreach (var patient in patients)
                    {
                        float value = (patient.lifeMax - patient.life + 1) / patient.Distance(npc.Center);
                        if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>())
                        {
                            value *= 0.6f;
                            if (patient.life > patient.lifeMax * 0.85 || (int)patient.ai[1] == npc.whoAmI)
                            {
                                value = 0;
                            }
                        }
                        if (patient.type == ModContent.NPCType<NPCs.Servants.EyeofCthulhu.SheildedServantofCthulhu>())
                        {
                            value *= 0.3f;
                        }
                        if (patient.whoAmI == npc.whoAmI)
                        {
                            value *= 0;
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
                for (int i = 0; i < free.Count; i++)
                {
                    NPC npc = free[i];
                    npc.ai[1] = -1;
                }
            }
        }
    }
}
