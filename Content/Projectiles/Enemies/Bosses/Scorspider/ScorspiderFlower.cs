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
    public class ScorspiderFlower : ModProjectile
    {
        int spikesCount
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        bool followPlayer
        {
            get => Projectile.ai[1] == 1;
            set => Projectile.ai[1] = value ? 1 : 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.timeLeft = 240;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
        }

        float speed;
        int ShootProjectiles;
        int timer;
        public override void OnSpawn(IEntitySource source)
        {
            speed = Projectile.velocity.Length();
            Projectile.rotation = Projectile.ai[2];
        }
        public override void AI()
        {
            if (followPlayer)
            {
                Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                Projectile.velocity = Projectile.DirectionTo(player.Center) * speed;
                if (speed > 0.3f)
                {
                    speed -= 0.3f;
                }
                else
                {
                    speed = 0;
                }
            }
            else
            {
                if (speed != 0)
                {
                    Projectile.velocity.Normalize();

                    Projectile.velocity *= speed;
                    if (speed > 0.3f)
                    {
                        speed -= 0.3f;
                    }
                    else
                    {
                        speed = 0;
                    }
                }
            }
            if (speed <= 4 && ShootProjectiles < spikesCount && timer <= 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Functions.UnitVectorFromRotation(Projectile.rotation + (float)ShootProjectiles / spikesCount * 2 * (float)Math.PI) * 20, ModContent.ProjectileType<ScorspiderSpike>(), Projectile.damage, Projectile.knockBack, -1, 0, 0, -1);
                ShootProjectiles += 1;
                timer = 8;
                Projectile.scale = 1.16f;
            }
            if (ShootProjectiles < spikesCount)
            {
                Projectile.timeLeft = 2;
            }
            if (Projectile.scale > 1)
            {
                Projectile.scale -= 0.02f;
            }
            timer--;
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
