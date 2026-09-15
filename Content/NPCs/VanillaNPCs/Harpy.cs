using Terraria;
using Terraria.ID;
using static Terrapain.Content.Functions;
using static Terrapain.Content.TUtilities.AIHelper;
using Terraria.ModLoader;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria.DataStructures;
using Terrapain.Common.System;

namespace Terrapain.Content.NPCs.VanillaNPCs
{
    public class Harpy : NPCBehaviour
    {
        public override int type => NPCID.Harpy;
        public override void ModSetDefaults(NPC entity)
        {
            entity.noGravity = true;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source is MiniBossSpawnSystem.MiniBossSpawnSource)
            {
                miniBoss = ((MiniBossSpawnSystem.MiniBossSpawnSource)source).spawnInfo;
                Power = miniBoss.power;
            }
            else
            {
                Power = random.NextFloat(WorldDifficultySystem.suicide? 0.8f : 0.5f, WorldDifficultySystem.suicide? 2.5f : 2f);
            }
            if (Main.hardMode)
            {
                Power *= 2;
            }
            npc.lifeMax = (int)(npc.lifeMax * (1 + Power * Power) * 0.8f);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * Power);
        }
        MiniBossSpawnSystem.MiniBossSpawnInfo miniBoss;
        float Power;
        public override bool ModPreAI(NPC npc)
        {
            if (CheckTarget(npc))
            {
                npc.TargetClosest();
            }
            else
            {
                Movement(npc);
                Attack(npc);
            }
            return false;
        }
        void Movement(NPC npc)
        {
            var scan = npc.ScanAround(5);
            if ((npc.ai[0] != 2 && npc.ai[0] != 3) || npc.ai[2] == 0)
            {
                npc.knockBackResist = 0.1f;
                var Points = npc.FindPath(t.Target.Center, 50);//AStarPathfinder.FindPath(npc.Center.ToTileCoordinates(), t.Target.Center.ToTileCoordinates(), IsWakeable, 500);
                if (Points != null && Points.Count > 1)
                {
                    CommonTerrapainFlyingMovement(npc, Points[0], 0.3f, 5, 0.3f, 0f, false);
                    WallsAvoidMovement(npc, scan, 0.2f, 0.3f);
                }
                else
                {
                    CommonTerrapainFlyingMovement(npc, t.Target.Center + npc.DirectionFrom(t.Target.Center) * 75, 0.3f, 5, 0.3f, 150f, true);
                }
            }
            else
            {
                npc.knockBackResist = 0f;
                npc.velocity = npc.velocity.Normalized() * Math.Max(5, npc.velocity.Length() - 0.2f);
                WallsAvoidMovement(npc, scan, 0.2f, 0.3f);
            }
        }
        int proj => ModContent.ProjectileType<HarpyFeatherHostile>();
        int dmg => (int)(6 * Power);
        int knb = 3;
        void Attack(NPC npc)
        {
            if (Terraria.Collision.CanHit(npc, t.Target))
            {
                float sqrt = MathF.Sqrt(Power);
                switch(npc.ai[0])
                {
                    case 0:
                        npc.ai[1]--;
                        if (npc.ai[1] <= 0)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis("0"), npc.Center, random.NextVector2Unit() * 10, proj, dmg, knb, -1, 120 - npc.ai[2] * 4, npc.target);
                            npc.ai[1] = 24f / sqrt;
                            npc.ai[2]++;
                        }
                        if (npc.ai[2] > 18 * sqrt)
                        {
                            npc.ai[1] = 100 - sqrt * 12;
                            npc.ai[2] = 0;
                            npc.ai[0] = 2;
                        }
                        break;
                    case 1:
                        npc.ai[1]--;
                        if (npc.ai[1] <= 0)
                        {
                            int count = 7 + (int)sqrt;
                            int style = Power > 1.5f ? 2 : -1;
                            float agleBetween = MathF.PI / count * 2;
                            float startAngle = random.NextFloat(agleBetween);
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(style.ToString()), npc.Center, Vector2.UnitX.RotatedBy(agleBetween * i + startAngle) * 15, proj, dmg, knb, -1, npc.target);
                            }
                            npc.ai[1] = 55 / sqrt;
                            npc.ai[2]++;
                        }
                        if (npc.ai[2] > 3)
                        {
                            npc.ai[1] = 100;
                            npc.ai[2] = 0;
                            npc.ai[0] = 3;
                        }
                        break;
                    case 2:
                        if (npc.ai[1] <= 0)
                        {
                            int count = 3 + (int)(Power / 2);
                            float agleBetween = MathF.PI / count * (0.7f + (int)(Power / 2) * 0.1f);
                            int style = Power > 1.5f ? 2 : -1;
                            Vector2 dir = npc.DirectionTo(t.Target.Center);
                            float startAngle = dir.ToRotation() - agleBetween * count / 2;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(style.ToString()), npc.Center, Vector2.UnitX.RotatedBy(agleBetween * i + startAngle) * 15, proj, dmg, knb, -1, npc.target);
                            }
                            npc.velocity = dir * 20;
                            npc.ai[1] = 45;
                            npc.ai[2]++;
                        }
                        npc.ai[1]--;
                        if (npc.ai[2] > 0 && npc.ai[1] <= 0)
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 40;
                            npc.ai[2] = 0;
                        }
                        break;
                    case 3:
                        if (npc.ai[1] <= 0)
                        {
                            int count = 5 + (int)(Power / 2);
                            float agleBetween = MathF.PI / count * (0.4f + (int)(Power / 2) * 0.1f);
                            Vector2 dir = npc.DirectionTo(t.Target.Center);
                            float startAngle = (-dir).ToRotation() - agleBetween * count * 0.75f;
                            float speed = 20;
                            Vector2 target = SmartShoot(npc.Center, speed, t.Target.Center, t.Target.velocity, 30);
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis("1"), npc.Center, Vector2.UnitX.RotatedBy(agleBetween * i + startAngle) * speed / 1.3f, proj, dmg, knb, -1, 15, target.X, target.Y);
                            }
                            npc.velocity = dir * 20;
                            npc.ai[1] = 45;
                            npc.ai[2]++;
                        }
                        npc.ai[1]--;
                        if (npc.ai[2] > 0 && npc.ai[1] <= 0)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 30;
                            npc.ai[2] = 0;
                        }
                        break;
                    default:
                        npc.ai[0] = 0;
                        npc.ai[1] = 40;
                        npc.ai[2] = 0;
                        break;
                }
            }
        }
        bool CheckTarget(NPC npc)
        {
            if (npc.target < 0)
            {
                return true;
            }
            if (!t.Target.active || t.Target.dead || t.Target.Distance(npc.Center) > 2500)
            {
                return true;
            }
            return false;
        }
        public override void OnKill(NPC npc)
        {
            if (miniBoss != null)
            {
                miniBoss.defeated = true;
            }
        }
    }
}