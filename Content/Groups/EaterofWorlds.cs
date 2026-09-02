using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terrapain.Content.TUtilities;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public class EaterofWorlds : Snake
    {
        public override void SetDefaults()
        {
            SegmentLength = 30;
            Draw = true;
            drawStyle = DrawStyle.SmoothedPartByPart;
            smoothTail = true;
        }
        public override int[] NPCType => [NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail];
        public int inTheBrain;
        public bool GoingToBrain;
        public Vector2 BrainPosition;
        public bool DrawOverBrain;
        public bool Dying;
        int dyingTimer;
        public override void UpdateGroup()
        {
            CheckSnake();
            NPC brain = Main.npc[EaterofWorldsHead.brainofCthulhu];
            if (!brain.active || brain.type != NPCID.BrainofCthulhu)
            {
                brain = null;
            }
            BrainPosition = brain?.Center?? Vector2.Zero;
            CheckMembers();
            if (Count == 0)
            {
                return;
            }
            if (GoingToBrain && inTheBrain < Count)
            {
                NPC npc = Main.npc[members[inTheBrain]];
                if (npc.velocity == Vector2.Zero)
                {
                    npc.velocity = npc.position - npc.oldPos[1];
                }
                AIHelper.CommonTerrapainFlyingMovement(npc, BrainPosition, 0.2f, 25, 0.25f, 75);
                npc.rotation = npc.velocity == Vector2.Zero? npc.rotation : npc.velocity.ToRotation();
            }
            float adjustRotation = VerticalSprites ? MathF.PI / 2 : 0;
            for (int i = inTheBrain + 1; i < members.Count; i++)
            {
                NPC npc = Main.npc[members[i]];
                NPC head = Main.npc[members[i - 1]];
                if (i == members.Count - 1 && !DrawOverBrain)
                {
                    npc.ai[3] = members[0];
                }
                if (i == members.Count - 1 && DrawOverBrain)
                {
                    if (npc.Distance(BrainPosition) > 120)
                    {
                        DrawOverBrain = false;
                    }
                    if (npc.Distance(BrainPosition) < 60)
                    {   
                        if (brain != null)
                        {
                            brain.GetGlobalNPC<BrainOfCthulhu>().openTimer = 12;
                        }
                    }
                }
                Vector2 targetPosition = head.Center - Vector2.UnitX.RotatedBy(head.rotation - adjustRotation) * SegmentLength * head.scale;
                npc.rotation = npc.DirectionTo(targetPosition).ToRotation();
                npc.rotation -= head.rotation - adjustRotation;
                npc.rotation = Functions.NormalizeRotation(npc.rotation, false);
                if (npc.rotation > MathF.PI * 0.3f)
                {
                    npc.rotation = MathF.PI * 0.3f;
                }
                else if (npc.rotation < -MathF.PI * 0.3f)
                {
                    npc.rotation = -MathF.PI * 0.3f;
                }
                npc.rotation *= 0.98f;
                npc.rotation += head.rotation - adjustRotation;
                npc.velocity = Vector2.Zero;// (targetPosition - Vector2.UnitX.RotatedBy(npc.rotation) * SegmentLength * npc.scale) - npc.Center;
                //npc.velocity *= 0.1f;
                npc.Center = targetPosition - Vector2.UnitX.RotatedBy(npc.rotation) * SegmentLength * npc.scale;
                npc.rotation += adjustRotation;
            }
            if (Dying)
            {
                dyingTimer--;
                if (dyingTimer <= 0)
                {
                    dyingTimer = 10;
                    Main.npc[members[Count - 1]].life = 0;
                    Main.npc[members[Count - 1]].ai[3] = -1;
                    Main.npc[members[Count - 1]].realLife = -1;
                    Main.npc[members[Count - 1]].checkDead();
                }
            }
            if (GoingToBrain)
            {
                if (inTheBrain >= members.Count)
                {
                    for (int i = 0; i < members.Count; i++)
                    {
                        BrainOfCthulhu.segmentsLifes.Add(Main.npc[members[i]].life);
                        Main.npc[members[i]].active = false;
                    }
                    Disable();
                    return;
                }
                if (inTheBrain > 0)
                {
                    if (brain != null)
                    {
                        brain.GetGlobalNPC<BrainOfCthulhu>().openTimer = 12;
                    }
                }
                NPC head = Main.npc[members[inTheBrain]];
                if (head.Distance(BrainPosition) < SegmentLength * head.scale)
                {
                    Main.npc[members[inTheBrain]].alpha = 255;
                    inTheBrain++;
                    hideHead++;
                    if (members.Count == inTheBrain)
                    {
                        for (int i = 0; i < members.Count; i++)
                        {
                            BrainOfCthulhu.segmentsLifes.Add(Main.npc[members[i]].life);
                            Main.npc[members[i]].active = false;
                        }
                        Disable();
                        return;
                    }
                    // else
                    // {
                    //     head.position = Main.npc[members[inTheBrain]].position;
                    //     head.rotation = Main.npc[members[inTheBrain]].rotation;
                    // }
                }
                else if (head.Distance(BrainPosition) > SegmentLength * 3 * head.scale)
                {
                    Main.npc[members[inTheBrain]].alpha = 0;
                    if (inTheBrain > 0)
                    {
                        NPC old = Main.npc[members[inTheBrain]];
                        inTheBrain--;
                        hideHead--;
                        Main.npc[members[inTheBrain]].Center = old.Center + (old.rotation - MathF.PI / 2).ToRotationVector2() * old.scale * SegmentLength;
                        Main.npc[members[inTheBrain]].position += Main.npc[members[inTheBrain]].DirectionTo(BrainPosition) * old.scale * SegmentLength;
                        // head.position = Main.npc[members[inTheBrain]].position;
                        // head.rotation = Main.npc[members[inTheBrain]].rotation;
                    }
                }
            }
            else
            {
                inTheBrain = 0;
                hideHead = 0;
            }
        }
        public override void PostDrawSegment(NPC member, int index, GetScreenCoordinatesContext context)
        {
            if (!Graphics.IsPointInBoundsOfScreen(member.Center, (int)(member.scale * 80)))
            {
                return;
            }
            if (member.type == NPCID.EaterofWorldsBody)
            {
                Texture2D Eye = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/EvilBosses/EaterofWorldsBody_Eye").Value; 
                Vector2 coordinates = new Vector2(24, 30) / member.frame.Size();
                GetScreenCoordinates(context, member, coordinates, out Vector2 screenCoordinates, out Color color, out _);
                float rotation = (screenCoordinates + Main.screenLastPosition).DirectionTo(Main.npc[members[0]].GetT().Target.Center).ToRotation();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(Eye, screenCoordinates, null, color, rotation, Eye.Size() / 2, member.scale, SpriteEffects.None, 0);
            }
            else if (member.type == NPCID.EaterofWorldsTail)
            {
                Texture2D Eye = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/EvilBosses/EaterofWorldsTail_Eye").Value; 
                Vector2 coordinates = new Vector2(50, 34) / member.frame.Size();
                GetScreenCoordinates(context, member, coordinates, out Vector2 screenCoordinates, out Color color, out _);
                float rotation = (screenCoordinates + Main.screenLastPosition).DirectionTo(Main.npc[members[0]].GetT().Target.Center).ToRotation();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(Eye, screenCoordinates, null, color, rotation, Eye.Size() / 2, member.scale, SpriteEffects.None, 0);
            }
            else
            {
                Texture2D Eye = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/EvilBosses/EaterofWorldsHead_Eye").Value; 
                Vector2 coordinates = new Vector2(16, 30) / member.frame.Size();
                GetScreenCoordinates(context, member, coordinates, out Vector2 screenCoordinates, out Color color, out _);
                float rotation = (screenCoordinates + Main.screenLastPosition).DirectionTo(Main.npc[members[0]].GetT().Target.Center).ToRotation();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(Eye, screenCoordinates, null, color, rotation, Eye.Size() / 2, member.scale, SpriteEffects.None, 0);
            }
        }
        public override bool PreDrawsegment(NPC member, int index, GetScreenCoordinatesContext context, ref ManagedShader shader)
        {
            if (member.type== NPCID.EaterofWorldsHead)
            {
                Texture2D Jaw = ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/EvilBosses/EaterofWorldsHead_Jaw").Value; 
                Vector2 origin = new Vector2(10, 14);
                Vector2 textureCoorinates = new Vector2(28, 50) / member.frame.Size();
                GetScreenCoordinates(context, member, textureCoorinates, out Vector2 screenCoordinates, out Color color, out float rotation);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                float adjustRotation = member.GetGlobalNPC<EaterofWorldsHead>().jawRotation;
                Main.spriteBatch.Draw(Jaw, screenCoordinates, null, color, rotation + adjustRotation, origin, member.scale, SpriteEffects.None, 0);
                textureCoorinates.Y = 1 - textureCoorinates.Y;
                GetScreenCoordinates(context, member, textureCoorinates, out screenCoordinates, out color, out rotation);
                origin.Y = Jaw.Height - origin.Y;
                Main.spriteBatch.Draw(Jaw, screenCoordinates, null, color, rotation - adjustRotation, origin, member.scale, SpriteEffects.FlipVertically, 0);
            }
            return true;
        }
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
        {
            if (!(GoingToBrain || DrawOverBrain))
            {
                base.PreDrawFirstNPCInGroup(spriteBatch);
            }
        }
        public override void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            if (GoingToBrain || DrawOverBrain)
            {
                base.PreDrawFirstNPCInGroup(spriteBatch);
            }
        }
    }
}
