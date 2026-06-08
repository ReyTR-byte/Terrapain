using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;
using static Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime.KingSlime;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime
{
    [AutoloadBossHead]
    public class CrownedKingSlime : ModNPC
    {
        public override string BossHeadTexture => "Terrapain/Content/NPCs/Bosses/VanillaBosses/KingSlime/NinjaKingSlime_Head_Boss";
        public override string Texture => "Terrapain/Content/NPCs/Bosses/VanillaBosses/KingSlime/NinjaKingSlime";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void SetDefaults()
        {
            NPC.width = 65;
            NPC.height = 69;
            NPC.lifeMax = 1400;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.scale = 0.707106f;
            NPC.boss = true;

            NPC.alpha = 30;

            NPC.knockBackResist = 0f;

            NPC.npcSlots = 10f;

            NPC.noTileCollide = false;
            NPC.noGravity = false;

            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.MaxFallSpeedMultiplier = MultipliableFloat.One * 500;

            AnimationType = NPCID.KingSlime;
        }
        public override void BossHeadSlot(ref int index)
        {
            if (KSCactive && KingSlimeCrown.ai[0] == -2)
            {
                index = 7;
            }
        }
        public int ninjaKingSlime;
        public NPC NinjaKingSlime => Main.npc[ninjaKingSlime];
        NinjaKingSlime NKS => (NinjaKingSlime)NinjaKingSlime.ModNPC;
        bool NKSactive => NinjaKingSlime != null && NinjaKingSlime.active && NinjaKingSlime.type == ModContent.NPCType<NinjaKingSlime>();

        public int kingSlimeCrown;
        public NPC KingSlimeCrown => Main.npc[kingSlimeCrown];
        KingSlimeCrown KSC => (KingSlimeCrown)KingSlimeCrown.ModNPC;
        bool KSCactive => KingSlimeCrown != null && KingSlimeCrown.active && KingSlimeCrown.type == ModContent.NPCType<KingSlimeCrown>();

        public int kingSlime;
        public NPC KingSlime => Main.npc[kingSlime];
        KingSlime KS => KingSlime.GetGlobalNPC<KingSlime>();
        bool KSactive => KingSlime != null && KingSlime.active && KingSlime.type == NPCID.KingSlime;

        public int CurentAttack;
        public int attackCounter = -1;
        public int[] phase1 = [0, 1, 0, 4, 0, 5];
        int mainTimer;
        int movementTimer;
        int[] timers = new int[2];
        //int phase
        //{
        //    get => (int)Main.npc[t.npcid].ai[0];
        //    set => Main.npc[t.npcid].ai[0] = value;
        //}
        public bool teleporting
        {
            get => NPC.ai[1] > 0;
            set => NPC.ai[1] = value ? 120 : 0;
        }
        public int teleportTimer
        {
            get => (int)NPC.ai[1] - 1;
            set => NPC.ai[1] = value + 1;
        }
        public Vector2 teleportPosition
        {
            get => new Vector2(NPC.ai[2], NPC.ai[3]);
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }
        bool oldCollideY;
        Player Target => Main.player[NPC.target];
        UnifiedRandom rand => TGlobalNPC.random;

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX * 2, SlimeWall, 0, 0, -1, 10);
            NextAttack1();
        }
        public override bool? CanFallThroughPlatforms()
        {
            return Target.position.Y > NPC.Bottom.Y && (CurentAttack != 2 && CurentAttack != 3);
        }
        int SlimeWall => ModContent.ProjectileType<SlimeWall>();

        int SlimeBall => ModContent.ProjectileType<KingSlimeBall>();
        public int SlimeBallDamage = 10;
        public float SlimeBallKnockback = 5.5f;

        public static int Slime => ModContent.ProjectileType<SlimeProjectile>();
        public static int SlimeDamage = 15;
        public static float SlimeKnockBack = 4;
        public override void AI()
        {
            if (!KSactive)
            {
                NPC.active = false;
            }
            NPC.defense = NPC.defDefense;
            NPC.immortal = false;
            NPC.TargetClosest();
            NPC.noTileCollide = false;
            //switch (phase)
            //{
            //    case 0:
            DoFirstPhase();
            //        break;
            //}
            if (mainTimer > 0)
            {
                mainTimer--;
            }
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] > 0)
                {
                    timers[i]--;
                }
            }
            if (teleporting && CurentAttack != 5)
            {
                int d = Dust.NewDust(teleportPosition, 0, 0, DustID.ShimmerSpark);
                Main.dust[d].velocity = rand.NextVector2Unit() * (3 + rand.NextFloat(5));
                teleportTimer--;
            }
            oldCollideY = NPC.collideY;
        }

        void ChillMovement()
        {
            if ((NPC.collideY || NPC.collideX) && !teleporting)
            {
                NPC.velocity = Vector2.Zero;
                if (movementTimer > 0)
                {
                    movementTimer--;
                }
                else
                {
                    NPC.velocity.Y = -MathHelper.Clamp((NPC.Center.Y - Target.Center.Y) / 30, 10, 30);
                    NPC.velocity.X = MathHelper.Clamp(MathF.Abs(Target.Center.X - NPC.Center.X) / 60, 10, 30) * (Target.Center.X - NPC.Center.X).NonZeroSign();
                }
            }
            else
            {
                movementTimer = (int)(50 * NPC.GetLifePercent()) + 20;
            }
            if (NPC.Distance(Target.Center) > 1500)
            {
                if (!teleporting)
                {
                    teleporting = true;
                    teleportPosition = Target.Center + new Vector2((rand.Next() == 0 ? -1 : 1) * 300, (rand.Next() == 0 ? -1 : 1) * 200);
                    if (!SimpleColision(teleportPosition, Target.position, Target.width, Target.height))
                    {
                        teleportPosition = Target.Center;
                    }
                }
                NPC.velocity = Vector2.Zero;
                if (teleportTimer == 0)
                {
                    NPC.Center = teleportPosition;
                    if (NPC.Distance(Target.Center) > 500)
                    {
                        NPC.velocity = NPC.DirectionTo(Target.Center) * 20;
                    }
                }
            }
            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
                NPC.collideY = false;
                NPC.collideX = false;
            }
        }
        List<RingOfSlimes> rings = new();
        float slimeSpeed = 22;
        int slimeRate = 50;
        void DoFirstPhase()
        {
            switch (CurentAttack)
            {
                case -1:
                    ChillMovement();
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1();
                    }
                    break;
                case 0:
                    ChillMovement();
                    if (mainTimer == 0 && !teleporting && (!NKSactive || NKS.CurentAttack == 0))
                    {
                        NextAttack1();
                    }
                    break;
                case 1:
                    ChillMovement();
                    if (timers[0] == 1)
                    {
                        Vector2 Pos = Target.Center + new Vector2((int)NPC.ai[0] / 100 - 200, NPC.ai[0] % 100 - 450);
                        Vector2 velo = Pos.DirectionTo(SmartShoot(Pos, slimeSpeed, Target.Center, Target.velocity, 60)) * slimeSpeed;

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), Pos, velo, Slime, SlimeDamage, SlimeKnockBack, -1, 2);
                    }
                    else if (timers[0] == 0)
                    {
                        timers[0] = slimeRate;
                        NPC.ai[0] = rand.Next(40000);
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        int count = 5;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 2f + NPC.DirectionTo(Target.Center) * 20, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        count = 4;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * 0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * -0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        NextAttack1();
                    }
                    break;
                case 2:
                    NPC.defense = 20;
                    if (NPC.collideY)
                    {
                        NPC.velocity = Vector2.Zero;
                    }
                    if (!teleporting)
                    {
                        if (rings.Count == 0)
                        {
                            rings.Add(new RingOfSlimes(50, NPC) { angularVelocity = 9, Center = NPC.Center });
                        }
                        RingOfSlimes ring = rings[0];
                        ring.Center = NPC.Center;
                        ring.Radius = MathF.Min(ring.Radius + 25, 1000);
                        ring.slimeMaxSpeed = ring.Radius == 1000 ? 25 : 50;
                        ring.dealDamage = ring.Radius == 1000;
                        ring.Update();
                        rings[0] = ring;

                    }
                    else
                    {
                        if (teleportPosition != Target.Center)
                        {
                            teleportPosition = Target.Center + Target.DirectionTo(teleportPosition) * MathF.Min(teleportPosition.Distance(Target.Center), 75);
                        }
                        if (teleportTimer == 1)
                        {
                            NPC.Center = teleportPosition;
                        }
                    }
                    if ((!NKSactive || NKS.CurentAttack == 0) && rings.Count > 0 && rings[0].Radius == 1000)
                    {
                        CurentAttack = 3;
                        mainTimer = 250;
                        NPC.ai[0] = 5;
                    }
                    break;
                case 3:
                    NPC.defense = 20;
                    int time = 90;
                    int num = WorldDifficultySystem.suicide? 2 : 3;
                    if (timers[0] == 0 && NPC.ai[0] > num)
                    {
                        if (NPC.ai[0] < 5)
                        {
                            foreach (int proj in rings[1].Projectiles)
                            {
                                Main.projectile[proj].active = false;
                            }
                            rings.RemoveAt(1);
                        }
                        List<int> projs = new List<int>();
                        for (int i = (int)(NPC.ai[0] / 2); i < rings[0].Count; i += (int)NPC.ai[0])
                        {
                            projs.Add(rings[0].Projectiles[i]);
                            rings[0].Projectiles.RemoveAt(i);
                        }
                        NPC.ai[0]--;
                        var ring1 = rings[0];
                        ring1.angularVelocity *= -1;
                        rings[0] = ring1;
                        rings.Add(new RingOfSlimes() { rotation = rings[0].Center.DirectionTo(Main.projectile[projs[0]].Center).ToRotation(), Center = rings[0].Center, angularVelocity = rings[0].angularVelocity * -1, Radius = ((NPC.ai[0] + 1) / 5 * 1000), slimeMaxSpeed = 50, dealDamage = true, Projectiles = projs });
                        timers[0] = time;
                    }
                    if (timers[0] == 0 && NPC.ai[0] == num)
                    {
                        foreach (int proj in rings[1].Projectiles)
                        {
                            Main.projectile[proj].active = false;
                        }
                        rings.RemoveAt(1);
                        NPC.ai[0]--;
                    }
                    if (NPC.ai[0] < 5 && NPC.ai[0] >= num)
                    {
                        var ring2 = rings[1];
                        ring2.Radius = MathF.Max(ring2.Radius - ((NPC.ai[0] + 1) / 5 * 1000) / time, 0);
                        rings[1] = ring2;
                        var ring1 = rings[0];
                        ring1.Radius = MathF.Max(NPC.ai[0] / 5 * 1000, ring2.Radius);
                        rings[0] = ring1;
                    }
                    for (int i = 0; i < rings.Count; i++)
                    {
                        var ring1 = rings[i];
                        ring1.Update();
                        rings[i] = ring1;
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        movementTimer = 100;
                        foreach (var ring3 in rings)
                        {
                            ring3.End();
                        }
                        rings = new();
                        attackCounter = -1;
                        NextAttack1();
                    }
                    break;
                case 4:
                    ChillMovement();
                    KingSlimeCrown.ai[0] = 0;
                    if (mainTimer == 0)
                    {
                        KingSlimeCrown.ai[0] = -1;
                        NextAttack1();
                    }
                    break;
                case 5:
                    float shirikinSpeed = 1.5f;
                    float targetX = Target.Center.X - 750 * NPC.ai[0].NonZeroSign();
                    if (mainTimer > 600)
                    {

                        canHit = false;
                        if (NPC.collideX || NPC.collideY)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        int d = Dust.NewDust(new Vector2(targetX, NPC.ai[2]), 0, 0, DustID.ShimmerSpark);
                        Main.dust[d].velocity = rand.NextVector2Unit() * (3 + rand.NextFloat(5));
                    }
                    else if (mainTimer == 600)
                    {
                        NPC.Center = new Vector2(targetX, NPC.ai[1]);
                        NPC.velocity = Vector2.Zero;
                    }
                    else
                    {
                        if (KSCactive && KingSlimeCrown.ai[0] == 0)
                            KingSlimeCrown.ai[0] = -1;
                        if (Target.Center.Y < NPC.ai[1] - 500 || Target.Center.Y > NPC.ai[1] + 500 || (Target.Center.X > NPC.Center.X && NPC.ai[0].NonZeroSign() == -1) || (Target.Center.X < NPC.Center.X && NPC.ai[0].NonZeroSign() == 1))
                        {
                            if (KSCactive)
                            {
                                KingSlimeCrown.ai[0] = 0;
                            }
                            float saveai1 = NPC.ai[1];
                            NPC.ai[1] = 0;
                            ChillMovement();
                            NPC.ai[1] = saveai1;
                            if (timers[0] > 15)
                            {
                                timers[0] = 15;
                            }
                            if (timers[0] == 0)
                            {
                                int count = 5;
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 2f + NPC.DirectionTo(Target.Center) * 20, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                                }
                                count = 4;
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * 0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                                }
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * -0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                                }
                                timers[0] = 15;
                            }
                        }
                        else
                        {
                            float minSpeed = 6;
                            float playerSpeedMultiply = (Target.Center.X - NPC.Center.X) / 750;
                            float speed = 0;
                            if (NPC.ai[0].NonZeroSign() == 1)
                            {
                                speed = MathF.Max(Target.velocity.X * NPC.ai[0].NonZeroSign() * playerSpeedMultiply, minSpeed);
                            }
                            else
                            {
                                speed = MathF.Min(Target.velocity.X * NPC.ai[0].NonZeroSign() * playerSpeedMultiply, -minSpeed);
                            }
                                NPC.velocity.X = speed;
                            if (NPC.collideY)
                            {
                                NPC.velocity.Y = -MathHelper.Clamp((NPC.Center.Y - Target.Center.Y) / 30, 10, 30);
                            }
                        }
                        canHit = true;
                    }
                    time = WorldDifficultySystem.suicide? 70 : 80;
                    if (NPC.ai[0] == 0)
                    {
                        Target.AddBuff(ModContent.BuffType<Slimed>(), 750);
                        NPC.velocity = Vector2.Zero;
                        teleportPosition = Vector2.Zero;
                        NPC.ai[0] = Target.Center.X * Target.velocity.X.NonZeroSign();
                        NPC.ai[1] = Target.Center.Y;
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] + 500), Vector2.UnitY * -shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                        p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] - 500), Vector2.UnitY * shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                    }
                    else if (Target.Center.X > MathF.Abs(NPC.ai[0]) + 800)
                    {
                        NPC.ai[0] += 1600 * NPC.ai[0].NonZeroSign();
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] + 500), Vector2.UnitY * -shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                        p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] - 500), Vector2.UnitY * shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                    }
                    else if (Target.Center.X < MathF.Abs(NPC.ai[0]) - 800)
                    {
                        NPC.ai[0] -= 1600 * NPC.ai[0].NonZeroSign();
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] + 500), Vector2.UnitY * -shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                        p = Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]), NPC.ai[1] - 500), Vector2.UnitY * shirikinSpeed, SlimeWall, 0, 0, -1, 40);
                        Main.projectile[p].timeLeft = (int)(475 / shirikinSpeed);
                    }
                    if (timers[0] == 0)
                    {
                        if (mainTimer < 250)
                        {
                            float speed = 17;
                            float angle = 0.2f;
                            int count = WorldDifficultySystem.suicide? 4 : 3;
                            for (int i = -count; i < count + 1; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.ai[1] + 1000f / count * i), Vector2.UnitX.RotatedBy(angle) * speed * NPC.ai[0].NonZeroSign(), Slime, SlimeDamage, SlimeKnockBack, -1, 2);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X, NPC.ai[1] + 1000f / count * i), Vector2.UnitX.RotatedBy(-angle) * speed * NPC.ai[0].NonZeroSign(), Slime, SlimeDamage, SlimeKnockBack, -1, 2);
                            }
                            timers[0] = time;
                        }
                        else if(mainTimer < 500)
                        {
                            int count = 5;
                            float randomRange = 0.3f;
                            float r = WorldDifficultySystem.suicide? rand.NextFloat(-randomRange, randomRange) : 0;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(15 * NPC.ai[0].NonZeroSign(), -15).RotatedBy(randomRange) + Vector2.UnitX.RotatedBy(MathF.PI / count * 2 * i) * 3.5f, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                            }
                            timers[0] = (int)(time * 1.5f);
                        }
                        else
                        {
                            float speed = WorldDifficultySystem.suicide? 15 : 14;
                            int range = 4;
                            int safeRange = WorldDifficultySystem.suicide? 2 : 3;
                            int count = 18;
                            int shift = WorldDifficultySystem.suicide? 2 : 3;
                            int hole = rand.Next(-range + shift, range + shift + 1);
                            for (int i = -count; i < count + 1; i++)
                            {
                                if (Math.Abs(i - hole) > safeRange)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(targetX, NPC.ai[1] + 500f / count * i), Vector2.UnitX * speed * NPC.ai[0].NonZeroSign(), Slime, SlimeDamage, SlimeKnockBack, -1, 2);
                                }
                            }
                            timers[0] = time;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(MathF.Abs(NPC.ai[0]) + 800 * NPC.ai[0].NonZeroSign(), NPC.ai[1]), Vector2.UnitX * -NPC.ai[0].NonZeroSign(), SlimeWall, 0, 0, -1, 40);
                        NPC.ai[1] = 0;
                        NextAttack1();
                    }
                    break;
            }
        }
        bool canHit = true;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return canHit;
        }
        void NextAttack1()
        {
            if (CurentAttack == 0 && NKSactive)
            {
                CurentAttack = -1;
            }
            else
            {
                attackCounter++;
                if (attackCounter >= phase1.Length)
                {
                    attackCounter = 0;
                }
                CurentAttack = phase1[attackCounter];
            }
            if (CurentAttack == 4 && !KSCactive)
            {
                attackCounter += 2;
                if (attackCounter >= phase1.Length)
                {
                    attackCounter = 0;
                }
                CurentAttack = phase1[attackCounter];
            }
            switch (CurentAttack)
            {
                case -1:
                case 0:
                    mainTimer = 180;
                    break;
                case 1:
                    mainTimer = 250;
                    break;
                case 4:
                    mainTimer = 350;
                    break;
                case 5:
                    mainTimer = 750;
                    NPC.ai[0] = 0;
                    timers[0] = 20;
                    break;
            }
        }
        public override void OnKill()
        {
            if(KSactive)
            {
                KS.crownedKingSlimeKilled = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(TextureAssets.Npc[NPCID.KingSlime].Value, NPC.Bottom - Main.screenPosition + Vector2.UnitY * 2, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            if (CurentAttack == 1)
            {
                ManagedShader Shade = ShaderManager.GetShader("Terrapain.PortalShader");
                Texture2D value = ExtraTextureRegistry.WhitePixel.Value;
                Vector2 pos = Target.Center + new Vector2((int)NPC.ai[0] / 100 - 200, NPC.ai[0] % 100 - 450);
                Vector2 velo = pos.DirectionTo(SmartShoot(pos, slimeSpeed, Target.Center, Target.velocity, 60));
                Vector2 scale = new(30 + 75 * (1 - (timers[0] / (float)slimeRate)), 30);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(value, pos - Main.screenPosition, null, Color.LightBlue * (1 - (timers[0] / (float)slimeRate)), velo.ToRotation(), new Vector2(0.1f, 0.5f), scale, SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }
    }
}
