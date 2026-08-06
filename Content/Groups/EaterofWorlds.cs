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
        }
        public override int[] NPCType => [NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail];
        public int inTheBrain;
        public bool GoingToBrain;
        public Vector2 BrainPosition;
        public override void UpdateGroup()
        {
            CheckMembers();
            float adjustRotation = VerticalSprites ? MathF.PI / 2 : 0;
            for (int i = inTheBrain + 1; i < members.Count; i++)
            {
                NPC npc = Main.npc[members[i]];
                NPC head = Main.npc[members[i - 1]];
                Vector2 targetPosition = head.Center - Vector2.UnitX.RotatedBy(head.rotation - adjustRotation) * SegmentLength * head.scale;
                npc.rotation = npc.DirectionTo(targetPosition).ToRotation();
                npc.rotation -= head.rotation - MathF.PI / 2;
                npc.rotation = Functions.NormalizeRotation(npc.rotation, false);
                if (npc.rotation > MathF.PI * 0.7f)
                {
                    npc.rotation = MathF.PI * 0.7f;
                }
                else if (npc.rotation < -MathF.PI * 0.7f)
                {
                    npc.rotation = -MathF.PI * 0.7f;
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
                NPC head = Main.npc[members[0]];
                if (head.Distance(BrainPosition) < SegmentLength * head.scale)
                {
                    Main.npc[members[inTheBrain]].alpha = 255;
                    inTheBrain++;
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
                        Main.npc[members[inTheBrain]].Center = old.Center + (old.rotation - MathF.PI / 2).ToRotationVector2() * old.scale * SegmentLength;
                        Main.npc[members[inTheBrain]].position += Main.npc[members[inTheBrain]].DirectionTo(BrainPosition) * old.scale * SegmentLength;
                        head.position = Main.npc[members[inTheBrain]].position;
                    }
                }

                Main.npc[members[inTheBrain]].position = head.position;
                Main.npc[members[inTheBrain]].rotation = head.rotation;
            }
            else
            {
                inTheBrain = 0;
            }
        }
    }
}
