using ILGPU.Runtime.Cuda;
using Luminance.Common.Utilities;
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
    public class EaterofWorldsHead : NPCBehaviour
    {
        public override int type => NPCID.EaterofWorldsHead;
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
            bool DrawOverBrain = false;
            if (npc.ai[1] < 0)
            {
                npc.alpha = 255;
                DrawOverBrain = true;
            }
            Group.NewGroup(new EaterofWorlds() { DrawOverBrain = DrawOverBrain, BrainPosition = npc.Center}, npc.whoAmI);
            if (Phase == 1 && attack == 2)
            {
                localTimer = 60;
            }
            LocalNextAttack(npc, 0, attack);
        }
        public static void Restart(int mainHead)
        {
            MainHead = mainHead;
            Phase = 1;
            attack = -1;
            attackCounter = -1;
            SetMainTimer(120);

        }
        public static int MainHead;
        public static int Phase;
        public static int attack;
        public static int attackCounter;
        public static int[] attacks1 = [1, 0, 3, 0, 2, 0, 4, 0];
        public static int MainTimer;
        public static float progress;
        public static int MainTimerMax;
        public static int timer;
        public static int target;
        public static Vector2 savedVector;
        public static int BrainofCthulhu;
        public int localTimer;
        public override bool ModPreAI(NPC npc)
        {
            if (npc.velocity != Vector2.Zero)
            {
                npc.rotation = npc.velocity.ToRotation() + MathF.PI / 2;
            }
            NPC brain = Main.npc[BrainofCthulhu];
            if (npc.ai[1] < 0 && npc.Distance(brain.Center) > npc.width)
            {
                npc.alpha = 0;
                npc.ai[1] = NewNPC(npc.GetSource_FromThis(), brain.Center, NPCID.EaterofWorldsBody, npc.whoAmI, npc.whoAmI, npc.ai[1] + 1);
                for (int i = 0; i < t.MyGroups.Count; i++)
                {
                    int g = t.MyGroups[i];
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                    }
                }
            }
            else if (npc.ai[1] >= 0)
            {
                NPC tail = Main.npc[(int)npc.ai[1]];
                if (!tail.active || (tail.type != NPCID.EaterofWorldsTail && tail.type != NPCID.EaterofWorldsBody))
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
            }
            npc.TargetClosest();
            switch(Phase)
            {
                case 1:
                    DoFirstPhase(npc);
                    break;
            }
            if (!Main.npc[MainHead].active || Main.npc[MainHead].type != npc.type)
            {
                MainHead = npc.whoAmI;
            }
            if (npc.whoAmI == MainHead)
            {
                if (MainTimer > 0)
                {
                    MainTimer--;
                }
                progress = 1 - (MainTimer / (float)MainTimerMax);
                if (timer > 0)
                {
                    timer--;
                }
                target = npc.target;
            }
            if (localTimer > 0)
            {
                localTimer--;
            }
            return false;
        }
        public float MaxSpeed = 20;
        public float acceleration = 0.25f;
        public float rotationSpeed = 0.2f;
        void Movement(NPC npc, Vector2 targetPosition, bool instantBreak = false)
        {
            CommonTerrapainFlyingMovement(npc, targetPosition, rotationSpeed, MaxSpeed, acceleration, instantBreak? 75 : 0, instantBreak);
        }
        void DoFirstPhase(NPC npc)
        {
            NPC brain = Main.npc[BrainofCthulhu];
            if (!brain.active || brain.type != NPCID.BrainofCthulhu)
            {
                brain = null;
            }
            switch (attack)
            {
                case -1:
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 0:
                    Vector2 targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.position).RotatedBy(0.2f) * 500;
                    Movement(npc, targetPosition);
                    if (MainTimer == 0 && MainHead == npc.whoAmI)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 1:
                    if (MainHead == npc.whoAmI)
                    {
                        if (npc.Distance(t.Target.Center) > 300 && localTimer == 0)
                        {
                            int direction = (npc.Center.X - t.Target.Center.X).NonZeroSign();
                            targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.position) * 300;
                            Movement(npc, targetPosition);
                        }
                        else if (localTimer == 0)
                        {
                            npc.velocity = npc.DirectionTo(t.Target.Center) * 35;
                            localTimer = 30;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.Normalized() * MathF.Max(npc.velocity.Length() - 0.2f, 20);
                        }
                        if (MainTimer == 0)
                        {
                            NextAttack1(npc);
                        }
                    }
                    else
                    {
                        if (brain != null && localTimer == 0)
                        {
                            for (int i = 0; i < t.MyGroups.Count; i++)
                            {
                                int g = t.MyGroups[i];
                                if (Terrapain.group[g] is EaterofWorlds)
                                {
                                    (Terrapain.group[g] as EaterofWorlds).BrainPosition = brain.Center;
                                    (Terrapain.group[g] as EaterofWorlds).GoingToBrain = true;
                                }
                            }
                            targetPosition = brain.Center;
                            Movement(npc, targetPosition);
                        }
                    }
                    break;
                case 2:
                    if (brain != null && localTimer == 0)
                    {
                        for (int i = 0; i < t.MyGroups.Count; i++)
                        {
                            int g = t.MyGroups[i];
                            if (Terrapain.group[g] is EaterofWorlds)
                            {
                                (Terrapain.group[g] as EaterofWorlds).BrainPosition = brain.Center;
                                (Terrapain.group[g] as EaterofWorlds).GoingToBrain = true;
                            }
                        }
                        targetPosition = brain.Center;
                        Movement(npc, targetPosition);
                    }
                    if (MainTimer == 0 && MainHead == npc.whoAmI && brain == null)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 3:
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.position).RotatedBy(0.2f) * 500;
                    Movement(npc, targetPosition);
                    if (timer == 0)
                    {
                        timer = 200;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.velocity / 2, ModContent.ProjectileType<CursedCloud>(), 0, 0);
                    }
                    if (MainTimer == 0 && MainHead == npc.whoAmI)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 4:
                    if (timer == 0)
                    {
                        timer = 18;
                    }
                    if (MainHead == npc.whoAmI)
                    {
                        targetPosition = savedVector + npc.DirectionFrom(savedVector).RotatedBy(0.2f) * 500;
                        Movement(npc, targetPosition);
                        if (t.Target.Distance(savedVector) > 500)
                        {
                            NextAttack1(npc);
                            NextAttack1(npc);
                        }
                    }
                    else
                    {
                        if (npc.ai[0] > -1)
                        {
                            NPC tail = Main.npc[(int)npc.ai[0]];
                            if (tail.active && tail.type == NPCID.EaterofWorldsTail)
                            {
                                targetPosition = tail.Center;
                                Movement(npc, targetPosition, true);
                            }
                            else
                            {
                                npc.ai[0] = -1;
                            }
                        }
                        if (npc.ai[0] == -1)
                        {
                            var tails = AllNPCByType(NPCID.EaterofWorldsTail);
                            int tail = -1;
                            float distance = 0;
                            for (int i = 0; i < tails.Count; i++)
                            {
                                if (tails[i].ai[1] != npc.whoAmI && tails[i].ai[2] == 0)
                                {
                                    if (tail == -1 || tails[i].Distance(npc.Center) < distance)
                                    {
                                        distance = tails[i].Distance(npc.Center);
                                        tail = i;
                                    }
                                }
                                if (tails[i].ai[1] == MainHead && tails[i].ai[2] == 0)
                                {
                                    tail = i;
                                    distance = tails[i].Distance(npc.Center);
                                    break;
                                }
                            }
                            targetPosition = tails[tail].Center;
                            Movement(npc, targetPosition, true);
                            if (distance < 60)
                            {
                                npc.ai[0] = tails[tail].whoAmI;
                                tails[tail].ai[2] = 1;
                            }
                        }
                    }
                    if (MainTimer == 0 && MainHead == npc.whoAmI)
                    {
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        public static void SetMainTimer(int timer)
        {
            MainTimer = timer;
            MainTimerMax = timer;
            progress = 0;
        }
        public void LocalNextAttack(NPC npc, int oldAttack, int newAttack)
        {
            if (oldAttack == 4)
            {
                npc.ai[0] = 0;
            }
            if (newAttack == 4)
            {
                npc.ai[0] = -1;
            }
        }
        public void NextAttack1(NPC npc)
        {
            int oldAttack = attack;
            attackCounter++;
            if (attackCounter >= attacks1.Length)
            {
                attackCounter = 0;
            }
            attack = attacks1[attackCounter];
            switch (attack)
            {
                case 0:
                    SetMainTimer(200);
                    break;
                case 1:
                    SetMainTimer(800);
                    break;
                case 2:
                    SetMainTimer(400);
                    break;
                case 3:
                    SetMainTimer(600);
                    break;
                case 4:
                    savedVector = t.Target.Center;
                    SetMainTimer(800);
                    break;
            }
            foreach(var n in Main.ActiveNPCs)
            {
                if (n.type == NPCID.EaterofWorldsHead)
                {
                    n.GetGlobalNPC<EaterofWorldsHead>().LocalNextAttack(n, oldAttack, attack);
                }
                if (n.type == NPCID.EaterofWorldsBody)
                {
                    n.GetGlobalNPC<EaterofWorldsBody>().NextAttack(n, oldAttack, attack);
                }
                if (n.type == NPCID.EaterofWorldsTail)
                {
                    n.GetGlobalNPC<EaterofWorldsTail>().NextAttack(n, oldAttack, attack);
                }
            }
        }
        public override bool CheckDead(NPC npc)
        {
            if (Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].type == NPCID.EaterofWorldsBody)
            {
                NPC replace = Main.npc[(int)npc.ai[1]];
                npc.position = replace.position;
                npc.rotation = replace.rotation;
                npc.life = replace.life;
                if (Main.npc[(int)replace.ai[1]].active && Main.npc[(int)replace.ai[1]].type == NPCID.EaterofWorldsBody)
                {
                    Main.npc[(int)replace.ai[1]].ai[0] = npc.whoAmI;
                    npc.ai[1] = (int)replace.ai[1];
                    replace.active = false;
                }
                else
                {
                    replace.life = 0;
                    replace.checkDead();
                    return true;
                }
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
            return true;
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
