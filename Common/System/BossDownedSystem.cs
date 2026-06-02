using Terrapain.Content;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Terrapain.Common.System
{
    public class BossDownedSystem : ModSystem
    {
        public static bool[] BossStory = new bool[3];
        public static int[] StoryLines = { 0, 14, 24 };
        public static int[][] DealayBetweenLines = {
            [0],
            [60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60],
            [60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60]
        };
        public static int[] bossBagsTorture = new int[3];
        public static int[] bossBagsSuicide = new int[3];
        public static bool tellingStory;
        public static int timer;
        public static int line;

        public static bool ScorspiderDowned;
        public static void BossDowned(int BossID)
        {
            BossStory[BossID] = true;
            if (WorldDifficultySystem.torture)
            {
                bossBagsTorture[BossID] += 1;
            }
            if (WorldDifficultySystem.suicide)
            {
                bossBagsSuicide[BossID] += 1;
            }
            switch (BossID)
            {
                case 0:
                    if (!NPC.downedSlimeKing)
                    {
                        tellingStory = true;
                        timer = DealayBetweenLines[0][0];
                    }
                    break;
                case 1:
                    if (!NPC.downedBoss1)
                    {
                        tellingStory = true;
                        timer = DealayBetweenLines[1][0];
                    }
                    break;
                case 2:
                    if (!ScorspiderDowned)
                    {   
                        tellingStory = true;
                        timer = DealayBetweenLines[2][0];
                        ScorspiderDowned = true;
                    }
                    break;
            }
        }
        public override void PreUpdateWorld()
        {
            if (tellingStory)
            {
                for (int i = 0; i < BossStory.Length; i++)
                {
                    if (BossStory[i] && timer <= 0)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey($"Mods.Terrapain.BossStory.{i}_{line}"), Microsoft.Xna.Framework.Color.Purple);
                        line += 1;
                        if (line == StoryLines[i])
                        {
                            line = 0;
                            BossStory[i] = false;
                            tellingStory = false;
                            for (int j = 0; j < BossStory.Length; j++)
                            {
                                if (BossStory[j])
                                {
                                    tellingStory = true;
                                    break;
                                }
                            }
                        }
                        timer = DealayBetweenLines[i][line];
                    }
                }
                timer--;
            }
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag["ScorspiderDowned"] = ScorspiderDowned;
            tag["BossBagsTorture"] = bossBagsTorture;
            tag["BossBagsSuicide"] = bossBagsSuicide;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            ScorspiderDowned = tag.Get<bool>("ScorspiderDowned");
            bossBagsTorture = tag.Get<int[]>("BossBagsTorture");
            if (bossBagsTorture == null)
            {
                bossBagsTorture = new int[3];
            }
            else
            {   
                int[] _bossBagsTorture = bossBagsTorture;        
                bossBagsTorture = new int[3];
                for (int i = 0; i < _bossBagsTorture.Length; i++)
                {
                    bossBagsTorture[i] = _bossBagsTorture[i];
                }
            }
            bossBagsSuicide = tag.Get<int[]>("BossBagsSuicide");
            if (bossBagsSuicide == null)
            {
                bossBagsSuicide = new int[3];
            }
            else
            {   
                int[] _bossBagsSuicide = bossBagsSuicide;        
                bossBagsSuicide = new int[3];
                for (int i = 0; i < _bossBagsSuicide.Length; i++)
                {
                    bossBagsSuicide[i] = _bossBagsSuicide[i];
                }
            }
        }
    }
}