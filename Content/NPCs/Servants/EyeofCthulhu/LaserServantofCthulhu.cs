using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Config;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies;
using Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Servants.EyeofCthulhu
{
    public class LaserServantofCthulhu : ModNPC
    {
        public int AIStyle;
        int owner;
        NPC Owner { get => Main.npc[owner]; }
        int animationSpeed = 12;
        int animationtimer;
        int frame;
        float maxVelocity = 10;
        float angularVelocity;
        float maxAngularVelocity = 0.08f;
        float angularAcceleration = 0.01f;
        float rotation;
        public override void FindFrame(int frameHeight)
        {
            if (animationtimer == 0)
            {
                frame++;
                if (frame == 2)
                {
                    frame = 0;
                }
                NPC.frame = new Rectangle(0, frame * 20, 40, 20);
                animationtimer = animationSpeed;
            }
            animationtimer--;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.GetT().drawCenter = new Vector2(20, 10);
            NPC.GetT().useModDrawingInPostDraw = true;
            NPC.GetT().useVanillaDrawing = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            rotation = NPC.AngleTo(NPC.GetT().Target.Center);
            owner = ((EntitySource_Parent)source).Entity.whoAmI;
            if (source.Context == null)
            {   
                NPC.ai[2] = 1;
                NPC.TargetClosest(false);
                if (Group.FindGroup("LaserServantofCthulhu").Count == 0)
                {
                    Group.NewGroup(new Groups.LaserServantofCthulhu());
                }
            }
            if (source.Context == "grid AI")
            {
                NPC.ai[3] = 100;
                NPC.ai[0] = -1;
                if (Group.FindGroup("GridLaserServants").Count == 0)
                {
                    Group.NewGroup(new GridLaserServants());
                }
                Terrapain.group[Group.FindGroup("GridLaserServants")[0]].AddMember(NPC.whoAmI);
                AIStyle = 1;
            }
            if (source.Context == "fly straight")
            {
                NPC.ai[0] = 200;
                NPC.knockBackResist = 0;
                AIStyle = 2;
            }
        }
        public override void AI()
        {
            if (AIStyle != -2)
            {
                if (!Owner.active || Owner.type != NPCID.EyeofCthulhu)
                {
                    AIStyle = -2;
                }
                else if (Owner.ai[0] == -201)
                {
                    AIStyle = -1;
                }
            }
            float maxVelocityMultyplier = 1;
            Vector2 targetPosition = Vector2.Zero;
            float targetRotation = 0;
            bool flag = true;
            switch (AIStyle)
            {
                case -2:
                    targetPosition = NPC.Center - Vector2.UnitY;
                    targetRotation = -MathF.PI / 2;
                    break;
                case -1:
                    targetPosition = Owner.Center;
                    targetRotation = (Owner.Center - NPC.Center).ToRotation();
                    if (NPC.Distance(Owner.Center) < 50)
                    {
                        NPC.active = false;
                    }
                    break;
                case 0:
                    Vector2 testpos = NPC.Center - NPC.GetT().Target.Center;
                    if (testpos.RotatedBy(-NPC.ai[1]).Y * NPC.ai[2] > 300)
                    {
                        NPC.ai[2] *= -1;
                    }
                    NPC.TargetClosest(false);
                    if (!NPC.GetT().Target.active || NPC.GetT().Target.dead)
                    {
                        targetPosition = NPC.Center - Vector2.UnitY;
                        targetRotation = MathF.PI / 2;
                    }
                    else
                    {
                        if (NPC.life >= NPC.lifeMax * 0.35f)
                        {
                            targetPosition = NPC.GetT().Target.Center + NPC.GetT().Target.velocity * 20 + Vector2.UnitX.RotatedBy(NPC.ai[1]) * 300 + Vector2.UnitY.RotatedBy(NPC.ai[1]) * NPC.ai[2] * 300;
                        }
                        else
                        {
                            if ((int)NPC.ai[3] < 0 || (int)NPC.ai[3] >= Main.npc.Length || !Main.npc[(int)NPC.ai[3]].active || Main.npc[(int)NPC.ai[3]].type != ModContent.NPCType<HealerServantofCthulhu>())
                            {
                                NPC doctor = FindClosestNPC(NPC.Center, null, ModContent.NPCType<HealerServantofCthulhu>());
                                if (doctor != null)
                                {
                                    targetPosition = doctor.Center;
                                    targetPosition += targetPosition.DirectionTo(NPC.GetT().Target.Center) * 100;
                                    if (NPC.Distance(targetPosition) < 75)
                                    {
                                        maxVelocityMultyplier = 1 - (75 - NPC.Distance(targetPosition)) / 75;
                                    }
                                }
                                else
                                {
                                    targetPosition = NPC.GetT().Target.Center + NPC.GetT().Target.velocity * 20 + (NPC.Center - NPC.GetT().Target.Center - NPC.GetT().Target.velocity * 20).ToUnit(new Vector2(0, NPC.spriteDirection)).RotatedBy(0.2f * NPC.ai[1]) * 200;
                                }
                            }
                            else
                            {
                                NPC doctor = Main.npc[(int)NPC.ai[3]];
                                targetPosition = doctor.Center;
                                targetPosition += targetPosition.DirectionTo(NPC.GetT().Target.Center) * 100;
                                if (NPC.Distance(targetPosition) < 75)
                                {
                                    maxVelocityMultyplier = 1 - (75 - NPC.Distance(targetPosition)) / 75;
                                }
                            }
                        }
                        if (NPC.ai[0] <= 0)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + rotation.ToRotationVector2() * 20, NPC.rotation.ToRotationVector2() * NPC.spriteDirection * 5, ModContent.ProjectileType<DemonicEyeLazer>(), 14, 1);
                            Main.projectile[proj].friendly = false;
                            Main.projectile[proj].hostile = true;
                            NPC.ai[0] = 25;
                        }
                        NPC.ai[0]--;
                        targetRotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation();
                    }
                    break;
                case 1:
                    if (NPC.ai[0] != -1)
                    {
                        targetPosition = new Vector2(NPC.ai[0], NPC.ai[1]);
                        targetRotation = NPC.ai[2];
                        maxVelocityMultyplier = 2.5f;
                        if (NPC.Distance(targetPosition) < 125)
                        {
                            maxVelocityMultyplier = 2.5f - (125 - NPC.Distance(targetPosition)) / 50;
                        }
                    }
                    else
                    {
                        NPC.TargetClosest(false);
                        targetPosition = Owner.Center + Vector2.UnitX.RotatedBy(NPC.DirectionTo(Owner.Center).ToRotation()) * 75;
                        targetRotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation();
                        if (NPC.Distance(targetPosition) < 75)
                        {
                            maxVelocityMultyplier = 1 - (75 - NPC.Distance(targetPosition)) / 75;
                        }
                    }
                    break;
                case 2:
                    if (NPC.ai[0] == 200)
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - MathF.PI * 0.5f;
                    }
                    flag = false;
                    NPC.ai[0]--;
                    rotation += MathF.PI / 15;
                    if ((int)NPC.ai[0] % 5 == 0)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.rotation.ToRotationVector2() * 20, NPC.rotation.ToRotationVector2() * NPC.spriteDirection * 5, ModContent.ProjectileType<DemonicEyeLazer>(), 14, 1);
                        Main.projectile[proj].friendly = false;
                        Main.projectile[proj].hostile = true;
                    }
                    if (NPC.ai[0] <= 0)
                    {
                        AIStyle = -1;
                    }
                    break;
            }
            if (flag)
            {
                NPC.velocity += NPC.DirectionTo(targetPosition) * 1.2f;
                Vector2 vectorToTargetPosition = targetPosition - NPC.Center;
                float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, NPC.velocity);
                positiveRotation = NormalizeRotation(positiveRotation);
                float negativeRotation = AngleBetweenVectors(NPC.velocity, vectorToTargetPosition);
                negativeRotation = NormalizeRotation(negativeRotation);
                if (positiveRotation > negativeRotation)
                {
                    NPC.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                }
                else
                {
                    NPC.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                }
                if (NPC.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                {
                    NPC.velocity = NPC.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                }
                AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, targetRotation, ref rotation);
            }
            NPC.rotation = rotation;
            NPC.spriteDirection = NPC.rotation.ToRotationVector2().X.NonZeroSign();
            if (NPC.spriteDirection == -1)
            {
                NPC.rotation = (NPC.rotation.ToRotationVector2() * -1).ToRotation();
            }
        }
        public override void OnKill()
        {
            if (!Main.getGoodWorld)
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.ProjectileType<ServantofCthulhuSoul>(), ServantofCthulhuSoul.defDamage, 0, ai0: owner);
            }
            else
            {
                Projectile.NewProjectile(NPC.GetSource_Death(owner.ToString()), NPC.Center, NPC.velocity, ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AIStyle == 0;
        }
    }
}
