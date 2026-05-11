using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Global;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime
{
    public class NinjaKingSlime : ModNPC
    {
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
        public NPC KrownedKingSlime;
        public NPC KingSlimeKrown;
        public int CurentAttack;
        public int attackCounter = -1;
        public int[] phase1 = [0, 1, 0, 2, 0, 3];
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

        int Shuriken => ModContent.ProjectileType<Shuriken>();
        public int ShurikenDamage = 12;
        public float ShurikenKnockBack = 3;

        int Kunai => ModContent.ProjectileType<Kunai>();
        public int KunaiDamage = 12;
        public float KunaiKnockBack = 3;

        int KatanaProjectile => ModContent.ProjectileType<KingSlimeKatana>();
        public int KatanaDamage = 25;
        public float KatanaKnockBack = 8.75f;

        Projectile Katana
        {
            get => Main.projectile[katana];
        }
        int katana;

        int SmokeBomb => ModContent.ProjectileType<SmokeBomb>();
        public int SmokeBombDamage = 10;
        public float SmokeBombKnockBack = 1.5f;
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
        void DoFirstPhase()
        {
            switch (CurentAttack)
            {
                case 0:
                    ChillMovement();
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1();
                    }
                    break;
                case 1:
                    ChillMovement();
                    if (mainTimer % 50 == 1)
                    {
                        int count;
                        float angle;
                        switch((int)NPC.ai[0] % 4)
                        {
                            case 0:
                                count = 4;
                                int count1 = 4;
                                angle = MathF.PI * 2 / count;
                                float angle1 = angle / count1;
                                for (int i = 0; i < count; i++)
                                {
                                    for (int l = 0; l < count1; l++)
                                    {
                                        float speed = 22 - 4 * l;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(i * angle + l * angle1) * speed, Kunai, KunaiDamage, KunaiKnockBack);
                                    }
                                }
                                break;
                            case 1:
                                count = 20;
                                angle = MathF.PI * 2 / count;
                                for (int i = 0; i < count; i++)
                                {
                                    float speed = 22 - 11 * (i % 2);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(i * angle) * speed, Kunai, KunaiDamage, KunaiKnockBack);
                                }
                                break;
                            case 2:
                                count = 4;
                                angle = 0.08f * MathF.PI;
                                float startAngle = NPC.DirectionTo(Target.Center).ToRotation() - angle * count / 2;
                                for (int i = 0; i < count; i++)
                                {
                                    float speed = 22;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(i * angle + startAngle) * speed, Kunai, KunaiDamage, KunaiKnockBack);
                                }
                                break;
                            case 3:
                                count = 16;
                                angle = MathF.PI * 2 / count;
                                for (int i = 0; i < count; i++)
                                {
                                    float speed = 17;
                                    int dir = i % 2 == 1 ? 1 : -1;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(i * angle) * speed, Shuriken, ShurikenDamage, ShurikenKnockBack, -1, dir * 1.5f);
                                }
                                break;
                        }
                        NPC.ai[0]++;
                    }
                    if (mainTimer == 0 && !teleporting)
                    {
                        NextAttack1();
                    }
                    break;
                case 2:
                    if (NPC.collideY)
                    {
                        NPC.velocity = Vector2.Zero;
                    }
                    if (mainTimer % 5 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(rand.NextFloat(MathF.PI * 2)) * 25 * rand.NextFloat(0.8f, 1.2f), SmokeBomb, 0, SmokeBombKnockBack, -1, SmokeBombDamage, rand.Next(25, 91));
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
                case 3:
                    if (katana < 0 || katana > Main.maxProjectiles - 1 || !Katana.active || Katana.type != KatanaProjectile)
                    {
                        katana = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, KatanaProjectile, KatanaDamage, KatanaKnockBack, -1, 0, NPC.whoAmI, 0);
                    }
                    Katana.timeLeft = 2;
                    if (mainTimer == 0)
                    {
                        NextAttack1();
                    }
                    break;
            }
        }
        void NextAttack1()
        {
            attackCounter++;
            if (attackCounter >= phase1.Length)
            {
                attackCounter = 0;
            }
            CurentAttack = phase1[attackCounter];
            switch (CurentAttack)
            {
                case 0:
                    mainTimer = 200;
                    break;
                case 1:
                    mainTimer = 450;
                    NPC.ai[0] = rand.Next(4);
                    break;
                case 2:
                    mainTimer = 450;
                    break;
                case 3:
                    mainTimer = 450;
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 zero = Vector2.Zero;
            float num33 = 0f;
            zero.Y -= NPC.velocity.Y;
            zero.X -= NPC.velocity.X * 2f;
            num33 += NPC.velocity.X * 0.05f;
            if (NPC.frame.Y == 120)
                zero.Y += 2f;

            if (NPC.frame.Y == 360)
                zero.Y -= 2f;

            if (NPC.frame.Y == 480)
                zero.Y -= 6f;

            spriteBatch.Draw(TextureAssets.Ninja.Value, new Vector2(NPC.position.X - screenPos.X + (float)(NPC.width / 2) + zero.X, NPC.position.Y - screenPos.Y + (float)(NPC.height / 2) + zero.Y), new Microsoft.Xna.Framework.Rectangle(0, 0, TextureAssets.Ninja.Width(), TextureAssets.Ninja.Height()), drawColor, num33, new Vector2(TextureAssets.Ninja.Width() / 2, TextureAssets.Ninja.Height() / 2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Npc[NPCID.KingSlime].Value, NPC.Bottom - Main.screenPosition + Vector2.UnitY * 2, NPC.frame, drawColor * ((255 - NPC.alpha) / 255f), NPC.rotation, new Vector2(NPC.frame.Width / 2, NPC.frame.Height), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
