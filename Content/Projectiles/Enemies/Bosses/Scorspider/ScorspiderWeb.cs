using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider
{
    public class ScorspiderWeb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;

            Projectile.alpha = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.knockBack = 0f;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            if (Projectile.hostile)
            {
                foreach (Player target in Main.player)
                {
                    if (target.active && !target.dead)
                    {
                        if (Functions.RectangleColision(target, Projectile))
                        {
                            target.velocity *= 0.8f;
                            target.runAcceleration *= 0.2f;
                            target.maxRunSpeed *= 0.4f;
                            target.maxFallSpeed *= 0.3f;
                            target.gravity *= 0.3f;
                        }
                    }
                }
                foreach (var target in Main.npc)
                {
                    if (target.active && target.friendly && target.knockBackResist != 0)
                    {
                        if (Functions.RectangleColision(target, Projectile))
                        {
                            target.velocity *= 0.8f * target.knockBackResist;
                            target.MaxFallSpeedMultiplier *= 0.3f * target.knockBackResist;
                            target.GravityMultiplier *= 0.3f * target.knockBackResist;
                        }
                    }
                }
            }

            if (Projectile.friendly)
            {
                foreach (var target in Main.npc)
                {
                    if (target.active && !target.friendly && target.knockBackResist != 0)
                    {
                        if (Functions.RectangleColision(target, Projectile))
                        {
                            target.velocity *= 0.8f * target.knockBackResist;
                            target.MaxFallSpeedMultiplier *= 0.3f * target.knockBackResist;
                            target.GravityMultiplier *= 0.3f * target.knockBackResist;
                        }
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0;
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
    }
}
