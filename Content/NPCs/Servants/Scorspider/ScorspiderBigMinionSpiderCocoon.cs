using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.NPCs.Servants.Scorspider
{
    public class ScorspiderBigMinionSpiderCocoon : ModNPC
    {
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
        public override void SetDefaults()
        {
            NPC.height = 50;
            NPC.width = 50;

            NPC.lifeMax = 500;
            NPC.friendly = false;
            NPC.damage = 20;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = -1;
        }
        float angularVelocity;
        UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(IEntitySource source)
        {
            if (VelocityX != float.NaN && VelocityY != float.NaN && (VelocityX != 0 || VelocityY != 0))
            {
                NPC.velocity.X = VelocityX;
                NPC.velocity.Y = VelocityY;
                NPC.velocity = Functions.Rotate(NPC.velocity, NPC.velocity.X);
            }
            angularVelocity = random.NextFloat() - 0.5f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SpawnMinions();
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void AI()
        {
            NPC.rotation += angularVelocity;
            if (NPC.collideX)
            {
                NPC.velocity.X *= -1;
                SpawnMinions();
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y *= -1;
                SpawnMinions();
            }
        }
        private void SpawnMinions()
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Granite, NPC.velocity.X, NPC.velocity.Y);
            }
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BigSpider>(), 0, NPC.velocity.X * (NPC.collideX ? -1 : 1), NPC.velocity.Y * (NPC.collideY ? -1 : 1));
            NPC.life = 0;
        }
    }
}
