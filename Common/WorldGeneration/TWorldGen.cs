using System.Configuration;
using Steamworks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Terrapain.Common.WorldGeneration
{
    public class TWorldGen : ModSystem
    {
        public static Mod _mod;
        public override void Load() 
        {
			WorldGen.DetourPass((PassLegacy)WorldGen.VanillaGenPasses["Shinies"], Detour_Shinies);
		}
        void Detour_Shinies(WorldGen.orig_GenPassDetour orig, object self, GenerationProgress progress, GameConfiguration configuration) 
        {
			orig(self, progress, configuration);

            int oreCount = Main.maxTilesX * Main.maxTilesY / 8000;
            GenerateOrePair(oreCount, WorldGen.SavedOreTiers.Copper, TileID.Copper, TileID.Tin);
            GenerateOrePair(oreCount, WorldGen.SavedOreTiers.Iron, TileID.Iron, TileID.Lead);
            GenerateOrePair(oreCount, WorldGen.SavedOreTiers.Silver, TileID.Silver, TileID.Tungsten);
            GenerateOrePair(oreCount, WorldGen.SavedOreTiers.Gold, TileID.Gold, TileID.Platinum);
		}

        static void GenerateOrePair(int oreCount, int selectedOre, ushort firstOre, ushort secondOre)
        {
            ushort missingOre = selectedOre == firstOre ? secondOre : firstOre;

            for (int index = 0; index < oreCount; index++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
                WorldGen.OreRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), missingOre);
            }
        }
        public override void OnModLoad()
        {
            _mod = Mod;
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Add(new ScorspiderPrisonGenPass("Scorspider Prison", 200));
            tasks.Add(new MiniBossesGenPass("Mini Bosses", 8998));
        }
        //public override void LoadWorldData(TagCompound tag)
        //{
        //    ScorspiderBody.PrisonPosition = tag.Get<Vector2>("ScorspiderBody.PrisonPosition");
        //    ScorspiderBody.prisonHeight = (int)tag["ScorspiderBody.prisonHeight"];
        //    ScorspiderBody.prisonWidth = (int)tag["ScorspiderBody.prisonWidth"];
        //}
        //public override void SaveWorldData(TagCompound tag)
        //{
        //    tag["ScorspiderBody.PrisonPosition"] = ScorspiderBody.PrisonPosition;
        //    tag["ScorspiderBody.prisonHeight"] = ScorspiderBody.prisonHeight;
        //    tag["ScorspiderBody.prisonWidth"] = ScorspiderBody.prisonWidth;
        //}
    }
}