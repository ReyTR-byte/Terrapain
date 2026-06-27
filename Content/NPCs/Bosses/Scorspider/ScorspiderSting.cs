using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    public class ScorspiderSting : ModNPC
    {
        private int Body
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        private int Head
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
            {
                PortraitScale = 0.6f,
                PortraitPositionYOverride = 0f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;

            NPC.damage = 40;
            NPC.defense = 20;

            NPC.lifeMax = 8000;

            NPC.knockBackResist = 0f;

            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;

            NPC.noTileCollide = true;
            NPC.noGravity = true;

            NPC.aiStyle = -1;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath28;

            NPC.GetT().drawCenter = new Vector2(30, 32);
            NPC.GetT().useModDrawingInPreDraw = true;
            NPC.GetT().useVanillaDrawing = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.realLife = Body;
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        int timer = 5;
        public override void AI()
        {
            timer--;
            if (!Main.npc[Body].active || Main.npc[Body].type != ModContent.NPCType<ScorspiderBody>() && timer <= 0)
            {
                NPC.life = 0;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (ShaderSystem.drawScorspiderBorders || ShaderSystem.ScorspiderTimer < 20)
            {
                if (ClientConfig.Instance.UseShaders)
                {
                    var blackTile = ExtraTextureRegistry.BlackPixel;

                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.ScorspiderShader");
                    Shade.TrySetParameter("height", 49f * 1000f / 74f);
                    Shade.TrySetParameter("startPosY", ShaderSystem.BottomOfScorspiderWalls);
                    Shade.TrySetParameter("playerPos", Main.player[Main.myPlayer].Center);
                    Shade.TrySetParameter("screenPosition", Main.screenPosition);
                    Shade.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
                    Shade.TrySetParameter("timer", ShaderSystem.ScorspiderTimer);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                    spriteBatch.Draw(blackTile.Value, rekt, null, Color.Black, 0f, blackTile.Value.Size() * 0.5f, 0, 1f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                else
                {

                    Vector2 center = new Vector2(Main.cameraX, ShaderSystem.BottomOfScorspiderWalls + ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2);
                    Vector2 screenSize = Main.ScreenSize.ToVector2();
                    Color drawerColor = new Color(0.5f, 0f, 0f, 0.5f);
                    Vector2 screenPosition = Main.screenPosition;

                    if ((int)(screenPos.Y + screenSize.Y - center.Y) > 0)
                    {
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y - screenPos.Y), (int)screenSize.X, (int)(screenPos.Y + screenSize.Y - center.Y)), drawerColor);
                    }

                    center = new Vector2(Main.cameraX, ShaderSystem.BottomOfScorspiderWalls - 49f * 1000f / 74f - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2);

                    if ((int)(center.Y - screenPos.Y) > 0)
                    {
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, 0, (int)screenSize.X, (int)(center.Y - screenPos.Y)), drawerColor);
                    }
                }
            }

            if (ShaderSystem.drawScorspiderAura || ShaderSystem.ScorspiderAuraTimer < 20)
            {
                if (ClientConfig.Instance.UseShaders)
                {
                    var blackTile = ExtraTextureRegistry.BlackPixel;

                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.ScorspiderAuraShader");
                    Shade.TrySetParameter("radius", ShaderSystem.AuraRadius);
                    Shade.TrySetParameter("Scorspider", ShaderSystem.ScorspiderPosition);
                    Shade.TrySetParameter("playerPos", Main.player[Main.myPlayer].Center);
                    Shade.TrySetParameter("screenPosition", Main.screenPosition);
                    Shade.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
                    Shade.TrySetParameter("timer", ShaderSystem.ScorspiderAuraTimer);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                    spriteBatch.Draw(blackTile.Value, rekt, null, Color.Black, 0f, blackTile.Value.Size() * 0.5f, 0, 1f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                else
                {
                    Vector2 center = ShaderSystem.ScorspiderPosition;
                    Vector2 screenSize = Main.ScreenSize.ToVector2();
                    Color drawerColor = new Color(0.5f, 0f, 0f, 0.5f);
                    Vector2 screenPosition = Main.screenPosition;
                    float radius = ShaderSystem.AuraRadius;

                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, 0, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2 * 3, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);

                    if ((int)(center.Y - screenPos.Y - radius) > 0)
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, 0, (int)screenSize.X, (int)(center.Y - screenPos.Y - radius)), drawerColor);
                    if ((int)(screenSize.Y - (center.Y + radius - screenPos.Y)) > 0)
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y + radius - screenPos.Y), (int)screenSize.X, (int)(screenSize.Y - (center.Y + radius - screenPos.Y))), drawerColor);
                    if ((int)(center.X - radius - screenPosition.X) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y - screenPos.Y - radius), (int)(center.X - radius - screenPosition.X) + 1, (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor);
                    if ((int)(screenSize.X - (center.X + radius - screenPos.X)) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
                        spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle((int)(center.X + radius - screenPos.X), (int)(center.Y - screenPos.Y - radius), (int)(screenSize.X - (center.X + radius - screenPos.X)), (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor);
                }
            }
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 900);
        }
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderSting_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderSting_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
        }
    }
}
