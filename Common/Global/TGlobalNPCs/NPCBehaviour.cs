using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terrapain.Content;
using Terraria.Utilities;
using Terrapain.Common.System;

namespace Terrapain.Common.Global.TGlobalNPCs
{
    public abstract class NPCBehaviour : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public TGlobalNPC t;
        public UnifiedRandom random = new UnifiedRandom();
        public virtual int type => 0;
        public virtual bool condition => !WorldDifficultySystem.clasic;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.type == type;
        }
        public override GlobalNPC NewInstance(NPC target)
        {
            if (condition)
                return base.NewInstance(target);
            return null;
        }
        public virtual void ModSetDefaults(NPC entity) { }
        public override void SetDefaults(NPC entity)
        {
            t = entity.GetT();
            t.NPCBehaviour = this;
            ModSetDefaults(entity);
        }
        public virtual void OnFirstTick(NPC npc) { }
        public virtual bool ModPreAI(NPC npc) { return true; }
        public virtual void ModPostAI(NPC npc) { }
        public virtual bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
            return true; 
        }
        public virtual void ModPostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture) { }
        public virtual void PreDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) { }
        public virtual void PostDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) { }
    }
}
