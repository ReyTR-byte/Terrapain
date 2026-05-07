using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terrapain.Common.System;
using Luminance.Common.Utilities;
using Terrapain.Common.Player;

namespace Terrapain.Content.NPCs
{
    // This NPC is simply an exhibition of the DrawBehind method.
    // The npc cycles between all the available "layers" that a ModNPC can be drawn at.
    // Spawn this NPC with something like Cheat Sheet or Hero's Mod to view the effect.
    public class Torture : ModNPC
    {
        private Player player
        {
            get => Main.player[(int)NPC.ai[0]];
        }
        private NPC Suicide
        {
            get => Main.npc[(int)NPC.ai[1]];
        }
        public bool CanBeRemovedByDropOfPain
        {
            get => NPC.ai[2] == 1;
            set => NPC.ai[2] = value? 1 : 0;
        }
        public override void SetDefaults()
        {
            NPC.width = 42; // The width of the npc hitbox
            NPC.height = 44; // The height of the npc hitbox
            NPC.aiStyle = -1; // Using custom AI
            NPC.damage = 50; // The amount of damage this NPC will deal on collision
            NPC.defense = 2; // How resistant to damage this NPC is
            NPC.lifeMax = 400; // The maximum life of this NPC
            NPC.HitSound = SoundID.NPCHit2; // The sound that plays when this npc is hit
            NPC.DeathSound = SoundID.NPCDeath2; // The sound that plays when this npc dies // If true, the npc will not be affected by gravity // If true, the npc does not collide with tiles
            NPC.knockBackResist = 0f; // How much of the knockback it receives will actually apply. 1f: full knockback; 0f: no knockback
            NPC.friendly = true;
            NPC.immortal = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }
        public override bool CanBeHitByNPC(NPC attacker)
        {
            return false;
        }
        
