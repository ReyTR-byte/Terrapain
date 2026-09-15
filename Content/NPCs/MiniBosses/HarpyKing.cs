using Luminance.Common.Utilities;
using ReLogic.Reflection;
using Terrapain.Common.Global;
using Terrapain.Content.Buffs;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.TUtilities.AIHelper;

namespace Terrapain.Content.NPCs.MiniBosses
{
    public class HarpyKing : ModNPC
    {
        int mainTimer;
        int timer;
        int attack;
        int nextAttack = 1;
        int phase = 1;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<ScorspiderAcid>()] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
            {
                PortraitScale = 0.6f,
                PortraitPositionYOverride = 0f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
        }
        public override void SetDefaults()
        {

            NPC.width = 40;
            NPC.height = 26;

            NPC.damage = 20;
            NPC.defense = 10;

            NPC.lifeMax = 2500;

            NPC.knockBackResist = 0f;

            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.value = 75000;

            NPC.noTileCollide = false;
            NPC.noGravity = true;

            NPC.aiStyle = -1;
            NPC.stairFall = true;
            NPC.HitSound = SoundID.NPCHit4;
            AIType = -1;
        }
        public override void AI()
        {
            if (CheckTarget())
            {
                NPC.TargetClosest();
                FlyAway();
            }
            else
            {
                Movement();
                Attack();
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        bool CheckTarget()
        {
            if (NPC.target < 0)
            {
                return true;
            }
            Player Target = Main.player[NPC.target];
            if (!Target.active || Target.dead || Target.Distance(NPC.Center) > 2500)
            {
                return true;
            }
            return false;
        }
        void FlyAway()
        {
            NPC.TargetClosest();
            var scan = NPC.ScanAround(5);
            if ((NPC.ai[0] != 2 && NPC.ai[0] != 3) || NPC.ai[2] == 0)
            {
                NPC.knockBackResist = 0.1f;
                var Points = NPC.FindPath(NPC.Center - Vector2.UnitY * 500, 50);
                if (Points != null && Points.Count > 1)
                {
                    CommonTerrapainFlyingMovement(NPC, Points[0], 0.3f, 5, 0.3f, 0f, false);
                    WallsAvoidMovement(NPC, scan, 0.2f, 0.3f);
                }
                else
                {
                    CommonTerrapainFlyingMovement(NPC, NPC.Center - Vector2.UnitY * 500, 0.3f, 5, 0.3f, 150f, true);
                }
            }
            else
            {
                NPC.knockBackResist = 0f;
                NPC.velocity = NPC.velocity.Normalized() * Math.Max(5, NPC.velocity.Length() - 0.2f);
                WallsAvoidMovement(NPC, scan, 0.2f, 0.3f);
            }
        }
        void Movement()
        {
            NPC.TargetClosest();
            Player Target = Main.player[NPC.target];
            var scan = NPC.ScanAround(5);
            if ((NPC.ai[0] != 2 && NPC.ai[0] != 3) || NPC.ai[2] == 0)
            {
                NPC.knockBackResist = 0.1f;
                var Points = NPC.FindPath(Target.Center, 50);
                if (Points != null && Points.Count > 1)
                {
                    CommonTerrapainFlyingMovement(NPC, Points[0], 0.3f, 5, 0.3f, 0f, false);
                    WallsAvoidMovement(NPC, scan, 0.2f, 0.3f);
                }
                else
                {
                    CommonTerrapainFlyingMovement(NPC, Target.Center + NPC.DirectionFrom(Target.Center) * 150, 0.3f, 5, 0.3f, 150f, true);
                }
            }
            else
            {
                NPC.knockBackResist = 0f;
                NPC.velocity = NPC.velocity.Normalized() * Math.Max(5, NPC.velocity.Length() - 0.2f);
                WallsAvoidMovement(NPC, scan, 0.2f, 0.3f);
            }
        }
        int proj => ModContent.ProjectileType<HarpyFeatherHostile>();
        int dmg => 10;
        int knb = 3;
        Vector2 savedVector;
        void Attack()
        {
            if (phase == 1)
            {    
                switch (attack)
                {
                    case 0:
                        mainTimer--;
                        if (mainTimer <= 0)
                        {
                            attack = nextAttack;
                            mainTimer = 600;
                            if (attack == 3)
                            {
                                timer = 120;
                            }
                            phase = NPC.life < 0.6f * NPC.lifeMax? 2 : 1;
                        }
                        break;
                    case 1:
                        float Progress = 1 - mainTimer / 600f;
                        if (timer <= 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis("0"), NPC.Center, TGlobalNPC.random.NextVector2Unit() * 12, proj, dmg, knb, -1, 120 - (int)(Progress * 30), NPC.target);
                            timer = 25 - (int)(Progress * 5);
                        }
                        mainTimer--;
                        timer--;
                        if (mainTimer <= 0)
                        {
                            nextAttack = TGlobalNPC.random.Next(2, 4);
                            attack = 0;
                            phase = NPC.life < 0.6f * NPC.lifeMax? 2 : 1;
                            mainTimer = 60;
                        }
                        break;
                    case 2:
                        Progress = 1 - mainTimer / 600f;
                        if (timer <= 0)
                        {
                            Player player = Main.player[NPC.target];
                            Vector2 pos = player.Center;
                            pos += Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(500, 700);
                            if (Collision.CanHit(pos - new Vector2(4), 8, 8, player.position, player.width, player.height))
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    Main.dust[Dust.NewDust(pos, 0, 0, DustID.PurpleTorch)].velocity = Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(5, 10);
                                }
                                Projectile.NewProjectile(NPC.GetSource_FromThis("0"), pos, TGlobalNPC.random.NextVector2Unit() * 13, proj, dmg, knb, -1, 120 - (int)(Progress * 30), NPC.target);
                            }
                            timer = 30 - (int)(Progress * 20);
                        }
                        mainTimer--;
                        timer--;
                        if (mainTimer <= 0)
                        {
                            nextAttack = TGlobalNPC.random.Next(2, 4);
                            if (nextAttack == 2)
                            {
                                nextAttack = 1;
                            }
                            attack = 0;
                            phase = NPC.life < 0.6f * NPC.lifeMax? 2 : 1;
                            mainTimer = 60;
                        }
                        break;
                    case 3:
                        {    
                            Progress = 1 - mainTimer / 600f;
                            Player player = Main.player[NPC.target];
                            Vector2 vel = Vector2.Zero;
                            for (int i = 0; i < 25; i++)
                            {
                                vel += player.Custom().oldVelocities[i];
                            }
                            Vector2 pos = player.Center + vel * 3;
                            if (timer <= 0)
                            {
                                for (int i = 0; i < 25; i++)
                                {
                                    Main.dust[Dust.NewDust(pos, 0, 0, DustID.PurpleTorch)].velocity = Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(10, 15);
                                }
                                int count = 12;
                                float agleBetween = MathF.PI / count * 2;
                                float startAngle = TGlobalNPC.random.NextFloat(agleBetween);
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis("2"), savedVector, Vector2.UnitX.RotatedBy(agleBetween * i + startAngle) * 12, proj, dmg, knb, -1, NPC.target);
                                }
                                timer = 120 - (int)(Progress * 40);
                            }
                            else
                            {
                                if (timer == 60)
                                {
                                    savedVector = pos;
                                }
                                if (timer < 60)
                                {
                                    for (int i = 0; i < 6; i++)
                                    {
                                        Main.dust[Dust.NewDust(savedVector, 0, 0, DustID.PurpleTorch)].velocity = Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(8, 11);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < 6; i++)
                                    {
                                        Main.dust[Dust.NewDust(pos, 0, 0, DustID.PurpleTorch)].velocity = Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(8, 11);
                                    }
                                }
                            }
                            mainTimer--;
                            timer--;
                            if (mainTimer <= 0)
                            {
                                nextAttack = TGlobalNPC.random.Next(1, 3);
                                if (nextAttack == 2)
                                {
                                    nextAttack = 1;
                                }
                                attack = 0;
                                phase = NPC.life < 0.6f * NPC.lifeMax? 2 : 1;
                                mainTimer = 60;
                            }
                        }
                    break;
                }
            }
            else
            {
                switch (attack)
                {
                    case 0:
                        mainTimer--;
                        if (mainTimer <= 0)
                        {
                            attack = nextAttack;
                            mainTimer = 600;
                        }
                        break;
                    case 1:
                        float Progress = 1 - mainTimer / 600f;
                        if (timer <= 0)
                        {
                            int count = 6;
                            float agleBetween = MathF.PI / count * 2;
                            float startAngle = TGlobalNPC.random.NextFloat(agleBetween);
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis("0"), NPC.Center, Vector2.UnitX.RotatedBy(agleBetween * i + startAngle) * 12, proj, dmg, knb, -1, 80 - (int)(Progress * 20), NPC.target);
                            }
                            timer = 90 - (int)(Progress * 20);
                        }
                        mainTimer--;
                        timer--;
                        if (mainTimer <= 0)
                        {
                            nextAttack = TGlobalNPC.random.Next(2, 4);
                            attack = 0;
                            mainTimer = 60;
                        }
                        break;
                    case 2:
                        Progress = 1 - mainTimer / 600f;
                        if (timer <= 0)
                        {
                            Player player = Main.player[NPC.target];
                            Vector2 pos = player.Center;
                            pos += Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(500, 700);
                            if (Collision.CanHit(pos - new Vector2(4), 8, 8, player.position, player.width, player.height))
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    Main.dust[Dust.NewDust(pos, 0, 0, DustID.PurpleTorch)].velocity = Vector2.UnitX.RotatedBy(TGlobalNPC.random.NextFloat(MathF.PI * 2)) * TGlobalNPC.random.NextFloat(5, 10);
                                }
                                Projectile.NewProjectile(NPC.GetSource_FromThis("2"), pos, TGlobalNPC.random.NextVector2Unit() * 13, proj, dmg, knb, -1, NPC.target);
                            }
                            timer = 30 - (int)(Progress * 20);
                        }
                        mainTimer--;
                        timer--;
                        if (mainTimer <= 0)
                        {
                            nextAttack = TGlobalNPC.random.Next(2, 4);
                            if (nextAttack == 2)
                            {
                                nextAttack = 1;
                            }
                            attack = 0;
                            mainTimer = 60;
                        }
                        break;
                    case 3:
                        {
                            Player player = Main.player[NPC.target];
                            if (timer <= 0)
                            {
                                int count = 12;
                                float agleBetween = MathF.PI / count * 2;
                                float startAngle = TGlobalNPC.random.NextFloat(agleBetween);
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis("3"), player.Center + Vector2.UnitX.RotatedBy(startAngle + agleBetween * i) * 500, new Vector2(18, 0), proj, dmg, knb, -1, 120, NPC.target);
                                }
                                timer = 120;
                            }
                            mainTimer--;
                            timer--;
                            if (mainTimer <= 0)
                            {
                                mainTimer = 120;
                                nextAttack = TGlobalNPC.random.Next(1, 3);
                                if (nextAttack == 2)
                                {
                                    nextAttack = 1;
                                }
                                attack = 0;
                            }
                        }
                    break;
                }
            }
        }
    }
}