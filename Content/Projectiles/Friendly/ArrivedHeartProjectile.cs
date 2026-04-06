using Terrapain.Common.Global;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class ArrivedHeartProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            int count = 20;
            Vector2 circleCenter = new Vector2(0.5f, -0.5f);
            float circleRadius = 0.5f;
            Vector2 bottom = new Vector2(0, 1);
            Vector2 transitionPoint = RightTriangle(bottom, circleCenter, circleRadius);
            float maxAngle = MathF.PI + (transitionPoint - circleCenter).ToRotation();
            float perimeter = circleRadius * maxAngle + bottom.Distance(transitionPoint);
            float step = perimeter / count;
            float stepsForCircle = circleRadius * maxAngle / perimeter * count;
            float stepOnCircle = maxAngle / stepsForCircle;
            Vector2[] vertices = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                if (i < stepsForCircle)
                {
                    vertices[i] = circleCenter - Vector2.UnitX.RotatedBy(stepOnCircle * i) * circleRadius;
                }
                else
                {
                    vertices[i] = transitionPoint + (bottom - transitionPoint) * ((i - stepsForCircle) / (count - 1 - stepsForCircle));
                }
            }
            HeartVertices = new Vector2[count * 2 - 2];
            for (int i = 0; i < count * 2 - 2; i++)
            {
                if (i < count)
                {
                    HeartVertices[i] = vertices[i];
                }
                else
                {
                    Vector2 vec = vertices[i - count + 1];
                    vec.X *= -1;
                    HeartVertices[i] = vec;
                }
            }
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 4;
            Projectile.GetT().drawTrail = true;
            Projectile.GetT().trailColor = Color.LightPink * 0.4f;
            Projectile.GetT().trailWidth = 20;
            Projectile.GetT().trailLength = 10;

            //{
            //    int count = 20;
            //    Vector2 circleCenter = new Vector2(0.5f, -0.5f);
            //    float circleRadius = 0.5f;
            //    Vector2 bottom = new Vector2(0, 1);
            //    Vector2 transitionPoint = RightTriangle(bottom, circleCenter, circleRadius);
            //    float maxAngle = MathF.PI + (transitionPoint - circleCenter).ToRotation();
            //    float perimeter = circleRadius * maxAngle + bottom.Distance(transitionPoint);
            //    float step = perimeter / count;
            //    float stepsForCircle = circleRadius * maxAngle / perimeter * count;
            //    float stepOnCircle = maxAngle / stepsForCircle;
            //    Vector2[] vertices = new Vector2[count];
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (i < stepsForCircle)
            //        {
            //            vertices[i] = circleCenter - Vector2.UnitX.RotatedBy(stepOnCircle * i) * circleRadius;
            //        }
            //        else
            //        {
            //            vertices[i] = transitionPoint + (bottom - transitionPoint) * ((i - stepsForCircle) / (count - 1 - stepsForCircle));
            //        }
            //    }
            //    HeartVertices = new Vector2[count * 2 - 2];
            //    for (int i = 0; i < count * 2 - 2; i++)
            //    {
            //        if (i < count)
            //        {
            //            HeartVertices[i] = vertices[i];
            //        }
            //        else
            //        {
            //            Vector2 vec = vertices[i - count + 1];
            //            vec.X *= -1;
            //            HeartVertices[i] = vec;
            //        }
            //    }
            //}
        }
        public static Vector2[] HeartVertices;
        int target;
        int oldTarget = -1;
        bool FoundTarget;
        static UnifiedRandom random = new UnifiedRandom();
        public override void OnSpawn(IEntitySource source)
        {
            foreach(var vec in HeartVertices)
            {
                int d = Dust.NewDust(Projectile.Center + vec * 15, 0, 0, ModContent.DustType<PinkHeart>());
                Main.dust[d].velocity = vec * 2;
                Main.dust[d].rotation = MathF.PI / 2;
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity == Vector2.Zero? 0 : Projectile.velocity.ToRotation();
            if (FoundTarget)
            {
                if (Main.npc[target].active)
                {
                    Vector2 vectorToTargetPosition = Main.npc[target].Center - Projectile.Center;
                    Projectile.velocity += vectorToTargetPosition.Normalized() * (0.015f + Projectile.velocity.Length());
                    float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
                    positiveRotation = NormalizeRotation(positiveRotation);
                    float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
                    negativeRotation = NormalizeRotation(negativeRotation);
                    if (positiveRotation > negativeRotation)
                    {
                        Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.025f));
                    }
                    else
                    {
                        Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.025f));
                    }
                    if (Projectile.velocity.Length() > 15)
                    {
                        Projectile.velocity = Projectile.velocity.Normalized() * 15;
                    }
                }
            }
            else if (Projectile.timeLeft % 6 == 0)
            {
                AISearchForTarget(out FoundTarget, out target);
            }
            if (Projectile.timeLeft % 4 == 0 && random.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<PinkHeart>());
                Main.dust[d].velocity = Projectile.velocity.Normalized() * 0.5f;
                Main.dust[d].rotation = Projectile.rotation;
                Main.dust[d].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 0.8f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            oldTarget = target.whoAmI;
            FoundTarget = false;
        }
        public override void OnKill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Terraria.Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            //SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            foreach (var vec in HeartVertices)
            {
                int d = Dust.NewDust(Projectile.Center + vec * 15, 0, 0, ModContent.DustType<PinkHeart>());
                Main.dust[d].velocity = vec * 2;
                Main.dust[d].rotation = MathF.PI / 2;
            }
        }
        private void AISearchForTarget(out bool foundTarget, out int target)
        {
            float distanceFromTarget = 700f;
            Vector2 targetCenter = Projectile.position;
            target = -1;
            foundTarget = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy && npc.whoAmI != oldTarget)
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center + Projectile.velocity * 30);
                    bool closest = Vector2.Distance(Projectile.Center + Projectile.velocity * 30, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = SimpleColision(Projectile.Center, npc.position - (npc.position - Projectile.Center) / (npc.position - Projectile.Center).Length() * 60, npc.width, npc.height);

                    if (((closest && inRange) || !foundTarget) && lineOfSight)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        target = npc.whoAmI;
                        foundTarget = true;
                    }
                }
            }
        }
    }
}
