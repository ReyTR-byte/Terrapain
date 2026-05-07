using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Chat;
using Terrapain.Common.System;
using Luminance.Common.Utilities;
using Terrapain.Common.Player;

namespace Terrapain.Content.NPCs
{
    // This NPC is simply an exhibition of the DrawBehind method.
    // The npc cycles between all the available "layers" that a ModNPC can be drawn at.
    // Spawn this NPC with something like Cheat Sheet or Hero's Mod to view the effect.
    public class Suicide : ModNPC
    {
        private Player player
        {
            get => Main.player[(int)NPC.ai[0]];
        }
        private NPC Torture
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
            NPC.width = 24; // The width of the npc hitbox
            NPC.height = 46; // The height of the npc hitbox
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
        int wait = 10;
        float playerVelocity;
        bool secondEtap;
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
                targetPosition.X += player.width + 8 + NPC.width / 2;

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
                    placed = true;
                }

                NPC.rotation = (10f / (Math.Abs(NPC.velocity.X) + 10f / (0.5f * (float)Math.PI)) - 0.5f * (float)Math.PI) * -NPC.velocity.X.NonZeroSign();
            }

            if (player.dead || !player.active || Functions.CheckBoss())
            {
                NPC.life = 0;
                Torture.life = 0;
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
                Torture.life = 0;
                WorldDifficultySystem.SetDifficulty(2, player);
                NPC.velocity = Vector2.Zero;
            }
            if (animation > 0)
            {
                player.GetModPlayer<PlayerCantMove>().cantMove = true;
                CanBeRemovedByDropOfPain = false;
                Vector2 NeckPosition;
                NeckPosition.Y = player.position.Y + 20 - NPC.height;
                NeckPosition.X = player.Center.X - NPC.width / 2;

                Vector2 VectorToNeck = NeckPosition - NPC.position;

                if ((NPC.position + NPC.velocity - NeckPosition).Length() > 6)
                {
                    NPC.velocity += (VectorToNeck / VectorToNeck.Length()) / 100;
                    player.velocity = Vector2.Zero;
                    player.position = player.oldPosition;
                }
                else
                {
                    NPC.velocity *= 0;
                    NPC.position = NeckPosition;

                    if (playerVelocity < 6 && !secondEtap)
                        playerVelocity += 0.1f;
                    else if (playerVelocity > 0)
                    {
                        secondEtap = true;
                        playerVelocity -= 0.3f;
                    }
                    else if (wait > 0)
                        wait--;
                    else if (player.difficulty != PlayerDifficultyID.Hardcore)
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerChooseSuicideDeath" + new string(player.Male ? "Male" : "Female"), player.name)), 999999999, -1, false, false, -1, true, player.statDefense);
                        NPC.life = 0;
                    }
                    else
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerChooseSuicideDeath", player.name)), player.statLife - 1, -1, false, false, -1, true, player.statDefense);
                        if (player.dead)
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerDiedSuicideHardcore" + new string(player.Male ? "Male" : "Female")), Color.White);
                        }
                        else
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerCooseSuicideHardcore" + new string(player.Male ? "Male" : "Female"), player.name), Color.White);
                        }
                        NPC.life = 0;
                    }

                    player.velocity.Y = -playerVelocity;
                    player.velocity.X = 0;
                }
            }
        }
    }
}