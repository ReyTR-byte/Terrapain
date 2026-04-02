using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terraria;
using Terraria.Utilities;

namespace Terrapain.Content.Auras
{
    public abstract class Aura
    {
        public Player owner = null;
        public bool isPlayerOwner => owner != null;
        public NPC npcOwner = null;
        public bool isNPCOwner => npcOwner != null;
        public Vector2 Center;
        public float Radius;
        public float InternalRadius;
        public int dust;
        public int dustCountMax;
        public int dustCountMin;
        public float dustChanse;
        public Color AuraColor;
        public Color internalColor;
        public bool checkPlayer = true;
        public bool checkNPC = true;
        public static UnifiedRandom random = new UnifiedRandom();
        public int timeLeft = 1;
        public virtual void OnNPCInAura(NPC npc) { }
        public virtual void OnPlayerInAura(Player player) { }
        public virtual void PostUpdate() { }
        public virtual void Update() 
        {
            float ran = random.NextFloat();
            if (ran < dustChanse)
            {
                int num = random.Next(dustCountMin, dustCountMax + 1);
                for (int i = 0; i < num; i++)
                {
                    float rot = random.NextFloat(MathF.PI * 2);
                    Vector2 pos = Functions.UnitVectorFromRotation(rot) * Radius;
                    pos += Center;
                    Dust.NewDust(pos, 0, 0, dust);
                }
            }
            if (checkNPC)
            {
                foreach (var npc in Main.npc)
                {
                    if (npc.active && Functions.CircleColision(Center, Radius, npc))
                    {
                        OnNPCInAura(npc);
                    }
                }
            }
            if (checkPlayer)
            {
                foreach (var npc in Main.player)
                {
                    if (npc.active && !npc.dead && Functions.CircleColision(Center, Radius, npc))
                    {
                        OnPlayerInAura(npc);
                    }
                }
            }
            PostUpdate();
            timeLeft--;
        }
        public virtual void PostDraw(SpriteBatch sprite) { }
        public virtual void Draw(SpriteBatch sprite)
        {
            if (Radius != 0 && InternalRadius != Radius)
            {
                ManagedShader Shade = ShaderManager.GetShader("Terrapain.AuraShader");
                Shade.TrySetParameter("internalRadius", (InternalRadius / Radius) * 0.5f);
                Shade.TrySetParameter("internalColor", internalColor.ToVector4());
                sprite.End();
                sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
                sprite.Draw(texture, Center - Main.screenPosition, null, AuraColor, 0, Vector2.One * 0.5f, Radius * 2, SpriteEffects.None, 0);
                sprite.End();
                sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            PostDraw(sprite);
        }
    }
}
