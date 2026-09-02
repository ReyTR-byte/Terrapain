using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.Groups;
using Terrapain.Content.NPCs.VanillaNPCs;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;
using static Terrapain.Content.TUtilities.AIHelper;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    public class EaterofWorldsHead : NPCBehaviour, ISnakePart
    {
        public override int type => NPCID.EaterofWorldsHead;
        public override void ModSetDefaults(NPC entity)
        {
            t.useVanillaDrawing = false;
            entity.knockBackResist = 0;
            entity.alpha = 0;
            entity.lifeMax = (int)(entity.lifeMax * 2.5f);
            if (Phase == 2)
            {
                entity.lifeMax = 10000;
            }
            if ((Phase == 2 && attack != -1) || Main.getGoodWorld)
            {
                entity.scale = 1.4f;
                entity.width = (int)(entity.width * 1.4f / 1.2f);
                entity.height = entity.width;
            }
            entity.GetT().drawCenter = new Vector2(30, 30);
            entity.GetT().despawnLikeABoss = true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            npc.frame = new Rectangle(0, 60, 46, 60);
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
                npc.ai[1]++;
                npc.alpha = 255;
                DrawOverBrain = true;
            }
            if (npc.ai[0] == 1)
            {
                npc.ai[0] = 0;
                Main.npc[(int)npc.ai[1]].ai[0] = npc.whoAmI;
            }
            Group.NewGroup(new EaterofWorlds() { DrawOverBrain = DrawOverBrain, BrainPosition = npc.Center }, npc.whoAmI);
            if (Phase == 1 && attack == 2)
            {
                localTimer = 60;
            }
            LocalNextAttack(npc, 0, attack);
            if (Phase == 1)
            {
                maxLife = npc.lifeMax;
            }
            else if(Phase == 2 && attack != -1)
            {
                MainHead = npc.whoAmI;
            }
            npc.dontTakeDamage = true;
        }
        public static int maxLife;
        public static void Restart(int mainHead)
        {
            MainHead = mainHead;
            Phase = 1;
            attack = -1;
            attackCounter = -1;
            SetMainTimer(120);

        }
        public static int SegmentsCount1 => WorldDifficultySystem.suicide? 90 : 80;
        public static int SegmentsCount2 => WorldDifficultySystem.suicide? 110 : 100;
        public static int cursedFire => ModContent.ProjectileType<CursedFire>();
        public static int cursedFireDamage = 20;
        public static float cursedFireKnockBack = 4.5f;
        public static int cursedFireSpirit => ModContent.ProjectileType<CursedFireSpirit>();
        public static int cursedFireSpiritDamage = 15;
        public static float cursedFireSpiritKnockBack = 4.5f;
        public static int MainHead;
        public static int Phase;
        public static int attack;
        public static float staticAI;
        public static int attackCounter;
        public static int[] attacks1 = [1, 0, 3, 0, 2, 0, 4, 0];
        public static int[] attacks2 = [0, 1, 0];
        public static int MainTimer;
        public static float progress;
        public static int MainTimerMax;
        public static int timer;
        public static int target;
        public static Vector2 savedVector;
        public static int brainofCthulhu;
        public int localTimer;
        public float jawRotation;
        float angularVelocity;
        float jawTargetRotation;
        float jawAngularVelocity;
        float targetRotation;
        public override void OnFirstTick(NPC npc)
        {
            npc.rotation = npc.velocity.ToRotation();
        }
        void CheckGroup(NPC npc)
        {
            for (int i = 0; i < t.MyGroups.Count; i++)
            {
                int g = t.MyGroups[i];
                if (Terrapain.group[g] is EaterofWorlds)
                {
                    return;
                }
            }
            Group.NewGroup(new EaterofWorlds(), npc.whoAmI);
        }
        public override bool ModPreAI(NPC npc)
        {
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            CheckGroup(npc);
            if (npc.dontTakeDamage && NPC.AnyNPCs(NPCID.EaterofWorldsTail))
            {
                npc.dontTakeDamage = false;
            }
            npc.spriteDirection = 1;
            if (npc.velocity != Vector2.Zero)
            {
                targetRotation = npc.velocity.ToRotation();
            }
            NPC brain = Main.npc[brainofCthulhu];
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
            npc.TargetClosest();
            if (!Main.npc[MainHead].active || Main.npc[MainHead].type != npc.type)
            {
                MainHead = npc.whoAmI;
            }
            switch(Phase)
            {
                case 1:
                    DoFirstPhase(npc);
                    break;
                case 2:
                    DoSecondPhase(npc);
                    break;
            }
            if (localTimer > 0)
            {
                localTimer--;
            }
            if (npc.whoAmI == MainHead)
            {
                if (attack == 4)
                {
                    SetLongestAsMain();
                    if (npc.whoAmI < MainHead)
                    {
                        return false;
                    }
                }
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
                if(AuraPowerProgress > 0)
                {
                    ManagedScreenFilter filter = ShaderManager.GetFilter("Terrapain.Blur");

                    filter.TrySetParameter("w", Main.screenWidth);
                    filter.TrySetParameter("h", Main.screenHeight);
                    filter.TrySetParameter("center", savedVector - Main.screenPosition);
                    filter.TrySetParameter("radius1", AuraRadius - 100);
                    filter.TrySetParameter("radius2", AuraRadius);
                    filter.TrySetParameter("power", AuraPowerProgress * 4f);

                    filter.Activate();

                    ManagedScreenFilter filter2 = ShaderManager.GetFilter("Terrapain.HotAir");

                    filter2.TrySetParameter("screenPosition", Main.screenPosition);
                    filter2.TrySetParameter("time", (float)Main.GameUpdateCount / 2);
                    filter2.TrySetParameter("w", Main.screenWidth);
                    filter2.TrySetParameter("h", Main.screenHeight);
                    filter2.TrySetParameter("center", savedVector);
                    filter2.TrySetParameter("radius1", AuraRadius - 100);
                    filter2.TrySetParameter("radius2", AuraRadius);
                    filter2.TrySetParameter("power", AuraPowerProgress * 0.08f);
                    
                    filter2.Activate();
                }
                if (AuraDisposing)
                {
                    AuraPowerProgress = AuraPowerProgress - 0.025f;
                    AuraRadius = 1500 - EasingOut(1, AuraPowerProgress) * 1000;
                    if (AuraPowerProgress <= 0)
                    {
                        AuraDisposing = false;
                    }
                }
            }

            AngularAcceleration(ref angularVelocity, 0.03f, 0.3f, targetRotation, ref npc.rotation);
            Vector2 dir1 = npc.DirectionTo(t.Target.Center);
            Vector2 dir2 = npc.velocity.Normalized();
            Vector2 dir3 = npc.rotation.ToRotationVector2();
            float value = dir1.X * dir2.X + dir1.Y * dir2.Y;
            value *= dir1.X * dir3.X + dir1.Y * dir2.Y;
            value *= Math.Min((300 - npc.Distance(t.Target.Center)) / 100, 1);
            value = Math.Max(value, 0);
            jawTargetRotation = value * 0.3f;
            float accelerat = jawTargetRotation > jawRotation? 0.1f : 0.01f;
            float velocity = jawTargetRotation > jawRotation? 0.5f : 0.15f;
            AngularAcceleration(ref jawAngularVelocity, accelerat, velocity, jawTargetRotation, ref jawRotation);
            return false;
        }
        public static void SetLongestAsMain()
        {
            List<NPC> NPCs = AllNPCByType(NPCID.EaterofWorldsHead);
            int longest = 0;
            foreach(var head in NPCs)
            {
                foreach(int g in head.GetT().MyGroups)
                {
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        if (Terrapain.group[g].Count > longest)
                        {
                            longest = Terrapain.group[g].Count;
                            MainHead = head.whoAmI;
                        }
                        break;
                    }
                }
            }
        }
        public float MaxSpeed = 20;
        public float acceleration = 0.25f;
        public float rotationSpeed = 0.1f;
        void Movement(NPC npc, Vector2 targetPosition, bool instantBreak = false)
        {
            rotationSpeed = 0.1f;
            CommonTerrapainFlyingMovement(npc, targetPosition, rotationSpeed, MaxSpeed, acceleration, instantBreak? 75 : 0, instantBreak);
        }
        public static float AuraRadius;
        public static float AuraPowerProgress;
        public static bool AuraDisposing;

        public static List<int> projectiles;
        struct Lines
        {
            public Vector2 start;
            public Vector2 direction;
            public Color color;
            static Color[] colors = [Color.Red, Color.Yellow, Color.Green];
            public Lines(Vector2 Start, Vector2 dir)
            {
                start = Start;
                direction = dir;
                color = colors[TGlobalNPC.random.Next(3)];
            }
            public Lines()
            {
                color = colors[TGlobalNPC.random.Next(3)];
            }
        }
        static float LinesAlpha;
        static List<Lines> lines = [];
        void DoFirstPhase(NPC npc)
        {
            NPC brain = Main.npc[brainofCthulhu];
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
                    if (npc.Distance(t.Target.Center) > 300 && localTimer == 0)
                    {
                        targetPosition = t.Target.Center;
                        if (WorldDifficultySystem.suicide)
                        {
                            npc.ai[3] *= random.NextDir();
                        }
                        Movement(npc, targetPosition);
                    }
                    else if (localTimer == 0)
                    {
                        npc.velocity = npc.DirectionTo(t.Target.Center) * (MainHead == npc.whoAmI? 35 : 30);
                        localTimer = MainHead == npc.whoAmI? 30 : 90;
                    }
                    else
                    {
                        npc.velocity = npc.velocity.Normalized() * MathF.Max(npc.velocity.Length() - 0.2f, 20);
                    }
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
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
                        //targetPosition = brain.Center;
                        //Movement(npc, targetPosition);
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
                    if (MainHead == npc.whoAmI)
                    {
                        float progress = EaterofWorldsHead.progress;
                        progress = 0.3f + progress * 0.7f;
                        progress *= progress;
                        float ro = progress * MathF.PI * 4.5f;
                        

                        var creps = Group.FindGroup<Creepers>();
                        creps = creps.Where(c => c.AIType == 1).ToList();
                        var crep = creps.Count > 0 ? creps[0] : null;
                        if (crep != null)
                        {
                            if (crep.attackCount == 2)
                            {
                                int count = 0;
                                float distance = 600;
                                switch (staticAI)
                                {
                                    case 0:
                                    case 1:
                                        count = 6;
                                        break;
                                    case 2:
                                    case 3:
                                        count = 10;
                                        break;
                                }

                                if (timer == 0)
                                {
                                    float speed = 15;
                                    switch (staticAI)
                                    {
                                        case 0:
                                            Vector2 targ = savedVector + (ro + MathF.PI / 2).ToRotationVector2() * 220;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX.RotatedBy(ro + MathF.PI * 2 * i / count) * distance;
                                                Vector2 direct = pos.DirectionTo(targ);
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), pos, direct * speed, cursedFireSpirit, cursedFireSpiritDamage, cursedFireSpiritKnockBack);
                                            }
                                            staticAI = 1;
                                            break;
                                        case 1:
                                            targ = savedVector - (ro + MathF.PI / 2).ToRotationVector2() * 220;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX.RotatedBy(ro + MathF.PI * 2 * i / count) * distance;
                                                Vector2 direct = pos.DirectionTo(targ);
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), pos, direct * speed, cursedFireSpirit, cursedFireSpiritDamage, cursedFireSpiritKnockBack);
                                            }
                                            staticAI = 2;
                                            break;
                                        case 2:
                                            int dir = 1;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitY * 500 - Vector2.UnitY * 1000f * i / (count - 1f) + -Vector2.UnitX * distance * dir;
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), pos, Vector2.UnitX * dir * speed, cursedFireSpirit, cursedFireSpiritDamage, cursedFireSpiritKnockBack);
                                                dir *= -1;
                                            }
                                            staticAI = 3;
                                            break;
                                        case 3:
                                            dir = 1;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX * 500 - Vector2.UnitX * 1000f * i / (count - 1f) + -Vector2.UnitY * distance * dir;
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), pos, Vector2.UnitY * dir * speed, cursedFireSpirit, cursedFireSpiritDamage, cursedFireSpiritKnockBack);
                                                dir *= -1;
                                            }
                                            staticAI = 4;
                                            break;
                                        case 4:
                                            lines = [];
                                            crep.attackCount = 0;
                                            staticAI = 0;
                                            break;
                                    }
                                    timer = 60;
                                }
                                else
                                { 
                                    if (lines.Count != count)
                                    {
                                        lines = new List<Lines>();
                                        for (int i = 0; i < count; i++)
                                        {
                                            lines.Add(new Lines());
                                        }
                                    }
                                    switch (staticAI)
                                    {
                                        case 0:
                                            Vector2 targ = savedVector + (ro + MathF.PI / 2).ToRotationVector2() * 220;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX.RotatedBy(ro + MathF.PI * 2 * i / count) * distance;
                                                Vector2 direct = pos.DirectionTo(targ);
                                                Lines lines1 = lines[i];
                                                lines1.start = pos;
                                                lines1.direction = direct;
                                                lines[i] = lines1;
                                            }
                                            break;
                                        case 1:
                                            targ = savedVector - (ro + MathF.PI / 2).ToRotationVector2() * 220;
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX.RotatedBy(ro + MathF.PI * 2 * i / count) * distance;
                                                Vector2 direct = pos.DirectionTo(targ);
                                                Lines lines1 = lines[i];
                                                lines1.start = pos;
                                                lines1.direction = direct;
                                                lines[i] = lines1;
                                            }
                                            break;
                                        case 2:
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitY * 500 - Vector2.UnitY * 1000f * i / (count - 1f) + -Vector2.UnitX * distance;
                                                Lines lines1 = lines[i];
                                                lines1.start = pos;
                                                lines1.direction = Vector2.UnitX;
                                                lines[i] = lines1;
                                            }
                                            break;
                                        case 3:
                                            for (int i = 0; i < count; i++)
                                            {
                                                Vector2 pos = savedVector + Vector2.UnitX * 500 - Vector2.UnitX * 1000f * i / (count - 1f) + -Vector2.UnitY * distance;
                                                Lines lines1 = lines[i];
                                                lines1.start = pos;
                                                lines1.direction = Vector2.UnitY;
                                                lines[i] = lines1;
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                timer = 90;
                            }
                        }

                        AuraPowerProgress = MathF.Min(1, AuraPowerProgress + 0.02f);
                        AuraRadius = 1500 - EasingOut(1, AuraPowerProgress) * 1000;

                        AuraHoldPlayer(500, savedVector);
                        if (projectiles == null)
                        {
                            projectiles = [];

                            for (int i = 0; i < 60; i++)
                            {
                                projectiles.Add(Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, cursedFire, cursedFireDamage, cursedFireKnockBack));
                            }
                        }
                        float prog1 = EasingOut(160, MainTimerMax - MainTimer);
                        float prog2 = EasingOut(160, MainTimer);
                        if (prog2 < 1)
                        {
                            prog1 = prog2;
                        }
                        for (int i = 0; i < projectiles.Count; i++)
                        {
                            Projectile proj = Main.projectile[projectiles[i]];
                            Vector2 position = (MathF.PI * 2 * i / projectiles.Count + MathF.PI * 4 * progress).ToRotationVector2() * 440 + savedVector;
                            Vector2 direction = position.DirectionTo(savedVector);
                            float rotation = direction.ToRotation() + ro;
                            rotation = NormalizeRotation(rotation, false);
                            rotation = Math.Abs(rotation);
                            float force = MathF.Cos(MathF.Abs(rotation - MathF.PI / 2) + MathF.PI / 2) + 1;
                            force = force * 0.8f + 0.2f;
                            force *= 500 * prog1;
                            if (proj.active && proj.type == cursedFire)
                            {
                                proj.timeLeft = 6;
                                proj.ai[0] = direction.ToRotation();
                                proj.ai[1] = force;
                                proj.Center = position;
                            }
                            else
                            {
                                projectiles[i] = Projectile.NewProjectile(npc.GetSource_FromThis(), position, Vector2.Zero, cursedFire, cursedFireDamage, cursedFireKnockBack, -1, rotation, force);
                            }
                        }
                        targetPosition = savedVector + npc.DirectionFrom(savedVector).RotatedBy(0.2f) * 500;
                        Movement(npc, targetPosition);
                        if (t.Target.Distance(savedVector) > 500)
                        {
                            projectiles = null;
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
                            int myTail = -1;
                            for (int i = 0; i < tails.Count; i++)
                            {
                                if (tails[i].ai[3] == npc.whoAmI)
                                {
                                    myTail = i;
                                }
                            }
                            int targetedOnMyTail = -1;
                            if (myTail != -1)
                            {
                                if (tails[myTail].ai[2] != -1)
                                {
                                    targetedOnMyTail = (int)tails[myTail].ai[2];
                                }
                                else
                                {
                                    targetedOnMyTail = (int)tails[myTail].ai[3];
                                }
                            }
                            for (int i = 0; i < tails.Count; i++)
                            {
                                int head = -1;
                                if(tails[i].TryGetGroup<EaterofWorlds>(out var g))
                                {
                                    head = g.members[0];
                                }
                                if (head != npc.whoAmI && head != targetedOnMyTail && (tails[i].ai[2] == -1 || !Main.npc[(int)tails[i].ai[2]].active || Main.npc[(int)tails[i].ai[2]].type != NPCID.EaterofWorldsHead))
                                {
                                    if (tail == -1 || tails[i].Distance(npc.Center) < distance)
                                    {
                                        distance = tails[i].Distance(npc.Center);
                                        tail = i;
                                    }
                                }
                                if (head == MainHead && (tails[i].ai[2] == -1 || !Main.npc[(int)tails[i].ai[2]].active || Main.npc[(int)tails[i].ai[2]].type != NPCID.EaterofWorldsHead))
                                {
                                    tail = i;
                                    distance = tails[i].Distance(npc.Center);
                                    break;
                                }
                            }
                            if (tail == -1)
                            {
                                targetPosition = savedVector + npc.DirectionFrom(savedVector).RotatedBy(0.2f) * 500;
                                Movement(npc, targetPosition);
                            }
                            else
                            {
                                tails[tail].ai[3] = npc.whoAmI;
                                targetPosition = tails[tail].Center;
                                Movement(npc, targetPosition, true);
                            }    
                            if (distance < 60 && tail > -1)
                            {
                                npc.ai[0] = tails[tail].whoAmI;
                                tails[tail].ai[2] = 1;
                            }
                        }
                    }
                    if (MainTimer == 0 && MainHead == npc.whoAmI)
                    {
                        if (renderTarget != null && !renderTarget.IsDisposed)
                        {
                            renderTarget.Dispose();
                        }

                        lines = [];
                        AuraDisposing = true;
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        void DoSecondPhase(NPC npc)
        {
            NPC brain = Main.npc[brainofCthulhu];
            if (!brain.active || brain.type != NPCID.BrainofCthulhu)
            {
                brain = null;
            }
            Vector2 targetPosition;
            if (attack == -1 || npc.ai[0] == -2200)
            {
                npc.ai[0] = -2200;
                if (brain != null)
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
                    // targetPosition = brain.Center;
                    // Movement(npc, targetPosition);
                }
                else
                {
                    npc.active = false;
                }   
            }
            switch (attack)
            {
                case 0:
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.position).RotatedBy(0.2f) * 500;
                    Movement(npc, targetPosition);
                    if (MainTimer == 0)
                    {
                        NextAttack2();
                    }
                    break;
                case 1:
                    switch (npc.ai[0])
                    {
                        case 0:
                            targetPosition = t.Target.Center;
                            Movement(npc, targetPosition);
                            if (npc.Distance(t.Target.Center) < 300)
                            {
                                npc.ai[0] = 1;
                                npc.ai[2] = 3;
                                timer = 100;
                            }
                            if (MainTimer == 0)
                            {
                                NextAttack2();
                            }
                            break;
                        case 1:
                            targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center).RotatedBy(npc.ai[3] * 0.003f) * 300;
                            acceleration = 0.05f;
                            Movement(npc, targetPosition, true);
                            acceleration = 0.25f;
                            int time = 100;
                            targetRotation = npc.DirectionTo(t.Target.Center).ToRotation();
                            if (timer == time / 2)
                            {
                                Vector2 dir = npc.DirectionTo(t.Target.Center);
                                int count = 7;
                                float speed = 18;
                                int _time = 25;
                                float angle = MathF.PI * 0.75f;
                                float startAngle = dir.ToRotation();
                                float angleBetween = angle / count;
                                startAngle -= angleBetween / 2 * count;
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, (startAngle + angleBetween * i).ToRotationVector2() * speed, BrainOfCthulhu.ichorSpike, BrainOfCthulhu.ichoreSpikeDamage, BrainOfCthulhu.ichoreSpikeKnockback, -1, _time, dir.X, dir.Y);
                                }
                            }
                            else if (timer == 0)
                            {
                                if (npc.ai[2] > 0)
                                {
                                    timer = time;
                                }
                                else
                                {
                                    npc.ai[0] = 2;
                                    if (WorldDifficultySystem.torture)
                                    {
                                        break;
                                    }
                                }
                                Vector2 dir = npc.DirectionTo(t.Target.Center);
                                int count = 5;
                                float speed = 15;
                                float angle = MathF.PI * 0.85f;
                                float startAngle = dir.ToRotation();
                                float angleBetween = angle / count;
                                startAngle -= angleBetween / 2 * count;
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, (startAngle + angleBetween * i).ToRotationVector2() * speed, cursedFireSpirit, cursedFireSpiritDamage, cursedFireSpiritKnockBack, -1, 1, npc.target);
                                }
                                npc.ai[2]--;
                            }
                            break;  
                        case 2:
                            if (WorldDifficultySystem.suicide)
                            {
                                npc.ai[3] *= random.NextDir();
                            }
                            npc.ai[0] = 3;
                            npc.velocity = npc.DirectionTo(t.Target.Center) * (MainHead == npc.whoAmI? 35 : 30);
                            timer = 30;
                            break;
                        case 3:
                            npc.velocity = npc.velocity.Normalized() * MathF.Max(npc.velocity.Length() - 0.2f, 20);
                            if (timer == 0)
                            {
                                npc.ai[0] = 0;
                            }
                            break;
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
            if (newAttack == 1)
            {
                if (WorldDifficultySystem.suicide)
                {
                    npc.ai[3] = random.NextDir();
                }
                else
                {
                    npc.ai[3] = 0;
                }
            }
        }
        public static bool CheckPhase ()
        {
            if (Phase == 1 && NPC.AnyNPCs(NPCID.EaterofWorldsTail))
            {
                int totalLife = 0;
                int targetLife = SegmentsCount1 * maxLife / 2;
                foreach (var life in EvilBosses.BrainOfCthulhu.segmentsLifes)
                {
                    totalLife += life;
                }
                List<NPC> segments = AllNPCByType(NPCID.EaterofWorldsHead);
                segments.AddRange(AllNPCByType(NPCID.EaterofWorldsBody));
                segments.AddRange(AllNPCByType(NPCID.EaterofWorldsTail));
                foreach (var npc in segments)
                {
                    totalLife += npc.life;
                }
                if (totalLife < targetLife)
                {
                    Phase = 2;
                    attackCounter = -1;
                    attack = -1;
                    return true;
                }
            }
            return false;
        }
        public void NextAttack1(NPC npc)
        {
            if(CheckPhase())
            {
                return;
            }
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
                    SetLongestAsMain();
                    SetMainTimer(800);
                    break;
                case 2:
                    SetMainTimer(400);
                    break;
                case 3:
                    SetMainTimer(600);
                    break;
                case 4:
                    staticAI = 0;
                    AuraRadius = 1500;
                    AuraPowerProgress = 0;
                    savedVector = t.Target.Center;
                    SetMainTimer(900);
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
        public static void NextAttack2()
        {
            if(CheckPhase())
            {
                return;
            }
            attackCounter++;
            if (attackCounter >= attacks2.Length)
            {
                attackCounter = 0;
            }
            attack = attacks2[attackCounter];
            NPC head = Main.npc[MainHead];
            if (!head.active || head.type != NPCID.EaterofWorldsHead)
            {
                head = null;
            }
            switch (attack)
            {
                case 0:
                    SetMainTimer(200);
                    break;
                case 1:
                    if (head != null)
                    {
                        head.ai[0] = 0;
                        head.ai[2] = 0;
                        head.ai[3] = WorldDifficultySystem.suicide? 1.2f : 1f;
                    }
                    SetLongestAsMain();
                    SetMainTimer(800);
                    break;
            }
        }
        public override bool OverrideTexture(ref Asset<Texture2D> texture)
        {
            texture = ModContent.Request<Texture2D>(this.GetPath());
            return true;
        }
        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0;
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return npc.alpha == 0? null : false;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return npc.alpha == 0;
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return npc.alpha == 0? null : false;
        }
        public override bool CheckActive(NPC npc)
        {
            return false;
        }
        public override bool CheckDead(NPC npc)
        {
            if (Phase == 2 && attack != -1)
            {
                var g = Terrapain.group[group];
                if (g != null && g is EaterofWorlds)
                {
                    (g as EaterofWorlds).Dying = true;
                }
                npc.life = 1;
                NPC tail = Main.npc[(int)npc.ai[1]];
                if (!tail.active || tail.ai[0] != npc.whoAmI || (tail.type != NPCID.EaterofWorldsBody && tail.type != NPCID.EaterofWorldsTail))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        static RenderTarget2D renderTarget;
        static GraphicsDevice renderTargetDevice;
        void updateTarget()
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            if (renderTarget == null || renderTarget.IsDisposed || renderTargetDevice != graphicsDevice || renderTarget.Height != Main.screenHeight || renderTarget.Width != Main.screenWidth)
            {
                renderTarget?.Dispose();
                renderTarget = new RenderTarget2D(
                    graphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
                renderTargetDevice = graphicsDevice;
            }
        }
        public override void DrawToRenderTarget(NPC npc)
        {
            if (MainHead == npc.whoAmI)
            {
                if (lines.Count > 0 && WorldDifficultySystem.suicide)
                {
                    Vector2 screenPos = Main.screenPosition;
                    SpriteBatch spriteBatch = Main.spriteBatch;

                    updateTarget();
                    Main.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                    Main.graphics.GraphicsDevice.Clear(Color.Transparent);

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Texture2D tex = null;
                    if (GraphicsConfig.Instance.shaders == GraphicsConfig.GraphicsLevel.Potato)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        tex = ExtraTextureRegistry.CubedGradient10Mirrored.Value;
                    }
                    foreach (var line in lines)
                    {
                        Vector2 p1 = line.start - screenPos;
                        float? num1 = line.direction.X == 0? null : (20 - p1.X) / line.direction.X;
                        float? num2 = line.direction.Y == 0? null : (20 - p1.Y) / line.direction.Y;
                        float? num3 = line.direction.X == 0? null : (Main.screenWidth - 20 - p1.X) / line.direction.X;
                        float? num4 = line.direction.Y == 0? null : (Main.screenHeight - 20 - p1.Y) / line.direction.Y;
                        Vector2 p2 = Vector2.Zero;
                        Vector2 p3 = Vector2.Zero;
                        if (num1.HasValue)
                        {
                            Vector2 _p2 = p1 + line.direction * num1.Value;
                            if (_p2.Y > 20 && _p2.Y < Main.screenHeight - 20)
                            {
                                p2 = _p2;
                            }
                        }
                        if (num2.HasValue && p2 == Vector2.Zero)
                        {
                            Vector2 _p2 = p1 + line.direction * num2.Value;
                            if (_p2.X > 20 && _p2.Y < Main.screenWidth - 20)
                            {
                                p2 = _p2;
                            }
                        }
                        if (p2 != Vector2.Zero)
                        {
                            if (num3.HasValue)
                            {
                                Vector2 _p3 = p1 + line.direction * num3.Value;
                                if (_p3.Y > 20 && _p3.Y < Main.screenHeight - 20)
                                {
                                    p3 = _p3;
                                }
                            }
                            if (num4.HasValue && p3 == Vector2.Zero)
                            {
                                Vector2 _p3 = p1 + line.direction * num4.Value;
                                if (_p3.X > 20 && _p3.X < Main.screenWidth - 20)
                                {
                                    p3 = _p3;
                                }
                            }
                            if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                            {
                                ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
                                Shade.TrySetParameter("lenght", 900);
                                Shade.TrySetParameter("width", 8);
                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                            }
                            spriteBatch.DrawLine(p3 + Main.screenPosition, p2 + Main.screenPosition, line.color, 8, tex);
                        }
                    }
                    //if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                    //{
                    //    spriteBatch.End();
                    //    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    //}
                    spriteBatch.End();
                    Main.graphics.GraphicsDevice.SetRenderTarget(null);
                }
            }
        }
        public override void PostDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (MainHead == npc.whoAmI)
            {
                if (lines.Count > 0)
                {
                    if (WorldDifficultySystem.torture)
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        Texture2D tex = null;
                        if (GraphicsConfig.Instance.shaders == GraphicsConfig.GraphicsLevel.Potato)
                        {
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                            tex = ExtraTextureRegistry.CubedGradient10Mirrored.Value;
                        }
                        foreach (var line in lines)
                        {
                            Vector2 p1 = line.start - screenPos;
                            float? num1 = line.direction.X == 0? null : (20 - p1.X) / line.direction.X;
                            float? num2 = line.direction.Y == 0? null : (20 - p1.Y) / line.direction.Y;
                            float? num3 = line.direction.X == 0? null : (Main.screenWidth - 20 - p1.X) / line.direction.X;
                            float? num4 = line.direction.Y == 0? null : (Main.screenHeight - 20 - p1.Y) / line.direction.Y;
                            Vector2 p2 = Vector2.Zero;
                            Vector2 p3 = Vector2.Zero;
                            if (num1.HasValue)
                            {
                                Vector2 _p2 = p1 + line.direction * num1.Value;
                                if (_p2.Y > 20 && _p2.Y < Main.screenHeight - 20)
                                {
                                    p2 = _p2;
                                }
                            }
                            if (num2.HasValue && p2 == Vector2.Zero)
                            {
                                Vector2 _p2 = p1 + line.direction * num2.Value;
                                if (_p2.X > 20 && _p2.Y < Main.screenWidth - 20)
                                {
                                    p2 = _p2;
                                }
                            }
                            if (p2 != Vector2.Zero)
                            {
                                if (num3.HasValue)
                                {
                                    Vector2 _p3 = p1 + line.direction * num3.Value;
                                    if (_p3.Y > 20 && _p3.Y < Main.screenHeight - 20)
                                    {
                                        p3 = _p3;
                                    }
                                }
                                if (num4.HasValue && p3 == Vector2.Zero)
                                {
                                    Vector2 _p3 = p1 + line.direction * num4.Value;
                                    if (_p3.X > 20 && _p3.X < Main.screenWidth - 20)
                                    {
                                        p3 = _p3;
                                    }
                                }
                                if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                                {
                                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
                                    Shade.TrySetParameter("lenght", 900);
                                    Shade.TrySetParameter("width", 8);
                                    spriteBatch.End();
                                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                                }
                                spriteBatch.DrawLine(p3 + Main.screenPosition, p2 + Main.screenPosition, line.color, 8, tex);
                            }
                        }
                        if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                        {
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        }
                    }
                    else
                    {
                        if (renderTarget != null)
                        {
                            ManagedShader shader = ShaderManager.GetShader("Terrapain.PlayerMask");
                            shader.TrySetParameter("player", Main.LocalPlayer.Center - screenPos);
                            shader.TrySetParameter("w", Main.screenWidth);
                            shader.TrySetParameter("h", Main.screenHeight);
                            shader.TrySetParameter("radius1", 100f);
                            shader.TrySetParameter("radius2", 150f);
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, shader.WrappedEffect, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        }
                    }
                }

                if (AuraPowerProgress > 0)
                {
                    var noise = ExtraTextureRegistry.EaterofWorldsNoise;

                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.EaterofWorldsAuraShader");
                    Shade.TrySetParameter("center", savedVector - Main.screenPosition);
                    Shade.TrySetParameter("time", Main.GameUpdateCount / 380f);
                    Shade.TrySetParameter("radius1", AuraRadius - 300);
                    Shade.TrySetParameter("radius2", AuraRadius);
                    Shade.TrySetParameter("w", Main.screenWidth);
                    Shade.TrySetParameter("h", Main.screenHeight);
                    Shade.TrySetParameter("color2", Color.Red * AuraPowerProgress);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                    spriteBatch.Draw(noise.Value, rekt, null, Color.Yellow * 0.5f * AuraPowerProgress, 0f, noise.Value.Size() * 0.5f, 0, 1f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
        }

        public void UpdateAsHead(NPC npc) { }

        public void UpdateAsBody(NPC npc)
        {
            NPC tail = Main.npc[(int)npc.ai[1]];
            if (!tail.active || tail.ai[0] != npc.whoAmI || (tail.type != NPCID.EaterofWorldsBody && tail.type != NPCID.EaterofWorldsTail))
            {    
                npc.life = 0;
                npc.checkDead();
            }
        }

        public void UpdateAsTail(NPC npc)
        {
            if (npc.ai[1] >= 0)
            {
                NPC tail = Main.npc[(int)npc.ai[1]];
                if (!tail.active || tail.ai[0] != npc.whoAmI || (tail.type != NPCID.EaterofWorldsBody && tail.type != NPCID.EaterofWorldsTail))
                {    
                    npc.life = 0;
                    npc.checkDead();
                }
            }
        }
        
        public int group;
        public void SetGroup(int group, int member)
        {
            this.group = group;
        }
    }
}
