using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Build.Construction;
using Terrapain.Content.Projectiles.Enemies;

namespace Terrapain.Content.Projectiles.Minions
{
    public class PetEye : ModProjectile
    {
        bool rotateToTarget = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 450;
            Projectile.alpha = 60;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = 0.25f * (float)Math.PI;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        float frameCounter;
        public override void AI()
        {
            if (frameCounter <= 0)
            {
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
                frameCounter = 100;
            }
            frameCounter -= Projectile.velocity.Length();
            if (Projectile.timeLeft > 0 && Projectile.timeLeft < 440)
            {
                AISearchForTarget(out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
                Vector2 VectorToTarget = targetCenter - Projectile.Center;
                AIRotating(VectorToTarget, foundTarget);
                AIMovment(foundTarget, VectorToTarget, distanceFromTarget);
                if (Projectile.timeLeft < 50)
                {
                    Projectile.alpha += 5;
                }
            }

            if (Projectile.position == new Vector2(float.NaN, float.NaN))
                Projectile.position = Projectile.oldPosition;
        }
        private void AISearchForTarget(
                                       out bool foundTarget,
                                       out float distanceFromTarget,
                                       out Vector2 targetCenter)
        {
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            //Projectile.friendly = foundTarget;
        }
        private void AIMovment(bool foundTarget,
                               Vector2 vectorToTarget,
                               float distanceFromTarget)
        {
            int speed = 8;
            if (foundTarget)
            {
                if (distanceFromTarget < 160)
                {
                    if (Projectile.timeLeft % 13 == 0)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-(float)Math.Cos(Projectile.rotation), -(float)Math.Sin(Projectile.rotation)) * 10, ModContent.ProjectileType<DemonicEyeLazer>(), Projectile.damage, 0.5f, Projectile.owner);

                    vectorToTarget.Y -= 100;
                    if (Projectile.velocity.Length() < speed && vectorToTarget != Vector2.Zero)
                    {
                        Projectile.velocity = vectorToTarget / vectorToTarget.Length() * (Projectile.velocity.Length() + 0.5f);
                    }
                    else if (vectorToTarget.Length() > 8 && vectorToTarget != Vector2.Zero)
                    {
                        Projectile.velocity = vectorToTarget / vectorToTarget.Length() * speed;
                    }
                    else
                    {
                        Projectile.velocity = vectorToTarget;
                    }
                }
                else
                {
                    rotateToTarget = false;
                    if (Projectile.velocity.Length() < speed)
                    {
                        vectorToTarget.Normalize();
                        Projectile.velocity += vectorToTarget * 0.5f;
                        if (distanceFromTarget <= 140f)
                            Projectile.velocity *= 0.02f;
                    }
                    else
                    {
                        Projectile.velocity *= speed / Projectile.velocity.Length() * 0.99f;
                    }
                }
            }
            else
            {
                if (!Collision.CanHitLine(Projectile.position, Projectile.height, Projectile.width, Projectile.position + Projectile.velocity * 30, Projectile.height, Projectile.width))
                {
                    Projectile.velocity = Functions.Rotate(Projectile.velocity, 3);
                }
                if (Projectile.velocity.Length() < speed * 0.6f)
                {
                    Projectile.velocity += Projectile.velocity / Projectile.velocity.Length() * 0.5f;
                }
                if (Projectile.velocity.Length() > speed * 0.6f)
                {
                    Projectile.velocity -= Projectile.velocity / Projectile.velocity.Length() * 0.5f;
                }
            }
        }
        private void AIRotating(Vector2 vectorToTarget, bool foundTarget)
        {
            if (Projectile.rotation >= 2 * Math.PI)
                Projectile.rotation -= Convert.ToSingle(2 * Math.PI);
            if (Projectile.rotation < 0)
                Projectile.rotation += Convert.ToSingle(2 * Math.PI);
            if (!rotateToTarget)
            {
                if (foundTarget)
                {
                    float goal = Functions.getAngel(vectorToTarget);
                    if (goal - Projectile.rotation < Math.PI || goal - Projectile.rotation < Math.PI)
                    {
                        if (Projectile.rotation + 0.15f < goal)
                        {
                            Projectile.rotation += 0.15f;
                        }
                        else
                        {
                            Projectile.rotation = goal;
                        }
                    }
                    else
                    {
                        if (Projectile.rotation - 0.15f > goal)
                        {
                            Projectile.rotation -= 0.15f;
                        }
                        else
                        {
                            Projectile.rotation = goal;
                        }
                    }
                }
                else
                {
                    if (Projectile.velocity.Y < 5)
                        Projectile.velocity.Y += 0.075f;
                    float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
                    if (Projectile.velocity.Y < 0)
                        angel = 2 * Convert.ToSingle(Math.PI) - angel;
                    Projectile.rotation = angel;
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SparkForLightDisc);
            }
        }
    }
}