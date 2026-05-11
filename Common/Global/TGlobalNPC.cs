using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.DrawTasks;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Content;
using Terrapain.Content.Items.DropRulls;
using Terrapain.Content.NPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.NPCs.Bosses.VanillaBosses.EyeofCthulhu;
using static Terrapain.Content.Functions;
using Terraria.Utilities;

namespace Terrapain.Common.Global
{
    public class TGlobalNPC : GlobalNPC
    {
        public bool fulllight;
        public Vector2 drawCenter;
        public Vector2 drawOffcet;
        public int textureDirection = 1;
        public bool useModDrawingInPreDraw;
        public bool useModDrawingInPostDraw;
        public bool useVanillaDrawing = true;
        public List<int> MyGroups = new List<int>();
        public int savedTarget = -1;
        public Terraria.Player SavedTarget => Main.player[savedTarget];
        public int npcid;
        public Terraria.Player Target => Main.npc[npcid].target >= 0? Main.player[Main.npc[npcid].target] : null;
        public bool hooked;
        public bool canBeHooked = true;
        public bool FirstTick;
        public Projectile hookProjectile;
        public Terraria.Player hookedBy => hookProjectile.owner == -1? null : Main.player[hookProjectile.owner];
        public bool afterimage;
        public int afterimagesCount;
        public Vector2[] oldPositions = new Vector2[60];
        public float[] oldRotation = new float[60];
        public Color[] oldColor = new Color[60];
        public Rectangle[] oldFrame = new Rectangle[60];
        public int[] oldDirections = new int[60];
        public NPCBehaviour NPCBehaviour;
        public static UnifiedRandom random = new UnifiedRandom();

        public float oldDamageMultiplier;
        public int oldIgnoreDefence;
        public int oldBonusDefence;
        public float oldTakenDamageMultiplier;
        public int? oldDefender;
        public float oldDefenderTakesDamage;
        public bool oldFallThroughtPlatforms;

        public float damageMultiplier;
        public int ignoreDefence;
        public int bonusDefence;
        public float takenDamageMultiplier;
        public int? defender;
        public float defenderTakesDamage;
        public bool fallThroughtPlatforms;
        public bool canselDeathHitEffect;

        
        public static List<DrawTask> PreDrawNPCsDrawTasks = new List<DrawTask>();
        public static List<DrawTask> PostDrawNPCsDrawTasks = new List<DrawTask>();

        public override void Load()
        {
            On_NPC.VanillaHitEffect += On_NPC_VanillaHitEffect;
        }
        public override void Unload()
        {
            On_NPC.VanillaHitEffect -= On_NPC_VanillaHitEffect;
        }

