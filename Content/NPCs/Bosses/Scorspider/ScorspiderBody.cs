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

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    [AutoloadBossHead]
    public class ScorspiderBody : ModNPC
    {
        int head;
        int sting;
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
                Legs[i] = new ScorspiderLeg(new Vector2(70 * (i % 4 > 1? -1 : 1), 10), 1, 70, 120, i);
            }
            LegBraces = new Vector2[8];
            for (int i = 0; i < 8; i++)
            {
                LegBraces[i] = new Vector2(60 - 40 * (i % 4), i % 4 == 1 || i % 4 == 2? 30 : 20);
            }
            

            NPC.width = 140;
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
        public override void OnSpawn(IEntitySource source)
        {
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
            return true;
        }
        public ScorspiderLeg[] Legs;
        public Vector2[] LegBraces;

        public static Point FindGround(Point p, Vector2 direction, string num)
        {
            if (p.X > 0 && p.Y > 0 && WorldGen.InWorld(p.X, p.Y, 2))
            {
                Point result = Utilities.FindGround(p, direction);
                if (result.X > 0 && result.Y > 0 && WorldGen.InWorld(result.X, result.Y, 2))
                    return result;
            }
            return p;
        }

        public override void AI()
        {
            if (Main.mouseLeft && Main.MouseWorld.X > NPC.position.X && Main.MouseWorld.X < NPC.TopRight.X && Main.MouseWorld.Y > NPC.position.Y && Main.MouseWorld.Y < NPC.BottomLeft.Y)
            {
                NPC.velocity = Main.MouseWorld - NPC.Center;
                NPC.direction = NPC.velocity.X.NonZeroSign();
            }
            foreach (var leg in Legs)
            {
                leg.Update(NPC);
            }
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

            //var tailSource = Main.projectile[tails[i]].GetSource_FromThis();

            //Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailFirstGoreType);
            //Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailSecondGoreType);

            int legFirstGoreType = Mod.Find<ModGore>("ScorspiderLeg_0").Type;
            int legSecondGoreType = Mod.Find<ModGore>("ScorspiderLeg_1").Type;
            
            //Gore.NewGore(tailSource, Main.projectile[leggs[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legFirstGoreType);
            //Gore.NewGore(tailSource, Main.projectile[leggs[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legSecondGoreType);

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
            for (int i = 4; i < 8; i++)
            {
                KinematicChain leg = Legs[i].Leg;
                Vector2 start = leg.StartingPoint;
                for (int j = 0; j < leg.JointCount; j++)
                {
                    Dust.NewDust(start, 0, 0, DustID.Torch);
                    Vector2 end = start + leg[j].Offset;
                    spriteBatch.DrawLine(start, end, Color.White, 8 - j * 2);
                    start = end;
                }
            }
        }
    }
}
