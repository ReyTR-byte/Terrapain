using Terrapain.Common.Global;
using Terrapain.Content.DamageClasses;
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
    public class Nugget : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<Unarmed>();
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 4;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().trail = new()
            {
                startColor = Color.Orange * 0.6f,
                endColor = Color.Red * 0.4f,
                length = 15,
                startWidth = 20
            };
        }
        int target;
        int oldTarget = -1;
        bool FoundTarget;
        static UnifiedRandom random = new UnifiedRandom();
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity == Vector2.Zero? 0 : Projectile.velocity.ToRotation();
            if (FoundTarget)
            {
                if (Main.npc[target].active)
                {
                    Vector2 vectorToTargetPosition = Main.npc[target].Center - Projectile.Center;
                    Projectile.velocity = Projectile.velocity.Normalized() * (0.1f + Projectile.velocity.Length());
                    float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
                    positiveRotation = NormalizeRotation(positiveRotation);
                    float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
                    negativeRotation = NormalizeRotation(negativeRotation);
                    if (positiveRotation > negativeRotation)
                    {
                        Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.05f));
                    }
                    else
                    {
                        Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.05f));
                    }
                    if (Projectile.velocity.Length() > 15)
                    {
                        Projectile.velocity = Projectile.velocity.Normalized() * 15;
                    }
                }
            }
            else
            {
                if (Projectile.timeLeft % 6 == 0)
                {
                    AISearchForTarget(out FoundTarget, out target);
                }
            }
            if (Projectile.timeLeft % 4 == 0 && random.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Torch, Scale: 2.5f);
                Main.dust[d].velocity = Projectile.velocity.Normalized() * 0.5f;
                Main.dust[d].rotation = Projectile.rotation;
                Main.dust[d].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.33f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
            oldTarget = target.whoAmI;
            FoundTarget = false;
        }
        public override void OnKill(int timeLeft)
        {
            Terraria.Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
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