        bool hold;
        bool justRealised;
        int animation;
        float angularVelocity;
        float angularAcceleration;
        float speed;
        float maxSpeed;
        bool placed;
        public override void AI()
        {

            justRealised = false;
            placed = false;
            if (animation == 0)
            {
                Vector2 targetPosition = player.position;
                targetPosition.Y -= 20 + NPC.height / 2;
                targetPosition.X -= 40 - NPC.width / 2;

                speed = NPC.velocity.Length() + 0.2f;
                maxSpeed = NPC.Distance(targetPosition) * 0.5f + 5;
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
                if (speed > NPC.Distance(targetPosition))
                {
                    speed = NPC.Distance(targetPosition);
                    placed = true;
                }
                if (NPC.Distance(targetPosition) != 0)
                {
                    NPC.velocity = NPC.DirectionTo(targetPosition) * speed;
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                }

                if (NPC.Distance(targetPosition) > 800)
                {
                    NPC.Center = targetPosition;
                    NPC.velocity = Vector2.Zero;
                    NPC.rotation = 0;
                    NPC.spriteDirection = 1;
                    placed = true;
                }
                
                float goalAngle;
                if (NPC.velocity.Length() > 4)
                {   
                    if (NPC.spriteDirection != NPC.velocity.X.NonZeroSign() * -1)
                    {
                        NPC.spriteDirection = NPC.velocity.X.NonZeroSign() * -1;
                        NPC.rotation += 0.5f * (float)Math.PI * NPC.spriteDirection;
                    }
                    goalAngle = NPC.spriteDirection * -1 * 0.25f * (float)Math.PI + Functions.AngleFromVector(NPC.DirectionTo(targetPosition) * NPC.spriteDirection * -1);
                }
                else
                {
                    if (NPC.spriteDirection != -1)
                    {
                        NPC.spriteDirection = -1;
                        NPC.rotation += 0.5f * (float)Math.PI * NPC.spriteDirection;
                    }
                    goalAngle = 0;
                }
                if (goalAngle < 0)
                {
                    goalAngle += (float)Math.PI * 2;
                }

                NPC.rotation = NPC.rotation % (2f * (float)Math.PI);
                if (NPC.rotation < 0)
                {
                    NPC.rotation += (float)Math.PI * 2;
                }

                if (NPC.rotation != goalAngle)
                {
                    if (goalAngle < (float)Math.PI)
                    {
                        if (NPC.rotation > goalAngle && NPC.rotation < goalAngle + Math.PI)
                        {
                            if (angularVelocity > -0.3f)
                                angularVelocity -= 0.03f;
                        }
                        else
                        {
                            if (angularVelocity < 0.3f)
                                angularVelocity += 0.03f;
                        }
                    }
                    else
                    {
                        if (NPC.rotation < goalAngle && NPC.rotation > goalAngle - Math.PI)
                        {
                            if (angularVelocity < 0.3f)
                                angularVelocity += 0.03f;
                        }
                        else
                        {
                            if (angularVelocity > -0.3f)
                                angularVelocity -= 0.03f;
                        }
                    }
                    if ((NPC.rotation + angularVelocity > goalAngle && NPC.rotation < goalAngle) || (NPC.rotation + angularVelocity < goalAngle && NPC.rotation > goalAngle))
                    {
                        NPC.rotation = goalAngle;
                        angularVelocity = 0;
                    }
                    goalAngle += 2 * (float)Math.PI;
                    if ((NPC.rotation + angularVelocity > goalAngle && NPC.rotation < goalAngle) || (NPC.rotation + angularVelocity < goalAngle && NPC.rotation > goalAngle))
                    {
                        NPC.rotation = goalAngle;
                        angularVelocity = 0;
                    }
                    goalAngle -= 4 * (float)Math.PI;
                    if ((NPC.rotation + angularVelocity > goalAngle && NPC.rotation < goalAngle) || (NPC.rotation + angularVelocity < goalAngle && NPC.rotation > goalAngle))
                    {
                        NPC.rotation = goalAngle;
                        angularVelocity = 0;
                    }   
                    else
                    {
                        NPC.rotation += angularVelocity;
                    }
                }
            }

            if (player.dead || !player.active  || Functions.CheckBoss())
            {
                NPC.life = 0;
                Suicide.life = 0;
            }
            else
            {
                Random rand = new Random();
                NPC.life = rand.Next(1, 9999999);
                NPC.lifeMax = rand.Next(1, 9999999);
            }

            if (Main.mouseLeft)
            {
                hold = true;
            }
            else if (hold)
            {
                hold = false;
                justRealised = true;
            }

            CanBeRemovedByDropOfPain = true;

            if (Main.MouseWorld.X > NPC.position.X && Main.MouseWorld.X < NPC.position.X + NPC.width && Main.MouseWorld.Y > NPC.position.Y && Main.MouseWorld.Y < NPC.position.Y + NPC.height)
            {
                CanBeRemovedByDropOfPain = false;
            }
            if (Main.MouseWorld.X > NPC.position.X && Main.MouseWorld.X < NPC.position.X + NPC.width && Main.MouseWorld.Y > NPC.position.Y && Main.MouseWorld.Y < NPC.position.Y + NPC.height && justRealised && placed)
            {
                animation = 60;
                Suicide.life = 0;
                WorldDifficultySystem.SetDifficulty(1, player);
                NPC.velocity = Vector2.Zero;
            }
            if (animation > 0)
            {
                player.GetModPlayer<PlayerCantMove>().cantMove = true;
                CanBeRemovedByDropOfPain = false;
                player.position = player.oldPosition;

                if (NPC.rotation < 0.5f * (float)Math.PI)
                {
                    angularAcceleration += 0.00001f;
                    angularVelocity += angularAcceleration;
                    NPC.rotation += angularVelocity;
                }
                else
                {
                    NPC.velocity += Vector2.One + NPC.velocity;
                    if (NPC.Center.X + NPC.velocity.X > player.position.X)
                    {
                        player.Hurt(PlayerDeathReason.ByNPC(Type), player.statLife - 1, 1, false, false, -1, true, player.statDefense);
                        NPC.life = 0;
                    }
                }
            }
        }
    }
}