using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.DrawTasks;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.Items.Bags;
using Terrapain.Content.Items.DropRulls;
using Terrapain.Content.Items.Placeable.Relics;
using Terrapain.Content.Items.Placeable.Trophies;
using Terrapain.Content.NPCs.Servants.Scorspider;
using Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    [AutoloadBossHead]
    public class ScorspiderBody : ModNPC
    {
        //настроить баланс на 528 строку
        //подобрать размер и длину хвоста на 203 строку
        int head;
        int sting;
        public float angularVelocity;
        public bool sleep;

        int mainTimer;
        int timer;
        int attackCounter;
        int attack;
        int phase = 1;
        int[] attacks1 = [0, 1, 0, 2, 0, 3];
        int[] attacks2 = [0, 1, 0, 2, 0, 3];
        int[] attacks3 = [1, 0, 2, 3, 1, 2, 0, 4, 5];
        int[] attacks4 = [0, 1, 0, 2];

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
                LegBraces[i] = new Vector2(60 - 40 * (i % 4), i % 4 == 1 || i % 4 == 2? 25 : 15);
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
        Vector2 tailPosition => NPC.Center + new Vector2(60 * NPC.spriteDirection, 0).RotatedBy(NPC.rotation) * NPC.scale;
        public Vector2 HeadPosition => NPC.Center + new Vector2(-70 * NPC.spriteDirection, 5).RotatedBy(NPC.rotation) * NPC.scale;
        public Vector2 StingPosition => tailPosition + new Vector2(110 * NPC.spriteDirection, -45) * NPC.scale;
        public override void OnSpawn(IEntitySource source)
        {
            head = NPC.NewNPC(source, (int)HeadPosition.X, (int)HeadPosition.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI,  NPC.whoAmI);
            sting = NPC.NewNPC(source, (int)StingPosition.X, (int)StingPosition.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, NPC.whoAmI, head);
            Main.npc[head].ai[1] = sting;
            tail = new SimulatedChain(8, 26, tailPosition, 0, 1, 1);
            tail.Fragments[tail.Count - 1].length = 10;
            tail.Fragments[tail.Count - 1].draw = false;
            var list = tail.Fragments.ToList();
            list.Add(new(10, 1, tailPosition));
            tail.Fragments = list.ToArray();

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

            if (source.Context == "Prison")
            {
                sleep = true;
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
            if (sleep)
            {
                return false;
            }
            if (timer == 0 && phase == 2 && attack == 3)
            {
                return NPC.GetT().Target.Center.Y > NPC.Center.Y;
            }
            return NPC.GetT().Target.Center.Y - NPC.Center.Y > 150 * NPC.scale;
        }
        public ScorspiderLeg[] Legs;
        public Vector2[] LegBraces;

        public static Vector2 FindGround(Vector2 position, Vector2 direction, NPC npc, out bool findGround)
        {
            Vector2 end = position + direction * 230 * npc.scale;
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

        int Web => ModContent.ProjectileType<ScorspiderWeb>();
        int WebDamage = 10;
        float WebKnockback = 1;

        int oldDirection;
        float tailFragmentLength;
        public override void AI()
        {
            tailFragmentLength = 26;
            NPC.scale = 1.5f;
            NPC.height = (int)(80 * NPC.scale);
            NPC.width = (int)(80 * NPC.scale);
            if (sleep)
            {
                {
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
                    }
                    AngularAcceleration(ref angularVelocity, 0.02f, 0.08f, targetRotation, ref NPC.rotation);
                }
                NPC.life = Math.Min(NPC.life + NPC.lifeMax / 1000, NPC.lifeMax);
                if (NPC.collideY)
                {
                    NPC.velocity.X = 0;
                }
                foreach (var leg in Legs)
                {
                    leg.Update(NPC);
                }
                UpdateBody();
                Main.npc[sting].ai[3] = -1;
                Main.npc[head].ai[3] = -1;
                if (phase == 3)
                {
                    DoThirdPhase();
                }
                else if (phase == 4)
                {
                    sleep = false;
                }
            }
            else
            {
                Main.npc[head].ai[3] = 0;
                Main.npc[sting].ai[3] = 0;
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
                    case 4:
                        DoFourthPhase();
                        Main.npc[head].ai[3] = 1;
                        break;
                }
                UpdateMovement();
                UpdateBody();
            }
            if (timer > 0)
            {
                timer--;
            }
            if (mainTimer > 0)
            {
                mainTimer--;
            }
            oldDirection = NPC.spriteDirection;
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
                if (!jumpAnimation && (NPC.Center.Y - NPC.GetT().Target.Center.Y > 150 * NPC.scale || FindHole() || FindWall()))
                {
                    jumpAnimation = true;
                }
                if (jumpAnimation && !dash)
                {
                    targetHeight -= MathF.Max(5, NPC.velocity.X * 1.2f) * NPC.scale;
                    if (targetHeight < 60 * NPC.scale)
                    {
                        NPC.velocity.Y = MathF.Max(NPC.velocity.Y - 5, -15);
                        NPC.velocity.X = maxSpeed * -NPC.spriteDirection;
                    }
                }
                else
                {
                    Vector2 playerCenter = NPC.GetT().Target.Center;
                    if (playerCenter.X > NPC.Left.X && playerCenter.X < NPC.Right.X && NPC.Bottom.Y < playerCenter.Y)
                    {
                        targetHeight = 40 * NPC.scale;
                    }
                    else
                    {
                        targetHeight = 150 * NPC.scale;
                    }
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
            Main.npc[head].scale = NPC.scale;
            Main.npc[sting].scale = NPC.scale;
            //if (Main.npc[sting].collideY)
            //{
            //    tail.Fragments[tail.Fragments.Length - 1].position = Main.npc[sting].Center;
            //    if (Main.npc[sting].oldVelocity.Y != 0)
            //    {
            //        tail.Fragments[tail.Fragments.Length - 1].velocity.Y = Main.npc[sting].velocity.Y;
            //    }
            //}
            //if (Main.npc[sting].collideX)
            //{
            //    tail.Fragments[tail.Fragments.Length - 1].position = Main.npc[sting].Center;
            //    if (Main.npc[sting].oldVelocity.X != 0)
            //    {
            //        tail.Fragments[tail.Fragments.Length - 1].velocity.X = Main.npc[sting].velocity.X;
            //    }
            //}
            tail.Fragments[tail.Fragments.Length - 1].position = Main.npc[sting].Center;
            tail.Fragments[tail.Fragments.Length - 1].velocity = Main.npc[sting].velocity;
            if (oldDirection != NPC.spriteDirection)
            {
                for (int i = 0; i < tail.Count; i++)
                {
                    var f = tail.Fragments[i];
                    var pos = f.position;
                    pos -= NPC.Center;
                    pos.RotateBy(-NPC.rotation);
                    pos.X *= -1;
                    pos.RotateBy(NPC.rotation);
                    pos += NPC.Center;
                    tail.Fragments[i].position = pos;
                }
                Main.npc[sting].Center = tail.Fragments[tail.Fragments.Length - 1].position;
            }
            if (!sleep)
            {
                CommonTerrapainFlyingMovement(tail.Fragments[tail.Fragments.Length - 1].position, ref tail.Fragments[tail.Fragments.Length - 1].velocity, StingPosition, MathF.PI / 2, 18, 0.55f, 45);
            }
            else if (Main.npc[sting].Center.Distance(NPC.Center) < 180)
            {
                tail.Fragments[tail.Fragments.Length - 1].velocity += Main.npc[sting].Center.DirectionFrom(NPC.Center);
            }
            for (int i = 0; i < tail.Count; i++)
            {
                tail.Fragments[i].velocity += (NPC.velocity - NPC.oldVelocity) * 0.7f;
            }
            for (int i = 0; i < tail.Count - 2; i++)
            {
                tail.Fragments[i].length = tailFragmentLength * NPC.scale;
            }
            tail.Fragments[tail.Count - 2].length = 10 * NPC.scale;
            tail.Fragments[0].backwardRotation = NPC.rotation + (NPC.direction == 1? MathF.PI : 0);
            tail.Fragments[0].fixedAt = tailPosition;
            tail.Update();
            Main.npc[sting].velocity = tail.Fragments[tail.Fragments.Length - 1].velocity;
            if (Main.npc[sting].collideY)
            {
                Main.npc[sting].velocity.X *= 0.2f;
            }
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
                    {
                        if (chat)
                        {
                            Chatic(1);
                        }
                        float _progress = (300 - mainTimer) / 300f;
                        ScorspiderSting Sting = (ScorspiderSting)Main.npc[sting].ModNPC;
                        Sting.NPC.ai[3] = 1;

                        //чем больше это число тем больше скорость вращения жала
                        float rotationSpeed = 22;
                        float value = _progress * rotationSpeed % 4;
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
                        Sting.TargetRotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation() + MathF.Asin(value) * rotationRange;
                        if (timer == 0)
                        {
                            //это скорость вылета шипов
                            float speed = 25;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.NPC.Center, Sting.TargetRotation.ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback);
                            //это чвстота вылета шипов в тиках
                            timer = 10;
                        }
                        if (mainTimer == 0)
                        {
                            Sting.NPC.ai[3] = 0;
                            NextAttack1();
                        }
                    }
                    break;
                case 2:
                    if (chat)
                    {
                        Chatic(2);
                    }
                    if (mainTimer > 120 && mainTimer <= 240)
                    {
                        Main.npc[sting].ai[3] = 1;
                    }
                    if (timer == 0)
                    {
                        NPC Sting = Main.npc[sting];
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
                            ScorspiderSting ss = (ScorspiderSting)Sting.ModNPC;
                            ss.TargetRotation = -MathF.PI / 2 + NPC.spriteDirection * -0.5f;
                            if (chat)
                            {
                                Chatic("података 2");
                            }
                            float speed = 27;
                            //размах рандомного угла
                            float range = 0.1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), Sting.Center, (ss.GetRotation() + TGlobalNPC.random.NextFloat(-range, range)).ToRotationVector2() * speed, Spike, SpikeDamage, SpikeKnockback, -1, NPC.target, 0, 9);
                            timer = 14;
                        }
                        else
                        {
                            Sting.ai[3] = 0;
                            if (chat)
                            {
                                Chatic("података 3");
                            }
                            //начальная скорость вылета ракеты, она потом разгоняется сильно
                            float startSpeed = 1.5f;
                            //ускорение
                            float acceleration = 0.25f;
                            //максимальная скорость
                            float maxSpeed = 19;
                            //ускорение поворота
                            float maxAngularVelocity = 0.03f;
                            //максимальная скорость поворота
                            float angularAcceleration = 0.003f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(acceleration.ToString()), Sting.Center, rotation.ToRotationVector2() * startSpeed, Rocket, RocketDamage, RocketKnockback, -1, angularAcceleration, maxSpeed, maxAngularVelocity);
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
                    float progress = (400 - mainTimer) / 400f;
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
                            int count = 9;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -15, Flower, SpikeDamage, SpikeKnockback, -1, count, 0, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * 15, Flower, SpikeDamage, SpikeKnockback, -1, count, 0, TGlobalNPC.random.NextFloat(MathF.PI * 2));
                            //время до атаки рокетой
                            timer = 35;
                            NPC.ai[3] = 1;
                        }
                        else
                        {
                            float startSpeed = 1.5f;
                            float acceleration = 0.25f;
                            float maxSpeed = 19;
                            float maxAngularVelocity = 0.03f;
                            float angularAcceleration = 0.003f;
                            //атака ракетой
                            Projectile.NewProjectile(NPC.GetSource_FromThis(acceleration.ToString()), NPC.Center, Vector2.UnitY * -startSpeed, Rocket, RocketDamage, RocketKnockback, -1, angularAcceleration, maxSpeed, maxAngularVelocity);
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
        void DoThirdPhase()
        {
            float radius = 1000;
            if (attackCounter == 0)
            {
                int time = Math.Max(mainTimer - 300, 0);
                radius += time * time * 2;
            }
            if (attackCounter == attacks3.Length - 1)
            {
                int time = Math.Max(50 - mainTimer, 0);
                radius += time * time * 2;
            }
            AuraHoldPlayer(radius, NPC.Center);
            ShaderSystem.drawScorspiderAura = true;
            ShaderSystem.AuraRadius = radius;
            ShaderSystem.ScorspiderPosition = NPC.Center;
            switch (attack)
            {
                case 0:
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
                case 1:
                    float progress = 1 - mainTimer / 350f;
                    if (timer == 0)
                    {
                        Vector2 dir = Vector2.UnitX.RotatedBy(MathF.PI * 2 * progress - MathF.PI / 2 + NPC.ai[3]);
                        float distance = 800;
                        float startSpeed = 1.5f;
                        float acceleration = 0.25f;
                        float maxSpeed = 19;
                        float maxAngularVelocity = 0.03f;
                        float angularAcceleration = 0.003f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(acceleration.ToString()), NPC.GetT().Target.Center + dir * distance, -dir * startSpeed, Rocket, RocketDamage, RocketKnockback, -1, angularAcceleration, maxSpeed, maxAngularVelocity);
                        timer = 30;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
                case 2:
                    if (timer == 0)
                    {
                        float range = 0.3f;
                        Vector2 dir = Vector2.UnitY.RotatedBy(TGlobalNPC.random.NextFloat(-range, range));
                        float speed = 6f;
                        float dist = 1500;
                        int anotherRange = 600;
                        float maxFallSpeed = 15f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.GetT().Target.Center + new Vector2(TGlobalNPC.random.Next(-anotherRange, anotherRange + 1), -dist), dir * speed, Spike, SpikeDamage, SpikeKnockback, -1, maxFallSpeed, 0, 8);
                        timer = 6;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
                case 3:
                    if (timer == 0 && NPC.ai[3] > 0)
                    {
                        int count = 10;
                        float speed = 11 * NPC.ai[3];
                        int num = 4;
                        float angle = MathF.PI * 2 / count;
                        float startAngle = angle / 2 - angle * NPC.ai[3] / num;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(startAngle + angle * i) * speed, Web, WebDamage, WebKnockback);
                        }
                        NPC.ai[3]--;
                        timer = 60;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
                case 4:
                    if (timer == 0 && NPC.ai[3] > 0)
                    {
                        int bigSpidersCount = 2;
                        float height = 1200;
                        Point position = (NPC.GetT().Target.Center - Vector2.UnitY * height).ToPoint();
                        if (NPC.ai[3] > bigSpidersCount)
                        {
                            int npc = NPC.NewNPC(NPC.GetSource_FromThis(), position.X, position.Y, ModContent.NPCType<ScorspiderLittleMinionSpidersCocoon>(), 0, position.Y + height + 20);
                            Main.npc[npc].velocity.X = TGlobalNPC.random.Next(-2, 3);
                        }
                        else
                        {
                            int npc = NPC.NewNPC(NPC.GetSource_FromThis(), position.X, position.Y, ModContent.NPCType<ScorspiderBigMinionSpiderCocoon>(), 0, position.Y + height + 20);
                            Main.npc[npc].velocity.X = TGlobalNPC.random.Next(-2, 3);
                        }
                        NPC.ai[3]--;
                        timer = 40;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
                case 5:
                    if (timer == 0)
                    {
                        int count = 6;
                        float distance = 250f;
                        float speed = 10f;
                        float startAngle = TGlobalNPC.random.NextFloat(MathF.PI * 2);
                        float angle = MathF.PI * 2 / count;
                        Vector2 pos = NPC.GetT().Target.Center;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), pos + Vector2.UnitX.RotatedBy(startAngle + angle * i) * distance, -Vector2.UnitX.RotatedBy(startAngle + angle * i) * speed, Spike, SpikeDamage, SpikeKnockback, -1, 0, 0, 7);
                        }
                        timer = 130;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3();
                    }
                    break;
            }
        }
        void DoFourthPhase()
        {
            switch (attack)
            {
                case 0:
                    if (mainTimer == 0)
                    {
                        NextAttack4();
                    }
                    break;
                case 1:
                    float progress = 1 - mainTimer / 450f;
                    if (timer == 0)
                    {
                        Vector2 dir = Vector2.UnitX.RotatedBy(MathF.PI * 2 * progress - MathF.PI / 2 + NPC.ai[3]);
                        float rocketDistance = 800;
                        float spikeDistance = 350;
                        float spikeSpeed = 16f;
                        float startSpeed = 1.5f;
                        float acceleration = 0.25f;
                        float maxSpeed = 19;
                        float maxAngularVelocity = 0.03f;
                        float angularAcceleration = 0.003f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(acceleration.ToString()), NPC.GetT().Target.Center + dir * rocketDistance, -dir * startSpeed, Rocket, RocketDamage, RocketKnockback, -1, angularAcceleration, maxSpeed, maxAngularVelocity);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(acceleration.ToString()), NPC.GetT().Target.Center - dir * spikeDistance, dir * spikeSpeed, Spike, SpikeDamage, SpikeKnockback, -1, 0, 0, -1);
                        timer = 45;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack4();
                    }
                    break;
                case 2:
                    if (timer == 0)
                    {
                        if (NPC.ai[3] == 0)
                        {
                            Vector2 pl = NPC.GetT().Target.Center;
                            NPC.ai[0] = pl.X;
                            NPC.ai[1] = pl.Y;
                            NPC.ai[2] = TGlobalNPC.random.NextFloat(MathF.PI * 2);
                        }
                        Vector2 position = new Vector2(NPC.ai[0], NPC.ai[1]);
                        float angle = NPC.ai[2]; 
                        float distance = 450f;
                        //количество снарядов в середине
                        float count1 = 16;
                        //количество снарядов в крайней части
                        float count2 = 12;
                        //растояние между снарядами в средней части
                        float width1 = 12;
                        //растояние между снарядами в крайней части
                        float width2 = 140;
                        float speed = 22;
                        //время которое снаряды будут висеть в воздухе
                        int time = 60;
                        Vector2 dir = Vector2.UnitY.RotatedBy(angle);
                        Vector2 dir2 = Vector2.UnitX.RotatedBy(angle);
                        Vector2 start = position + dir2 * distance;
                        Vector2 start1 = start + dir * (count1 - 1) / 2f * width1;
                        Vector2 end = start - dir * (count1 - 1) / 2f * width1;
                        if (NPC.ai[3] == 0)
                        {
                            for (int i = 0; i < count1; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), start1 - dir * i * width1, -dir2 * speed, Spike, SpikeDamage, SpikeKnockback, -1, time, 0, 6);
                            }
                        }
                        else
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), start1 + dir * NPC.ai[3] * width2, -dir2 * speed, Spike, SpikeDamage, SpikeKnockback, -1, time, 0, 6);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), end - dir * NPC.ai[3] * width2, -dir2 * speed, Spike, SpikeDamage, SpikeKnockback, -1, time, 0, 6);
                        }
                        NPC.ai[3]++;
                        timer = 3;
                        if (NPC.ai[3] == count2)
                        {
                            NPC.ai[3] = 0;
                            timer = 120;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack4();
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
        void NextAttack3()
        {
            attackCounter++;
            if (attackCounter > attacks3.Length - 1)
            {
                phase = 4;
                attackCounter = 0;
            }
            attack = attacks3[attackCounter];
            switch (attack)
            {
                case 0:
                    mainTimer = 120;
                    break;
                case 1:
                    NPC.ai[3] = TGlobalNPC.random.NextFloat(MathF.PI * 2);
                    mainTimer = 350;
                    break;
                case 2:
                    mainTimer = 500;
                    NPC.ai[3] = 0;
                    break;
                case 3:
                    NPC.ai[3] = 4;
                    mainTimer = 300;
                    break;
                case 4:
                    NPC.ai[3] = 5;
                    mainTimer = 330;
                    break;
                case 5:
                    mainTimer = 600;
                    break;
            }
        }
        void NextAttack4()
        {
            attackCounter++;
            if (attackCounter > attacks4.Length - 1)
            {
                phase = 4;
                attackCounter = 0;
            }
            attack = attacks4[attackCounter];
            switch (attack)
            {
                case 0:
                    mainTimer = 120;
                    break;
                case 1:
                    NPC.ai[3] = TGlobalNPC.random.NextFloat(MathF.PI * 2);
                    mainTimer = 350;
                    break;
                case 2:
                    mainTimer = 500;
                    NPC.ai[3] = 0;
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
            if (phase != 3)
            {
                sleep = false;
            }
            NPC.immortal = false;
            if (NPC.life <= 0 && phase < 3)
            {
                sleep = true;
                phase = 3;
                attackCounter = 0;
                NPC.life = 1;
                timer = 0;
                mainTimer = 350;
                attack = 1;
                NPC.life = 1;
            }
            if (phase == 3)
            {
                NPC.life = Math.Max(NPC.life, 1);
            }
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
            tail.Draw(spriteBatch, TailTexture.Value, null, Color.White, true, TailTexture.Size() / 2, Vector2.One * NPC.scale, NPC.spriteDirection == -1? SpriteEffects.None : SpriteEffects.FlipVertically, 1);

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
