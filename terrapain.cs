using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terrapain.Content.Items.ItemDropRules;
using System.Text.RegularExpressions;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.NPCs.Bosses.Scorspider;

namespace Terrapain
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Terrapain : Mod
    {
        public static Color screenColor;
        public static bool vanillaHit = true;
        public struct LightningDrawInfo
        {
            public List<LightningPartInfo> lightningPartInfos;
            public Color color;
            public float width;
            public Vector2 start;
            public Vector2 end;
            public Vector2 dist => end - start;
        }
        public struct LightningPartInfo
        {
            public Vector2 start;
            public Vector2 end;
            public float startWidth;
            public float endWidth;
        }
        public static List<TitleLinkButton> terrapainTitleLinks = new List<TitleLinkButton>();
        public static DropOneByOne.Parameters SuicideTrophyDropParameters = new DropOneByOne.Parameters();
        public static DropOneByOne.Parameters NormalTrophyDropParameters = new DropOneByOne.Parameters();
        public static TerrapainDropRull.Parameters DefaultIngredientsDropParameters = new TerrapainDropRull.Parameters();
        public static TerrapainDropRull.Parameters DefaultRaretiesDropParameters = new TerrapainDropRull.Parameters();
        public static Content.Groups.Group[] group = new Content.Groups.Group[25];

        public override void Load()
        {
            SuicideTrophyDropParameters.MaximumItemDropsCount = 1;
            SuicideTrophyDropParameters.MinimumItemDropsCount = 1;
            SuicideTrophyDropParameters.ChanceNumerator = 1;
            SuicideTrophyDropParameters.ChanceDenominator = 1;
            SuicideTrophyDropParameters.MinimumStackPerChunkBase = 1;
            SuicideTrophyDropParameters.MaximumStackPerChunkBase = 1;
            SuicideTrophyDropParameters.BonusMinDropsPerChunkPerPlayer = 0;
            SuicideTrophyDropParameters.BonusMaxDropsPerChunkPerPlayer = 0;

            NormalTrophyDropParameters = SuicideTrophyDropParameters;
            NormalTrophyDropParameters.ChanceDenominator = 10;

            DefaultIngredientsDropParameters.MaximumItemDropsCount = 1;
            DefaultIngredientsDropParameters.MinimumItemDropsCount = 1;
            DefaultIngredientsDropParameters.ChanceNumerator = 1;
            DefaultIngredientsDropParameters.ChanceDenominator = 1;
            DefaultIngredientsDropParameters.MinimumStackPerChunkBase = 1;
            DefaultIngredientsDropParameters.MaximumStackPerChunkBase = 1;
            DefaultIngredientsDropParameters.BonusMinDropsPerChunkPerPlayer = 0;
            DefaultIngredientsDropParameters.BonusMaxDropsPerChunkPerPlayer = 0;
            DefaultIngredientsDropParameters.GaranteedInSuicide = true;
            DefaultIngredientsDropParameters.MultiplyByDifficulty = true;

            DefaultRaretiesDropParameters = DefaultIngredientsDropParameters;
            DefaultRaretiesDropParameters.MultiplyByDifficulty = false;
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.GoldBar), group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldOre)}", ItemID.GoldOre, ItemID.PlatinumOre);
            RecipeGroup.RegisterGroup(nameof(ItemID.GoldOre), group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IronOre)}", ItemID.IronOre, ItemID.LeadOre);
            RecipeGroup.RegisterGroup(nameof(ItemID.IronOre), group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.ShadowScale)}", ItemID.ShadowScale, ItemID.TissueSample);
            RecipeGroup.RegisterGroup(nameof(ItemID.ShadowScale), group);
            
            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.DemoniteBar)}", ItemID.DemoniteBar, ItemID.CrimtaneBar);
            RecipeGroup.RegisterGroup(nameof(ItemID.DemoniteBar), group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.TungstenBullet)}", ItemID.TungstenBullet, ItemID.SilverBullet);
            RecipeGroup.RegisterGroup(nameof(ItemID.TungstenBullet), group);

            group = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.ViciousPowder)}", ItemID.ViciousPowder, ItemID.VilePowder);
            RecipeGroup.RegisterGroup(nameof(ItemID.ViciousPowder), group);
        }
        
        public override void AddRecipes()
        {
            foreach (var r in Main.recipe)
            {
                if (r.createItem.type == ItemID.FlinxFurCoat)
                {
                    r.DisableRecipe();
                }
                else if(r.createItem.type == ItemID.SapphireStaff)
                {
                    r.AddIngredient<SuperDenseGel>(6);
                }
                else if (r.createItem.type == ItemID.RubyStaff)
                {
                    r.AddIngredient<Content.Items.Ingredients.ScorspiderShellShard>(4);
                    r.AddIngredient<Content.Items.Ingredients.ScorspiderCobweb>(4);
                }
                else if (r.createItem.type == ItemID.AmberStaff)
                {
                    r.AddIngredient(ItemID.BeeWax, 5);
                }
                else if (r.createItem.type == ItemID.DiamondStaff)
                {
                    r.AddIngredient(ItemID.Bone, 25);
                }
            }
			Recipe recipe = Recipe.Create(ItemID.FlinxFurCoat);
			recipe.AddIngredient(ItemID.FlinxFur, 4);
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 8);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
        }
        
        public static void DrawTitleLinks(Color menuColor, float upBump)
        {
            List<TitleLinkButton> titleLinks = terrapainTitleLinks;
            Vector2 anchorPosition = new Vector2(18f, (float)(Main.screenHeight - 85 - 22) - upBump);
            for (int i = 0; i < titleLinks.Count; i++)
            {          
                titleLinks[i].Draw(Main.spriteBatch, anchorPosition);
                anchorPosition.X += 30f;
            }
        }
    }
}
