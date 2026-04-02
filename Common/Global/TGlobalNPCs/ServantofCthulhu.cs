using ILGPU.IR;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.System;
using Terrapain.Content;
using Terrapain.Content.Groups;
using Terrapain.Content.Items.ItemDropRules;
using Terrapain.Content.NPCs.Servants.EyeofCthulhu;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalNPCs
{
    public class ServantofCthulhu : NPCBehaviour
    {
        int attack
        {
            get => (int)This.ai[0];
            set => This.ai[0] = value;
        }
        int timer
        {
            get => (int)This.ai[1];
            set => This.ai[1] = value;
        }
        public int AIStyle;
        int owner;
        NPC Owner { get => Main.npc[owner]; }
        NPC This;
        float maxVelocity = 15;
        float angularVelocity = 0;
        float maxAngularVelocity = 0.3f;
        float angularAcceleration = 0.03f;
        public override int type => NPCID.ServantofCthulhu; 
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!WorldDifficultySystem.clasic)
            {
                This = npc;
                base.OnSpawn(npc, source);
                owner = ((EntitySource_Parent)source).Entity.whoAmI;
                if (source.Context == "ring AI")
                {
                    AIStyle = 1;
                    if (Group.FindGroup("RingAIServantsofCthulhu").Count == 0)
                    {
                        Group.NewGroup(new RingAIServantsofCthulhu());
                    }
                    Terrapain.group[Group.FindGroup("RingAIServantsofCthulhu")[0]].AddMember(npc.whoAmI);
                }
                if (source.Context == "fly straight")
                {
                    timer = 200;
                    npc.knockBackResist = 0;
                    AIStyle = 2;
                }
            }
        }
        public override void ModSetDefaults(NPC entity)
        {
            entity.lifeMax = 50;
            entity.GetT().drawCenter = new Vector2(10, 20);
            entity.GetT().useModDrawingInPreDraw = true;
            entity.GetT().useVanillaDrawing = false;
            entity.damage = 20;
        }
        public override void OnFirstTick(NPC npc)
        {
            if (!WorldDifficultySystem.clasic)
            {
                npc.TargetClosest(false);
                npc.rotation = npc.AngleTo(npc.GetT().Target.Center) + 0.5f * MathF.PI;
                t.useModDrawingInPreDraw = true;
                t.useVanillaDrawing = false;
                t.textureDirection = -1;
                t.drawCenter = new Vector2(npc.height / 2);
                timer = 50;
            }
        }
        public override bool ModPreAI(NPC npc)
        {
            if (!WorldDifficultySystem.clasic)
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
                bool flag = true;
                if (timer > 0)
                {
                    timer--;
                }
                float maxVelocityMultyplier = 1;
                float targetRotation = 0;
                Vector2 targetPosition = Vector2.Zero;
                switch (AIStyle)
                {
                    case -2:
                        targetPosition = npc.Center - Vector2.UnitY;
                        targetRotation = -MathF.PI / 2;
                        break;
                    case -1:
                        targetPosition = Owner.Center;
                        targetRotation = (Owner.Center - npc.Center).ToRotation();
                        if (npc.Distance(Owner.Center) < 50)
                        {
                            npc.active = false;
                        }
                        break;
                    case 0:
                        if (attack == 0)
                        {
                            npc.TargetClosest();
                            if (!t.Target.active || t.Target.dead)
                            {
                                targetPosition = npc.Center + Vector2.Zero;
                                targetRotation = MathF.PI / 2;
                            }
                            else
                            {
                                targetRotation = AngleFromVector(t.Target.Center - npc.Center);
                                if (npc.life >= npc.lifeMax * 0.30f)
                                {
                                    targetPosition = npc.GetT().Target.Center + npc.GetT().Target.velocity * 20;
                                    if (npc.Distance(targetPosition) < 250)
                                    {
                                        maxVelocityMultyplier = 1 - (50 - npc.Distance(targetPosition) + 200) / 50;
                                    }
                                }
                                else
                                {
                                    if ((int)npc.ai[3] < 0 || (int)npc.ai[3] >= Main.npc.Length || !Main.npc[(int)npc.ai[3]].active || Main.npc[(int)npc.ai[3]].type != ModContent.NPCType<Content.NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>())
                                    {
                                        NPC doctor = FindClosestNPC(npc.Center, null, ModContent.NPCType<Content.NPCs.Servants.EyeofCthulhu.HealerServantofCthulhu>());
                                        if (doctor != null)
                                        {
                                            targetPosition = doctor.Center;
                                            targetPosition += targetPosition.DirectionTo(npc.GetT().Target.Center) * 100;
                                            if (npc.Distance(targetPosition) < 75)
                                            {
                                                maxVelocityMultyplier = 1 - (75 - npc.Distance(targetPosition)) / 75;
                                            }
                                        }
                                        else
                                        {
                                            targetPosition = npc.GetT().Target.Center + npc.GetT().Target.velocity * 20 + (npc.Center - npc.GetT().Target.Center - npc.GetT().Target.velocity * 20).ToUnit(new Vector2(0, npc.spriteDirection)).RotatedBy(0.2f * npc.ai[1]) * 200;
                                        }
                                    }
                                    else
                                    {
                                        NPC doctor = Main.npc[(int)npc.ai[3]];
                                        targetPosition = doctor.Center;
                                        targetPosition += targetPosition.DirectionTo(npc.GetT().Target.Center) * 100;
                                        if (npc.Distance(targetPosition) < 75)
                                        {
                                            maxVelocityMultyplier = 1 - (75 - npc.Distance(targetPosition)) / 75;
                                        }
                                    }
                                }
                                if (timer <= 0 && npc.GetT().Target.Distance(npc.Center) < 200)
                                {
                                    attack = 1;
                                    timer = 15;
                                }
                            }
                        }
                        else if (attack == 1)
                        {
                            flag = false;
                            npc.rotation = npc.DirectionTo(t.Target.Center).ToRotation() - MathF.PI / 2;
                            Lighting.AddLight(npc.Center, 1f, 1f, 1f);
                            npc.velocity = npc.DirectionFrom(t.Target.Center) * 3;
                            if (timer == 0)
                            {
                                attack = 2;
                            }
                        }
                        else if (attack == 2)
                        {
                            flag = false;
                            timer = 15;
                            t.afterimage = true;
                            t.afterimagesCount = 10;
                            npc.velocity = UnitVectorFromRotation(npc.rotation + MathF.PI / 2) * 35;
                            attack = 3;
                        }
                        else if (attack == 3)
                        {
                            flag = false;
                            npc.velocity *= (npc.velocity.Length() - 1) / npc.velocity.Length();
                            if (timer == 0)
                            {
                                attack = 0;
                                timer = 20;
                                t.afterimage = false;
                            }
                        }
                        break;
                    case 1:
                        if (npc.ai[2] == 0)
                        {
                            maxVelocityMultyplier = 1.5f;
                            if (npc.target == -1)
                            {
                                npc.TargetClosest(false);
                                targetPosition = t.Target.Center + (npc.Center - t.Target.Center).Normalized() * 100;
                            }
                            else
                            {
                                targetPosition = t.Target.Center + Vector2.UnitX.RotatedBy(npc.ai[0]) * npc.ai[1];
                            }
                            if (npc.Distance(targetPosition) < 75)
                            {
                                maxVelocityMultyplier = 1.5f - (75 - npc.Distance(targetPosition)) / 50;
                            }
                            t.afterimage = false;
                            targetRotation = (t.Target.Center - npc.Center).ToRotation();
                        }
                        else if (npc.ai[2] == 1)
                        {
                            flag = false;
                            npc.velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 25;
                            t.afterimage = true;
                            t.afterimagesCount = 10;
                        }
                        else
                        {
                            flag = false;
                            npc.velocity = npc.velocity.Normalized() * (npc.velocity.Length() - 0.5f);
                        }
                        break;
                    case 2:
                        Lighting.AddLight(npc.Center, 1, 1, 1);
                        flag = false;
                        npc.rotation = npc.velocity.ToRotation() - MathF.PI * 0.5f;
                        if (timer == 100)
                        {
                            npc.velocity = npc.DirectionTo(t.Target.Center) * npc.ai[0];
                            t.afterimage = true;
                            t.afterimagesCount = 10;
                        }
                        if (WorldDifficultySystem.suicide)
                        {
                            if (timer == 50)
                            {
                                npc.velocity = npc.DirectionTo(t.Target.Center) * npc.ai[0];
                                t.afterimage = true;
                                t.afterimagesCount = 10;
                            }
                        }
                        timer--;
                        if (npc.velocity.Length() > 20)
                        {
                            npc.velocity -= npc.velocity.Normalized() * 0.5f;
                        }
                        else
                        {
                            t.afterimage = false;
                        }
                        if (timer == 0)
                        {
                            AIStyle = -1;
                        }
                        break;
                }
                if (flag)
                {
                    npc.velocity += npc.DirectionTo(targetPosition) * 1.2f;
                    Vector2 vectorToTargetPosition = targetPosition - npc.Center;
                    float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, npc.velocity);
                    positiveRotation = NormalizeRotation(positiveRotation);
                    float negativeRotation = AngleBetweenVectors(npc.velocity, vectorToTargetPosition);
                    negativeRotation = NormalizeRotation(negativeRotation);
                    if (positiveRotation > negativeRotation)
                    {
                        npc.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                    }
                    else
                    {
                        npc.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                    }
                    if (npc.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                    {
                        npc.velocity = npc.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                    }
                    AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, targetRotation - MathF.PI / 2, ref npc.rotation);
                }
                return false;
            }
            return true;
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (attack == 1 && AIStyle == 0)
            {
                spriteBatch.DrawLine(npc.Center - Vector2.UnitY.RotatedBy(npc.rotation) * 10, npc.Center + Vector2.UnitY.RotatedBy(npc.rotation) * 120, Color.LightGray * (0.5f * timer / 15f), 20);
            }
            return true;
        }
        public override void OnKill(NPC npc)
        {
            if (!Main.getGoodWorld)
            {
                Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, npc.velocity, ModContent.ProjectileType<ServantofCthulhuSoul>(), ServantofCthulhuSoul.defDamage, 0, ai0: owner);
            }
            else
            {
                Projectile.NewProjectile(npc.GetSource_Death(owner.ToString()), npc.Center, npc.velocity, ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
            }
        }
        public override bool CanHitPlayer(NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            return !(AIStyle == -1 || (AIStyle == 1 && npc.ai[2] == 0));
        }
    }
}
