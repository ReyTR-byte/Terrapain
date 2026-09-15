using ILGPU;
using ILGPU.IR.Values;
using ILGPU.Runtime;
using ReLogic.Reflection;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Light;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Terrapain.Common.System
{
    public class MiniBossSpawnSystem : ModSystem
    {
        public class MiniBossSpawnInfo
        {
            public int type;
            public float power;
            public Vector2 coordinates;
            public object context;
            public int whoAmI;
            public bool defeated;
            public bool active => whoAmI == -1 || !Main.npc[whoAmI].active || Main.npc[whoAmI].type != type;
            public void SpawnNPC()
            {
                whoAmI = Functions.NewNPC(new MiniBossSpawnSource(this), coordinates, type);
            }
            public MiniBossSpawnInfo()
            {
                whoAmI = -1;
            }
            public MiniBossSpawnInfo(TagCompound tag)
            {
                type = tag.Get<int>("Type");
                power = tag.Get<float>("Power");
                coordinates = tag.Get<Vector2>("Coordinates");
                whoAmI = -1;
            }
        }
        public struct MiniBossSpawnSource : IEntitySource
        {
            public string Context => null;
            public MiniBossSpawnInfo spawnInfo;
            public MiniBossSpawnSource(MiniBossSpawnInfo info)
            {
                spawnInfo = info;
            }
        }
        public override void PreUpdateWorld()
        {
            for(int i = 0; i < miniBosses.Count; i++)
            {
                Point p = Main.LocalPlayer.Center.ToTileCoordinates();
                float k = Main.maxTilesY / (float)p.Y;
                MiniBossSpawnInfo miniBoss = miniBosses[i];
                if (miniBoss.defeated)
                {
                    miniBosses.RemoveAt(i);
                    i--;
                    continue;
                }
                if (miniBoss.active)
                {
                    Vector2 pos = miniBoss.coordinates;
                    Vector2 pos2 = Main.LocalPlayer.Center;
                    if ((MathF.Abs(pos.X - pos2.X) > 1500 && MathF.Abs(pos.X - pos2.X) < 2000) || (MathF.Abs(pos.Y - pos2.Y) > 1000 && MathF.Abs(pos.Y - pos2.Y) < 1500))
                    {
                        miniBoss.SpawnNPC();
                    }
                }
            }    
        }
        public override void OnWorldUnload()
        {
            miniBosses = [];
        }
        public static List<MiniBossSpawnInfo> miniBosses = [];
        public override void SaveWorldData(TagCompound tag)
        {
            List<TagCompound> savedMiniBosses = [];
            foreach(var mb in miniBosses)
            {
                savedMiniBosses.Add(new TagCompound
                {
                    ["Type"] = mb.type,
                    ["Power"] = mb.power,
                    ["Coordinates"] = mb.coordinates
                });
            }
            tag["MiniBosses"] = savedMiniBosses;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            miniBosses = [];
            var mb = tag.GetList<TagCompound>("MiniBosses");
            if (mb != null)
            {
                foreach (var _mb in mb)
                {
                    miniBosses.Add(new MiniBossSpawnInfo(_mb));
                }
            }
        }
    }
}