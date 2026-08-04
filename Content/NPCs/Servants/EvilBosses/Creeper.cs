using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content.Groups;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Servants.EvilBosses
{
    public class Creeper : NPCBehaviour
    {
        public override int type => NPCID.Creeper;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[type] = 20;
            NPCID.Sets.TrailingMode[type] = 1;
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.hide = true;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (source.Context == "MakeGroup")
            {
                Group.NewGroup(new Creepers(npc.whoAmI, (int)npc.ai[0], 0.6f));
            }
        }
        float charge;
        float rotationSpeed;
        public override bool ModPreAI(NPC npc)
        {
            npc.TargetClosest();
            float charge2 = charge * charge;
            if (npc.ai[2] <= 0)
            {
                Vector2 targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center).RotatedBy(0.05f * (1 - charge2)) * (210 + charge2 * 40);
                rotationSpeed += 0.001f;
                rotationSpeed = MathF.Min(0.1f, rotationSpeed);
                CommonTerrapainFlyingMovement(npc, targetPosition, rotationSpeed, 16, 0.08f, 10);
                if (npc.Distance(t.Target.Center) < 275)
                {
                    charge += 0.02f;
                }
                else
                {
                    charge = MathF.Max(charge - 0.05f, 0);
                }
                if (charge >= 1)
                {
                    charge = 0;
                    npc.ai[2] = 60;
                    if (WorldDifficultySystem.suicide)
                    {
                        npc.velocity = npc.DirectionTo(SmartShoot(npc.Center, 25, t.Target.Center, t.Target.velocity, 30)) * 25;
                    }
                    else
                    {
                        npc.velocity = npc.DirectionTo(t.Target.Center) * 25;
                    }
                }
            }
            else
            {
                trailLength = Math.Min(trailLength + 1, Math.Min((int)npc.ai[2] - 1, 20));
                npc.velocity = npc.velocity.Normalized() * MathF.Max(npc.velocity.Length() - 0.2f, 0);
            }
            npc.ai[2]--;
            npc.color = new Color(Color.White.ToVector3() * (1 - charge2) + Color.ForestGreen.ToVector3() * charge2);
            return false;
        }
        public override void DrawBehind(NPC npc, int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }
        int trailLength;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (trailLength > 1)
            {
                float startWidth = npc.width + 8;
                float endWidth = 0;
                Color startColor = new Color(Color.Pink.ToVector4() * drawColor.ToVector4() * 0.5f);
                Color endColor = Color.Transparent;
                float WidthFunction(float progress, float length, float totalLength, Vector2 position)
                {
                    return MathHelper.Lerp(startWidth, endWidth, progress) / 2;
                }
                Color ColorFunction(float progress, float length, float totalLength, Vector2 position)
                {
                    return new Color(startColor.ToVector4() * (1 - progress) + endColor.ToVector4() * progress);
                }
                List<Vector2> points = new();
                for (int i = 0; i < trailLength; i++)
                {
                    if (npc.oldPos[i] == Vector2.Zero)
                    {
                        break;
                    }
                    points.Add(npc.oldPos[i] + npc.Size / 2);
                }
                ManagedShader shader = ShaderManager.GetShader("Terrapain.TrailShader");
                TrailSettings ts = new TrailSettings(WidthFunction, ColorFunction, Shader: shader);
                Graphics.RenderTrail(points, ts);
                ManagedShader startShader = ShaderManager.GetShader("Terrapain.TrailStart");
                var blackTile = ExtraTextureRegistry.BlackPixel;
                float rotation = points[1].DirectionTo(points[0]).ToRotation();
                int num = 0;
                Vector2 pos = points[num];
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, startShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(blackTile.Value, pos - Main.screenPosition, null, startColor, rotation, new Vector2(0, 0.5f), new Vector2(startWidth / 2, startWidth), SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return true;
        }
    }
}