        private void On_NPC_VanillaHitEffect(On_NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg, bool instantKill)
        {
            int realLife = self.life;
            if (self.GetT().canselDeathHitEffect)
            {
                self.life = Math.Max(self.life, 1);
            }
            orig(self, hitDirection, dmg, instantKill);
            self.life = realLife;
        }
        public override void ResetEffects(NPC npc)
        {
            oldDamageMultiplier = damageMultiplier;
            oldIgnoreDefence = ignoreDefence;
            oldBonusDefence = bonusDefence;
            oldTakenDamageMultiplier = takenDamageMultiplier;
            oldDefender = defender;
            oldDefenderTakesDamage = defenderTakesDamage;
            oldFallThroughtPlatforms = fallThroughtPlatforms;

            damageMultiplier = 1;
            takenDamageMultiplier = 1;
            ignoreDefence = 0;
            defender = null;
            defenderTakesDamage = 0;
            bonusDefence = 0;
            fallThroughtPlatforms = false;
        }
        public override bool? CanFallThroughPlatforms(NPC npc)
        {
            if (oldFallThroughtPlatforms)
            {
                return true;
            }
            if (hooked && npc.knockBackResist > 0)
            {
                return hookedBy.Bottom.Y > npc.Bottom.Y + 8;
            }
            return null;
        }
        public override void SetDefaults(NPC entity)
        {
            if (drawCenter == Vector2.Zero) 
                drawCenter = entity.Hitbox.Size() * 0.5f;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            npcid = npc.whoAmI;
            savedTarget = -1;
            FirstTick = true;
        }
        public override void ModifyHitPlayer(NPC npc, Terraria.Player target, ref Terraria.Player.HurtModifiers modifiers)
        {
            modifiers.FinalDamage *= oldDamageMultiplier;
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += oldIgnoreDefence;
            modifiers.Defense.Flat += oldBonusDefence;
            modifiers.FinalDamage *= oldTakenDamageMultiplier; 
            if (oldDefender.HasValue)
            {
                modifiers.FinalDamage *= 1 - oldDefenderTakesDamage;
            }
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (oldDefender.HasValue)
            {
                NPC.HitInfo newHit = hit;
                newHit.Damage = (int)(hit.SourceDamage * oldDefenderTakesDamage);
                Main.npc[oldDefender.Value].StrikeNPC(newHit);
                oldDefender = null;
            }
        }
        public override bool? CanBeHitByItem(NPC npc, Terraria.Player player, Item item)
        {
            //if (Terrapain.vanillaHit && item.useStyle == TGlobalItem.MassiveSwing)
            //{
            //    return false;
            //}
            return base.CanBeHitByItem(npc, player, item);
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    foreach (var loot in npcLoot.Get(false))
                    {
                        if (loot is ItemDropWithConditionRule)
                        {
                            if (((ItemDropWithConditionRule)loot).itemId == ItemID.EyeofCthulhuTrophy)
                            {
                                npcLoot.Remove(loot);
                                break;
                            }
                        }
                    }
                    LeadingConditionRule suicide = new(new SuicideDropRule());
                    suicide.OnSuccess(new DropOneByOne(ItemID.EyeofCthulhuTrophy, Terrapain.SuicideTrophyDropParameters));
                    npcLoot.Add(suicide);

                    LeadingConditionRule notSuicide = new(new NotSuicideDropRule());
                    notSuicide.OnSuccess(new DropOneByOne(ItemID.EyeofCthulhuTrophy, Terrapain.NormalTrophyDropParameters));
                    npcLoot.Add(notSuicide);

                    LeadingConditionRule Torture = new(new TortureDropRule());
                    Torture.OnSuccess(new DropOneByOne(4924 /*Eye of Cthulhu relic*/, Terrapain.SuicideTrophyDropParameters));
                    npcLoot.Add(Torture);
                    break;
            }
        }
        public int afterimageTimer;
        public void PreDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            NPCBehaviour?.PreDrawNPCs(npc, spriteBatch, screenPos);
        }
        //public virtual bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture) => true;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color col = npc.GetAlpha(Color.White);
            Color col2 = npc.GetColor(Color.White);
            if (npc.type == NPCID.YellowSlime)
            {

            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture;
            if (npc.type < NPCID.Count)
            {
                texture = TextureAssets.Npc[npc.type].Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(npc.type).Texture).Value;
            }

            if (afterimage || afterimageTimer > 0)
            {
                if (afterimageTimer < afterimagesCount && afterimage && !Main.gamePaused && !Main.gameInactive)
                {
                    afterimageTimer++;
                }
                for (int i = 0; i < (afterimageTimer > afterimagesCount? afterimagesCount : afterimageTimer); i++)
                {
                    Color color = oldColor[i];
                    if (color == Color.Transparent)
                    {
                        color = drawColor;
                    }
                    color.A = (byte)(color.A * (afterimagesCount - i) / (float)afterimagesCount * 0.5f);
                    Vector2 DrawCenter = drawCenter;
                    if (oldDirections[i] == -1)
                    {
                        DrawCenter.X = oldFrame[i].Width - DrawCenter.X;
                    }
                    Vector2 textureCenter = npc.frame.Size() * 0.5f;
                    Main.EntitySpriteDraw(texture, oldPositions[i] + npc.Hitbox.Size() / 2 - Main.screenPosition + drawOffcet, oldFrame[i], color, oldRotation[i], DrawCenter, 1, (oldDirections[i] * textureDirection) == 1? SpriteEffects.None : SpriteEffects.FlipHorizontally);  
                }
            }
            if (!afterimage && afterimageTimer > 0 && !Main.gamePaused)
            {
                afterimageTimer--;
            }
            if (ClientConfig.Instance.drawHitbox)
            {
                if (npc.type != NPCID.KingSlime)
                {
                    Rectangle rectangle = npc.Hitbox;
                    rectangle.X -= (int)screenPos.X;
                    rectangle.Y -= (int)screenPos.Y;
                    spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rectangle, ClientConfig.Instance.hitboxColor);
                }
            }

            bool? draw = NPCBehaviour?.ModPreDraw(npc, spriteBatch, screenPos, drawColor, texture);
            if (useModDrawingInPreDraw)
            {
                Color color = Lighting.GetColor(npc.Center.ToTileCoordinates());
                if (fulllight)
                {
                    color = Color.White;
                }
                color.A = (byte)(255 - npc.alpha);
                Vector2 textureCenter = npc.frame.Size() * 0.5f;
                Main.EntitySpriteDraw(texture, npc.Center - Main.screenPosition + drawOffcet, npc.frame, color, npc.rotation, drawCenter, 1, npc.spriteDirection * textureDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (draw == null)
                draw = true;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return draw.Value? useVanillaDrawing : false;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture;
            if (npc.type < NPCID.Count)
            {
                texture = TextureAssets.Npc[npc.type].Value;
            }
            else
            {
                texture = ModContent.Request<Texture2D>(ModContent.GetModNPC(npc.type).Texture).Value;
            }
            if (useModDrawingInPostDraw)
            {

                Color color = Lighting.GetColor(npc.Center.ToTileCoordinates());
                if (fulllight)
                {
                    color = Color.White;
                }
                color.A = (byte)(255 - npc.alpha);
                Vector2 textureCenter = npc.frame.Size() * 0.5f;
                Main.EntitySpriteDraw(texture, npc.Center - Main.screenPosition + drawOffcet, npc.frame, color, npc.rotation, drawCenter, 1, npc.spriteDirection * textureDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            NPCBehaviour?.ModPostDraw(npc, spriteBatch, screenPos, drawColor, texture);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public void PostDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            NPCBehaviour?.PostDrawNPCs(npc, spriteBatch, screenPos);
        }
        int timer;
        public override bool PreAI(NPC npc)
        {
            if (FirstTick)
            {
                NPCBehaviour?.OnFirstTick(npc);
                FirstTick = false;
            }
            return NPCBehaviour?.ModPreAI(npc)?? true;
        }
        public override void PostAI(NPC npc)
        {
            NPCBehaviour?.ModPostAI(npc);

            for (int i = oldPositions.Length - 1; i > 0; i--)
            {
                oldPositions[i] = oldPositions[i - 1];
            }
            for (int i = oldRotation.Length - 1; i > 0; i--)
            {
                oldRotation[i] = oldRotation[i - 1];
            }
            for (int i = oldColor.Length - 1; i > 0; i--)
            {
                oldColor[i] = oldColor[i - 1];
            }
            for (int i = oldFrame.Length - 1; i > 0; i--)
            {
                oldFrame[i] = oldFrame[i - 1];
            }
            for (int i = oldDirections.Length - 1; i > 0; i--)
            {
                oldDirections[i] = oldDirections[i - 1];
            }

            oldPositions[0] = npc.position;
            oldRotation[0] = npc.rotation;
            oldColor[0] = npc.color;
            oldFrame[0] = npc.frame;
            oldDirections[0] = npc.spriteDirection;
        }
        
        public override bool InstancePerEntity => true;
    }
    public class TGlobalNPCSystem : ModSystem
    {
        public override void Load()
        {
            On_Main.DoDraw_WallsTilesNPCs += On_Main_DoDraw_WallsTilesNPCs;
            On_Main.DrawItems += On_Main_DrawItems;
            On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
        }
        public override void Unload()
        {
            On_Main.DoDraw_WallsTilesNPCs -= On_Main_DoDraw_WallsTilesNPCs;
            On_Main.DrawItems -= On_Main_DrawItems;
            On_Main.DrawInfernoRings -= On_Main_DrawInfernoRings;
        }
        private void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rekt, null, Terrapain.screenColor, 0f, ExtraTextureRegistry.WhitePixel.Value.Size() * 0.5f, 0, 1f);
        }
        private void On_Main_DoDraw_WallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
            foreach (var npc in Main.npc)
            {
                if (npc.active)
                {
                    npc.GetT().PreDrawNPCs(npc, Main.spriteBatch, Main.screenPosition);
                }
            }
            while (TGlobalNPC.PreDrawNPCsDrawTasks.Count > 0)
            {
                TGlobalNPC.PreDrawNPCsDrawTasks[0].Draw(Main.spriteBatch);
                TGlobalNPC.PreDrawNPCsDrawTasks.RemoveAt(0);
            }
            orig(self);
        }
        private void On_Main_DrawItems(On_Main.orig_DrawItems orig, Main self)
        {
            foreach (var group in Terrapain.group)
            {
                if (group != null)
                {
                    group.PostDrawNPCs(Main.spriteBatch, Main.screenPosition);
                }
            }
            foreach (var npc in Main.npc)
            {
                if (npc.active)
                {
                    npc.GetT().PostDrawNPCs(npc, Main.spriteBatch, Main.screenPosition);
                }
            }
            while (TGlobalNPC.PostDrawNPCsDrawTasks.Count > 0)
            {
                TGlobalNPC.PostDrawNPCsDrawTasks[0].Draw(Main.spriteBatch);
                TGlobalNPC.PostDrawNPCsDrawTasks.RemoveAt(0);
            }
            orig(self);
        }
    }
}