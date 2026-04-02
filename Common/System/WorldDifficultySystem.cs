using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terrapain.Content;

namespace Terrapain.Common.System
{
    public class WorldDifficultySystem : ModSystem
    {
        public static int TerrapainDifficulty;
        public static bool clasic => TerrapainDifficulty == 0;
        public static bool torture => TerrapainDifficulty == 1;
        public static bool suicide => TerrapainDifficulty == 2;

        public static void SetDifficulty(int Difficulty, Terraria.Player player)
        {
            if (!Functions.CheckBoss())
            {
                if (TerrapainDifficulty == Difficulty)
                {
                    TerrapainDifficulty = 0;
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.SetDifficultyToZero"), Color.White);
                }
                else
                {
                    TerrapainDifficulty = Difficulty;

                    if (TerrapainDifficulty == 1)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerChooseTorture"), Color.Red);
                    }

                    if (TerrapainDifficulty == 2)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.PlayerChooseSuicide"), Color.DarkRed);
                    }
                }
            }
            else
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.NetworkText.DifficultyChooseWhileBossAlive"), Color.Red);
            }
        } 
        public override void SaveWorldData(TagCompound tag)
        {
            tag["TerrapainDifficulty"] = TerrapainDifficulty;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            try
            {
                TerrapainDifficulty = (int)tag["TerrapainDifficulty"];
            }
            catch
            {
                TerrapainDifficulty = 0;
            }
        }
    }
    public class TerrapainDifficultyID
    {
        public const int clasic = 0;
        public const int torture = 1;
        public const int suicide = 2; 
    }
}