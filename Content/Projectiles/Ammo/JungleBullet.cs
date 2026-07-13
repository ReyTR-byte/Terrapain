using Terrapain.Common.Global;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Ammo
{
    public class JungleBullet: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.2f;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 4;
        }
        int target;
        bool FoundTarget;
        static UnifiedRandom random = new UnifiedRandom();
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (FoundTarget)
            {
                if (Main.npc[target].active)
                {
                    Vector2 vectorToTargetPosition = Main.npc[target].Center - Projectile.Center;
                    float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
                    positiveRotation = NormalizeRotation(positiveRotation);
                    float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
                    negativeRotation = NormalizeRotation(negativeRotation);
                    if (positiveRotation > negativeRotation)
                    {
                        Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.006f));
                    }
                    else
                    {
                        Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.006f));
                    }
                }
            }
            else if (Projectile.timeLeft % 6 == 0)
            {
                AISearchForTarget(out FoundTarget, out target);
            }
            if (Projectile.timeLeft % 5 == 0 && random.NextBool(6))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleSpore);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Terraria.Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
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
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy)
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center + Projectile.velocity * 60);
                    bool closest = Vector2.Distance(Projectile.Center + Projectile.velocity * 60, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = CanHit(Projectile.Center, npc.position - (npc.position - Projectile.Center) / (npc.position - Projectile.Center).Length() * 60, npc.width, npc.height);

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
