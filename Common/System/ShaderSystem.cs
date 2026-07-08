using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.DrawTasks;
using Terrapain.Common.Global;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using static Terrapain.Terrapain;

namespace Terrapain.Common.System
{
    public class ShaderSystem : ModSystem
    {
        public static ManagedRenderTarget target;
        public static bool drawScorspiderBorders;
        public static float BottomOfScorspiderWalls;
        public static int ScorspiderTimer;
        public static bool drawScorspiderAura;
        public static int ScorspiderAuraTimer;
        public static Vector2 ScorspiderPosition;
        public static float AuraRadius;
        public override void PostDrawTiles()
        {
            if (drawScorspiderAura)
            {
                drawScorspiderAura = false;
                if (ClientConfig.Instance.UseShaders)
                {
                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.ScorspiderAuraShader");
                    Shade.TrySetParameter("radius", AuraRadius);
                    Shade.TrySetParameter("Scorspider", ScorspiderPosition);
                    Shade.TrySetParameter("playerPos", Main.player[Main.myPlayer].Center);
                    Shade.TrySetParameter("screenPosition", Main.screenPosition);
                    Shade.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());

                    TGlobalNPC.PostDrawNPCsDrawTasks.Add(new ShaderFullscreenDrawTask(Shade));
                }
                else
                {
                    Vector2 center = ScorspiderPosition;
                    Vector2 screenSize = Main.ScreenSize.ToVector2();
                    Color drawerColor = new Color(0.5f, 0f, 0f, 0.5f);
                    Vector2 screenPosition = Main.screenPosition;
                    float radius = AuraRadius;
                    Vector2 screenPos = Main.screenPosition;

                    TGlobalNPC.PostDrawNPCsDrawTasks.Add(new DefaultDrawTask(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, 0, Vector2.Zero, radius / 2000, SpriteEffects.None));
                    TGlobalNPC.PostDrawNPCsDrawTasks.Add(new DefaultDrawTask(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2, Vector2.Zero, radius / 2000, SpriteEffects.None));
                    TGlobalNPC.PostDrawNPCsDrawTasks.Add(new DefaultDrawTask(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI, Vector2.Zero, radius / 2000, SpriteEffects.None));
                    TGlobalNPC.PostDrawNPCsDrawTasks.Add(new DefaultDrawTask(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2 * 3, Vector2.Zero, radius / 2000, SpriteEffects.None));

                    if ((int)(center.Y - screenPos.Y - radius) > 0)
                        TGlobalNPC.PostDrawNPCsDrawTasks.Add(new RectangleDrawTask(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, 0, (int)screenSize.X, (int)(center.Y - screenPos.Y - radius)), drawerColor));
                    if ((int)(screenSize.Y - (center.Y + radius - screenPos.Y)) > 0)
                        TGlobalNPC.PostDrawNPCsDrawTasks.Add(new RectangleDrawTask(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y + radius - screenPos.Y), (int)screenSize.X, (int)(screenSize.Y - (center.Y + radius - screenPos.Y))), drawerColor));
                    if ((int)(center.X - radius - screenPosition.X) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
                        TGlobalNPC.PostDrawNPCsDrawTasks.Add(new RectangleDrawTask(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y - screenPos.Y - radius), (int)(center.X - radius - screenPosition.X) + 1, (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor));
                    if ((int)(screenSize.X - (center.X + radius - screenPos.X)) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
                        TGlobalNPC.PostDrawNPCsDrawTasks.Add(new RectangleDrawTask(ExtraTextureRegistry.WhitePixel.Value, new Rectangle((int)(center.X + radius - screenPos.X), (int)(center.Y - screenPos.Y - radius), (int)(screenSize.X - (center.X + radius - screenPos.X)), (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor));
                }
            }
        }
    }
}