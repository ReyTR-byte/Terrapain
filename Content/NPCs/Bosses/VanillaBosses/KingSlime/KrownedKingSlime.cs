using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;
using static Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime.KingSlime;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime
{
    public class KrownedKingSlime : ModNPC
    {
        public override string Texture => "Terrapain/Content/NPCs/Bosses/VanillaBosses/KingSlime/NinjaKingSlime";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void SetDefaults()
        {
            NPC.width = 65;
            NPC.height = 69;
            NPC.lifeMax = 1400;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.scale = 0.707106f;

            NPC.alpha = 30;

            NPC.knockBackResist = 0f;

            NPC.npcSlots = 10f;

            NPC.noTileCollide = false;
            NPC.noGravity = false;

            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.MaxFallSpeedMultiplier = MultipliableFloat.One * 500;

            AnimationType = NPCID.KingSlime;
        }
        public int ninjaKingSlime;
        public NPC NinjaKingSlime => Main.npc[ninjaKingSlime];
        NinjaKingSlime NKS => (NinjaKingSlime)NinjaKingSlime.ModNPC;
        bool NKSactive => NinjaKingSlime != null && NinjaKingSlime.active && NinjaKingSlime.type == ModContent.NPCType<NinjaKingSlime>();
        public int kingSlimeKrown;
        public NPC KingSlimeKrown => Main.npc[kingSlimeKrown];
        //KingSlimeKrown KSK => (KingSlimeKrown)KingSlimeKrown.ModNPC;
        //bool KSKactive => KingSlimeKrown != null && KingSlimeKrown.active && KingSlimeKrown.type == ModContent.NPCType<KingSlimeKrown>();
        public int CurentAttack;
        public int attackCounter = -1;
        public int[] phase1 = [0, 1];
        int mainTimer;
        int movementTimer;
        int[] timers = new int[2];
        //int phase
        //{
        //    get => (int)Main.npc[t.npcid].ai[0];
        //    set => Main.npc[t.npcid].ai[0] = value;
        //}
        public bool teleporting
        {
            get => NPC.ai[1] > 0;
            set => NPC.ai[1] = value ? 120 : 0;
        }
        public int teleportTimer
        {
            get => (int)NPC.ai[1] - 1;
            set => NPC.ai[1] = value + 1;
        }
        public Vector2 teleportPosition
        {
            get => new Vector2(NPC.ai[2], NPC.ai[3]);
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }
        bool oldCollideY;
        Player Target => Main.player[NPC.target];
        UnifiedRandom rand => TGlobalNPC.random;

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX * 2, SlimeWall, 0, 0, -1, 10);
            NextAttack1();
        }
        public override bool? CanFallThroughPlatforms()
        {
            return (Target.position.Y > NPC.Bottom.Y) && CurentAttack != 2;
        }
        int SlimeWall => ModContent.ProjectileType<SlimeWall>();

        int SlimeBall => ModContent.ProjectileType<KingSlimeBall>();
        public int SlimeBallDamage = 10;
        public float SlimeBallKnockback = 5.5f;

        public static int Slime => ModContent.ProjectileType<SlimeProjectile>();
        public static int SlimeDamage = 15;
        public static float SlimeKnockBack = 4;
        public override void AI()
        {
            NPC.TargetClosest();
            NPC.noTileCollide = false;
            //switch (phase)
            //{
            //    case 0:
            DoFirstPhase();
            //        break;
            //}
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
            oldCollideY = NPC.collideY;
        }

        void ChillMovement()
        {
            if ((NPC.collideY || NPC.collideX) && !teleporting)
            {
                NPC.velocity = Vector2.Zero;
                if (movementTimer > 0)
                {
                    movementTimer--;
                }
                else
                {
                    NPC.velocity.Y = -MathHelper.Clamp((NPC.Center.Y - Target.Center.Y) / 30, 10, 30);
                    NPC.velocity.X = MathHelper.Clamp(MathF.Abs(Target.Center.X - NPC.Center.X) / 60, 10, 30) * (Target.Center.X - NPC.Center.X).NonZeroSign();
                }
            }
            else
            {
                movementTimer = (int)(50 * NPC.GetLifePercent()) + 20;
            }
            if (NPC.Distance(Target.Center) > 1500)
            {
                if (!teleporting)
                {
                    teleporting = true;
                    teleportPosition = Target.Center + new Vector2((rand.Next() == 0 ? -1 : 1) * 300, (rand.Next() == 0 ? -1 : 1) * 200);
                    if (!SimpleColision(teleportPosition, Target.position, Target.width, Target.height))
                    {
                        teleportPosition = Target.Center;
                    }
                }
                NPC.velocity = Vector2.Zero;
                if (teleportTimer == 0)
                {
                    NPC.Center = teleportPosition;
                    if (NPC.Distance(Target.Center) > 500)
                    {
                        NPC.velocity = NPC.DirectionTo(Target.Center) * 20;
                    }
                }
            }
            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
                NPC.collideY = false;
                NPC.collideX = false;
            }
        }
        List<RingOfSlimes> rings = new();
        void DoFirstPhase()
        {
            switch (CurentAttack)
            {
                case -1:
                    ChillMovement();
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1();
                    }
                    break;
                case 0:
                    ChillMovement();
                    if (mainTimer == 0 && !teleporting && (!NKSactive || NKS.CurentAttack == 0))
                    {
                        NextAttack1();
                    }
                    break;
                case 1:
                    ChillMovement();
                    int rate = 50;
                    if (mainTimer % rate == 1)
                    {
                        Vector2 Pos = Target.Center + new Vector2(rand.Next(-200, 201), rand.Next(-500, -399));
                        float speed = 22;
                        Vector2 velo = Pos.DirectionTo(SmartShoot(Pos, speed, Target.Center, Target.velocity, 60)) * speed;

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), Pos, velo, Slime, SlimeDamage, SlimeKnockBack, -1, 2);
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        int count = 5;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 2f + NPC.DirectionTo(Target.Center) * 20, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        count = 4;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * 0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitX.RotatedBy(i * 2f / count * MathF.PI) * 1.5f + NPC.DirectionTo(Target.Center).RotatedBy(MathF.PI * -0.2f) * 25, SlimeBall, SlimeBallDamage, SlimeBallKnockback);
                        }
                        NextAttack1();
                    }
                    break;
                case 2:
                    if (NPC.collideY)
                    {
                        NPC.velocity = Vector2.Zero;
                    }
                    if (!teleporting)
                    {
                        if (rings.Count == 0)
                        {
                            rings.Add(new RingOfSlimes(50, NPC) { angularVelocity = 9, Center = NPC.Center });
                        }
                        RingOfSlimes ring = rings[0];
                        ring.Center = NPC.Center;
                        ring.Radius = MathF.Min(ring.Radius + 25, 1000);
                        ring.slimeMaxSpeed = ring.Radius == 1000 ? 25 : 50;
                        ring.dealDamage = ring.Radius == 1000;
                        ring.Update();
                        rings[0] = ring;

                    }
                    else
                    {
                        if (teleportTimer == 1)
                        {
                            NPC.Center = teleportPosition;
                        }
                    }
                    if ((!NKSactive || NKS.CurentAttack == 0) && rings[0].Radius == 1000)
                    {
                        CurentAttack = 3;
                        mainTimer = 250;
                        NPC.ai[0] = 5;
                    }
                    break;
                case 3:
                    int time = 90;
                    if (timers[0] == 0 && NPC.ai[0] > 2)
                    {
                        if (NPC.ai[0] < 5)
                        {
                            foreach (int proj in rings[1].Projectiles)
                            {
                                Main.projectile[proj].active = false;
                            }
                            rings.RemoveAt(1);
                        }
                        List<int> projs = new List<int>();
                        for (int i = (int)(NPC.ai[0] / 2); i < rings[0].Count; i += (int)NPC.ai[0])
                        {
                            projs.Add(rings[0].Projectiles[i]);
                            rings[0].Projectiles.RemoveAt(i);
                        }
                        NPC.ai[0]--;
                        var ring1 = rings[0];
                        ring1.angularVelocity *= -1;
                        rings[0] = ring1;
                        rings.Add(new RingOfSlimes() { rotation = rings[0].Center.DirectionTo(Main.projectile[projs[0]].Center).ToRotation(), Center = rings[0].Center, angularVelocity = rings[0].angularVelocity * -1, Radius = ((NPC.ai[0] + 1) / 5 * 1000), slimeMaxSpeed = 50, dealDamage = true, Projectiles = projs });
                        timers[0] = time;
                    }
                    if (timers[0] == 0 && NPC.ai[0] == 2)
                    {
                        foreach (int proj in rings[1].Projectiles)
                        {
                            Main.projectile[proj].active = false;
                        }
                        rings.RemoveAt(1);
                        NPC.ai[0]--;
                    }
                    if (NPC.ai[0] < 5 && NPC.ai[0] > 1)
                    {
                        var ring2 = rings[1];
                        ring2.Radius = MathF.Max(ring2.Radius - ((NPC.ai[0] + 1) / 5 * 1000) / time, 0);
                        rings[1] = ring2;
                        var ring1 = rings[0];
                        ring1.Radius = MathF.Max(NPC.ai[0] / 5 * 1000, ring2.Radius);
                        rings[0] = ring1;
                    }
                    for (int i = 0; i < rings.Count; i++)
                    {
                        var ring1 = rings[i];
                        ring1.Update();
                        rings[i] = ring1;
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        foreach (var ring3 in rings)
                        {
                            ring3.End();
                        }
                        rings = new();
                        attackCounter = -1;
                        NextAttack1();
                    }
                    break;
            }
        }
        void NextAttack1()
        {
            if (CurentAttack == 0 && NKSactive)
            {
                CurentAttack = -1;
            }
            else
            {
                attackCounter++;
                if (attackCounter >= phase1.Length)
                {
                    attackCounter = 0;
                }
                CurentAttack = phase1[attackCounter];
            }
            switch (CurentAttack)
            {
                case -1:
                case 0:
                    mainTimer = 180;
                    break;
                case 1:
                    mainTimer = 250;
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(TextureAssets.Npc[NPCID.KingSlime].Value, NPC.Bottom - Main.screenPosition + Vector2.UnitY * 2, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
