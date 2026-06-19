using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terraria;
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
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 36;

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
        }
        int timer = 5;
        public override void AI()
        {
            timer--;
            if (!Main.npc[Body].active || Main.npc[Body].type != ModContent.NPCType<ScorspiderBody>() && timer <= 0)
            {
                NPC.life = 0;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderHead_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderHead_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life - hit.Damage <= 1 && (Main.npc[Body].ai[0] == 1 || Main.npc[Body].ai[0] == 0))
            {
                NPC.life = 1;
                NPC.immortal = true;
                Main.npc[Body].life = 1;
                Main.npc[Sting].life = 1;
                Main.npc[Body].immortal = true;
                Main.npc[Sting].immortal = true;
            }
            else
            {
                if (Main.npc[Body].life - hit.Damage <= 0)
                {
                    hit.HideCombatText = true;
                    Main.npc[Body].StrikeNPC(hit);
                    Main.npc[Sting].StrikeNPC(hit);
                }
                else
                {
                    Main.npc[Body].life -= hit.Damage;
                    Main.npc[Sting].life -= hit.Damage;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame = new Rectangle(0, frame * NPC.height, NPC.width, NPC.height);
        }
    }
}
