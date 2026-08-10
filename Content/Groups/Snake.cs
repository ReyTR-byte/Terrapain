using JetBrains.Annotations;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.TUtilities.Graphics;
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
        public Rectangle BodyFrame;
        public int hideTail;
        public int hideHead;
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
        List<Vector4> GetAlphaColors;
        List<Vector4> GetColorColors;
        List<Vector2> SmoothPoints;
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
        {
            if (Draw)
            {
                if (Smoothing && members.Count - hideHead - hideTail > 2)
                {
                    NPC head = Main.npc[members[hideHead]];
                    Texture2D texture;
                    if (head.type < NPCID.Count)
                    {
                        texture = TextureAssets.Npc[head.type].Value;
                    }
                    else
                    {
                        texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(head.type).Texture).Value;
                    }
                    head.GetT().TDrawNPC(spriteBatch, head, texture);


                    Vector4 tailAlpha = hideTail > 0? Vector4.Zero : head.GetAlpha(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                    Vector4 tailColor = hideTail > 0? Vector4.Zero : head.GetColor(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                    NPC tail = Main.npc[members[members.Count - 1 - hideTail]];
                    Vector4 headAlpha = hideHead > 0? Vector4.Zero : tail.GetAlpha(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
                    Vector4 headColor = hideHead > 0? Vector4.Zero : tail.GetColor(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
                    int segmentsCount = members.Count - 2 - hideHead - hideTail;
                    float adjustRotation = VerticalSprites ? MathF.PI / 2 : 0;
                    Vector2 dirIn = (tail.rotation - adjustRotation).ToRotationVector2();
                    Vector2 dirOut = (head.rotation - adjustRotation).ToRotationVector2();
                    if (!Main.gamePaused)
                    {
                        List<Vector2> Points = [];
                        for (int i = members.Count - 1 - hideTail; i > hideHead; i--)
                        {
                            NPC mem = Main.npc[members[i]];
                            Points.Add(mem.Center + Vector2.UnitX.RotatedBy(mem.rotation - adjustRotation) * SegmentLength * mem.scale);
                        }
                        SmoothPoints = Graphics.SmoothTrail(Points, 6, dirIn, dirOut);
                        GetAlphaColors = [];
                        GetColorColors = [];
                        bool GetColorisNotNull = false;
                        for (int i = members.Count - hideTail - 2; i > hideHead; i--)
                        {
                            NPC mem = Main.npc[members[i]];
                            Color lightColor = Lighting.GetColor(mem.Center.ToTileCoordinates());
                            GetAlphaColors.Add(mem.GetAlpha(lightColor).ToVector4());
                            Color color = mem.GetColor(lightColor);
                            if (color != Color.Transparent)
                            {
                                GetColorisNotNull = true;
                            }
                            GetColorColors.Add(color.ToVector4());
                        }
                        if (!GetColorisNotNull)
                        {
                            GetColorColors = null;
                        }
                    }
                    NPC body = Main.npc[members[hideHead + 1]];
                    if (tail.type < NPCID.Count)
                    {
                        texture = TextureAssets.Npc[body.type].Value;
                    }
                    else
                    {
                        texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(body.type).Texture).Value;
                    }
                    float halfWidth = 0;
                    if (VerticalSprites)
                    {
                        halfWidth = texture.Width / 2;
                    }
                    else
                    {
                        halfWidth = texture.Height / 2;
                    }
                    halfWidth *= body.scale;
                    ManagedShader shader = null;
                    if (VerticalSprites)
                    {
                        shader = ShaderManager.GetShader("Terrapain.VerticalSnake");
                    }
                    Graphics.RenderSnakeBody(SmoothPoints, halfWidth, members.Count - hideHead - hideTail - 2, BodyFrame, texture, GetAlphaColors, GetColorColors, headAlpha, headColor, tailAlpha, tailColor, shader, dirIn, dirOut);
                    if (tail.type < NPCID.Count)
                    {
                        texture = TextureAssets.Npc[tail.type].Value;
                    }
                    else
                    {
                        texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(tail.type).Texture).Value;
                    }
                    tail.GetT().TDrawNPC(spriteBatch, tail, texture);
                }
                else
                {
                    for (int i = members.Count - 1 - hideTail; i > -1 + hideHead; i--)
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
}
