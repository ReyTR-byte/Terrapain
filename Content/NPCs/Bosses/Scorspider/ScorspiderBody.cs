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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    [AutoloadBossHead]
    public class ScorspiderBody : ModNPC
    {
        int head;
        int sting;
        public float angularVelocity;
        private int state
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private int subState
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private float timer
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private int timer2
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        private bool secondPhase => state == 1;
        private bool thirdPhase => state == 2;

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
        public Vector2 StingPosition => tailPosition + new Vector2(60 * NPC.spriteDirection, -40);
        public override void OnSpawn(IEntitySource source)
        {
            head = NPC.NewNPC(source, (int)HeadPosition.X, (int)HeadPosition.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI,  NPC.whoAmI);
            sting = NPC.NewNPC(source, (int)StingPosition.X, (int)StingPosition.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, NPC.whoAmI, head);
            Main.npc[head].ai[1] = sting;
            tail = new SimulatedChain(7, 26, tailPosition, 0, 1);
            TailTexture = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/Scorspider/ScorspiderTail");
            ShaderSystem.ScorspiderTimer = 20;
            ShaderSystem.drawScorspiderBorders = false;
            ShaderSystem.ScorspiderAuraTimer = 20;
            ShaderSystem.drawScorspiderAura = false;
            //head = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI);
            //sting = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, head);
            //Main.npc[head].ai[1] = sting;
            timer = 300;
            timer2 = 60;

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

        public override void AI()
        {
            maxSpeed = MathF.Max(6, MathF.Abs(NPC.Center.X - NPC.GetT().Target.Center.X) / 70);
            NPC.TargetClosest();
            foreach (var leg in Legs)
            {
                leg.Update(NPC);
            }
            UpdateMovement();
            UpdateBody();
        }
        bool jumpAnimation;
        float targetHeight = 120;
        float maxSpeed;
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
                if (jumpAnimation)
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
                NPC.velocity.X += dir.X.NonZeroSign() * 0.3f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
            }
            else
            {
                jumpAnimation = false;
            }
            NPC.spriteDirection = dir.X.NonZeroSign() * -1;
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
            tail.Fragments[0].fixedAt = tailPosition;
            tail.Fragments[tail.Fragments.Length - 1].fixedAt = Main.npc[sting].Center;
            tail.Update();
            if (Main.mouseLeft && Main.MouseWorld.X > NPC.position.X && Main.MouseWorld.X < NPC.TopRight.X && Main.MouseWorld.Y > NPC.position.Y && Main.MouseWorld.Y < NPC.BottomLeft.Y)
            {
                NPC.velocity = Main.MouseWorld - NPC.Center;
                NPC.direction = NPC.velocity.X.NonZeroSign();
            }
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
                Gore.NewGore(entitySource, position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legFirstGoreType);
                Gore.NewGore(entitySource, position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legSecondGoreType);
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
