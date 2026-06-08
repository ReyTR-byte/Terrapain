using Terrapain.Content;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Terrapain.Common.Player
{
    public class MaxLifeCristalCheck : ModPlayer
    {
        public override void Load()
        {
            On_Player.ItemCheck_UseLifeCrystal += On_Player_ItemCheck_UseLifeCrystal;
        }
        public override void Unload()
        {
            On_Player.ItemCheck_UseLifeCrystal -= On_Player_ItemCheck_UseLifeCrystal;
        }
        private void On_Player_ItemCheck_UseLifeCrystal(On_Player.orig_ItemCheck_UseLifeCrystal orig, Terraria.Player self, Item sItem)
        {
            if (sItem.type == ItemID.LifeCrystal)
            {
                if (self.GetModPlayer<MaxLifeCristalCheck>().MaxLifeCristals < self.ConsumedLifeCrystals)
                {
                    orig(self, sItem);
                }
                if (self.itemAnimation == self.itemAnimationMax)
                {
                    self.Custom().CurentHeart = "LifeCrystal";
                }
            }
        }

        public int MaxLifeCristals = 7;
        public override void PostUpdate()
        {
            if (Player.ConsumedLifeCrystals > MaxLifeCristals)
                Player.ConsumedLifeCrystals = MaxLifeCristals;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["CurentHeart"] = Player.Custom().CurentHeart;
            tag["MaxLifeCristals"] = MaxLifeCristals;
        }
        public override void LoadData(TagCompound tag)
        {
            MaxLifeCristals = (int)tag["MaxLifeCristals"];
            try
            {
                Player.Custom().CurentHeart = tag.Get<string>("CurentHeart");
            }
            catch { }
        }
    }
}