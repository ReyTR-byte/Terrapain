using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Player;
using Terrapain.Content.Buffs;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Minions
{
    public class ScorspiderSting : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        int width = 42; 
        int height = 44;
        Vector2 size => new Vector2(width, height);
        Vector2 HitboxPosition => Projectile.Center + Functions.UnitVectorFromRotation(Projectile.rotation) * (Projectile.Hitbox.Size() / 2).Distance(size / 2) - size / 2;
        Vector2 idlePosition => player.Center + new Vector2(-39 * player.direction, -38);
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 64;

            Projectile.friendly = true;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (!player.GetModPlayer<PlayerOrganismOverload>().RemovedBuffs.Contains(ModContent.BuffType<ScorspiderStingBuff>()) && player.HasBuff<ScorspiderStingBuff>())
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.timeLeft = 0;
                return;
            }

            NPC target = FindTarget();
            
            if (target == null)
            {
                IdleMovement();
            }
            else
            {
                Movement (target);
            }
            if (Projectile.Distance(player.Center) > 300)
            {
                Projectile.Center = player.Center;
            }
            if (Projectile.Distance(player.Center) > 200)
            {
                player.velocity += (Projectile.Distance(player.Center) - 200) * 0.1f * player.DirectionTo(Projectile.Center);
                Projectile.velocity -= (Projectile.Distance(player.Center) - 200) * 0.9f * player.DirectionTo(Projectile.Center);
            }

            if (Projectile.velocity.Length() > 16)
            {
                Projectile.velocity *= 16 / Projectile.velocity.Length();
            }

            Projectile.frame = Math.Abs(Projectile.rotation) > 0.5 * Math.PI? 1 : 0;
        }
        NPC FindTarget()
        {
            NPC npc = null;
            float distance = 0;
            foreach (var n in Main.npc)
            {
                if (n.active && !n.friendly && Functions.CircleColision(player, 200, n) && (npc == null || distance < Functions.DistanceBetweenHitboxes(n, HitboxPosition, width, height)))
                {
                    npc = n;
                    distance = Functions.DistanceBetweenHitboxes(npc, n);
                }
            }
            return npc;
        }
        float angularVelocity;
        void IdleMovement()
        {
            if (Projectile.Distance(idlePosition) != 0)
            {
                Projectile.velocity += Projectile.DirectionTo(idlePosition) * ((Projectile.Distance(idlePosition) < 15)? 0.2f : (Projectile.Distance(idlePosition) > 70)? 1 : (Projectile.Distance(idlePosition) - 15) * 1 / 55);
                float rotation = Functions.NormalizeRotation(Functions.AngleFromVector(idlePosition - Projectile.Center) - Functions.AngleFromVector(Projectile.velocity), false);
                Projectile.velocity = Projectile.velocity.RotatedBy((Math.Abs(rotation) > 0.15 * Math.PI)? 0.15 * Math.PI * rotation.NonZeroSign() : rotation);                
            }
            if (Projectile.Distance(idlePosition) < 12)
            {
                Projectile.velocity *= 0.6f;
            }

            float goalAngle = (Projectile.Center.X - player.Center.X) > 0? (float)Math.PI : 0;
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

            if (Projectile.rotation != goalAngle)
            {
                if (goalAngle < (float)Math.PI)
                {
                    if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.03f;
                    }
                    else
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.03f;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.03f;
                    }
                    else
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.03f;
                    }
                }
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                goalAngle += 2 * (float)Math.PI;
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                goalAngle -= 4 * (float)Math.PI;
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                else
                {
                    Projectile.rotation += angularVelocity;
                }
            }        
        }
        void Movement(NPC target)
        {
            if (Projectile.Distance(idlePosition) != 0)
            {
                Projectile.velocity += Projectile.DirectionTo(target.Center);
                float rotation = Functions.NormalizeRotation(Functions.AngleFromVector(target.Center - Projectile.Center) - Functions.AngleFromVector(Projectile.velocity), false);
                
                Projectile.velocity = Projectile.velocity.RotatedBy((Math.Abs(rotation) > 0.15 * Math.PI)? 0.15 * Math.PI * rotation.NonZeroSign() : rotation);
            }

            float goalAngle = Functions.AngleFromVector(Projectile.DirectionTo(target.Center));
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

            if (Projectile.rotation != goalAngle)
            {
                if (goalAngle < (float)Math.PI)
                {
                    if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.03f;
                    }
                    else
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.03f;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.03f;
                    }
                    else
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.03f;
                    }
                }
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                goalAngle += 2 * (float)Math.PI;
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                goalAngle -= 4 * (float)Math.PI;
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                else
                {
                    Projectile.rotation += angularVelocity;
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (!target.friendly && Functions.RectangleColision(target, HitboxPosition, width, height) && target.immune[Projectile.owner] == 0)
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 210);
        }
        public override bool PreDrawExtras()
        {
            Texture2D texture = ModContent.Request<Texture2D>(ModContent.GetModProjectile(ModContent.ProjectileType<ScorspiderTail>()).Texture).Value;
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float distanceToPlayer = directionToPlayer.Length();
            Vector2 center = player.Center - directionToPlayer / distanceToPlayer * texture.Width / 2;
            Vector2 directionToProjectile = center - Projectile.Center;
            float chainRotation = directionToProjectile.ToRotation();
            float distanceToProjectile = directionToProjectile.Length();

            while (distanceToProjectile > 15f && !float.IsNaN(distanceToPlayer) && distanceToProjectile < 1000f)
            {
                directionToProjectile /= distanceToProjectile; // get unit vector
                directionToProjectile *= texture.Width / 2; // multiply by chain link length

                center += directionToProjectile; // update draw position
                directionToPlayer = playerCenter - center; // update distance
                distanceToPlayer = directionToPlayer.Length();
                directionToProjectile = Projectile.Center - center; // update distance
                distanceToProjectile = directionToProjectile.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));
                Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height / 2);
                if (Math.Abs(chainRotation) > Math.PI / 2)
                {
                    frame.Y = texture.Height / 2;
                }
                // Draw chain
                Main.EntitySpriteDraw(texture, center - Main.screenPosition,
                    frame, drawColor, chainRotation,
                    frame.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);
            }
            // Stop vanilla from drawing the default chain.
            return false;
        }
    }
}