using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class TopazStaffProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == 122;
        }
        public override bool InstancePerEntity => true;

        bool FoundTarget;
        int target;
        public override void AI(Projectile projectile)
        {
            if (FoundTarget)
            {
                if (Main.npc[target].active)
                {
                    Vector2 vectorToTargetPosition = Main.npc[target].Center - projectile.Center;
                    float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, projectile.velocity);
                    positiveRotation = NormalizeRotation(positiveRotation);
                    float negativeRotation = AngleBetweenVectors(projectile.velocity, vectorToTargetPosition);
                    negativeRotation = NormalizeRotation(negativeRotation);
                    if (positiveRotation > negativeRotation)
                    {
                        projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.0075f));
                    }
                    else
                    {
                        projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.0075f));
                    }
                }
            }
            else if (projectile.timeLeft % 6 == 0)
            {
                AISearchForTarget(out FoundTarget, out target, projectile);
            }
        }
        private void AISearchForTarget(out bool foundTarget, out int target, Projectile projectile)
        {
            float distanceFromTarget = 700f;
            Vector2 targetCenter = projectile.position;
            target = -1;
            foundTarget = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy)
                {
                    float between = Vector2.Distance(npc.Center, projectile.Center + projectile.velocity * 30);
                    bool closest = Vector2.Distance(projectile.Center + projectile.velocity * 35, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = SimpleColision(projectile.Center, npc.position - (npc.position - projectile.Center) / (npc.position - projectile.Center).Length() * 60, npc.width, npc.height);

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
