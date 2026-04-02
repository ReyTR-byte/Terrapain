using Luminance.Common.Utilities;
using Terrapain.Common.System;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Servants.EyeofCthulhu
{
    public class FireShooterServantofCthulhu : ModNPC
    {
        public int AIStyle;
        int owner;
        NPC Owner { get => Main.npc[owner]; }
        int animationSpeed = 12;
        int animationtimer;
        int frame;
        float maxVelocity = 7;
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
                NPC.frame = new Rectangle(0, frame * 32, 46, 32);
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
            NPC.damage = 14;
            NPC.defense = 0;
            NPC.lifeMax = 40;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.GetT().drawCenter = new Vector2(20, 16);
            NPC.GetT().useModDrawingInPreDraw = true;
            NPC.GetT().useVanillaDrawing = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
            rotation = NPC.AngleTo(NPC.GetT().Target.Center);
            if (Group.FindGroup("FireShooterServantofCthulhu").Count == 0)
            {
                Group.NewGroup(new Groups.FireShooterServantofCthulhu());
            }
            owner = ((EntitySource_Parent)source).Entity.whoAmI;
            if (source.Context == "fly straight")
            {
                NPC.ai[0] = 200;
                NPC.knockBackResist = 0;
                AIStyle = 1;
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
            float targetRotation = 0;
            Vector2 targetPosition = Vector2.Zero;
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
                    NPC.TargetClosest(false);
                        if (NPC.life >= NPC.lifeMax * 0.35f)
                        {
                            targetPosition = NPC.GetT().Target.Center + NPC.GetT().Target.velocity * 20 + (NPC.Center - NPC.GetT().Target.Center - NPC.GetT().Target.velocity * 20).ToUnit(new Vector2(0, NPC.spriteDirection)).RotatedBy(0.2f * NPC.ai[1]) * 200;
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
                        NPC.velocity += NPC.DirectionTo(targetPosition) * 1.2f;
                        targetRotation = NPC.DirectionTo(NPC.GetT().Target.Center).ToRotation();
                        if (NPC.ai[0] <= 0 && NPC.GetT().Target.Distance(NPC.Center) < 200)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + rotation.ToRotationVector2() * 16, NPC.rotation.ToRotationVector2() * NPC.spriteDirection * 5, ProjectileID.FlamesTrap, 14, 1);
                            Main.projectile[proj].friendly = false;
                            NPC.ai[0] = 4;
                        }
                        NPC.ai[0]--;
                    break;
                case 1:
                    if (NPC.ai[0] == 200)
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - MathF.PI * 0.5f;
                    }
                    flag = false;
                    NPC.ai[0]--;
                    rotation += MathF.PI / 7.5f;
                    if ((int)NPC.ai[0] % 2 == 0)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + rotation.ToRotationVector2() * 16, NPC.rotation.ToRotationVector2() * NPC.spriteDirection * (2.5f + WorldDifficultySystem.TerrapainDifficulty * 0.5f), ProjectileID.FlamesTrap, 14, 1);
                        Main.projectile[proj].friendly = false;
                        NPC.ai[0] = 4;
                    }
                    if (NPC.ai[0] <= 0)
                    {
                        AIStyle = -1;
                    }
                    break;
            }
            if (flag)
            {
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
                AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, targetRotation, ref rotation, NPC.GetT().Target.Distance(NPC.Center) > 200);
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
