using Terrapain.Content.TUtilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Ammo.Arrows
{
    public class GoldenArrow: ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().afterimage = true;
            Projectile.GetT().afterimagesCount = 3;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Projectile.velocity.Length() * 2;
        }
        int target;
        bool FoundTarget;
        static UnifiedRandom random = new UnifiedRandom();
        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.Normalized() * MathF.Min(Projectile.velocity.Length() + 0.1f, Projectile.ai[0]);
            Projectile.velocity.Y += 0.1f;
            if (FoundTarget)
            {
                if (Main.npc[target].active)
                {
                    AIHelper.OnlyRotationalMovement(Projectile, Main.npc[target].Center, 0.008f);
                }
                if (Projectile.timeLeft % 5 == 0 && random.NextBool(6))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold);
                }
            }
            else if (Projectile.timeLeft % 6 == 0)
            {
                AISearchForTarget(out FoundTarget, out target);
            }
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2;
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