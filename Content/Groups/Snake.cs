using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public abstract class Snake : Group
    {
        public bool Smoothing;
        public bool Draw;
        public float SegmentLength;
        public bool VerticalSprites;
        public virtual void RebuidSnake()
        {
            if (!Main.npc[members[0]].active || (NPCType.Length != 0 && !NPCType.Contains(Main.npc[members[0]].type)))
            {
                Disable();
                return;
            }
            CheckMembers();
            int end = -1;
            for (int i = 1; i < members.Count; i++)
            {
                NPC head = Main.npc[members[i - 1]];
                NPC mem = Main.npc[members[i]];
                end = i + 1;
                if (head.ai[1] != mem.whoAmI)
                {
                    end--;
                    break;
                }
            }
            if (end > 0)
            {
                for (int i = end; i < members.Count; i++)
                {
                    DelMember(i);
                }
            }
            else
            {
                end = 1;
            }

            while(true)
            {
                if (Main.npc[members[end - 1]].ai[1] >= 0 && Main.npc[members[end - 1]].ai[1] < Main.maxNPCs)
                {
                    NPC tail = Main.npc[(int)Main.npc[members[end - 1]].ai[1]];
                    if (NPCType.Length == 0 || NPCType.Contains(tail.type))
                    {
                        AddMember(tail.whoAmI);
                        end++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        public virtual void SetDefaults()
        {

        }
        public override void OnInitialize()
        {
            RebuidSnake();
            SetDefaults();
        }
        public override void UpdateGroup()
        {
            CheckMembers();
            float adjustRotation = VerticalSprites? MathF.PI / 2 : 0;
            for (int i = 1; i < members.Count; i++)
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
        }
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
        {
            if (Draw)
            {
                for (int i = members.Count - 1; i > -1; i--)
                {
                    NPC npc = Main.npc[members[i]];
                    Texture2D texture;
                    if (npc.type < NPCID.Count)
                    {
                        texture = TextureAssets.Npc[npc.type].Value;
                    }
                    else
                    {
                        texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(npc.type).Texture).Value;
                    }
                    npc.GetT().TDrawNPC(spriteBatch, npc, texture);
                }
            }
        }
    }
}
