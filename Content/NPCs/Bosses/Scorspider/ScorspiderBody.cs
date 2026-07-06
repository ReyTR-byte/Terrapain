using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.Items.Bags;
using Terrapain.Content.Items.DropRulls;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Content.Items.Placeable.Relics;
using Terrapain.Content.Items.Placeable.Trophies;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.TUtilities.Kinematic;
using ReLogic.Content;
using static Terrapain.Content.Functions;
using Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider;
using Terrapain.Common.Global;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    [AutoloadBossHead]
    public class ScorspiderBody : ModNPC
    {
        //перейди на 355 строчку
        int head;
        int sting;
        public float angularVelocity;

        int mainTimer;
        int timer;
        int attackCounter;
        int attack;
        int phase = 1;
        int[] attacks1 = [0, 1, 0, 2, 0, 3];
        int[] attacks2 = [0, 1, 0, 2, 0, 3];

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
            Legs = new ScorspiderLeg[8];
            for (int i = 0; i < 8; i++)
            {
                Legs[i] = new ScorspiderLeg(new Vector2(70 * (i % 4 > 1? -1 : 1), -50), 1, 100, 150, i, 60);
            }
            LegBraces = new Vector2[8];
            for (int i = 0; i < 8; i++)
            {
                LegBraces[i] = new Vector2(60 - 40 * (i % 4), i % 4 == 1 || i % 4 == 2? 30 : 20);
            }

            NPC.width = 80;
            NPC.height = 80;

            NPC.damage = 20;
            NPC.defense = 20;

            NPC.lifeMax = 6500;

            NPC.knockBackResist = 0f;

            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.value = 750000;

            NPC.noTileCollide = false;
            NPC.noGravity = false;

            NPC.aiStyle = -1;
            NPC.stairFall = true;
            NPC.HitSound = SoundID.NPCHit4;
            AIType = -1;
        }
        SimulatedChain tail;
        Asset<Texture2D> TailTexture;
        Vector2 tailPosition => NPC.Center + new Vector2(60 * NPC.spriteDirection, 0).RotatedBy(NPC.rotation);
        public Vector2 HeadPosition => NPC.Center + new Vector2(-70 * NPC.spriteDirection, 5).RotatedBy(NPC.rotation);
        public Vector2 StingPosition => tailPosition + new Vector2(100 * NPC.spriteDirection, -40);
        public override void OnSpawn(IEntitySource source)
        {
            head = NPC.NewNPC(source, (int)HeadPosition.X, (int)HeadPosition.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI,  NPC.whoAmI);
            sting = NPC.NewNPC(source, (int)StingPosition.X, (int)StingPosition.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, NPC.whoAmI, head);
            Main.npc[head].ai[1] = sting;
            tail = new SimulatedChain(8, 26, tailPosition, 0, 1);

            TailTexture = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/Scorspider/ScorspiderTail");
            ShaderSystem.ScorspiderTimer = 20;
            ShaderSystem.drawScorspiderBorders = false;
            ShaderSystem.ScorspiderAuraTimer = 20;
            ShaderSystem.drawScorspiderAura = false;
            //head = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI);
            //sting = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, head);
            //Main.npc[head].ai[1] = sting;

            if (Main.GameMode != 3)
            {
                NPC.lifeMax = 6500 * (Main.GameMode + 3) / 3;
                NPC.life = 6500 * (Main.GameMode + 3) / 3;
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Ingredients.ScorspiderShellShard>(), 1, 15, 20));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Ingredients.ScorspiderCobweb>(), 1, 15, 20));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Tools.ScorspiderHook>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Accessories.ScorspiderHeartAccesory>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Weapons.MeleeWeapons.Sharper>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Weapons.MagicWeapons.GranithBook>(), 3));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ScorspiderBag>()));

            LeadingConditionRule suicide = new(new SuicideDropRule());
            suicide.OnSuccess(new DropOneByOne(ModContent.ItemType<ScorspiderTrophy>(), Terrapain.SuicideTrophyDropParameters));
            npcLoot.Add(suicide);

            LeadingConditionRule notSuicide = new(new NotSuicideDropRule());
            notSuicide.OnSuccess(new DropOneByOne(ModContent.ItemType<ScorspiderTrophy>(), Terrapain.NormalTrophyDropParameters));
            npcLoot.Add(notSuicide);

            LeadingConditionRule masterOrTorture = new(new MasterOrTortureDropRule());
            masterOrTorture.OnSuccess(new DropOneByOne(ModContent.ItemType<ScorspiderRelic>(), Terrapain.SuicideTrophyDropParameters));
            npcLoot.Add(masterOrTorture);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (timer == 0 && phase == 2 && attack == 3)
            {
                return NPC.GetT().Target.Center.Y > NPC.Center.Y;
            }
            return NPC.GetT().Target.Center.Y - NPC.Center.Y > 150;
        }
        public ScorspiderLeg[] Legs;
        public Vector2[] LegBraces;

        public static Vector2 FindGround(Vector2 position, Vector2 direction, NPC npc, out bool findGround)
        {
            Vector2 end = position + direction * 230;
            Vector2 result = RayColisionInTheWorld(position, end, !NPCLoader.CanFallThroughPlatforms(npc)?? true);
            findGround = result != Vector2.Zero && result != position;
            return findGround? result : Vector2.Zero;
        }

        int Spike => ModContent.ProjectileType<ScorspiderSpike>();
        int SpikeDamage = 18;
        float SpikeKnockback = 6;

        int Flower => ModContent.ProjectileType<ScorspiderFlower>();

        int Rocket => ModContent.ProjectileType<ScorspiderRocket>();
        int RocketDamage = 21;
        float RocketKnockback = 8.5f;

        public override void AI()
        {
            NPC.noGravity = dash;
            maxSpeed = MathF.Max(6, MathF.Abs(NPC.Center.X - NPC.GetT().Target.Center.X) / 70);
            NPC.TargetClosest();
            foreach (var leg in Legs)
            {
                leg.Update(NPC);
            }
            switch (phase)
            {
                case 1:
                    DoFirstPhase();
                    break;
                case 2:
                    DoSecondPhase();
                    break;
            }
            if (timer > 0)
            {
                timer--;
            }
            if (mainTimer > 0)
            {
                mainTimer--;
            }
            UpdateMovement();
            UpdateBody();
        }
        bool jumpAnimation;
        float targetHeight = 120;
        float maxSpeed;
        bool dash;
        void UpdateMovement()
        {
            if (NPC.Distance(NPC.GetT().Target.Center) > 1500)
            {
                NPC.Center = NPC.GetT().Target.Center - Vector2.UnitY * 1100;
                NPC.velocity = new Vector2(NPC.GetT().Target.velocity.X, 3);
            }
            Vector2 dir = NPC.DirectionTo(NPC.GetT().Target.Center);
            float targetRotation = 0;
            Vector2 averagePosition = Vector2.Zero;
            Vector2 mostRight = Vector2.Zero;
            Vector2 mostLeft = Vector2.Zero;
            int grounded = 0;
            for (int i = 0; i < 8; i++)
            {
                if (Legs[i].Grounded)
                {
                    if (i % 4 < 2)
                    {
                        if (mostRight == Vector2.Zero || mostRight.X < Legs[i].Leg.EndEffectorPosition.X)
                        {
                            mostRight = Legs[i].Leg.EndEffectorPosition;
                        }
                    }
                    else
                    {
                        if (mostLeft == Vector2.Zero || mostLeft.X > Legs[i].Leg.EndEffectorPosition.X)
                        {
                            mostLeft = Legs[i].Leg.EndEffectorPosition;
                        }
                    }
                    grounded++;
                    averagePosition += Legs[i].Leg.EndEffectorPosition;
                }
            }
            if (grounded > 1)
            {
                if (targetHeight > 60)
                {
                    Vector2 rotatedVelocity = NPC.velocity;//.RotatedBy(-NPC.rotation);
                    averagePosition /= grounded;
                    averagePosition -= Vector2.UnitY * targetHeight;
                    rotatedVelocity.Y += (averagePosition.Y - NPC.Center.Y).NonZeroSign() * 2f;
                    rotatedVelocity.Y = MathHelper.Clamp(rotatedVelocity.Y, averagePosition.Y - NPC.Center.Y, -averagePosition.Y + NPC.Center.Y);
                    NPC.velocity = rotatedVelocity;//.RotatedBy(NPC.rotation);
                }
                if (mostRight == Vector2.Zero)
                {
                    targetRotation = MathF.PI / 2.2f;
                }
                else if (mostLeft == Vector2.Zero)
                {
                    targetRotation = -MathF.PI / 2.2f;
                }
                else
                {
                    targetRotation = mostLeft.DirectionTo(mostRight).ToRotation();
                }
                if (!jumpAnimation && (NPC.Center.Y - NPC.GetT().Target.Center.Y > 150 || FindHole() || FindWall()))
                {
                    jumpAnimation = true;
                }
                if (jumpAnimation && !dash)
                {
                    targetHeight -= MathF.Max(5, NPC.velocity.X * 1.2f);
                    if (targetHeight < 60)
                    {
                        NPC.velocity.Y = MathF.Max(NPC.velocity.Y - 5, -15);
                        NPC.velocity.X = maxSpeed * -NPC.spriteDirection;
                    }
                }
                else
                {
                    targetHeight = 150;
                }
                if (!dash)
                {
                    NPC.velocity.X += dir.X.NonZeroSign() * 0.3f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
                }
            }
            else
            {
                jumpAnimation = false;
            }
            if (dash)
            {
                NPC.spriteDirection = NPC.velocity.X.NonZeroSign() * -1;
                NPC.velocity.Y += 0.05f;
                NPC.velocity.X *= 0.99f;
                if (MathF.Abs(NPC.velocity.X) < maxSpeed)
                {
                    dash = false;
                }
            }
            else
            {
                NPC.spriteDirection = dir.X.NonZeroSign() * -1;
            }
            float rotationToTarget = dir.ToRotation();
            float realRotation = NPC.rotation + (NPC.spriteDirection == -1? 0 : MathF.PI);
            float rot = NormalizeRotation(rotationToTarget - realRotation, false);
            if (rot < MathF.PI / 2 && rot > -MathF.PI / 2)
            {
                targetRotation = MathHelper.Clamp(rot, -0.12f, 0.12f);
            }
            AngularAcceleration(ref angularVelocity, 0.02f, 0.08f, targetRotation, ref NPC.rotation);
        }
        void UpdateBody()
        {
            Main.npc[sting].Center = StingPosition;
            tail.Fragments[0].backwardRotation = NPC.rotation + (NPC.direction == 1? MathF.PI : 0);
            tail.Fragments[0].fixedAt = tailPosition;
            tail.Fragments[tail.Fragments.Length - 1].fixedAt = Main.npc[sting].Center + (Main.npc[sting].rotation + MathF.PI / 2 * Main.npc[sting].spriteDirection).ToRotationVector2() * 10 * Main.npc[sting].spriteDirection;
            tail.Fragments[tail.Fragments.Length - 1].forwardRotation = Main.npc[sting].rotation + MathF.PI / 2 * Main.npc[sting].spriteDirection;
            tail.Update();
            //if (Main.mouseLeft && Main.MouseWorld.X > NPC.position.X && Main.MouseWorld.X < NPC.TopRight.X && Main.MouseWorld.Y > NPC.position.Y && Main.MouseWorld.Y < NPC.BottomLeft.Y)
            //{
            //    NPC.velocity = Main.MouseWorld - NPC.Center;
            //    NPC.direction = NPC.velocity.X.NonZeroSign();
            //}
        }
        bool FindHole()
        {
            bool value1 = false;
            bool value2 = false;
            bool value3 = false;
            int dir = NPC.velocity.X.NonZeroSign();
            FindGround(Legs[dir == 1? 0 : 3].DefaultPosition(this), new Vector2(dir * 0.8f, 1.2f), NPC, out value1);
            //Dust.NewDust(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), 0, 0, DustID.Torch);
            //Dust.NewDust(Legs[dir == 1 ? 0 : 3].DefaultPosition(this) + new Vector2(dir * 0.8f, 1.2f) * 220, 0, 0, DustID.Torch);
            FindGround(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), new Vector2(dir * 0.25f, 1f), NPC, out value2);
            //Dust.NewDust(Legs[dir == 1 ? 0 : 3].DefaultPosition(this) + new Vector2(dir * 0.25f, 1f) * 220, 0, 0, DustID.Torch);
            //FindGround(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), new Vector2(dir * 1.5f, 1.5f), NPC, out value2);
            //Dust.NewDust(Legs[dir == 1 ? 0 : 3].DefaultPosition(this) + new Vector2(dir * 1.5f, 1.5f) * 220, 0, 0, DustID.Torch);
            return !(value1 || value2 || value3);
        }
        bool FindWall()
        {
            bool value1 = false;
            bool value2 = false;
            bool value3 = false;
            int dir = NPC.velocity.X.NonZeroSign();
            FindGround(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), new Vector2(dir * 3, -0.5f).RotatedBy(NPC.rotation), NPC, out value1);
            FindGround(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), new Vector2(dir * 3, 0).RotatedBy(NPC.rotation), NPC, out value2);
            FindGround(Legs[dir == 1 ? 0 : 3].DefaultPosition(this), new Vector2(dir * 3, 0.5f).RotatedBy(NPC.rotation), NPC, out value3);
            return value1 && value2 && value3;
        }
        void DoFirstPhase()
        {
            // если хочешь добавить разницу между пыткой и самоубийством => переменная = WorldDifficultySystem.Suicide? значение для самоубийства : значение для пытки;
            // если хочешь добавить рандома используй TGlobalNPC.random
            // если не понимаешь где какая атака поставь тут true
            bool chat = false;
            switch (attack)
            {
                case 0:
                    if (chat)
                    {
                        Chatic(0);
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
                case 1:
                    if (chat)
                    {
                        Chatic(1);
                    }
                    float progress = (300 - mainTimer) / 300f;
                    NPC Sting = Main.npc[sting];
                    Sting.ai[3] = 1;

                    //чем больше это число тем больше скорость вращения жала
                    float rotationSpeed = 22;
                    float value = progress * rotationSpeed % 4;
                    if (value > 1)
                    {
                        value = 2 - value;
                    }
                    if (value < -1)
                    {
                        value = -2 - value;
                    }

                    //чем больше это число тем больше угол вращения жала
                    float rotationRange = 0.666f;
                    Sting.rotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation() + MathF.Asin(value) * rotationRange;
                    if (timer == 0)
                    {
                        //это скорость вылета шипов
                        float speed = 25;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.Center, Sting.rotation.ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback);
                        //это чвстота вылета шипов в тиках
                        timer = 10;
                    }
                    if (Sting.rotation > MathF.PI / 2 || Sting.rotation < -MathF.PI / 2)
                    {
                        Sting.rotation += MathF.PI;
                        Sting.spriteDirection = -1;
                    }
                    else
                    {
                        Sting.spriteDirection = 1;
                    }
                    if (mainTimer == 0)
                    {
                        Sting.ai[3] = 0;
                        NextAttack1();
                    }
                    break;
                case 2:
                    if (chat)
                    {
                        Chatic(2);
                    }
                    if (timer == 0)
                    {
                        Sting = Main.npc[sting];
                        float rotation = Sting.DirectionTo(NPC.GetT().Target.Center).ToRotation();
                        if (mainTimer > 240)
                        {
                            if (chat)
                            {
                                Chatic("података 1");
                            }
                            float speed = 20;
                            //количество шипов за 1 выстрел
                            int count = 4;
                            //угол между ними в радианах
                            //если не понимаешь в радианах в градусах будет так 
                            //MathHelper.ToRadians(угол);
                            float angle = 0.198f;
                            //не трогай
                            float start = rotation - (count - 1) / 2 * angle;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.Center, (start + angle * i).ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback, -1, 0, 0, -1);
                            }
                            timer = 30;
                        }
                        else if (mainTimer > 120)
                        {
                            if (chat)
                            {
                                Chatic("података 2");
                            }
                            float speed = 27;
                            //размах рандомного угла
                            float range = 0.5f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.Center, (rotation + TGlobalNPC.random.NextFloat(-range, range)).ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback, -1, 0, 0, -1);
                            timer = 7;
                        }
                        else
                        {
                            if (chat)
                            {
                                Chatic("података 3");
                            }
                            //начальная скорость вылета ракеты, она потом разгоняется сильно
                            float speed = 3;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.Center, rotation.ToRotationVector2() * speed, Rocket, RocketDamage, RocketKnockback);
                            timer = 25;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
                case 3:
                    if (chat)
                    {
                        Chatic(3);
                    }
                    if (mainTimer == 349)
                    {
                        //скорость деша
                        float speed = 30;
                        NPC.velocity.X = NPC.DirectionTo(NPC.GetT().Target.Center).X.NonZeroSign() * speed;
                        NPC.velocity.Y = 0;
                        dash = true;
                    }
                    progress = (400 - mainTimer) / 400f;
                    if (mainTimer > 250 && timer == 0)
                    {
                        //количестро шипов которые вылетят из цветка
                        int count = 9;
                        //эти летят вверх
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -15, Flower, SpikeDamage, SpikeKnockback, -1, count, 1, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                        //эти летят вниз
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * 15, Flower, SpikeDamage, SpikeKnockback, -1, count, 1, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                        timer = 30;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
            }
        }
        void DoSecondPhase()
        {
            bool chat = false;
            switch (attack)
            {
                case 0:
                    if (chat)
                    {
                        Chatic(0);
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack2();
                    }
                    break;
                case 1:
                    //тут все тоже самое что и с 1 атакой 1 фазы
                    if (chat)
                    {
                        Chatic(1);
                    }
                    float progress = (300 - mainTimer) / 300f;
                    NPC Sting = Main.npc[sting];
                    Sting.ai[3] = 1;

                    float rotationSpeed = 22;
                    float value = progress * rotationSpeed % 4;
                    if (value > 1)
                    {
                        value = 2 - value;
                    }
                    if (value < -1)
                    {
                        value = -2 - value;
                    }

                    float rotationRange = 0.666f;
                    Sting.rotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation() + MathF.Asin(value) * rotationRange;
                    if (timer == 0)
                    {
                        float speed = 25;
                        Projectile.NewProjectile(Sting.GetSource_FromThis(), Sting.Center, Sting.rotation.ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback, -1, 0, 0, 5);
                        timer = 13;
                    }
                    if (Sting.rotation > MathF.PI / 2 || Sting.rotation < -MathF.PI / 2)
                    {
                        Sting.rotation += MathF.PI;
                        Sting.spriteDirection = -1;
                    }
                    else
                    {
                        Sting.spriteDirection = 1;
                    }
                    if (mainTimer == 0)
                    {
                        Sting.ai[3] = 0;
                        NextAttack2();
                    }
                    break;
                case 2:
                    if (chat)
                    {
                        Chatic(2);
                    }
                    if (mainTimer == 499 || NPC.Distance(NPC.GetT().Target.Center) > 1000)
                    {
                        //скорость деша
                        float speed = 30;
                        NPC.velocity.X = NPC.DirectionTo(NPC.GetT().Target.Center).X.NonZeroSign() * speed;
                        NPC.velocity.Y = 0;
                        dash = true;
                    }
                    progress = (400 - mainTimer) / 400f;
                    if (mainTimer > 250 && timer == 0)
                    {
                        if (NPC.ai[3] == 0)
                        {
                            //количество шипов из цветка
                            int count = 8;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -15, Flower, SpikeDamage, SpikeKnockback, -1, count, 0, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * 15, Flower, SpikeDamage, SpikeKnockback, -1, count, 0, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                            //время до атаки рокетой
                            timer = 35;
                            NPC.ai[3] = 1;
                        }
                        else
                        {
                            //атака рокетой
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -1, Rocket, RocketDamage, RocketKnockback);
                            //время до атаки цветком
                            timer = 40;
                            NPC.ai[3] = 0;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
                case 3:
                    if (chat)
                    {
                        Chatic(3);
                    }
                    if (NPC.Center.Y > NPC.GetT().Target.Center.Y && timer == 0)
                    {
                        jumpAnimation = true;
                    }
                    if (timer == 0 && MathF.Abs(NPC.Center.Y - NPC.GetT().Target.Center.Y) < 10)
                    {
                        //скорость деша
                        float speed = 30;
                        NPC.velocity.X = NPC.DirectionTo(NPC.GetT().Target.Center).X.NonZeroSign() * speed;
                        NPC.velocity.Y = 0;
                        dash = true;
                        timer = 120;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
            }
        }
        void NextAttack1()
        {
            if (CheckPhase())
            {
                return;
            }
            attackCounter++;
            if (attackCounter > attacks1.Length - 1)
            {
                attackCounter = 0;
            }
            attack = attacks1[attackCounter];
            switch (attack)
            {
                case 0:
                    mainTimer = 120;
                    break;
                case 1:
                    mainTimer = 300;
                    break;
                case 2:
                    mainTimer = 360;
                    break;
                case 3:
                    timer = 30;
                    mainTimer = 350;
                    break;
            }
        }
        void NextAttack2()
        {
            attackCounter++;
            if (attackCounter > attacks2.Length - 1)
            {
                attackCounter = 0;
            }
            attack = attacks2[attackCounter];
            switch (attack)
            {
                case 0:
                    mainTimer = 120;
                    break;
                case 1:
                    mainTimer = 300;
                    break;
                case 2:
                    mainTimer = 500;
                    NPC.ai[3] = 0;
                    break;
                case 3:
                    timer = 60;
                    mainTimer = 700;
                    break;
            }
        }
        bool CheckPhase()
        {
            if (phase == 1 && NPC.life < NPC.lifeMax * 0.6f)
            {
                phase = 2;
                attackCounter = -1;
                return true;
            }
            return false;
        }
        
        public override void HitEffect(NPC.HitInfo hit)
        {
            NPC.immortal = false;
            //if (NPC.life - hit.Damage <= 0 && (state == 0 || secondPhase))
            //{
            //    NPC.life = 1;
            //    NPC.immortal = true;
            //    Main.npc[head].life = 1;
            //    Main.npc[head].immortal = true;
            //    Main.npc[sting].life = 1;
            //    Main.npc[sting].immortal = true;
            //}
            //else
            //{
            //    if (NPC.life < hit.Damage)
            //    {
            //        hit.HideCombatText = true;
            //        Main.npc[head].StrikeNPC(hit);
            //        Main.npc[sting].StrikeNPC(hit);
            //    }
            //    Main.npc[head].life -= hit.Damage;
            //    Main.npc[sting].life -= hit.Damage;
            //}
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderBody_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderBody_1").Type;
            int thirdGoreType = Mod.Find<ModGore>("ScorspiderBody_2").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), thirdGoreType);

            int tailFirstGoreType = Mod.Find<ModGore>("ScorspiderTail_0").Type;
            int tailSecondGoreType = Mod.Find<ModGore>("ScorspiderTail_1").Type;

            //Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailFirstGoreType);
            //Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailSecondGoreType);

            int legFirstGoreType = Mod.Find<ModGore>("ScorspiderLeg_0").Type;
            int legSecondGoreType = Mod.Find<ModGore>("ScorspiderLeg_1").Type;

            for (int i = 0; i < tail.Count - 1; i++)
            {
                Vector2 position = (tail.Fragments[i].position + tail.Fragments[i + 1].position) / 2;
                Gore.NewGore(entitySource, position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailFirstGoreType);
                Gore.NewGore(entitySource, position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailSecondGoreType);
            }

            Main.npc[head].ModNPC.OnKill();
            Main.npc[sting].ModNPC.OnKill();

            BossDownedSystem.BossDowned(2);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < 4; i++)
            {
                KinematicChain leg =  Legs[i].Leg;
                Vector2 start = leg.StartingPoint;
                for (int j = 0; j < leg.JointCount; j++)
                {
                    Vector2 end = start + leg[j].Offset;
                    spriteBatch.DrawLine(start, end, Color.White, 8 - j * 2);
                    start = end;
                }
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            tail.Draw(spriteBatch, TailTexture.Value, null, Color.White, true, TailTexture.Size() / 2, Vector2.One, NPC.spriteDirection == -1? SpriteEffects.None : SpriteEffects.FlipVertically, 1);

            for (int i = 4; i < 8; i++)
            {
                KinematicChain leg = Legs[i].Leg;
                Vector2 start = leg.StartingPoint;
                for (int j = 0; j < leg.JointCount; j++)
                {
                    Vector2 end = start + leg[j].Offset;
                    spriteBatch.DrawLine(start, end, Color.Red, 8 - j * 2);
                    start = end;
                }
            }
        }
    }
}
