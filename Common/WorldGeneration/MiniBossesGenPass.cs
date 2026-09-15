using Terrapain.Common.System;
using Terrapain.Content;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Terrapain.Common.WorldGeneration
{
    public class MiniBossesGenPass : GenPass
    {
        public MiniBossesGenPass (string name, float weight) : base(name, weight) {}
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            SpawnHarpys();
        }
        void SpawnHarpys()
        {
            int Attempts = 200;
            //FastParallel.For(0, Attempts, delegate(int start, int end, object context)
            //{
                for (int i = 0; i < Attempts; i++)
                {
                    int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100) - 25;
                    int y = WorldGen.genRand.Next(80, Main.maxTilesY / 8) + 25;
                    Point spawn = new Point(x + 25, y - 25);
                    for (int X = -1; X < 2; X++)
                    {
                        for (int Y = -1; Y < 2; Y++)
                        {
                            Tile tile = Main.tile[spawn + new Point(X, Y)];
                            if (tile.IsSolid() || tile.WallType != WallID.None)
                            {
                                goto End;
                            }
                        }
                    }
                    bool canSpawn = false;
                    for (int X = x; X < x + 50; X++)
                    {
                        for (int Y = y; Y < y + 50; Y++)
                        {
                            if (Main.tile[X, Y].IsSolid() && (Main.tile[X, Y].TileType == TileID.Cloud || Main.tile[X, Y].TileType == TileID.RainCloud || Main.tile[X, Y].TileType == TileID.SolarBrick))
                            {
                                canSpawn = true;
                                break;
                            }
                        }
                        if (canSpawn)
                        {
                            break;
                        }
                    }
                    if (canSpawn)
                    {    
              //          lock (MiniBossSpawnSystem.miniBosses)
              //          {
                            MiniBossSpawnSystem.miniBosses.Add(new MiniBossSpawnSystem.MiniBossSpawnInfo() { type = NPCID.Harpy, power = WorldGen.genRand.NextFloat(2.2f, 2.5f), coordinates = spawn.ToWorldCoordinates() });
              //          }
                    }
                    End:;
                } 
            //});
            Console.WriteLine(MiniBossSpawnSystem.miniBosses.Count);
        }
    }
}