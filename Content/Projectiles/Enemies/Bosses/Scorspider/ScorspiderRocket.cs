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
        float angularAcceleration = 0.003f;
        float maxSpeed = 19f;
        float MaxAngularVelocity = 0.03f;
        float acceleration = 0.25f;
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (Projectile.ai[0] != 0)
            {
                angularAcceleration = Projectile.ai[0];
            }
            if (Projectile.ai[1] != 0)
            {
                maxSpeed = Projectile.ai[1];
            }
            if (Projectile.ai[2] != 0)
            {
                MaxAngularVelocity = Projectile.ai[2];
            }
            float num = Convert.ToSingle(source.Context);
            if (num != 0)
            {
                acceleration = num;
            }
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

            Projectile.velocity = Projectile.rotation.ToRotationVector2() * (Projectile.velocity.Length() + (Projectile.velocity.Length() > maxSpeed? 0 : acceleration));
            if (player != null)
            {
                float goalAngle = Projectile.AngleTo(player.Center);
                goalAngle = Functions.NormalizeRotation(goalAngle);
                Projectile.rotation = Functions.NormalizeRotation(Projectile.rotation);

                if (goalAngle < (float)Math.PI)
                {
                    if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
                    {
                        if (angularVelocity > -MaxAngularVelocity)
                            angularVelocity -= angularAcceleration;
                    }
                    else
                    {
                        if (angularVelocity < MaxAngularVelocity)
                            angularVelocity += angularAcceleration;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < MaxAngularVelocity)
                            angularVelocity += angularAcceleration;
                    }
                    else
                    {
                        if (angularVelocity > -MaxAngularVelocity)
                            angularVelocity -= angularAcceleration;
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
