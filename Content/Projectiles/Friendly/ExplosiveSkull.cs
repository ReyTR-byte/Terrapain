using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Utilities;
using Terraria.ID;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class ExplosiveSkull : ModProjectile
    {
        bool explode;
        private bool pet
        {
            get => Projectile.ai[0] == 1;
            set => Projectile.ai[0] = value ? 1 : 0;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 0;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
        }
        bool FoundTarget;
        float distanceFromTarget;
        Vector2 targetCenter;
        UnifiedRandom rand = new UnifiedRandom();
        int useFrame;
        int timer;
        public override void AI()
        {
            if (!pet)
            {
                AISearchForTarget(out FoundTarget, out distanceFromTarget, out targetCenter);
                AIMovement(FoundTarget, targetCenter);
                if (Projectile.timeLeft % 6 == 0)
                {
                    useFrame = useFrame == 1 ? 0 : 1;
                }
                if (Projectile.timeLeft == 1)
                {
                    Blast();
                }
                Lighting.AddLight(Projectile.Center, 0.5f, 0, 0.8f);
                Projectile.frame = (Functions.UnitVectorFromRotation(Projectile.rotation).X < 0 ? 2 : 0) + useFrame;
            }
            else
            {
                Projectile.tileCollide = false;
                Projectile.friendly = false;
                Projectile.hostile = false;
                Projectile.penetrate = -1;
                Vector2 IdlePosition = Main.player[Projectile.owner].Center + new Vector2(-22 * Main.player[Projectile.owner].direction, -26);
                if ((IdlePosition - Projectile.Center).Length() > 800)
                {
                    Projectile.position = Main.player[Projectile.owner].Center - new Vector2(Projectile.width / 2, Projectile.height / 2);
                    Projectile.velocity = Vector2.Zero;
                }
                float MaxSpeed = Projectile.Distance(IdlePosition) * 0.2f;
                if (MaxSpeed < 3)
                {
                    MaxSpeed = 3;
                }
                if (MaxSpeed > 40)
                {
                    MaxSpeed = 40;
                }
                if (Projectile.Center != IdlePosition)
                {
                    Projectile.velocity = (IdlePosition - Projectile.Center) / (IdlePosition - Projectile.Center).Length() * (Projectile.velocity.Length() + 0.5f);
                }
                if (Projectile.velocity.Length() > MaxSpeed)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= MaxSpeed;
                }
                if ((Projectile.Center - IdlePosition).Length() < Projectile.velocity.Length())
                {
                    Projectile.position = IdlePosition - new Vector2(Projectile.width / 2, Projectile.height / 2);
                    Projectile.velocity = Vector2.Zero;
                }
                timer++;
                if (timer % 6 == 0)
                {
                    useFrame = useFrame == 1 ? 0 : 1;
                }
                if (Projectile.velocity.Length() < 5)
                {
                    Projectile.rotation = 0;
                    Projectile.frame = 4 + useFrame;
                }
                else
                {
                    Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
                    Projectile.frame = (Functions.UnitVectorFromRotation(Projectile.rotation).X < 0 ? 2 : 0) + useFrame;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Blast();
            target.AddBuff(BuffID.OnFire, 180);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Functions.CircleColision(Projectile.Center, 75, target.Center, target.width, target.height))
            {
                return null;
            }
            else return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Blast();
            return false;
        }
        private void AISearchForTarget(out bool foundTarget,
                                       out float distanceFromTarget,
                                       out Vector2 targetCenter)
        {
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Functions.CanHit(Projectile.Center, npc.position - (npc.position - Projectile.Center) / (npc.position - Projectile.Center).Length() * 60, npc.width, npc.height);

                    if (((closest && inRange) || !foundTarget) && lineOfSight)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
        }
        float angularVelocity;
        private void AIMovement(bool foundTarget, Vector2 targetCenter)
        {
            if (foundTarget)
            {
                float goalAngle = Projectile.AngleTo(targetCenter);
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
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.1f;
                    }
                    else
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.1f;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.1f;
                    }
                    else
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.1f;
                    }
                }
                if ((Projectile.rotation + angularVelocity > goalAngle && Projectile.rotation < goalAngle) || (Projectile.rotation + angularVelocity < goalAngle && Projectile.rotation > goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                else
                {
                    Projectile.rotation += angularVelocity;
                }
                Projectile.velocity = new Vector2((float)Math.Cos(Projectile.rotation), (float)Math.Sin(Projectile.rotation)) * Projectile.velocity.Length();
            }
        }
        private void Blast()
        {
            if (!explode)
            {
                Projectile.height = 150;
                Projectile.width = 150;
                Projectile.position.X -= 85;
                Projectile.position.Y -= 84;
                Projectile.alpha = 255;
                Projectile.timeLeft = 2;
                for (int i = 0; i < 200; i++)
                {
                    float angle = rand.NextFloat() * (float)Math.PI * 2;
                    Vector2 posInCircle = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * rand.NextFloat() * 75;
                    Dust.NewDust(Projectile.Center + posInCircle, 0, 0, DustID.Torch, posInCircle.X / 25, posInCircle.X / 25);
                }
                explode = true;
            }
        }
    }
}