using Luminance.Common.Utilities;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    public class ScorspiderHead : ModNPC
    {
        private int Body
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        NPC body => Main.npc[Body];
        ScorspiderBody sb => (ScorspiderBody)body.ModNPC;
        private int Sting
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        private int frame
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        float angularVelocity;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
            {
                PortraitScale = 0.6f,
                PortraitPositionYOverride = 0f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
        }
        public override void SetDefaults()
        {
            NPC.width = 78;
            NPC.height = 50;

            NPC.damage = 40;
            NPC.defense = 20;

            NPC.lifeMax = 8000;

            NPC.knockBackResist = 0f;

            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;

            NPC.noTileCollide = false;
            NPC.noGravity = true;

            NPC.aiStyle = -1;
            NPC.stairFall = true;

            NPC.HitSound = SoundID.NPCHit4;

            NPC.GetT().useVanillaDrawing = false;
            NPC.GetT().useModDrawingInPreDraw = true;
            NPC.GetT().drawCenter = new Vector2(5, 25);
            NPC.GetT().textureDirection = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.realLife = Body;
        }
        int animationTimer = 12;
        public override void AI()
        {
            NPC.width = (int)(78 * NPC.scale);
            NPC.height = (int)(50 * NPC.scale);
            if (!Main.npc[Body].active || Main.npc[Body].type != ModContent.NPCType<ScorspiderBody>())
            {
                NPC.active = false;
            }
            else
            {
                animationTimer--;
                if (NPC.ai[3] == -1)
                {
                    if (animationTimer <= 0)
                    {
                        animationTimer = 12;
                        frame = Math.Min(frame + 1, 5);
                    }
                }
                else if (NPC.ai[3] == 0)
                {
                    frame = 0;
                }
                else
                {
                    frame = 6;
                }
                if (NPC.spriteDirection != body.spriteDirection)
                {
                    NPC.rotation *= -1;
                    NPC.ai[3] *= -1;
                }
                NPC.spriteDirection = body.spriteDirection;
                float realRotation = NPC.rotation + (NPC.spriteDirection == 1? MathF.PI : 0);
                NPC.Center = sb.HeadPosition;
                NPC.velocity = Vector2.Zero;
                float targetRotation = 0;
                float r = body.rotation + (body.spriteDirection == 1 ? MathF.PI : 0);
                if (NPC.ai[3] == -1)
                {
                    targetRotation = r + (body.spriteDirection == -1? 1.2f : -1.2f);
                }
                else
                {
                    targetRotation = NPC.DirectionTo(body.GetT().Target.Center).ToRotation();
                }
                if (!Functions.IsAngleBetweenAngles(r + 1.2f, targetRotation, r - 1.2f))
                {
                    int dir = Functions.NormalizeRotation(targetRotation - r, false).NonZeroSign();

                    targetRotation = r + 1.2f * dir;
                }
                Functions.AngularAcceleration(ref angularVelocity, 0.03f, 0.3f, targetRotation, ref realRotation);
                NPC.rotation = realRotation - (NPC.spriteDirection == 1? MathF.PI : 0);
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            Main.npc[Body].ModNPC.HitEffect(hit);
            NPC.life = Math.Max(NPC.life, 1);
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderHead_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderHead_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame = new Rectangle(0, frame * frameHeight, NPC.frame.Width, frameHeight);
        }
    }
}
