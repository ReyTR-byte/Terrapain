using ILGPU.IR.Values;
using ReLogic.Reflection;
using Terrapain.Content.TUtilities;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Ammo.Arrows.Bouquet
{
    public class SomeFlower : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = false;
            Main.projPet[Projectile.type] = false;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 900;
        }
        public override void OnSpawn(IEntitySource source)
        {
            chain = new SimulatedChain(2, 24, Projectile.position, 0, 1, MathF.PI, -0.1f, 0.94f);
            int len = chain.Fragments.Length - 1;
            chain.Fragments[len].breaking = 1;
            chain.Fragments[len].mass = 25;
            chain.Fragments[len].velocity = Projectile.velocity;
            chain.Fragments[len].accessfulRotation = MathF.PI;
            Projectile.velocity = Vector2.Zero;

            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == Type && proj.owner == Projectile.owner)
                {
                    (proj.ModProjectile as SomeFlower).spaceLeft--;
                }
            }
            spaceLeft = 3;
        }
        public int spaceLeft;
        int target;
        bool FoundTarget;
        float angularVelocity;
        SimulatedChain chain;
        public override void AI()
        {
            Player own = Main.player[Projectile.owner];
            int len = chain.Fragments.Length - 1;
            if (spaceLeft <= 0)
            {
                Projectile.timeLeft = Math.Min(59, Projectile.timeLeft);
            }
            if (Projectile.timeLeft > 60)
            {    
                if (Projectile.ai[0] <= 0)
                {
                    Projectile.rotation = Projectile.velocity == Vector2.Zero? 0 : Projectile.velocity.ToRotation();
                    if (FoundTarget)
                    {
                        if (Main.npc[target].active && own.Distance(Main.npc[target].Center) < 440)
                        {
                            Vector2 targetPosition = Main.npc[target].Center;
                            AIHelper.CommonTerrapainFlyingMovement(chain.Fragments[len].position, ref chain.Fragments[len].velocity, targetPosition, 0.3f, 25f, 0.35f, 75);
                        }
                        else
                        {
                            FoundTarget = false;
                        }
                    }
                    else
                    {
                        GoToPlayer();
                        if (Projectile.timeLeft % 6 == 0)
                        {
                            AISearchForTarget(out FoundTarget, out target);
                        }
                    }
                }
                else
                {
                    Projectile.ai[0]--;
                    GoToPlayer();
                }
            }

            if (chain.Fragments.Length == 20)
            {
                chain.Fragments[0].fixedAt = Main.player[Projectile.owner].Center;
            }
            else if (chain.Fragments[0].position.Distance(own.Center) >= chain.Fragments[0].length)
            {
                List<SimulatedJoint> joints = [new SimulatedJoint(chain.Fragments[0].length, 1, own.Center, chain.Fragments[0].accessfulRotation, chain.Fragments[0].breaking)];
                joints.AddRange(chain.Fragments);
                chain.Fragments = joints.ToArray();
                len++;
            }
            chain.Update();
            if (Projectile.timeLeft <= 60)
            {
                chain.gravity = new Vector2(0, 0.3f);
                Projectile.alpha = (int)(255 * ((60 - Projectile.timeLeft) / 60f));
            }
            Projectile.rotation = chain.Fragments[len - 1].position.DirectionTo(chain.Fragments[len].position).ToRotation();
            Projectile.Center = chain.Fragments[len].position;
        }
        void GoToPlayer()
        {
            Player own = Main.player[Projectile.owner];
            int len = chain.Fragments.Length - 1;
            int num = 3 - spaceLeft;
            Vector2 targetPosition = own.Center + new Vector2((200 - 40 * num) * own.direction, -300 + 20 * num);
            AIHelper.CommonTerrapainFlyingMovement(chain.Fragments[len].position, ref chain.Fragments[len].velocity, targetPosition, 0.3f, 25f, 0.35f, 150);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 60;
            
            FoundTarget = false;
        }
        private void AISearchForTarget(out bool foundTarget, out int target)
        {
            float distanceFromTarget = 440f;
            Vector2 targetCenter = Projectile.position;
            Player own = Main.player[Projectile.owner];
            target = -1;
            foundTarget = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy)
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center + Projectile.velocity * 30);
                    bool closest = Vector2.Distance(Projectile.Center + Projectile.velocity * 30, targetCenter) > between;
                    bool inRange = between < distanceFromTarget && own.Center.Distance(npc.Center) < distanceFromTarget;
                    bool lineOfSight = CanHit(Projectile.Center, npc.position - (npc.position - Projectile.Center) / (npc.position - Projectile.Center).Length() * 60, npc.width, npc.height);

                    if (((closest && inRange) || !foundTarget) && lineOfSight)
                    {
                        targetCenter = npc.Center;
                        target = npc.whoAmI;
                        foundTarget = true;
                    }
                }
            }
        }
        public override bool PreDrawExtras()
        {
            chain.DrawAsLines(Main.spriteBatch, 8, Color.White * ((255f - Projectile.alpha) / 255f));
            return true;
        }
    }
}