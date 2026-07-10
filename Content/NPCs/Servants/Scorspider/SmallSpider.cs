using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.NPCs.Servants.Scorspider
{
    public class SmallSpider : ModNPC
    {
        static UnifiedRandom rand = new UnifiedRandom();
        private float VelocityX
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private float VelocityY
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }
        public override void SetDefaults()
        {
            NPC.height = 20;
            NPC.width = 28;

            NPC.lifeMax = 20;
            NPC.aiStyle = -1;
            NPC.friendly = false;
            NPC.damage = 50;

            NPC.HitSound = SoundID.NPCHit1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.velocity.X = VelocityX;
            NPC.velocity.Y = VelocityY;
        }

        float Length;

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.X = (((int)Length / 6) % 2) == 0 ? 1 : 0;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override bool? CanFallThroughPlatforms()
        {
            return NPC.Bottom.Y < NPC.GetT().Target.Bottom.Y;
        }

        float jumpForce = 15;
        float speed => (8f + (float)(3 + WorldDifficultySystem.TerrapainDifficulty) / 3) / (WorldDifficultySystem.suicide ? 1.5f : 1.7f);
        int timer = 60;
        public override void AI()
        {
            Length += NPC.velocity.X;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            NPC.spriteDirection = NPC.position.X < target.position.X ? 1 : -1;
            timer--;

            if (NPC.collideY)
            {
                if (Math.Abs(NPC.velocity.X) < speed)
                    NPC.velocity.X += 0.5f * NPC.direction;
                else if (NPC.velocity.X > speed)
                    NPC.velocity.X -= 0.5f;
                else
                    NPC.velocity.X += 0.5f;
            }

            if (NPC.collideY && NPC.Distance(target.Center) < 800 && timer <= 0)
            {
                NPC.velocity = (-NPC.Center + target.Center) / 90f;
                NPC.velocity.Y -= jumpForce;
                NPC.velocity.X += rand.Next(-8, 7) + rand.NextFloat();
            }
            timer--;
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("LittleSpider_0").Type;
            int secondGoreType = Mod.Find<ModGore>("LittleSpider_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);

            SoundEngine.PlaySound(SoundID.NPCDeath2, NPC.position);
        }
    }
}
