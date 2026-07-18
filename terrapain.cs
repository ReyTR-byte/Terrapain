using Microsoft.Xna.Framework;
using System.Data;
using System.Text.RegularExpressions;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Items.ItemDropRules;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Collision;

namespace Terrapain
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Terrapain : Mod
    {
        public static bool ignoreTiles;
        public static bool ignorePlatforms;

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

            On_Collision.TileCollision += On_Collision_TileCollision;
            On_Collision.SlopeCollision += On_Collision_SlopeCollision;
            On_Collision.WaterCollision += On_Collision_WaterCollision;
            On_Collision.WetCollision += On_Collision_WetCollision;
            On_Collision.SolidCollision_Vector2_int_int += On_Collision_SolidCollision_Vector2_int_int;
            On_Collision.SolidCollision_Vector2_int_int_bool += On_Collision_SolidCollision_Vector2_int_int_bool;
            On_Collision.LavaCollision += On_Collision_LavaCollision;
            On_Collision.StickyTiles += On_Collision_StickyTiles;
            On_Collision.DrownCollision += On_Collision_DrownCollision;
            //On_Collision.AnyCollision += On_Collision_AnyCollision;
            //On_Collision.AdvancedTileCollision += On_Collision_AdvancedTileCollision;
            //On_Collision.AnyCollisionWithSpecificTiles += On_Collision_AnyCollisionWithSpecificTiles;
            //On_Collision.noSlopeCollision += On_Collision_noSlopeCollision;
            On_Collision.SolidTiles_int_int_int_int += On_Collision_SolidTiles_int_int_int_int;
            On_Collision.SolidTilesVersatile += On_Collision_SolidTilesVersatile;
            On_Collision.SwitchTiles_Entity_Vector2_int_int_Vector2_int += On_Collision_SwitchTiles_Entity_Vector2_int_int_Vector2_int;
            On_Collision.HurtTiles += On_Collision_HurtTiles;
            On_Collision.StepDown += On_Collision_StepDown;
            On_Collision.StepUp += On_Collision_StepUp;
        }
        public override void Unload()
        {
            On_Collision.TileCollision -= On_Collision_TileCollision;
            On_Collision.SlopeCollision -= On_Collision_SlopeCollision;
            On_Collision.WaterCollision -= On_Collision_WaterCollision;
            On_Collision.WetCollision -= On_Collision_WetCollision;
            On_Collision.SolidCollision_Vector2_int_int -= On_Collision_SolidCollision_Vector2_int_int;
            On_Collision.SolidCollision_Vector2_int_int_bool -= On_Collision_SolidCollision_Vector2_int_int_bool;
            On_Collision.LavaCollision -= On_Collision_LavaCollision;
            On_Collision.StickyTiles -= On_Collision_StickyTiles;
            On_Collision.DrownCollision -= On_Collision_DrownCollision;
            //On_Collision.AnyCollision -= On_Collision_AnyCollision;
            //On_Collision.AdvancedTileCollision -= On_Collision_AdvancedTileCollision;
            //On_Collision.AnyCollisionWithSpecificTiles -= On_Collision_AnyCollisionWithSpecificTiles;
            //On_Collision.noSlopeCollision -= On_Collision_noSlopeCollision;
            On_Collision.SolidTiles_int_int_int_int -= On_Collision_SolidTiles_int_int_int_int;
            On_Collision.SolidTilesVersatile -= On_Collision_SolidTilesVersatile;
            On_Collision.SwitchTiles_Entity_Vector2_int_int_Vector2_int -= On_Collision_SwitchTiles_Entity_Vector2_int_int_Vector2_int;
            On_Collision.HurtTiles -= On_Collision_HurtTiles;
            On_Collision.StepDown -= On_Collision_StepDown;
            On_Collision.StepUp -= On_Collision_StepUp;
        }

        private void On_Collision_StepUp(On_Collision.orig_StepUp orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool holdsMatching, int specialChecksMode)
        {
            if (ignoreTiles)
                return;
            orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, holdsMatching, specialChecksMode);
        }

        private void On_Collision_StepDown(On_Collision.orig_StepDown orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool waterWalk)
        {
            if (ignoreTiles)
                return;
            orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, waterWalk);
        }

        private HurtTile On_Collision_HurtTiles(On_Collision.orig_HurtTiles orig, Vector2 Position, int Width, int Height, Player player)
        {
            if (ignoreTiles)
            {
                var result = default(HurtTile);
                result.type = -1;
                return result;
            }
            return orig(Position, Width, Height, player);
        }

        private bool On_Collision_SwitchTiles_Entity_Vector2_int_int_Vector2_int(On_Collision.orig_SwitchTiles_Entity_Vector2_int_int_Vector2_int orig, Entity entity, Vector2 Position, int Width, int Height, Vector2 oldPosition, int objType)
        {
            if (ignoreTiles)
                return false;
            return orig(entity, Position, Width, Height, oldPosition, objType);
        }

        private bool On_Collision_SolidTilesVersatile(On_Collision.orig_SolidTilesVersatile orig, int startX, int endX, int startY, int endY)
        {
            if (ignoreTiles)
                return false;
            return orig(startX, endX, startY, endY);
        }

        private bool On_Collision_SolidTiles_int_int_int_int(On_Collision.orig_SolidTiles_int_int_int_int orig, int startX, int endX, int startY, int endY)
        {
            if (ignoreTiles)
                return false;
            return orig(startX, endX, startY, endY);
        }

        private bool On_Collision_DrownCollision(On_Collision.orig_DrownCollision orig, Vector2 Position, int Width, int Height, float gravDir, bool includeSlopes)
        {
            if (ignoreTiles)
                return false;
            return orig(Position, Width, Height, gravDir, includeSlopes);
        }
        private Vector2 On_Collision_StickyTiles(On_Collision.orig_StickyTiles orig, Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            if (ignoreTiles)
                return new(-1, -1);
            return orig(Position, Velocity, Width, Height);
        }
        private bool On_Collision_LavaCollision(On_Collision.orig_LavaCollision orig, Vector2 Position, int Width, int Height)
        {
            if (ignoreTiles)
                return false;
            return orig(Position, Width, Height);
        }
        private bool On_Collision_SolidCollision_Vector2_int_int(On_Collision.orig_SolidCollision_Vector2_int_int orig, Vector2 Position, int Width, int Height)
        {
            if (ignoreTiles)
                return false;
            return orig(Position, Width, Height);
        }
        private bool On_Collision_SolidCollision_Vector2_int_int_bool(On_Collision.orig_SolidCollision_Vector2_int_int_bool orig, Vector2 Position, int Width, int Height, bool acceptTopSurfaces)
        {
            if (ignoreTiles)
                return false;
            return orig(Position, Width, Height, acceptTopSurfaces);
        }
        private bool On_Collision_WetCollision(On_Collision.orig_WetCollision orig, Vector2 Position, int Width, int Height)
        {
            if (ignoreTiles)
            {
                Collision.honey = false;
                Collision.shimmer = false;
                return false;
            }
            return orig(Position, Width, Height);
        }
        private Vector2 On_Collision_WaterCollision(On_Collision.orig_WaterCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, bool lavaWalk)
        {
            if (ignoreTiles)
            {
                Collision.up = false;
                Collision.down = false;
                return Velocity;
            }
            fallThrough |= ignorePlatforms;
            fall2 |= ignorePlatforms;
            return orig(Position, Velocity, Width, Height, fallThrough, fall2, lavaWalk);
        }
        private Vector4 On_Collision_SlopeCollision(On_Collision.orig_SlopeCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity, bool fall)
        {
            if (ignoreTiles)
            {
                Collision.sloping = false;
                Collision.stair = false;
                Collision.stairFall = false;
                return new(Position, Velocity.X, Velocity.Y);
            }
            fall |= ignorePlatforms;
            return orig(Position, Velocity, Width, Height, gravity, fall);
        }
        private Vector2 On_Collision_TileCollision(On_Collision.orig_TileCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
        {
            if (ignoreTiles)
            {
                Collision.up = false;
                Collision.down = false;
                return Velocity;
            }
            fallThrough |= ignorePlatforms;
            fall2 |= ignorePlatforms;
            return orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
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
