using JetBrains.Annotations;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Config;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Terrapain.Content.Groups
{
    public abstract class Snake : Group
    {
        public enum DrawStyle
        {
            Default,
            Smoothed,
            SmoothedPartByPart
        }
        public bool smoothHead;
        public bool smoothTail;
        int drawSmoothedCount => Count - hideTail - hideHead - (smoothHead? 0 : 1) - (smoothTail? 0 : 1);
        public DrawStyle drawStyle;
        public bool Draw;
        public float SegmentLength;
        public bool VerticalSprites;
        public Rectangle BodyFrame;
        public int hideTail;
        public int hideHead;
        public virtual void RebuidSnake()
        {
            if (members.Count == 0)
            {
                Disable();
                return;
            }
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
                end = i;
                if (head.ai[1] != mem.whoAmI || mem.ai[0] != head.whoAmI)
                {
                    break;
                }
            }
            if (end > 0)
            {
                for (int i = end; i < members.Count;)
                {
                    DelMember(i);
                }
                end--;
            }
            else
            {
                end = 0;
            }
            while(true)
            {
                if (end >= Count)
                {
                    foreach(var npc in members)
                    {
                        Main.npc[npc].active = false;
                    }
                    break;
                }
                if (Main.npc[members[end]].ai[1] >= 0 && Main.npc[members[end]].ai[1] < Main.maxNPCs)
                {
                    NPC tail = Main.npc[(int)Main.npc[members[end]].ai[1]];
                    if (tail.ai[0] == members[end] && (NPCType.Length == 0 || NPCType.Contains(tail.type)))
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
        public virtual void CheckSnake()
        {
            int state = 0;
            NPC old = null;
            for (int i = 0; i < Count; i++)
            {
                NPC mem = Main.npc[members[i]];
                if (mem.active && (NPCType.Length == 0 || NPCType.Contains(mem.type)))
                {
                    if (old != null)
                    {    
                        if (old.GetT().NPCBehaviour is ISnakePart)
                        {
                            var s = old.GetT().NPCBehaviour as ISnakePart;
                            s.SetGroup(whoAmI, i - 1);
                            if (state == 0)
                            {
                                s.UpdateAsHead(old);
                                state = 1;
                            }
                            else if (state == 1)
                            {
                                s.UpdateAsBody(old);
                            }
                        }
                    }
                    old = mem;
                }
                else
                {
                    if (old != null)
                    {    
                        if (old.GetT().NPCBehaviour is ISnakePart)
                        {
                            var s = old.GetT().NPCBehaviour as ISnakePart;
                            s.SetGroup(whoAmI, i - 1);
                            s.UpdateAsTail(old);
                        }
                    }
                    old = null;
                    state = 0;

                    DelMember(i);
                    i--;
                }
            }
            if (old != null)
            {    
                if (old.GetT().NPCBehaviour is ISnakePart)
                {
                    var s = old.GetT().NPCBehaviour as ISnakePart;
                    s.SetGroup(whoAmI, Count - 1);
                    s.UpdateAsTail(old);
                }
            }
            RebuidSnake();
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
                npc.rotation -= head.rotation - adjustRotation;
                npc.rotation = Functions.NormalizeRotation(npc.rotation, false);
                if (npc.rotation > MathF.PI * 0.7f)
                {
                    npc.rotation = MathF.PI * 0.7f;
                }
                else if (npc.rotation < -MathF.PI * 0.7f)
                {
                    npc.rotation = -MathF.PI * 0.7f;
                }
                npc.rotation += head.rotation - adjustRotation;
                npc.velocity = Vector2.Zero;
                npc.Center = targetPosition - Vector2.UnitX.RotatedBy(npc.rotation) * SegmentLength * npc.scale;
                npc.rotation += adjustRotation;
            }
        }
        public virtual bool PreDrawsegment(NPC member, int index, GetScreenCoordinatesContext context, ref ManagedShader shader)
        {
            return true;
        }
        public virtual void PostDrawSegment(NPC member, int index, GetScreenCoordinatesContext context)
        {
            
        }
        List<Vector4> GetAlphaColors;
        List<Vector4> GetColorColors;
        List<Vector2> SmoothPoints;

        public enum GetScreenCoordinatesContext
        {
            smoothed,
            normal,
        }
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
        {
            if (Draw)
            {
                if (drawStyle == DrawStyle.Default || drawSmoothedCount <= 0 || !GraphicsConfig.Instance.smoothing)
                {
                    for (int i = members.Count - 1 - hideTail; i > -1 + hideHead; i--)
                    {
                        NPC npc = Main.npc[members[i]];
                        ManagedShader _ = null;
                        if (PreDrawsegment(npc, i, GetScreenCoordinatesContext.normal, ref _))
                        {
                            Texture2D texture = npc.GetT().TGetTexture().Value;
                            npc.GetT().TDrawNPC(spriteBatch, npc, texture);
                        }
                        PostDrawSegment(npc, i, GetScreenCoordinatesContext.normal);
                    }
                }
                else
                {
                    switch (drawStyle)
                    {
                        case DrawStyle.Smoothed:
                            {
                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                                NPC head = Main.npc[members[hideHead]];
                                NPC tail = Main.npc[members[members.Count - 1 - hideTail]];
                                Texture2D texture;
                                if (hideTail == 0)
                                {
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


                                Vector4 headAlpha = hideHead > 0? Vector4.Zero : head.GetAlpha(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                                Vector4 headColor = hideHead > 0? Vector4.Zero : head.GetColor(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                                Vector4 tailAlpha = hideTail > 0? Vector4.Zero : tail.GetAlpha(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
                                Vector4 tailColor = hideTail > 0? Vector4.Zero : tail.GetColor(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
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
                                    for (int i = Math.Min(members.Count - 2, members.Count - hideTail - 1); i > Math.Max(hideHead - 1, 0); i--)
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
                                if (body.type < NPCID.Count)
                                {
                                    texture = TextureAssets.Npc[body.type].Value;
                                }
                                else
                                {
                                    texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(body.type).Texture).Value;
                                }
                                float halfWidth;
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
                                Graphics.RenderSnakeBody(SmoothPoints, halfWidth, GetAlphaColors.Count, BodyFrame, texture, GetAlphaColors, GetColorColors, tailAlpha, tailColor, headAlpha, headColor, shader, dirIn, dirOut);


                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                                if (hideHead == 0)
                                {
                                    if (head.type < NPCID.Count)
                                    {
                                        texture = TextureAssets.Npc[head.type].Value;
                                    }
                                    else
                                    {
                                        texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(head.type).Texture).Value;
                                    }
                                    head.GetT().TDrawNPC(spriteBatch, head, texture);
                                }
                            }
                            break;
                        case DrawStyle.SmoothedPartByPart:
                            {
                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                                NPC head = Main.npc[members[hideHead]];
                                NPC tail = Main.npc[members[members.Count - 1 - hideTail]];
                                Texture2D texture;
                                if (hideTail == 0 && !smoothTail)
                                {
                                    ManagedShader _ = null;
                                    if (PreDrawsegment(tail, members.Count - 1 - hideTail, GetScreenCoordinatesContext.normal, ref _))
                                    {
                                        texture = head.GetT().TGetTexture().Value;
                                        tail.GetT().TDrawNPC(spriteBatch, tail, texture);
                                    }
                                    PostDrawSegment(tail, members.Count - 1 - hideTail, GetScreenCoordinatesContext.normal);
                                }


                                Vector4 headAlpha = hideHead > 0? Vector4.Zero : head.GetAlpha(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                                Vector4 headColor = hideHead > 0? Vector4.Zero : head.GetColor(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                                Vector4 tailAlpha = hideTail > 0? Vector4.Zero : tail.GetAlpha(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
                                Vector4 tailColor = hideTail > 0? Vector4.Zero : tail.GetColor(Lighting.GetColor(tail.Center.ToTileCoordinates())).ToVector4();
                                float adjustRotation = VerticalSprites ? MathF.PI / 2 : 0;
                                Vector2 dirIn = (tail.rotation - adjustRotation).ToRotationVector2();
                                Vector2 dirOut = (head.rotation - adjustRotation).ToRotationVector2();

                                int start = Count - 1 - hideTail - (smoothTail? 0 : 1);
                                int end = start - drawSmoothedCount;
                                if (!Main.gamePaused)
                                {
                                    List<Vector2> Points = [];
                                    for (int i = start; i > end; i--)
                                    {
                                        NPC mem = Main.npc[members[i]];
                                        Points.Add(mem.Center - Vector2.UnitX.RotatedBy(mem.rotation - adjustRotation) * SegmentLength * mem.scale);
                                    }
                                    {
                                        NPC mem = Main.npc[members[end + 1]];
                                        Points.Add(mem.Center + Vector2.UnitX.RotatedBy(mem.rotation - adjustRotation) * SegmentLength * mem.scale);
                                    }
                                    SmoothPoints = Graphics.SmoothTrail(Points, 6, dirIn, dirOut);
                                    GetAlphaColors = [];
                                    GetColorColors = [];
                                    for (int i = start; i > end; i--)
                                    {
                                        NPC mem = Main.npc[members[i]];
                                        Color lightColor = Lighting.GetColor(mem.Center.ToTileCoordinates());
                                        GetAlphaColors.Add(mem.GetAlpha(lightColor).ToVector4());
                                    }
                                }
                                NPC body = Main.npc[members[hideHead + 1]];
                                float halfWidth;
                                if (VerticalSprites)
                                {
                                    halfWidth = body.frame.Width / 2;
                                }
                                else
                                {
                                    halfWidth = body.frame.Height / 2;
                                }
                                halfWidth *= body.scale;
                                float WidthFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
                                {
                                    return halfWidth;
                                }
                                ManagedShader shader;
                                if (VerticalSprites)
                                {
                                    shader = ShaderManager.GetShader("Terrapain.VerticalSnake");
                                }
                                else
                                {
                                    shader = ShaderManager.GetShader("Terrapain.SnakeShader");
                                }
                                float TotalLength = 0;
                                float[] lengthes = new float[SmoothPoints.Count - 1];
                                for (int i = 0; i < SmoothPoints.Count - 1; i++)
                                {
                                    TotalLength += SmoothPoints[i].Distance(SmoothPoints[i + 1]);
                                    lengthes[i] = TotalLength;
                                }
                                float segmentLength = TotalLength / drawSmoothedCount;
                                int j = 0;
                                int g = 0;
                                Vector4 color3 = GetAlphaColors[0];
                                dirOut = SmoothPoints[0].DirectionTo(SmoothPoints[1]);
                                if (!smoothTail)
                                {
                                    color3 = tailAlpha;
                                    dirOut = (Main.npc[members[start + 1]].rotation + adjustRotation).ToRotationVector2();
                                }
                                for (int i = start; i > end; i--)
                                {
                                    g++;
                                    int old = j;
                                    float targetLength = g * segmentLength;
                                    while (j < lengthes.Length)
                                    {
                                        if (lengthes[j] >= targetLength)
                                        {
                                            j++;
                                            break;
                                        }
                                        j++;
                                    }
                                    List<Vector2> points = new (SmoothPoints);
                                    if (j < SmoothPoints.Count - 1)
                                    {
                                        points.RemoveRange(j + 1, SmoothPoints.Count - 1 - j);
                                    }
                                    if (old > 0)
                                    {
                                        points.RemoveRange(0, old);
                                    }

                                    Vector4 color1 = color3;
                                    Vector4 color2 = GetAlphaColors[g - 1];
                                    if(g == GetAlphaColors.Count)
                                    {
                                        if (smoothHead)
                                        {
                                            color3 = color2;
                                        }    
                                        else
                                        {
                                            color3 = head.GetAlpha(Lighting.GetColor(head.Center.ToTileCoordinates())).ToVector4();
                                        }
                                    }
                                    else
                                    {
                                        color3 = (color2 + GetAlphaColors[g]) * 0.5f;
                                    }
                                    Color ColorFunction(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
                                    {
                                        //return Color.White;
                                        trailLengthInterpolant = trailLengthInterpolant * 2;
                                        if (trailLengthInterpolant < 1)
                                        {
                                            return new Color(color1 * (1 - trailLengthInterpolant) + color2 * (trailLengthInterpolant));
                                        }
                                        else
                                        {
                                            trailLengthInterpolant -= 1;
                                            return new Color(color2 * (1 - trailLengthInterpolant) + color3 * (trailLengthInterpolant));
                                        }
                                    }
                                    TrailSettings settings = new TrailSettings(WidthFunction, ColorFunction);
                                    dirIn = dirOut;
                                    if (j + 1 < SmoothPoints.Count)
                                    {
                                        dirOut = SmoothPoints[j].DirectionFrom(SmoothPoints[j - 1]) + SmoothPoints[j].DirectionTo(SmoothPoints[j + 1]);
                                        dirOut.Normalize();
                                    }
                                    else
                                    {
                                        if (smoothHead)
                                        {
                                            dirOut = SmoothPoints[j].DirectionFrom(SmoothPoints[j - 1]);
                                        }
                                        else
                                        {
                                            dirOut = (head.rotation + adjustRotation).ToRotationVector2();
                                            color3 = headAlpha;
                                        }
                                    }
                                    
                                    Graphics.MakeTrailVertices(points, settings, dirIn, dirOut);

                                    NPC targetNPC = Main.npc[members[i]];
                                    texture = targetNPC.GetT().TGetTexture().Value;
                                    Rectangle Frame = targetNPC.frame;
                                    Vector2 size = texture.Size();
                                    Vector4 frame = new Vector4(Frame.X / size.X, Frame.Y / size.Y, Frame.Width / size.X, Frame.Height / size.Y);
                                    shader.TrySetParameter("frame", frame);

                                    if (PreDrawsegment(targetNPC, i, GetScreenCoordinatesContext.smoothed, ref shader))
                                    {    
                                        Main.instance.GraphicsDevice.Textures[1] = texture;
                                        Graphics.DrawPimitives(shader);
                                    }
                                    PostDrawSegment(targetNPC, i, GetScreenCoordinatesContext.smoothed);
                                }
                                //Graphics.RenderSnakeBody(SmoothPoints, halfWidth, GetAlphaColors.Count, BodyFrame, texture, GetAlphaColors, GetColorColors, tailAlpha, tailColor, headAlpha, headColor, shader, dirIn, dirOut);


                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                                if (hideHead == 0 && !smoothHead)
                                {
                                    texture = head.GetT().TGetTexture().Value;
                                    ManagedShader _ = null;
                                    if (PreDrawsegment(head, hideHead, GetScreenCoordinatesContext.normal, ref _))
                                        head.GetT().TDrawNPC(spriteBatch, head, texture);
                                    PostDrawSegment(head, hideHead, GetScreenCoordinatesContext.normal);
                                }
                            }
                            break; 
                    }
                }
            }
        }
        public void GetScreenCoordinates(GetScreenCoordinatesContext context, NPC npc, Vector2 textureCoordinates, out Vector2 screenCoordinates, out Color color, out float rotation)
        {
            screenCoordinates = Vector2.Zero;
            rotation = 0;
            color = Color.Transparent;
            switch (context)
            {
                case GetScreenCoordinatesContext.normal:
                    Vector2 size = npc.frame.Size();
                    if (npc.spriteDirection == -1)
                    {
                        textureCoordinates.X = 1 - textureCoordinates.X;
                    }
                    Vector2 offset = size * (textureCoordinates);
                    offset -= npc.GetT().drawCenter;
                    offset *= npc.scale;
                    offset.RotateBy(npc.rotation);
                    offset += npc.GetT().drawOffcet;
                    screenCoordinates = npc.Center + offset - Main.screenPosition;
                    rotation = npc.rotation;
                    color = npc.GetAlpha(Lighting.GetColor(npc.Center.ToTileCoordinates()));
                    break;
                case GetScreenCoordinatesContext.smoothed:
                        screenCoordinates = Vector2.Zero;
                        color = Color.Transparent;
                        rotation = 0;
                        for (int i = 0; i < Graphics.VerticesIndex - 2; i += 2)
                        {
                            if (Graphics.MainVertices[i].TextureCoordinates.X < textureCoordinates.X && Graphics.MainVertices[i + 2].TextureCoordinates.X > textureCoordinates.X)
                            {
                                float value = (textureCoordinates.X - Graphics.MainVertices[i].TextureCoordinates.X) / (Graphics.MainVertices[i + 2].TextureCoordinates.X - Graphics.MainVertices[i].TextureCoordinates.X);
                                Vector4 color1 = Graphics.MainVertices[i].Color.ToVector4();
                                Vector4 color2 = Graphics.MainVertices[i + 2].Color.ToVector4();
                                color = new Color(color1 * (1 - value) + color2 * (value));
                                Vector2 pos1 = Graphics.MainVertices[i + 1].Position * textureCoordinates.Y + Graphics.MainVertices[i].Position * (1 - textureCoordinates.Y);
                                Vector2 pos2 = Graphics.MainVertices[i + 3].Position * textureCoordinates.Y + Graphics.MainVertices[i + 2].Position * (1 - textureCoordinates.Y);
                                screenCoordinates = pos1 * (1 - value) + pos2 * (value);
                                rotation = pos1.DirectionTo(pos2).ToRotation();
                                return;
                            }
                        }
                    break;
            }
        }
    }
    public interface ISnakePart
    {
        public abstract void UpdateAsHead(NPC npc);

        public abstract void UpdateAsBody(NPC npc);

        public abstract void UpdateAsTail(NPC npc);

        public abstract void SetGroup(int group, int member);
    }
}
