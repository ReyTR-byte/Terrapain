using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.DisplaySets.SlimeHeart
{
    public class LifeOverlay : ModResourceOverlay
    {
        // This field is used to cache vanilla assets used in the CompareAssets helper method further down in this file
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        // These fields are used to cache the result of ModContent.Request<Texture2D>()
        public static string[] hearts = ["SlimeHeart"];
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            if (hearts.Contains(Main.LocalPlayer.Custom().CurentHeart))
            {
                string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

                bool drawingBarsPanels = CompareAssets(asset, barsFolder + "HP_Panel_Right");

                // NOTE: CompareAssets is defined below this method's body
                if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
                {
                    // Draw over the Classic hearts
                    DrawClassicFancyOverlay(context);
                }
                else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                {
                    // Draw over the Fancy hearts
                    DrawClassicFancyOverlay(context);
                }
                else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                {
                    // Draw over the Bars life bars
                    DrawBarsOverlay(context);
                }
                else if (CompareAssets(asset, fancyFolder + "Heart_Left") || CompareAssets(asset, fancyFolder + "Heart_Middle") || CompareAssets(asset, fancyFolder + "Heart_Right") || CompareAssets(asset, fancyFolder + "Heart_Right_Fancy") || CompareAssets(asset, fancyFolder + "Heart_Single_Fancy"))
                {
                    // Draw over the Fancy heart panels
                    DrawFancyPanelOverlay(context);
                }
                else if (drawingBarsPanels)
                {
                    // Draw over the Bars middle life panels
                    DrawBarsPanelOverlay(context);
                }
            }
            if (Main.LocalPlayer.Custom().brokenHeartLevel > 0)
            {
                int AmountOfBrokenHearts = Main.LocalPlayer.Custom().brokenHeartLevel + Main.LocalPlayer.Custom().brokenHeartLevel - 1 + Math.Max(Main.LocalPlayer.Custom().brokenHeartLevel - 2, 0);
                if (context.resourceNumber >= (context.snapshot.AmountOfLifeHearts - AmountOfBrokenHearts))
                {
                    if (CompareAssets(asset, fancyFolder + "Heart_Left"))
                    {
                        DrawBrokenHeart(context, "Heart_Left");
                    }
                    else if (CompareAssets(asset, fancyFolder + "Heart_Right"))
                    {
                        DrawBrokenHeart(context, "Heart_Right");
                    }
                    else if (CompareAssets(asset, fancyFolder + "Heart_Middle"))
                    {
                        DrawBrokenHeart(context, "Heart_Middle");
                    }
                    else if (CompareAssets(asset, fancyFolder + "Heart_Right_Fancy"))
                    {
                        DrawBrokenHeart(context, "Heart_Right_Fancy");
                    }
                    else if (CompareAssets(asset, fancyFolder + "Heart_Single_Fancy"))
                    {
                        DrawBrokenHeart(context, "Heart_Single_Fancy");
                    }
                }
            }
        }

        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }

        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Classic / Fancy hearts
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla hearts, just replace the texture and have the context draw the new texture
            context.texture = ModContent.Request<Texture2D>($"Terrapain/Common/UI/Assets/DisplaySets/{Main.LocalPlayer.Custom().CurentHeart}/Heart");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This example just showcases changing the "inner" part of the heart panels to more closely resemble the example life fruit.
        private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Fancy heart panels
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";

            // The original position refers to the entire panel slice.
            // However, since this overlay only modifies the "inner" portion of the slice (aka the part behind the heart),
            // the position should be modified to compensate for the sprite size difference
            Vector2 positionOffset;

            if (context.resourceNumber == context.snapshot.AmountOfLifeHearts - 1)
            {
                // Final panel to draw has a special "Fancy" variant.  Determine whether it has panels to the left of it
                if (CompareAssets(context.texture, fancyFolder + "Heart_Single_Fancy"))
                {
                    // First and only panel in this panel's row
                    positionOffset = new Vector2(8, 8);
                }
                else
                {
                    // Other panels existed in this panel's row
                    // Vanilla texture is "Heart_Right_Fancy"
                    positionOffset = new Vector2(8, 8);
                }
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Left"))
            {
                // First panel in this row
                positionOffset = new Vector2(4, 4);
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Middle"))
            {
                // Any panel that has a panel to its left AND right
                positionOffset = new Vector2(0, 4);
            }
            else
            {
                // Final panel in the first row
                // Vanilla texture is "Heart_Right"
                positionOffset = new Vector2(0, 4);
            }

            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla hearts, just replace the texture and have the context draw the new texture
            context.texture = ModContent.Request<Texture2D>($"Terrapain/Common/UI/Assets/DisplaySets/{Main.LocalPlayer.Custom().CurentHeart}/Heart_Fill");
            // Due to the replacement texture and the vanilla texture having different dimensions, the source needs to also be modified
            context.source = context.texture.Frame();
            context.position += positionOffset;
            context.Draw();
        }

        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Bars life bars
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla bars, just replace the texture and have the context draw the new texture
            context.texture = ModContent.Request<Texture2D>($"Terrapain/Common/UI/Assets/DisplaySets/{Main.LocalPlayer.Custom().CurentHeart}/HP_Fill");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This example just showcases changing the "inner" part of the bar panels to more closely resemble the example life fruit.
        private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context)
        {
            context.texture = ModContent.Request<Texture2D>($"Terrapain/Common/UI/Assets/DisplaySets/{Main.LocalPlayer.Custom().CurentHeart}/HP_Panel_Right");
            context.Draw();
        }
        private void DrawBrokenHeart(ResourceOverlayDrawContext context, string Heart)
        {
            context.texture = ModContent.Request<Texture2D>($"Terrapain/Common/UI/Assets/DisplaySets/BrokenHeart/{Heart}");
            context.Draw();
        }
    }
}
