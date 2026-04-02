using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class BatProjectile : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/BlackPixel";
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            if (Projectile.ai[0] > 0)
            {
                if (Main.npc[(int)Projectile.ai[0]].active)
                {
                    Projectile.Hitbox = Main.npc[(int)Projectile.ai[0]].Hitbox;
                    Main.npc[(int)Projectile.ai[0]].GetT().fallThroughtPlatforms = true;
                }
                else
                {
                    Projectile.active = false;
                }
            }
            else
            {
                if (Main.player[-(int)Projectile.ai[0]].active)
                {
                    Projectile.Hitbox = Main.player[-(int)Projectile.ai[0]].Hitbox;
                }
                else
                {
                    Projectile.active = false;
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[0])
            {
                return false;
            }
            return null;
        }
        public override bool CanHitPlayer(Player target)
        {
            if (target.whoAmI == -(int)Projectile.ai[0])
            {
                return false;
            }
            return true;
        }
    }
}
