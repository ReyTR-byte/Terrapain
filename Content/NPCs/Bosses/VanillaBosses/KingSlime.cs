using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Security;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses
{
    public class KingSlime : NPCBehaviour
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.type == NPCID.KingSlime;
        }
        public int CurentAttack;
        public int attackCounter = -1;
        public int[] phase1 = [0, 1];
        int mainTimer;
        int movementTimer;
        int[] timers = new int[2];
        int phase
        {
            get => (int)Main.npc[t.npcid].ai[0];
            set => Main.npc[t.npcid].ai[0] = value;
        }
        public bool teleporting
        {
            get => Main.npc[t.npcid].ai[1] > 0;
            set => Main.npc[t.npcid].ai[1] = value? 120 : 0;
        }
        public int teleportTimer
        {
            get => (int)Main.npc[t.npcid].ai[1] - 1;
            set => Main.npc[t.npcid].ai[1] = value + 1;
        }
        public Vector2 teleportPosition
        {
            get => new Vector2(Main.npc[t.npcid].ai[2], Main.npc[t.npcid].ai[3]);
            set 
            {
                Main.npc[t.npcid].ai[2] = value.X;
                Main.npc[t.npcid].ai[3] = value.Y;
            } 
        }
        UnifiedRandom rand => TGlobalNPC.random;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            //Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX * 2, SlimeWall, 0, 0, -1, 10);
            NextAttack1(npc);
        }
        public override void ModSetDefaults(NPC entity)
        {
            entity.aiStyle = -1;
        }
        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            return t.Target.position.Y > npc.Bottom.Y;
        }
        protected int SlimeWall => ModContent.ProjectileType<SlimeWall>();
        protected int SlimeBall => ModContent.ProjectileType<KingSlimeBall>();
        public int SlimeBallDamage = 10;
        public float SlimeBallKnockback = 5.5f;
        public override bool ModPreAI(NPC npc)
        {
            npc.MaxFallSpeedMultiplier = MultipliableFloat.One * 500;
            npc.noTileCollide = false;
            switch (phase)
            {
                case 0:
                    DoFirstPhase(npc);
                    break;
            }
            if (mainTimer > 0)
            {
                mainTimer--;
            }
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] > 0)
                {
                    timers[i]--;
                }
            }
            if (teleporting)
            {
                int d = Dust.NewDust(teleportPosition, 0, 0, DustID.ShimmerSpark);
                Main.dust[d].velocity = rand.NextVector2Unit() * (3 + rand.NextFloat(5));
                teleportTimer--;
            }
            phase1 = [0, 1, 0, 2];
            return false;
        }
        
        void ChillMovement(NPC npc)
        {
            if ((npc.collideY || npc.collideX) && !teleporting)
            {
                if(t.Target.controlJump)
                {

                }
                npc.velocity = Vector2.Zero;
                if (movementTimer > 0)
                {
                    movementTimer--;
                }
                else
                {
                    npc.velocity.Y = -MathHelper.Clamp((npc.Center.Y - t.Target.Center.Y) / 30, 10, 30);
                    npc.velocity.X = MathHelper.Clamp(MathF.Abs(t.Target.Center.X - npc.Center.X) / 60, 10, 30) * (t.Target.Center.X - npc.Center.X).NonZeroSign();
                }
            }
            else
            {
                movementTimer = (int)(50 * npc.GetLifePercent()) + 20;
            }
            if (npc.Distance(t.Target.Center) > 1500)
            {
                if (!teleporting)
                {
                    teleporting = true;
                    teleportPosition = t.Target.Center + new Vector2((rand.Next() == 0? -1 : 1) * 300, (rand.Next() == 0 ? -1 : 1) * 200);
                    if (!SimpleColision(teleportPosition, t.Target.position, t.Target.width, t.Target.height))
                    {
                        teleportPosition = t.Target.Center;
                    }
                }
                npc.velocity = Vector2.Zero;
                if (teleportTimer == 0)
                {
                    npc.Center = teleportPosition;
                    if (npc.Distance(t.Target.Center) > 500)
                    {
                        npc.velocity = npc.DirectionTo(t.Target.Center) * 20;
                    }
                }
            }
            if (npc.velocity.Y < 0)
            {
                npc.noTileCollide = true;
                npc.collideY = false;
                npc.collideX = false;
            }
        }
        void DoFirstPhase(NPC npc)
        {
            switch(CurentAttack)
            {
                case 0:
                    ChillMovement(npc);
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 1:
                    if ((npc.collideY || teleportTimer == 0) && teleporting)
                    {
                        teleportTimer = 80;
                        npc.velocity = Vector2.UnitY * 20;
                        int count = WorldDifficultySystem.torture ? 6 : 8;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(0.1f * MathF.PI + (i / (count - 1f)) * 0.8f * MathF.PI) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        npc.Center = teleportPosition;
                        teleporting = false;
                    }
                    else
                    {
                        if (!teleporting)
                        {
                            teleportPosition = t.Target.Center + new Vector2((rand.Next(2) == 0 ? -1 : 1) * 150, -1000);
                            teleportTimer = 100;
                        }
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 2:
                    ChillMovement(npc);
                    if (timers[0] == 0)
                    {
                        int count = 5;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count  * MathF.PI) * 2f + npc.DirectionTo(t.Target.Center) * 20, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        count = 4;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + npc.DirectionTo(t.Target.Center).RotatedBy(MathF.PI * 0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + npc.DirectionTo(t.Target.Center).RotatedBy(MathF.PI * -0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        timers[0] = 45;
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        void NextAttack1(NPC npc)
        {
            attackCounter++;
            if (attackCounter >= phase1.Length)
            {
                attackCounter = 0;
            }
            CurentAttack = phase1[attackCounter];
            switch(CurentAttack)
            {
                case 0:
                    mainTimer = 100;
                    break;
                case 1:
                    mainTimer = 300;
                    break;
                case 2:
                    timers[0] = 100;
                    mainTimer = 450;
                    break;
            }
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (phase == 0 && CurentAttack == 1)
            {
                Rectangle rectangle = new Rectangle((int)npc.Center.X - 45 - (int)screenPos.X, 0, 90, Main.screenHeight);
                spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rectangle, Color.Blue * 0.5f);
            }
            return true;
        }
    }
}
