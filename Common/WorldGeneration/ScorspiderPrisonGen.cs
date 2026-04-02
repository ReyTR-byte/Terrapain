using Microsoft.Xna.Framework;
using Terrapain.Content.Items.Summons;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Terrapain.Common.WorldGeneration
{
    public class ScorspiderPrisonGen : ModSystem
    {
        bool summonInChest;
        public static Mod _mod;
        public override void OnModLoad()
        {
            _mod = Mod;
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Add(new ScorspiderPrisonGenPass("Scorspider Prison", 200));
        }
        public override void LoadWorldData(TagCompound tag)
        {
            ScorspiderBody.PrisonPosition = tag.Get<Vector2>("ScorspiderBody.PrisonPosition");
            ScorspiderBody.prisonHeight = (int)tag["ScorspiderBody.prisonHeight"];
            ScorspiderBody.prisonWidth = (int)tag["ScorspiderBody.prisonWidth"];
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag["ScorspiderBody.PrisonPosition"] = ScorspiderBody.PrisonPosition;
            tag["ScorspiderBody.prisonHeight"] = ScorspiderBody.prisonHeight;
            tag["ScorspiderBody.prisonWidth"] = ScorspiderBody.prisonWidth;
        }
    }
}