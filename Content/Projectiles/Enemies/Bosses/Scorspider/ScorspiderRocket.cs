using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider
{
    public class ScorspiderRocket : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
        }

        float angularVelocity;
        public override void OnSpawn(IEntitySource source)
        {
            float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
            if (Projectile.velocity.Y < 0)
                angel = 2 * Convert.ToSingle(Math.PI) - angel;
            Projectile.rotation = angel;
        }
        public override void AI()
        {
            if (Projectile.friendly)
            {
                Projectile.tileCollide = true;
            }
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * -3, Projectile.velocity.Y * -3);
            Entity player = null;
            if (Projectile.hostile)
            {
                player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            }
            else
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && (player == null || Projectile.Distance(player.Center) > Projectile.Distance(npc.Center)))
                    {
                        player = npc;
                    }
                }
            }

            Projectile.velocity = Projectile.rotation.ToRotationVector2() * (Projectile.velocity.Length() + (Projectile.velocity.Length() > 19? 0 : 0.25f));
            if (player != null)
            {
                float goalAngle = Projectile.AngleTo(player.Center);
                goalAngle = goalAngle % (2f * (float)Math.PI);
                if (goalAngle < 0)
                {
                    goalAngle += (float)Math.PI * 2;
                }
                Projectile.rotation = Projectile.rotation % (2f * (float)Math.PI);
                if (Projectile.rotation < 0)
                {
                    Projectile.rotation += (float)Math.PI * 2;
                }

                if (goalAngle < (float)Math.PI)
                {
                    if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
                    {
                        if (angularVelocity > -0.03f)
                            angularVelocity -= 0.003f;
                    }
                    else
                    {
                        if (angularVelocity < 0.03f)
                            angularVelocity += 0.003f;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < 0.03f)
                            angularVelocity += 0.003f;
                    }
                    else
                    {
                        if (angularVelocity > -0.03f)
                            angularVelocity -= 0.003f;
                    }
                }
                Projectile.rotation += angularVelocity;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
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
