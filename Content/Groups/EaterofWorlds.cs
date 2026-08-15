using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terraria;
using Terraria.ID;

namespace Terrapain.Content.Groups
{
    public class EaterofWorlds : Snake
    {
        public override void SetDefaults()
        {
            VerticalSprites = true;
            SegmentLength = 23;
            Smoothing = true;
            Draw = true;
            BodyFrame = new Rectangle(0, 22, 46, 46);
        }
        public override int[] NPCType => [NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail];
        public int inTheBrain;
        public bool GoingToBrain;
        public Vector2 BrainPosition;
        public bool DrawOverBrain;
        public override void UpdateGroup()
        {
            NPC brain = Main.npc[EaterofWorldsHead.BrainofCthulhu];
            BrainPosition = brain.Center;
            CheckMembers();
            if (GoingToBrain)
            {
                NPC head = Main.npc[members[0]];
                Main.npc[members[inTheBrain]].position = head.position;
                Main.npc[members[inTheBrain]].rotation = head.rotation;
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
                        brain.GetGlobalNPC<BrainOfCthulhu>().openTimer = 12;
                    }
                }
                Vector2 targetPosition = head.Center - Vector2.UnitX.RotatedBy(head.rotation - adjustRotation) * SegmentLength * head.scale;
                npc.rotation = npc.DirectionTo(targetPosition).ToRotation();
                npc.rotation -= head.rotation - MathF.PI / 2;
                npc.rotation = Functions.NormalizeRotation(npc.rotation, false);
                if (npc.rotation > MathF.PI * 0.3f)
                {
                    npc.rotation = MathF.PI * 0.3f;
                }
                else if (npc.rotation < -MathF.PI * 0.3f)
                {
                    npc.rotation = -MathF.PI * 0.3f;
                }
                npc.rotation += head.rotation - MathF.PI / 2;
                npc.velocity = Vector2.Zero;
                npc.Center = targetPosition - Vector2.UnitX.RotatedBy(npc.rotation) * SegmentLength * npc.scale;
                npc.rotation += adjustRotation;
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
                    brain.GetGlobalNPC<BrainOfCthulhu>().openTimer = 12;
                    
                }
                NPC head = Main.npc[members[0]];
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
                    else
                    {
                        head.position = Main.npc[members[inTheBrain]].position;
                    }
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
                        head.position = Main.npc[members[inTheBrain]].position;
                    }
                }
            }
            else
            {
                inTheBrain = 0;
                hideHead = 0;
            }
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
