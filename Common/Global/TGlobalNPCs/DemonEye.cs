using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.System;
using Terrapain.Content;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalNPCs
{
    public class DemonEye : NPCBehaviour
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.aiStyle == NPCAIStyleID.DemonEye;
        }
        public override void ModSetDefaults(NPC entity)
        {
            if (entity.aiStyle == NPCAIStyleID.DemonEye)
            {
                t.textureDirection = -1;
            }
        }
        public override void OnFirstTick(NPC npc)
        {
            if (npc.aiStyle == NPCAIStyleID.DemonEye)
            {
                npc.width = npc.frame.Height;
                npc.height = npc.width;
                t.drawCenter = npc.Hitbox.Size() * 0.5f;
            }
        }
        int timer;
        public override bool ModPreAI(NPC npc)
        {
            if (!WorldDifficultySystem.clasic)
            {
                timer++;
                if (!npc.noTileCollide)
                {
                    if (npc.collideY)
                    {
                        npc.velocity.Y = npc.oldVelocity.Y * -0.9f;
                    }
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.oldVelocity.X * -0.9f;
                    }
                }

                npc.scale = 1;
                npc.noGravity = true;
                npc.spriteDirection = npc.direction;
                if (npc.velocity.Length() > 8 && npc.ai[0] != 2)
                {
                    npc.velocity *= 8 / npc.velocity.Length();
                }
                switch (npc.ai[0])
                {
                    case 0:
                        npc.TargetClosest();
                        //Functions.Chatic(Target.whoAmI);
                        if (npc.target == 0)
                        {
                            float dist = 0;
                            float closest = -1;
                            foreach (var pl in Main.player)
                            {
                                if (pl.active && !pl.dead && pl.npcTypeNoAggro[npc.type])
                                    dist = Functions.DistanceBetweenHitboxes(pl, npc);
                                if (closest == -1 || dist < closest)
                                {
                                    closest = dist;
                                    npc.target = pl.whoAmI;
                                }
                            }
                        }
                        npc.velocity += npc.DirectionTo(t.Target.Center) * 2;
                        npc.ai[2] = Functions.AngleFromVector(npc.DirectionTo(t.Target.Center) * npc.spriteDirection);
                        if (npc.Distance(t.Target.Center) < 500 && npc.ai[3] <= 0 && !(WorldDifficultySystem.suicide && t.savedTarget > -1 && t.savedTarget < Main.player.Length && t.SavedTarget.active && !Functions.CanHit(npc.Center, t.SavedTarget.position, t.SavedTarget.width, t.SavedTarget.height)))
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;
                        }
                        if ((npc.Distance(t.Target.Center) < 200 || (WorldDifficultySystem.suicide && t.savedTarget > -1 && t.savedTarget < Main.player.Length && t.SavedTarget.active && !Functions.CanHit(npc.Center, t.SavedTarget.position, t.SavedTarget.width, t.SavedTarget.height) && npc.Distance(t.Target.Center) < 500)) && npc.ai[3] >= 0)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }

                        if (npc.ai[3] > 0)
                        {
                            npc.ai[3]--;
                        }
                        if (npc.ai[3] < 0)
                        {
                            npc.ai[3]++;
                        }
                        break;
                    case 1:
                        Vector2 smartShoot = Functions.SmartShoot(npc.Center, 20, t.Target.Center, t.Target.velocity, 100);
                        npc.ai[2] = Functions.AngleFromVector(npc.DirectionTo(smartShoot) * npc.spriteDirection);
                        npc.velocity += npc.DirectionTo(t.Target.Center) * 0.1f;
                        if (timer % (int)((7 - WorldDifficultySystem.TerrapainDifficulty) * (Main.hardMode ? 1.5f : 1)) == 0)
                        {
                            Vector2 velocity = npc.DirectionTo(smartShoot) * 5;
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ModContent.ProjectileType<DemonicEyeLazer>(), 12, 2);
                            Main.projectile[proj].friendly = false;
                            Main.projectile[proj].hostile = true;
                            npc.ai[1]++;
                        }
                        if (npc.ai[1] > (Main.hardMode ? 15 : 4))
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                            npc.ai[3] = Main.hardMode ? 60 : 45;
                        }
                        break;
                    case 2:
                        npc.velocity *= 0.98f;
                        if (npc.ai[1] == 0)
                        {
                            Vector2 smartShoot2 = SmartShoot(npc.Center, 22, t.Target.Center, t.Target.velocity, 100);
                            npc.velocity = npc.DirectionTo(smartShoot2) * 22;
                            t.afterimagesCount = 15;
                            t.afterimage = true;
                            t.savedTarget = npc.target;
                            npc.target = -1;
                        }
                        npc.ai[2] = Functions.AngleFromVector(npc.velocity * npc.spriteDirection);

                        if (WorldDifficultySystem.suicide && !Functions.CanHit(npc.Center, t.SavedTarget.position, t.SavedTarget.width, t.SavedTarget.height))
                        {
                            npc.noTileCollide = true;
                        }
                        else if (!Functions.HitTiles(npc))
                        {
                            npc.noTileCollide = false;
                        }

                        if (npc.ai[1] > 30)
                        {
                            t.afterimage = false;
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                            npc.ai[3] = -5;
                            npc.noTileCollide = false;
                        }
                        npc.ai[1]++;
                        if (npc.ai[3] > 0)
                        {
                            npc.ai[3]--;
                        }
                        break;
                }
                return false;
            }
            return true;
        }
        public override void ModPostAI(NPC npc)
        {
            npc.rotation = npc.ai[2];
        }
        public override bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
            npc.rotation = npc.ai[2];
            Vector2 DrawCenter = t.drawCenter;
            if (t.oldDirections[0] == 1)
            {
                DrawCenter.X = npc.frame.Width - DrawCenter.X;
            }
            Main.EntitySpriteDraw(texture, npc.position - Main.screenPosition + t.drawCenter + t.drawOffcet, npc.frame, drawColor, npc.rotation, DrawCenter, 1, t.oldDirections[0] == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            return false;
        }
    }
}