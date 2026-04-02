using Microsoft.Xna.Framework;
using StructureHelper.API;
using StructureHelper.Models;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Terrapain.Common.WorldGeneration
{
    public class ScorspiderPrisonGenPass : GenPass
    {
        public ScorspiderPrisonGenPass (string name, float weight) : base(name, weight) {}
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Spawning Scorspider prison";
            
            int attempts = 0;
            bool goodPlace = false;
            int x = 0;
            int y = 0;
            while(!goodPlace && attempts < 50)
            {
                x = WorldGen.genRand.Next(Main.maxTilesX / 2 - 500, Main.maxTilesX / 2 + 501);
                y = WorldGen.genRand.Next(Main.maxTilesY / 2 - 250, Main.maxTilesY / 2 + 251);
                for (int i = -12; i < 12; i ++)
                {
                    for (int j = -12; j < 12; j++)
                    {
                        if (Main.tile[x + i, y + j].TileType == TileID.Granite)
                        {
                            goodPlace = true;
                        }
                    }
                }
                attempts++;
            }
            Generator.GenerateStructure("Common/WorldGeneration/Structures/ScorspiderPrison.shstruct", new Point16(x, y), ScorspiderPrisonGen._mod);
            ScorspiderBody.PrisonPosition = new Vector2(x, y);
            StructureData sd = Generator.GetStructureData("Common/WorldGeneration/Structures/ScorspiderPrison.shstruct", ScorspiderPrisonGen._mod);
            ScorspiderBody.prisonWidth = sd.width;
            ScorspiderBody.prisonHeight = sd.height;
        }
    }
}