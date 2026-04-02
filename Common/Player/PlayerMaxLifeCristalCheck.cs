using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Terrapain.Common.Player
{
    public class MaxLifeCristalCheck : ModPlayer
    {
        public int MaxLifeCristals = 7;
        public override void PostUpdate()
        {
            if (Player.ConsumedLifeCrystals > MaxLifeCristals)
                Player.ConsumedLifeCrystals = MaxLifeCristals;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["MaxLifeCristals"] = MaxLifeCristals;
        }

        public override void LoadData(TagCompound tag)
        {
            MaxLifeCristals = (int)tag["MaxLifeCristals"];
        }
    }
}