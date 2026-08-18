using ILGPU.Runtime.Cuda;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content.Auras;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;
using static Terrapain.Content.TUtilities.AIHelper;

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
                npc.ai[1]++;
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
            if (!Main.npc[MainHead].active || Main.npc[MainHead].type != npc.type)
            {
                MainHead = npc.whoAmI;
            }
            switch(Phase)
            {
                case 1:
                    DoFirstPhase(npc);
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
        public float rotationSpeed = 0.2f;
        void Movement(NPC npc, Vector2 targetPosition, bool instantBreak = false)
        {
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
                    if (npc.Distance(t.Target.Center) > 300 && localTimer == 0)
                    {
                        targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.position) * 300;
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
                    if (MainHead == npc.whoAmI)
                    {
                        float progress = EaterofWorldsHead.progress;
                        progress = 0.3f + progress * 0.7f;
                        progress *= progress;
                        float ro = progress * MathF.PI * 5;
                        

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
                                        count = 8;
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
                                    timer = 90;
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
                            for (int i = 0; i < tails.Count; i++)
                            {
                                if (tails[i].ai[3] != npc.whoAmI && tails[i].ai[2] == 0)
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
                        renderTarget.Dispose();
                        saveScreenRenderTarget.Dispose();

                        lines = [];
                        AuraDisposing = true;
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
        static RenderTarget2D renderTarget;
        static RenderTarget2D saveScreenRenderTarget;
        void updateTarget()
        {
            if (renderTarget == null || renderTarget.Height != Main.screenHeight || renderTarget.Width != Main.screenWidth)
            {
                renderTarget?.Dispose();
                renderTarget = new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
            if (saveScreenRenderTarget == null || saveScreenRenderTarget.Height != Main.screenHeight || saveScreenRenderTarget.Width != Main.screenWidth)
            {
                saveScreenRenderTarget?.Dispose();
                saveScreenRenderTarget = new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
        }
        public override void PostDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (MainHead == npc.whoAmI)
            {
                if (lines.Count > 0)
                {
                    var originalRenderTargets = Main.graphics.GraphicsDevice.GetRenderTargets();
                    if (WorldDifficultySystem.suicide)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                        updateTarget();
                        Main.graphics.GraphicsDevice.SetRenderTarget(saveScreenRenderTarget);
                        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                        spriteBatch.Draw(originalRenderTargets[0].RenderTarget as RenderTarget2D, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                        Main.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                    }
                    spriteBatch.End();
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
                    if (WorldDifficultySystem.suicide)
                    {
                        spriteBatch.End();
                        Main.graphics.GraphicsDevice.SetRenderTargets(originalRenderTargets);

                        if (saveScreenRenderTarget != null)
                        {
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                            spriteBatch.Draw(saveScreenRenderTarget, Vector2.Zero, Color.White);
                            spriteBatch.End();
                        }
                        if (renderTarget != null)
                        {
                            ManagedShader shader = ShaderManager.GetShader("Terrapain.PlayerMask");
                            shader.TrySetParameter("player", Main.LocalPlayer.Center - screenPos);
                            shader.TrySetParameter("w", Main.screenWidth);
                            shader.TrySetParameter("h", Main.screenHeight);
                            shader.TrySetParameter("radius1", 100f);
                            shader.TrySetParameter("radius2", 150f);
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, shader.WrappedEffect, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                            spriteBatch.End();
                        }
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
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
    }
}
