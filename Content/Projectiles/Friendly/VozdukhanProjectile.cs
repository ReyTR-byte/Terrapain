using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class VozdukhanProjectile : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/BlackPixel";
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 20;
            Projectile.height = 20;
        }
        UnifiedRandom random = new();
        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = Projectile.Center;
                position.X += random.NextFloat(-Projectile.width * Projectile.scale, Projectile.width * Projectile.scale) / 2;
                position.Y += random.NextFloat(-Projectile.width * Projectile.scale, Projectile.width * Projectile.scale) / 2;
                if (Functions.CanHit(Projectile.Center, position, 1, 1))
                {
                    Dust.NewDust(position, 0, 0, DustID.SteampunkSteam, Scale: 1.2f);
                }
            }
            Projectile.scale += 0.3f;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Point position = (Projectile.Center - new Vector2(Projectile.scale * Projectile.width / 2)).ToPoint();
            hitbox = new(position.X, position.Y, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale));
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback /= MathF.Sqrt(Projectile.scale);
            modifiers.FinalDamage /= MathF.Sqrt(Projectile.scale);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback /= MathF.Sqrt(Projectile.scale);
            modifiers.FinalDamage /= MathF.Sqrt(Projectile.scale);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Functions.CanHit(Projectile.Center, target.position, target.width, target.height)? null : false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return Functions.CanHit(Projectile.Center, target.position, target.width, target.height);
        }
        public override bool CanHitPvp(Player target)
        {
            return Functions.CanHit(Projectile.Center, target.position, target.width, target.height);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}
