using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Content.NPCs.Servants.Scorspider
{
    public class ScorspiderLittleMinionSpidersCocoon : ModNPC
    {
        private float TargetY
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public override void SetDefaults()
        {
            NPC.height = 30;
            NPC.width = 30;

            NPC.lifeMax = 500;
            NPC.friendly = false;
            NPC.damage = 20;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = -1;
        }
        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            angularVelocity = TGlobalNPC.random.NextFloat(-0.2f, 0.2f);
        }
        public override bool? CanFallThroughPlatforms()
        {
            return NPC.Bottom.Y < TargetY;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SpawnMinions();
        }
        public override void AI()
        {
            if (NPC.collideX || NPC.collideY)
            {
                SpawnMinions();
            }
            NPC.rotation += angularVelocity;
        }
        private void SpawnMinions()
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<ScorspiderWebDust>(), NPC.velocity.X, NPC.velocity.Y);
            }
            for (int i = 0; i < 5; i++)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SmallSpider>(), 0, NPC.velocity.X * (NPC.collideX ? -1 : 1) - 10 + 5 * i, NPC.velocity.Y * (NPC.collideY ? -1 : 1));
            }
            NPC.life = 0;
        }
    }
}
