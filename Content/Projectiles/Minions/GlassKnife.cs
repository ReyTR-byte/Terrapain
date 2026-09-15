using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terrapain.Content.Buffs;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.Global;
using Terrapain.Common.Global.Trails;
using Terrapain.Content.TUtilities;
using ReLogic.Reflection;

namespace Terrapain.Content.Projectiles.Minions
{
    public class GlassKnife : ModProjectile
    {
        int damageBoost = 0;
        int dashTime = 0;
        bool rotateToTarget = false;
        bool dash = false;
        Vector2 oldTarget;
        int charge;
        bool canCharge;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 14;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 60;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.GetT().drawCenter = new Vector2(18);
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().trail = new MultipleTrail()
            {
                trails = [new SwordTrail()
                {
                    Top = new Vector2(18, -18),
                    Bottom = new Vector2(-5, 5),
                    startColor = Color.White,
                    endColor = Color.Transparent,
                    length = 0,
                }]
            };
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool MinionContactDamage()
        {
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.velocity = Vector2.Zero;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (canCharge && dash)
            {
                charge++;
                Projectile.frame++;
            }
            if (charge == 14)
            {
                target.AddBuff(BuffID.OnFire, 500);
                damageBoost = 500;
                Projectile.frame = 0;
                charge = 0;
            }
        }
        Player owner;
        public override void AI()
        {
            if (damageBoost == 500)
                Projectile.damage = Convert.ToInt32(Convert.ToDouble(Projectile.damage) * 1.5);
            if (damageBoost == 0)
                Projectile.damage = Convert.ToInt32(Convert.ToDouble(Projectile.damage) / 1.5);
            if (damageBoost > 0)
            {
                Lighting.AddLight(Projectile.position, 3, 3, 0);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }
            damageBoost--;
            owner = Main.player[Projectile.owner];
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<GlassKnifeBuff>());
                return;
            }
            if (owner.HasBuff(ModContent.BuffType<GlassKnifeBuff>()))
                Projectile.timeLeft = 2;
            AIGeneral(owner, out Vector2 VectorToIdlePosition, out float distanceToIdlePosition);
            AISearchForTarget(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Vector2 VectorToTarget = targetCenter - Projectile.Center;
            AIRotating(VectorToTarget, foundTarget, VectorToIdlePosition);
            AIMovment(foundTarget, VectorToTarget, distanceFromTarget, VectorToIdlePosition);
            TrailManagement();
        }
        private void AIGeneral(Player owner, out Vector2 VectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idelPosition = owner.GetModPlayer<TerrapainPlayer>().oldCenters[3 + 3 * Projectile.minionPos];
            float minionPositionOffset = (30 + Projectile.minionPos * 20) * -owner.direction;
            idelPosition.X += minionPositionOffset;

            VectorToIdlePosition = idelPosition - Projectile.Center;
            distanceToIdlePosition = VectorToIdlePosition.Length();
            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idelPosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
                Projectile.GetT().trail = new MultipleTrail()
                {
                    trails = [new SwordTrail()
                    {
                        Top = new Vector2(18, -18),
                        Bottom = new Vector2(-5, 5),
                        startColor = Color.White,
                        endColor = Color.Transparent,
                        length = 0,
                    }]
                };
            }
        }
        private void AISearchForTarget(Player owner,
                                       out bool foundTarget,
                                       out float distanceFromTarget,
                                       out Vector2 targetCenter)
        {
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);
                if (between < 1200f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

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
                        bool lineOfSight = Functions.CanHit(Projectile.Center, npc.position, npc.width, npc.height) || Functions.CanHit(owner.Center, npc.position, npc.width, npc.height);

                        bool closeThroughWall = between < 100f;

                        if (inRange && (closest || !foundTarget) && (lineOfSight || closeThroughWall))
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
        bool oldDash;
        private void AIMovment(bool foundTarget,
                               Vector2 vectorToTarget,
                               float distanceFromTarget,
                               Vector2 vectorToIdelPosition)
        {
            int speed = 16;
            if (dashTime <= 0 && dash)
            {
                rotateToTarget = false;
                oldTarget = Vector2.Zero;
                Projectile.velocity *= 0.02f;
            }
            if (rotateToTarget && !dash)
            {
                dashTime = 20;
                canCharge = true;
            }
            if (damageBoost > 0)
                canCharge = false;
            if (foundTarget || (dash && dashTime > 0))
            {
                if (distanceFromTarget < 160f && dashTime > 0 && rotateToTarget)
                {
                    if (dash)
                        oldDash = true;
                    dash = true;
                    dashTime--;
                    if (Projectile.velocity.Length() < speed * 2)
                    {
                        vectorToTarget.Normalize();
                        if (!oldDash)
                        {
                            oldTarget = vectorToTarget;
                            Projectile.velocity = Vector2.Zero;
                        }
                        Projectile.velocity += oldTarget * 5;
                    }
                    Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
                    Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 15;
                }
                else
                {
                    Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = false;
                    rotateToTarget = false;
                    if (!dash)
                        oldDash = false;
                    dash = false;
                    dashTime = 0;
                    if (Projectile.velocity.Length() < speed)
                    {
                        vectorToTarget.Normalize();
                        Projectile.velocity += vectorToTarget * 2;
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
                Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = false;
                if (!dash)
                {
                    rotateToTarget = false;
                    if (vectorToIdelPosition.Length() * 0.2f > Convert.ToSingle(speed))
                    {
                        vectorToIdelPosition.Normalize();
                        Projectile.velocity = vectorToIdelPosition * speed;
                    }
                    else
                        Projectile.velocity = vectorToIdelPosition * 0.2f;
                }
            }
        }
        float angularVelocity;
        private void AIRotating(Vector2 vectorToTarget, bool foundTarget, Vector2 vectorToIdelPosition)
        {
            if (Projectile.rotation >= 2 * Math.PI)
                Projectile.rotation -= Convert.ToSingle(2 * Math.PI);
            if (!rotateToTarget)
            {
                if (foundTarget)
                {
                    float goalAngle = Projectile.AngleTo(Projectile.Center + vectorToTarget) + (float)Math.PI * 0.25f;
                    rotateToTarget = AIHelper.AngularAcceleration(ref angularVelocity, 0.1f, 0.3f, goalAngle, ref Projectile.rotation);
                }
                else
                {
                    if (vectorToIdelPosition.Length() > 150)
                    {
                        float goalAngle = Projectile.AngleTo(Projectile.Center + Projectile.velocity) + (float)Math.PI * 0.25f;
                        rotateToTarget = AIHelper.AngularAcceleration(ref angularVelocity, 0.1f, 0.3f, goalAngle, ref Projectile.rotation);
                    }
                    else
                    {
                        float goalAngle = Projectile.AngleTo(Projectile.Center + Vector2.UnitY) + (float)Math.PI * 0.25f;
                        rotateToTarget = AIHelper.AngularAcceleration(ref angularVelocity, 0.03f, 0.3f, goalAngle, ref Projectile.rotation);
                    }
                }
            }
        }
        void TrailManagement()
        {
            List<ProjectileTrail> trails = (Projectile.GetT().trail as MultipleTrail).trails;
            if (dash)
            {   
                if (trails.Count == 1)
                {    
                    trails.Add(new ProjectileTrail
                    {
                        startColor = Color.Transparent,
                        endColor = Color.Transparent,
                        Offset = new Vector2(16, -16),
                        smooth = false,
                        length = 10,
                        startWidth = 18,
                        endWidth = 18
                    });
                }
                else
                {
                    trails[1].length = 10;
                }
                trails[1].startColor = new Color(trails[1].startColor.ToVector4() + new Vector4(0.1f));
            }
            else if (!dash && trails.Count == 2)
            {
                trails[1].startColor = new Color(trails[1].startColor.ToVector4() - new Vector4(0.1f));
                trails[1].length--;
                if (trails[1].length < 2)
                {
                    trails.RemoveAt(1);
                }
            }
            float rotation = Projectile.rotation + MathF.PI * 0.25f;
            rotation -= Projectile.velocity.ToRotation();
            float s = Projectile.velocity.Length() * MathF.Abs(MathF.Cos(rotation));
            trails[0].startColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()) * (s / 10);
            trails[0].length = (int)MathHelper.Clamp(s * 1.5f, 2, 15);
        }
    }
}