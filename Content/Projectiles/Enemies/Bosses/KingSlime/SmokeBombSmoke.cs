using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.System;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class SmokeBombSmoke : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/BlackPixel";
        public override void SetDefaults()
        {
            Projectile.timeLeft = WorldDifficultySystem.suicide? 800 : 600;
            Projectile.hostile = true;
            Projectile.width = WorldDifficultySystem.suicide? 150 : 100;
            Projectile.height = WorldDifficultySystem.suicide? 150 : 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.GetT().useVanillaDrawing = false;
        }
        static UnifiedRandom random = new();
        public override void AI()
        {
            if (random.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeCloud>(), newColor: Color.Gray, Scale: random.NextFloat(1, 1.5f));
                Main.dust[d].customData = -100;
            }
        }
    }
}
