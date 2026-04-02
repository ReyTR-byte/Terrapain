using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public class GridLaserServants : Group
    {
        public GridLaserServants()
        {
            NPCType = [ModContent.NPCType<NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu>()];
        }
        public Player target;
        public bool buildGrid;
        public List<Vector2> points;
        public List<float> rotatinos;
        int timer;
        public override void UpdateGroup()
        {
            CheckMembers();
            AfterCheckMembers();
            if (buildGrid)
            {
                Vector2 CommonCenter = Vector2.Zero;
                foreach(var mem in members)
                {
                    NPC Mem = Main.npc[mem];
                    CommonCenter += Mem.Center;
                }
                CommonCenter /= members.Count;
                float CommonRotation = 0;
                for (int i = 0; i < members.Count; i++)
                {
                    NPC Mem = Main.npc[members[i]];
                    CommonRotation = (Mem.Center - CommonCenter).ToRotation();
                    sort[i] = (Mem.Center - CommonCenter).ToRotation();
                }
                CommonRotation /= members.Count;
                for (int i = 0; i < members.Count; i++)
                {
                    sort[i] -= CommonRotation;
                    sort[i] = Functions.NormalizeRotation(sort[i], false);
                }
                Sort();
                for (int i = 0; i < members.Count; i++)
                {
                    Main.npc[members[i]].ai[0] = 0;
                    if (i >= points.Count)
                    Main.npc[members[i]].ai[0] = -1;
                }
                timer = 100;
                buildGrid = false;
            }
            if (timer > 40)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    NPC Mem = Main.npc[members[i]];
                    if (Mem.ai[0] != -1)
                    {
                        Mem.ai[0] = points[i].X + target.Center.X;
                        Mem.ai[1] = points[i].Y + target.Center.Y;
                        Mem.ai[2] = rotatinos[i];
                        Mem.ai[3] = timer;
                    }
                    else
                    {
                        Mem.ai[3] = 100;
                    }    
                }
            }
            else if (timer <= 20 && timer > 0 && timer % 5 == 0)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    NPC Mem = Main.npc[members[i]];
                    if (Mem.ai[0] != -1)
                    {
                        int proj = Projectile.NewProjectile(Mem.GetSource_FromThis(), Mem.Center, Mem.rotation.ToRotationVector2() * 5 * Mem.spriteDirection, ModContent.ProjectileType<DemonicEyeLazer>(), 20, 3, ai0: 2);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                    }
                }
            }
            timer--;
        }
        void AfterCheckMembers()
        {
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (((NPCs.Servants.EyeofCthulhu.LaserServantofCthulhu)member.ModNPC).AIStyle != 1)
                {
                    DelMember(i);
                    i--;
                }
            }
            if (members.Count == 0)
            {
                Terrapain.group[whoAmI] = null;
            }
        }
        public override void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            Color color = Color.Red;
            if (ClientConfig.Instance.UseShaders)
            {
                color *= (100 - timer) / 80f * 0.8f;
            }
            else
            {
                color.A = (byte)(color.A * ((100 - timer) / 80f) * 0.8f);
            }
            foreach (var mem in members)
            {
                NPC npc = Main.npc[mem];

                if (npc.ai[0] != -1)
                {
                    Texture2D tex = null;
                    if (ClientConfig.Instance.UseShaders)
                    {
                        ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
                        Shade.TrySetParameter("lenght", 900);
                        Shade.TrySetParameter("width", 8);
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    }
                    else
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                        tex = ExtraTextureRegistry.CubedGradient10Mirrored.Value;
                    }
                    spriteBatch.DrawLine(npc.Center, npc.Center + Vector2.UnitX.RotatedBy(npc.rotation + (npc.spriteDirection == 1 ? 0 : MathF.PI)) * 900, color, 8, tex);
                    if (ClientConfig.Instance.UseShaders)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
            }
        }
    }
}
