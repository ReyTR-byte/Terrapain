using Terrapain.Content;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terrapain.Content.Items.System;

namespace Terrapain.Common.Player
{
    public class playerStartInventory : ModPlayer
    {
        bool forTheFirstTime = true; 
        public static List<int>StarterItems = [ModContent.ItemType<TheDropOfPain>()];
        public override void OnEnterWorld()
        {
            if (forTheFirstTime)
            {
                int giveItems = 0;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    if (giveItems == StarterItems.ToArray().Length)
                    {
                        break;
                    }
                    if (Player.inventory[i].type == 0)
                    {
                        Player.inventory[i] = new Item(StarterItems[giveItems]);
                        giveItems++;
                    }
                }
                if (giveItems < StarterItems.ToArray().Length)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.terrapain.Warns.CantAddAllItems"), Color.White);
                }
                forTheFirstTime = false;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag["forTheFirstTime"] = forTheFirstTime;
        }

        public override void LoadData(TagCompound tag)
        {
            try
            {
                forTheFirstTime = tag["forTheFirstTime"].Equals(true);
            }
            catch
            {
                forTheFirstTime = true;
            }
        }
    }
}