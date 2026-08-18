using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terrapain.Content.NPCs.Servants.EvilBosses;
using Terrapain.Content.TUtilities;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public class Creepers : Group
    {
        public Creepers(float rotationSpeed, int AIType) 
        {
            this.rotationSpeed = rotationSpeed;
            maxRotationSpeed = rotationSpeed;
            this.AIType = AIType;
        }
        public override void OnInitialize()
        {
            int creeper = (int)Main.npc[members[1]].ai[0];
            while (creeper > 0)
            {
                AddMember(creeper);
                creeper = (int)Main.npc[creeper].ai[0];
            }
            maxRotationSpeed = rotationSpeed;
            autoDisable = false;
            for (int i = 0; i < Count; i++)
            {
                int mem1;
                int mem2;
                if (i == Count - 1)
                {
                    if (Count == 2)
                    {
                        return;
                    }
                    mem1 = members[Count - 1];
                    mem2 = members[0];
                }
                else
                {
                    mem1 = members[i];
                    mem2 = members[i + 1];
                }
                k.Add(new kishki(new SimulatedChain(AIType == 1? 16 : 8, 16, Main.npc[mem1].Center, 0, 1), mem1, mem2));
            }
        }
        public int attackCount;
        public int AIType;
        float rotation;
        public override int[] NPCType => [NPCID.Creeper];
        float maxRotationSpeed;
        float rotationSpeed;
        private struct kishki
        {
            public SimulatedChain chain;
            public int owner1;
            public int owner2;
            public int timeLeft = 60;
            public float progress => timeLeft / 60f;
            public kishki(SimulatedChain chain, int owner1, int owner2)
            {
                this.chain = chain;
                this.owner1 = owner1;
                this.owner2 = owner2;
            }
            public void Update()
            {
                if (owner1 > -1 && Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
                {
                    chain.Fragments[0].fixedAt = Main.npc[owner1].Center;
                }
                else
                {
                    owner1 = -1;
                    chain.Fragments[0].fixedAt = null;
                    timeLeft--;
                }
                if (owner2 > -1 && Main.npc[owner2].active && Main.npc[owner2].type == NPCID.Creeper)
                {
                    chain.Fragments[chain.Count - 1].fixedAt = Main.npc[owner2].Center;
                }
                else
                {
                    owner2 = -1;
                    chain.Fragments[chain.Count - 1].fixedAt = null;
                    if (owner1 > -1 && Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
                    {
                        timeLeft--;
                    }
                }
                if (timeLeft > 0)
                    chain.Update();
            }
        }
        List<kishki> k = new();
        public override void UpdateGroup()
        {
            if (k.Count == 0)
            {
                Disable();
                return;
            }
            if (Count > 0)
            {
                CheckMembers();
                if (AIType == 0)
                {    
                    Vector2 velocity = AverageVelocity;
                    Vector2 center = Center;
                    float maxCharge = 0;
                    foreach (var member in members)
                    {
                        NPC mem = Main.npc[member];
                        Vector2 dir1 = mem.Center - center;
                        dir1.RotateBy(rotationSpeed);
                        mem.velocity = center + dir1 - mem.Center;
                        Vector2 dir = mem.Center - center;
                        float distance = mem.Distance(center);
                        float force = distance / 80;
                        force -= 0.5f;
                        force *= 4;
                        mem.velocity -= force * dir.ToUnit();
                        mem.velocity += velocity;
                        maxCharge = MathF.Max(mem.GetGlobalNPC<Creeper>().charge, maxCharge);
                    }
                    if (maxCharge >= 1)
                    {
                        rotationSpeed = maxRotationSpeed;
                    }
                    foreach (var member in members)
                    {
                        NPC mem = Main.npc[member];
                        mem.GetGlobalNPC<Creeper>().charge = maxCharge;
                    }
                    rotationSpeed = MathF.Max(rotationSpeed - 0.005f, 0);
                }
                else if (EaterofWorldsHead.attack == 4)
                {
                    if (Main.npc[members[0]].ai[2] <= 0)
                    {
                        if (lines.Count != Count)
                        {
                            lines = new();
                            for (int i = 0; i < Count; i++)
                            {
                                lines.Add(new Lines());
                            }
                        }
                        float minCharge = -1;
                        for (int i = 0; i < Count; i++)
                        {
                            NPC mem = Main.npc[members[i]];
                            float rot = rotation + MathF.PI * 2 * i / Count;
                            Vector2 targetPosition = EaterofWorldsHead.savedVector + Vector2.UnitX.RotatedBy(rot) * (500 + mem.GetGlobalNPC<Creeper>().charge * 60);
                            AIHelper.CommonTerrapainFlyingMovement(mem, targetPosition, 3f, 30, 1f, 75);
                            if(minCharge == -1)
                            {
                                minCharge = mem.GetGlobalNPC<Creeper>().charge;
                            }
                            else
                            {
                                minCharge = MathF.Min(mem.GetGlobalNPC<Creeper>().charge, minCharge);
                            }
                            Lines l = lines[i];
                            l.start = mem.Center;
                            l.direction = mem.DirectionTo(EaterofWorldsHead.savedVector);
                            lines[i] = l;
                        }
                        if (attackCount == 2)
                        {
                            lines = new();
                            minCharge = 0;
                        }

                        foreach (var member in members)
                        {
                            NPC mem = Main.npc[member];
                            mem.GetGlobalNPC<Creeper>().charge = minCharge;
                        }
                        rotation += 0.03f * (1 - (minCharge * minCharge * minCharge));
                    }
                    else
                    {
                        lines = new();
                        if (Main.npc[members[0]].ai[2] == 1)
                        {
                            attackCount++;
                            List<int> newMembers = new List<int>(members);
                            for (int i = 0; i < members.Count; i++)
                            {
                                int index = (i + Count / 2) % Count;
                                newMembers[index] = members[i];
                            }
                            members = newMembers;
                        }
                    } 
                }
                else
                {
                    lines = new();
                }
            }
            for (int i = 0; i < k.Count; i++)
            {
                kishki kishka = k[i];
                kishka.Update();
                k[i] = kishka;
                if (k[i].timeLeft <= 0)
                {
                    k.RemoveAt(i);
                    i--;
                }
            }
        }
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terrapain/Content/Groups/kishka").Value;
            for (int i = 0; i < k.Count; i++)
            {
                k[i].chain.DrawSmoothed(spriteBatch, texture, null, Color.White * k[i].progress, true, 1);
            }
        }
        struct Lines
        {
            public Vector2 start;
            public Vector2 direction;
            public Color color;
            static Color[] colors = [Color.Red, Color.Yellow, Color.Green];
            public Lines(Vector2 Start, Vector2 dir)
            {
                start = Start;
                direction = dir;
                color = colors[TGlobalNPC.random.Next(3)];
            }
            public Lines()
            {
                color = colors[TGlobalNPC.random.Next(3)];
            }
        }
        static float LinesAlpha;
        static List<Lines> lines = new();


        static RenderTarget2D renderTarget;
        static RenderTarget2D saveScreenRenderTarget;
        void updateTarget()
        {
            if (renderTarget == null || renderTarget.Height != Main.screenHeight || renderTarget.Width != Main.screenWidth)
            {
                renderTarget = new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
            if (saveScreenRenderTarget == null || saveScreenRenderTarget.Height != Main.screenHeight || saveScreenRenderTarget.Width != Main.screenWidth)
            {
                saveScreenRenderTarget = new RenderTarget2D(
                    Main.graphics.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight,
                    false,
                    Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
        }
        public override void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (Count == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terrapain/Content/Groups/kishka").Value;
                for (int i = 0; i < k.Count; i++)
                {
                    k[i].chain.DrawSmoothed(spriteBatch, texture, null, Color.White * k[i].progress, true, 1);
                }
            }
            if (lines.Count > 0)
                {
                    var originalRenderTargets = Main.graphics.GraphicsDevice.GetRenderTargets();
                    if (WorldDifficultySystem.suicide)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                        updateTarget();
                        Main.graphics.GraphicsDevice.SetRenderTarget(saveScreenRenderTarget);
                        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                        spriteBatch.Draw(originalRenderTargets[0].RenderTarget as RenderTarget2D, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                        Main.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                        Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Texture2D tex = null;
                    if (GraphicsConfig.Instance.shaders == GraphicsConfig.GraphicsLevel.Potato)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        tex = ExtraTextureRegistry.CubedGradient10Mirrored.Value;
                    }
                    foreach (var line in lines)
                    {
                        Vector2 p1 = line.start - screenPos;
                        float? num1 = line.direction.X == 0? null : (20 - p1.X) / line.direction.X;
                        float? num2 = line.direction.Y == 0? null : (20 - p1.Y) / line.direction.Y;
                        float? num3 = line.direction.X == 0? null : (Main.screenWidth - 20 - p1.X) / line.direction.X;
                        float? num4 = line.direction.Y == 0? null : (Main.screenHeight - 20 - p1.Y) / line.direction.Y;
                        Vector2 p2 = Vector2.Zero;
                        Vector2 p3 = Vector2.Zero;
                        if (num1.HasValue)
                        {
                            Vector2 _p2 = p1 + line.direction * num1.Value;
                            if (_p2.Y > 20 && _p2.Y < Main.screenHeight - 20)
                            {
                                p2 = _p2;
                            }
                        }
                        if (num2.HasValue && p2 == Vector2.Zero)
                        {
                            Vector2 _p2 = p1 + line.direction * num2.Value;
                            if (_p2.X > 20 && _p2.Y < Main.screenWidth - 20)
                            {
                                p2 = _p2;
                            }
                        }
                        if (p2 != Vector2.Zero)
                        {
                            if (num3.HasValue)
                            {
                                Vector2 _p3 = p1 + line.direction * num3.Value;
                                if (_p3.Y > 20 && _p3.Y < Main.screenHeight - 20)
                                {
                                    p3 = _p3;
                                }
                            }
                            if (num4.HasValue && p3 == Vector2.Zero)
                            {
                                Vector2 _p3 = p1 + line.direction * num4.Value;
                                if (_p3.X > 20 && _p3.X < Main.screenWidth - 20)
                                {
                                    p3 = _p3;
                                }
                            }
                            if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                            {
                                ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
                                Shade.TrySetParameter("lenght", 900);
                                Shade.TrySetParameter("width", 8);
                                spriteBatch.End();
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                            }
                            spriteBatch.DrawLine(p3 + Main.screenPosition, p2 + Main.screenPosition, line.color, 8, tex);
                        }
                    }
                    if (GraphicsConfig.Instance.shaders != GraphicsConfig.GraphicsLevel.Potato)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                    if (WorldDifficultySystem.suicide)
                    {
                        spriteBatch.End();
                        Main.graphics.GraphicsDevice.SetRenderTargets(originalRenderTargets);

                        if (saveScreenRenderTarget != null)
                        {
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                            spriteBatch.Draw(saveScreenRenderTarget, Vector2.Zero, Color.White);
                            spriteBatch.End();
                        }
                        if (renderTarget != null)
                        {
                            ManagedShader shader = ShaderManager.GetShader("Terrapain.PlayerMask");
                            shader.TrySetParameter("player", Main.LocalPlayer.Center - screenPos);
                            shader.TrySetParameter("w", Main.screenWidth);
                            shader.TrySetParameter("h", Main.screenHeight);
                            shader.TrySetParameter("radius1", 100f);
                            shader.TrySetParameter("radius2", 150f);
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, shader.WrappedEffect, new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
                            spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                            spriteBatch.End();
                        }
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
        }
    }
}
