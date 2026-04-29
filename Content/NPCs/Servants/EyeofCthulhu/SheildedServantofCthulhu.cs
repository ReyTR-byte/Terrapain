using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.Auras;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Servants.EyeofCthulhu
{
    public class SheildedServantofCthulhu : ModNPC
    {
        public int AIStyle;
        int owner;
        NPC Owner { get => Main.npc[owner]; }
        SheildAura aura;
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
                NPC.frame = new Rectangle(0, frame * 20, 32, 20);
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
            NPC.damage = 8;
            NPC.defense = 10;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.GetT().drawCenter = new Vector2(20, 10);
            NPC.GetT().useModDrawingInPreDraw = true;
            NPC.GetT().useVanillaDrawing = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            owner = ((EntitySource_Parent)source).Entity.whoAmI;
            NPC.TargetClosest(false);
            rotation = NPC.AngleTo(NPC.GetT().Target.Center);

            List<int> whiteList =
            [
                5,
                ModContent.NPCType<FireShooterServantofCthulhu>(),
                ModContent.NPCType<LaserServantofCthulhu>(),
                ModContent.NPCType<HealerServantofCthulhu>(),
            ];
            aura = new SheildAura(250, 10, 1, 0.5f, NPC, false, whiteList);
            aura.Center = NPC.Center;
            if (Group.FindGroup("SheildedServantofCthulhu").Count == 0)
            {
                Group.NewGroup(new Groups.SheildedServantofCthulhu());
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
            aura.Center = NPC.Center;
            aura.Update();
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
                    NPC.TargetClosest(false);
                    if (NPC.ai[1] != -1 && Main.npc[(int)NPC.ai[1]].active && NPC.ai[0] == 0)
                    {
                        NPC patient = Main.npc[(int)NPC.ai[1]];
                        targetPosition = patient.Center - (patient.Center - NPC.GetT().Target.Center).ToUnit(new Vector2(0, NPC.spriteDirection)) * 100;
                        if (targetPosition.Distance(NPC.GetT().Target.Center) < 350)
                        {
                            targetPosition += targetPosition.DirectionFrom(NPC.GetT().Target.Center) * MathF.Min(50, 350 - targetPosition.Distance(NPC.GetT().Target.Center));
                        }
                        else
                        {
                            targetPosition -= targetPosition.DirectionFrom(NPC.GetT().Target.Center) * MathF.Min(50, targetPosition.Distance(NPC.GetT().Target.Center) - 350);
                        }
                        if (NPC.Distance(targetPosition) < 75)
                        {
                            maxVelocityMultyplier = 1 - (75 - NPC.Distance(targetPosition)) / 75;
                        }
                        targetRotation = AngleFromVector(NPC.GetT().Target.Center - NPC.Center);
                    }
                    else
                    {
                        if (NPC.ai[0] == 0)
                        {
                            NPC.TargetClosest(false);
                            if (!NPC.GetT().Target.active || NPC.GetT().Target.dead)
                            {
                                targetPosition = NPC.Center - Vector2.UnitY;
                                targetRotation = -MathF.PI / 2;
                            }
                            else
                            {
                                if (NPC.life >= NPC.lifeMax * 0.35f)
                                {
                                    targetPosition = NPC.GetT().Target.Center + NPC.GetT().Target.velocity * 20;
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
                            targetRotation = AngleFromVector(NPC.GetT().Target.Center - NPC.Center);
                            }
                        }
                        else if (NPC.ai[0] == 1)
                        {
                            flag = false;
                            NPC.ai[2] = 15;
                            NPC.GetT().afterimage = true;
                            NPC.GetT().afterimagesCount = 10;
                            NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.spriteDirection * 20;
                            NPC.ai[0] = 2;
                        }
                        else if (NPC.ai[0] == 2)
                        {
                            flag = false;
                            NPC.velocity *= (NPC.velocity.Length() - 1) / NPC.velocity.Length();
                            if (NPC.ai[2] <= 0)
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[2] = 30;
                                NPC.GetT().afterimage = false;
                            }
                        }
                        NPC.ai[2]--;
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
                NPC.rotation = rotation;
                NPC.spriteDirection = NPC.rotation.ToRotationVector2().X.NonZeroSign();
                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation = (NPC.rotation.ToRotationVector2() * -1).ToRotation();
                }
                if (NPC.ai[2] <= 0 && NPC.GetT().Target.Distance(NPC.Center) < 200)
                {
                    NPC.ai[0] = 1;
                }
                if (NPC.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                {
                    NPC.velocity = NPC.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                }
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            aura.Draw(spriteBatch);
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
