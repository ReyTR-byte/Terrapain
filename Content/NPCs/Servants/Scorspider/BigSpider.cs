using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.NPCs.Servants.Scorspider
{
    public class BigSpider : ModNPC
    {
        int damage = 20;
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
            Main.npcFrameCount[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.height = 34;
            NPC.width = 40;

            NPC.lifeMax = 50;
            NPC.aiStyle = -1;
            NPC.friendly = false;
            NPC.damage = damage;

            NPC.HitSound = SoundID.NPCHit1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.velocity.X = VelocityX;
            NPC.velocity.Y = VelocityY;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        float jumpForce = 15;
        float speed => (8f + (float)(3 + WorldDifficultySystem.TerrapainDifficulty) / 3) / (WorldDifficultySystem.suicide ? 1.5f : 1.7f);
        int timer = 5;
        float oldFallSpeed;
        float length;
        Player target;

        public override void FindFrame(int frameHeight)
        {
            if (NPC.Distance(target.Center) < 600)
            {
                NPC.frame.Y = NPC.height * 4;
            }
            else
            {
                NPC.frame.Y = NPC.height * ((int)length / 2) % 4;
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void AI()
        {
            NPC.TargetClosest();
            target = Main.player[NPC.target];


            if (NPC.Distance(target.Center) < 600)
            {
                length = 0;
                if (NPC.collideY)
                {
                    float t;
                    Vector2 targetPosition = target.Center + new Vector2(0, -100) - NPC.Center;
                    t = (float)Math.Sqrt(Math.Abs((targetPosition.Y * 2) / NPC.gravity));
                    NPC.velocity.X = targetPosition.X / t;
                    NPC.velocity.Y = targetPosition.Y / t - NPC.gravity * t / 2;
                    NPC.rotation = 0;
                    NPC.DirectionTo(target.Center - NPC.Center);
                    timer = 15;
                    NPC.velocity += target.velocity;
                }
                else
                {
                    NPC.rotation += (float)Math.PI / 30;
                    timer--;
                    if (timer == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Functions.UnitVectorFromRotation(NPC.rotation + 0.5f * (float)Math.PI) * 8, ModContent.ProjectileType<BigSpiderSpike>(), damage, 4f);
                        timer = 7;
                    }
                    oldFallSpeed = NPC.velocity.Y;
                }
            }
            else
            {
                if (NPC.collideY)
                {
                    length += NPC.velocity.X;
                }
                else
                {
                    length = 0;
                }
                NPC.spriteDirection = (NPC.DirectionTo(target.Center).X >= 0) ? 1 : -1;
                if (Math.Abs(NPC.velocity.X) < 12)
                {
                    NPC.velocity.X += 0.2f * NPC.spriteDirection;
                }
                else
                {
                    if (NPC.velocity.X > 0)
                    {
                        NPC.velocity.X -= 0.2f;
                    }
                    else
                    {
                        NPC.velocity.X += 0.2f;
                    }
                }
            }
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("BigSpider_0").Type;
            int secondGoreType = Mod.Find<ModGore>("BigSpider_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);

            SoundEngine.PlaySound(SoundID.NPCDeath2, NPC.position);
        }
    }
}
