using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System.Net.Security;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content.Items.DropRulls;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime
{
    public class KingSlime : NPCBehaviour
    {

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.type == NPCID.KingSlime;
        }
        public int CurentAttack;
        public int attackCounter = -1;
        public int[] phase1 = [0, 1, 0, 2, 0, 3, 0, 4, 5];
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
            get => Main.npc[t.npcid].ai[1] > 0;
            set => Main.npc[t.npcid].ai[1] = value? 120 : 0;
        }
        public int teleportTimer
        {
            get => (int)Main.npc[t.npcid].ai[1] - 1;
            set => Main.npc[t.npcid].ai[1] = value + 1;
        }
        public Vector2 teleportPosition
        {
            get => new Vector2(Main.npc[t.npcid].ai[2], Main.npc[t.npcid].ai[3]);
            set 
            {
                Main.npc[t.npcid].ai[2] = value.X;
                Main.npc[t.npcid].ai[3] = value.Y;
            } 
        }
        bool oldCollideY;
        UnifiedRandom rand => TGlobalNPC.random;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            //Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX * 2, SlimeWall, 0, 0, -1, 10);
            NextAttack1(npc);
        }
        public override void ModSetDefaults(NPC entity)
        {
            entity.GetT().canselDeathHitEffect = true;
            entity.aiStyle = -1;
            entity.lifeMax = (int)(entity.lifeMax * 1.5f);
        }
        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            if ((npc.ai[0] == 1 && CurentAttack == 4) || CurentAttack == 5)
            {
                return false;
            }
            return t.Target.position.Y > npc.Bottom.Y;
        }
        int SlimeWall => ModContent.ProjectileType<SlimeWall>();
        int SlimeBall => ModContent.ProjectileType<KingSlimeBall>();
        public int SlimeBallDamage = 10;
        public float SlimeBallKnockback = 5.5f;
        int Shuriken => ModContent.ProjectileType<Shuriken>();
        public int ShurikenDamage = 12;
        public float ShurikenKnockBack = 3;
        int Laser => ModContent.ProjectileType<KingSlimeCrownLaser>();
        public int LaserDamage = 15;
        public float LaserKnockBack = 2;
        public static int Slime => ModContent.ProjectileType<SlimeProjectile>();
        public static int SlimeDamage = 15;
        public static float SlimeKnockBack = 4;
        public override void BossHeadSlot(NPC npc, ref int index)
        {
            if (died)
            {
                index = -1;
            }
        }
        public override bool ModPreAI(NPC npc)
        {
            if (died)
            {
                if (NinjaKingSlime != null && NinjaKingSlime.active && NinjaKingSlime.type == ModContent.NPCType<NinjaKingSlime>())
                {
                    npc.Center = NinjaKingSlime.Center;
                }
                else if (CrownedKingSlime != null && CrownedKingSlime.active && CrownedKingSlime.type == ModContent.NPCType<CrownedKingSlime>())
                {
                    npc.Center = CrownedKingSlime.Center;
                }
                else if (KingSlimeCrown != null && KingSlimeCrown.active && KingSlimeCrown.type == ModContent.NPCType<KingSlimeCrown>())
                {
                    npc.Center = KingSlimeCrown.Center;
                }
                else
                {
                    if (kingSlimeCrownKilled && ninjaKingSlimeKilled && crownedKingSlimeKilled)
                    {
                        npc.immortal = false;
                        npc.StrikeInstantKill();
                    }
                    else
                    {
                        npc.active = false;
                    }
                }
            }
            else
            {
                npc.MaxFallSpeedMultiplier = MultipliableFloat.One * 500;
                npc.noTileCollide = false;
                //switch (phase)
                //{
                //    case 0:
                DoFirstPhase(npc);
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
                if (teleporting)
                {
                    int d = Dust.NewDust(teleportPosition, 0, 0, DustID.ShimmerSpark);
                    Main.dust[d].velocity = rand.NextVector2Unit() * (3 + rand.NextFloat(5));
                    teleportTimer--;
                }
                oldCollideY = npc.collideY;
            }
            return false;
        }
        
        void ChillMovement(NPC npc)
        {
            if ((npc.collideY || npc.collideX) && !teleporting)
            {
                npc.velocity = Vector2.Zero;
                if (movementTimer > 0)
                {
                    movementTimer--;
                }
                else
                {
                    npc.velocity.Y = -MathHelper.Clamp((npc.Center.Y - t.Target.Center.Y) / 30, 10, 30);
                    npc.velocity.X = MathHelper.Clamp(MathF.Abs(t.Target.Center.X - npc.Center.X) / 60, 10, 30) * (t.Target.Center.X - npc.Center.X).NonZeroSign();
                }
            }
            else
            {
                float num = WorldDifficultySystem.suicide? 0.8f : 1;
                movementTimer = (int)(((50 * npc.GetLifePercent()) + 20) * num);
            }
            if (npc.Distance(t.Target.Center) > 1500)
            {
                if (!teleporting)
                {
                    teleporting = true;
                    teleportPosition = t.Target.Center + new Vector2((rand.Next() == 0? -1 : 1) * 300, (rand.Next() == 0 ? -1 : 1) * 200);
                    if (!SimpleColision(teleportPosition, t.Target.position, t.Target.width, t.Target.height))
                    {
                        teleportPosition = t.Target.Center;
                    }
                }
                npc.velocity = Vector2.Zero;
                if (teleportTimer == 0)
                {
                    npc.Center = teleportPosition;
                    if (npc.Distance(t.Target.Center) > 500)
                    {
                        npc.velocity = npc.DirectionTo(t.Target.Center) * 20;
                    }
                }
            }
            if (npc.velocity.Y < 0)
            {
                npc.noTileCollide = true;
                npc.collideY = false;
                npc.collideX = false;
            }
        }
        public struct RingOfSlimes
        {
            public bool dealDamage;
            public float Radius;
            public float slimeMaxSpeed;
            public float rotation;
            public List<int> Projectiles;
            public Vector2 Center;
            public int Count => Projectiles.Count;
            /// <summary>
            /// Units not Radians!!!
            /// </summary>
            public float angularVelocity;
            public RingOfSlimes(int count, NPC npc)
            {
                Projectiles = new();
                for (int i = 0; i < count; i++)
                {
                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, Slime, SlimeDamage, SlimeKnockBack);
                    Projectiles.Add(p);
                }
            }
            public void Update()
            {
                rotation += angularVelocity / Radius;
                for(int i = 0; i < Count; i++)
                {
                    Projectile proj = Main.projectile[Projectiles[i]];
                    if (!proj.active || proj.type != Slime)
                    {
                        Projectiles.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        proj.hostile = dealDamage;
                        proj.timeLeft = 2;
                        Vector2 targetPosition = Center + Vector2.UnitX.RotatedBy(rotation + i / (float)Count * MathF.PI * 2) * Radius;
                        float maxVelocity = slimeMaxSpeed;
                        float maxVelocityMultyplier = 1;
                        if (targetPosition != proj.Center)
                        {
                            proj.velocity = proj.DirectionTo(targetPosition) * proj.velocity.Length();
                            proj.velocity += proj.DirectionTo(targetPosition) * 1.2f;
                        }
                        if (proj.Distance(targetPosition) < 75)
                        {
                            maxVelocityMultyplier = 1 - (75 - proj.Distance(targetPosition)) / 75;
                        }
                        if (proj.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                        {
                            proj.velocity = proj.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                        }
                    }
                }
            }
            public void End()
            {
                for (int i = 0; i < Count; i++)
                {
                    Projectile proj = Main.projectile[Projectiles[i]];
                    if (!proj.active || proj.type != Slime)
                    {
                        Projectiles.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        proj.timeLeft = 200;
                        proj.ai[0] = 1;
                        proj.tileCollide = true;
                        proj.velocity += proj.DirectionFrom(Center) * 10;
                    }
                }
            }
        }
        List<RingOfSlimes> rings = new();
        void DoFirstPhase(NPC npc)
        {
            switch(CurentAttack)
            {
                case 0:
                    ChillMovement(npc);
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 1:
                    if (npc.Bottom.Y < t.Target.Top.Y - 150)
                    {
                        npc.noTileCollide = true;
                    }
                    if ((npc.collideY || teleportTimer == 0) && teleporting)
                    {
                        teleportTimer = 80;
                        npc.velocity = Vector2.UnitY * (WorldDifficultySystem.suicide? 22 : 20);
                        int count = WorldDifficultySystem.torture ? 6 : 8;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(0.1f * MathF.PI + (i / (count - 1f)) * 0.8f * MathF.PI) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        npc.Center = teleportPosition;
                        teleporting = false;
                    }
                    else
                    {
                        if (!teleporting)
                        {
                            teleportPosition = t.Target.Center + new Vector2((rand.Next(2) == 0 ? -1 : 1) * 150, -1000);
                            teleportTimer = 100;
                        }
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 2:
                    ChillMovement(npc);
                    npc.velocity.X *= 0.975f;
                    if (timers[0] == 0)
                    {
                        int time1 = WorldDifficultySystem.suicide? 75 : 87;
                        int count = 5;
                        float speed = WorldDifficultySystem.suicide? 18 : 17;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count  * MathF.PI) * 2f + npc.DirectionTo(t.Target.Center) * speed, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        count = WorldDifficultySystem.suicide? 5 : 4;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + npc.DirectionTo(t.Target.Center).RotatedBy(MathF.PI * 0.2f) * speed * 1.25f, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + npc.DirectionTo(t.Target.Center).RotatedBy(MathF.PI * -0.2f) * speed * 1.25f, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        timers[0] = time1;
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 3:
                    int count1 = WorldDifficultySystem.suicide? 5 : 4;
                    if (teleporting || !npc.collideY || npc.ai[0] == 0 || npc.ai[0] == count1)
                    { 
                        ChillMovement(npc); 
                    }
                    if (npc.collideY)
                    {
                        if (npc.ai[0] == 0 && !oldCollideY)
                        {
                            float count = 16;
                            float dir = 1.25f * (WorldDifficultySystem.suicide? 1.05f : 1);
                            float speed = WorldDifficultySystem.suicide? 16.25f : 15;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX.RotatedBy(i / count * 2 * MathF.PI + 1 / count * MathF.PI) * speed, Shuriken, ShurikenDamage, ShurikenKnockBack, -1, dir);
                                dir *= -1;
                            }
                            npc.ai[0] = count1;
                            timers[0] = 60;
                        }
                        else if (npc.ai[0] != 0 && (npc.ai[0] != count1 || !oldCollideY) && timers[0] == 0)
                        {
                            npc.ai[0]--;
                            Vector2 dir = npc.DirectionTo(t.Target.Center);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, dir.RotatedBy(npc.ai[0] / (count1 - 1) * 0.25f * MathF.PI - 0.125f * MathF.PI) * 15, Shuriken, ShurikenDamage, ShurikenKnockBack, -1, (int)npc.ai[0] % 2 == 1? 1 : -1);
                            timers[0] = 10;
                        }
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 4:
                    if (mainTimer == 499)
                    {
                        int dir = WorldDifficultySystem.suicide? (rand.NextBool(2)? 1 : -1) : 1;
                        rings.Add(new RingOfSlimes(50, npc) { angularVelocity = 9 * dir, Center = npc.Center });
                    }
                    RingOfSlimes ring = rings[0];
                    ring.Center = npc.Center;
                    ring.Radius = MathF.Min(ring.Radius + 25, 1000);
                    ring.slimeMaxSpeed = ring.Radius == 1000? 25 : 50;
                    ring.dealDamage = ring.Radius == 1000;
                    ring.Update();
                    rings[0] = ring;

                    if (npc.collideY)
                    {

                        npc.velocity = Vector2.Zero;
                        mainTimer = Math.Min(350, mainTimer);
                        if (npc.ai[0] == 0)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromThis("Main"), npc.Center, npc.DirectionTo(t.Target.Center).RotatedBy((WorldDifficultySystem.suicide? 0.8f : 1) * MathF.PI * -ring.angularVelocity.NonZeroSign()) * 2.3f, Laser, LaserDamage, LaserKnockBack, -1, 0, npc.whoAmI, 0.004f * ring.angularVelocity.NonZeroSign());
                            Main.projectile[proj].timeLeft = mainTimer;

                            npc.ai[0] = 1;
                        }
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 5:
                    int time = 90;
                    if (WorldDifficultySystem.suicide)
                    {
                        ChillMovement(npc);
                    }
                    int num = WorldDifficultySystem.suicide? 2 : 3;
                    if (timers[0] == 0 && npc.ai[0] > num)
                    {
                        if (npc.ai[0] < 5)
                        {
                            foreach(int proj in rings[1].Projectiles)
                            {
                                Main.projectile[proj].active = false;
                            }
                            rings.RemoveAt(1);
                        }
                        List<int> projs = new List<int>();
                        for (int i = (int)(npc.ai[0] / 2); i < rings[0].Count; i += (int)npc.ai[0])
                        {
                            projs.Add(rings[0].Projectiles[i] );
                            rings[0].Projectiles.RemoveAt(i);
                        }
                        npc.ai[0]--;
                        var ring1 = rings[0];
                        ring1.angularVelocity *= -1;
                        rings[0] = ring1;
                        rings.Add(new RingOfSlimes() { rotation = rings[0].Center.DirectionTo(Main.projectile[projs[0]].Center).ToRotation(), Center = rings[0].Center, angularVelocity = rings[0].angularVelocity * -1, Radius = ((npc.ai[0] + 1) / 5 * 1000), slimeMaxSpeed = 50, dealDamage = true, Projectiles = projs });
                        timers[0] = time;
                    }
                    if (timers[0] == 0 && npc.ai[0] == num)
                    {
                        foreach (int proj in rings[1].Projectiles)
                        {
                            Main.projectile[proj].active = false;
                        }
                        rings.RemoveAt(1);
                        npc.ai[0]--;
                    }
                    if (npc.ai[0] < 5 && npc.ai[0] >= num)
                    {
                        var ring2 = rings[1];
                        ring2.Radius = MathF.Max(ring2.Radius - ((npc.ai[0] + 1) / 5 * 1000) / time, 0);
                        rings[1] = ring2;
                        var ring1 = rings[0];
                        ring1.Radius = MathF.Max(npc.ai[0] / 5 * 1000, ring2.Radius);
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
                        foreach (var ring3 in rings)
                        {
                            ring3.End();
                        }
                        rings = new();
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        void NextAttack1(NPC npc)
        {
            attackCounter++;
            if (attackCounter >= phase1.Length)
            {
                attackCounter = 0;
            }

            CurentAttack = phase1[attackCounter];
            switch(CurentAttack)
            {
                case 0:
                    mainTimer = 200;
                    break;
                case 1:
                    mainTimer = 300;
                    break;
                case 2:
                    timers[0] = 100;
                    mainTimer = 450;
                    break;
                case 3:
                    mainTimer = 450;
                    npc.ai[0] = 0;
                    break;
                case 4:
                    npc.ai[0] = 0;
                    mainTimer = 500;
                    break;
                case 5:
                    npc.ai[0] = 5;
                    mainTimer = 250;
                    break;
            }
        }
        int crownedKingSlime;
        NPC CrownedKingSlime
        {
            get {
                if (crownedKingSlime < 0 && crownedKingSlime >= Main.maxNPCs)
                {
                    return null;
                }
                return Main.npc[crownedKingSlime];
            }
        }
        public bool crownedKingSlimeKilled;

        int ninjaKingSlime;    
        NPC NinjaKingSlime
        { 
            get
            {
                if (ninjaKingSlime < 0 && ninjaKingSlime >= Main.maxNPCs)
                {
                    return null;
                }
                return Main.npc[ninjaKingSlime];
            }
        }
        public bool ninjaKingSlimeKilled;

        int kingSlimeCrown;
        NPC KingSlimeCrown
        {
            get
            {
                if (kingSlimeCrown < 0 && kingSlimeCrown >= Main.maxNPCs)
                {
                    return null;
                }
                return Main.npc[kingSlimeCrown];
            }
        }
        public bool kingSlimeCrownKilled;

        bool died;
        public override bool CheckActive(NPC npc)
        {
            return false;
        }
        public override bool CheckDead(NPC npc)
        {
            if (npc.life <= 0 && !died)
            {
                SoundEngine.PlaySound(npc.DeathSound, npc.Center);
                npc.DeathSound = null;
                npc.HitSound = null;
                npc.immortal = true;
                npc.damage = 0;
                npc.alpha = 255;
                npc.life = npc.lifeMax;
                npc.scale = 0;
                died = true;
                foreach (var Ring in rings)
                {
                    Ring.End();
                }
                rings = new();
                ninjaKingSlime = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X + 25, (int)npc.Center.Y, ModContent.NPCType<NinjaKingSlime>());
                Vector2 Center = npc.Top;
                switch (npc.frame.Y)
                {
                    case 0:
                        Center = npc.Top - Vector2.UnitY * 15;
                        break;
                    case 120:
                        Center = npc.Top - Vector2.UnitY * 25;
                        break;
                    case 240:
                        Center = npc.Top - Vector2.UnitY * 15;
                        break;
                    case 360:
                        Center = npc.Top - Vector2.UnitY * 5;
                        break;
                    case 480:
                        Center = npc.Top - Vector2.UnitY * 15;
                        break;
                    case 600:
                        Center = npc.Top - Vector2.UnitY * 17;
                        break;
                }
                kingSlimeCrown = NPC.NewNPC(npc.GetSource_FromThis(), (int)Center.X, (int)Center.Y, ModContent.NPCType<KingSlimeCrown>(), 0, -1);
                crownedKingSlime = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X - 25, (int)npc.Center.Y, ModContent.NPCType<CrownedKingSlime>(), kingSlimeCrown);
                NinjaKingSlime.velocity = new Vector2(10, -4);
                CrownedKingSlime.velocity = new Vector2(-10, -4);
                ((NinjaKingSlime)NinjaKingSlime.ModNPC).crownedKingSlime = crownedKingSlime;
                ((NinjaKingSlime)NinjaKingSlime.ModNPC).kingSlimeCrown = kingSlimeCrown;
                ((NinjaKingSlime)NinjaKingSlime.ModNPC).kingSlime = npc.whoAmI;
                ((CrownedKingSlime)CrownedKingSlime.ModNPC).ninjaKingSlime = ninjaKingSlime;
                ((CrownedKingSlime)CrownedKingSlime.ModNPC).kingSlimeCrown = kingSlimeCrown;
                ((CrownedKingSlime)CrownedKingSlime.ModNPC).kingSlime = npc.whoAmI;
                ((KingSlimeCrown)KingSlimeCrown.ModNPC).crownedKingSlime = crownedKingSlime;
                ((KingSlimeCrown)KingSlimeCrown.ModNPC).ninjaKingSlime = ninjaKingSlime;
                ((KingSlimeCrown)KingSlimeCrown.ModNPC).kingSlime = npc.whoAmI;
                return false;
            }
            return true;
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return !died ? null : false;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return !died;
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return !died ? null : false;
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !died;
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            foreach (var loot in npcLoot.Get(false))
            {
                if (loot is ItemDropWithConditionRule)
                {
                    if (((ItemDropWithConditionRule)loot).itemId == ItemID.KingSlimeTrophy)
                    {
                        npcLoot.Remove(loot);
                        break;
                    }
                }
            }
            LeadingConditionRule suicide = new(new SuicideDropRule());
            suicide.OnSuccess(new DropOneByOne(ItemID.KingSlimeTrophy, Terrapain.SuicideTrophyDropParameters));
            npcLoot.Add(suicide);

            LeadingConditionRule notSuicide = new(new NotSuicideDropRule());
            notSuicide.OnSuccess(new DropOneByOne(ItemID.KingSlimeTrophy, Terrapain.NormalTrophyDropParameters));
            npcLoot.Add(notSuicide);

            LeadingConditionRule Torture = new(new TortureDropRule());
            Torture.OnSuccess(new DropOneByOne(4929 /*King slime relic*/, Terrapain.SuicideTrophyDropParameters));
            npcLoot.Add(Torture);
        }
        public override void OnKill(NPC npc)
        {
            BossDownedSystem.BossDowned(0);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (/*phase == 0 &&*/ CurentAttack == 1 && MathF.Abs(npc.velocity.X) < 0.05f && !died)
            {
                Rectangle rectangle = new Rectangle((int)npc.Center.X - 45 - (int)screenPos.X, 0, 90, Main.screenHeight);
                spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rectangle, Color.Blue * 0.5f);
            }
            return !died;
        }
    }
}
