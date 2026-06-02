using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace Terrapain.Common.Global.TGlobalItems
{
    public class NightBossesSummon : GlobalItem
    {
        public override void Load()
        {
            On_Player.ItemCheck_CheckCanUse += On_Player_ItemCheck_CheckCanUse;
        }
        public override void Unload()
        {
            On_Player.ItemCheck_CheckCanUse -= On_Player_ItemCheck_CheckCanUse;
        }

        private bool On_Player_ItemCheck_CheckCanUse(On_Player.orig_ItemCheck_CheckCanUse orig, Terraria.Player self, Item sItem)
        {
            if (NightSummons.Contains(sItem.type))
            {
                int boss = 0;
                switch (sItem.type)
                {
                    case ItemID.SuspiciousLookingEye:
                        boss = NPCID.EyeofCthulhu;
                        break;
                }
                return !NPC.AnyNPCs(boss);
            }
            return orig(self, sItem);
        }
        int[] NightSummons = [ItemID.SuspiciousLookingEye];
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.SuspiciousLookingEye; 
        }
        public override bool InstancePerEntity => true;
        public override bool CanRightClick(Item item)
        {
            return true;
        }
        public int mode = 0;
        public override void RightClick(Item item, Terraria.Player player)
        {
            mode += 1;
            if (mode >= 3)
            {
                mode = 0;
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Time set", NetworkText.FromKey($"Mods.Terrapain.NightBossSummon_{mode}").ToString()));
        }
        public override bool? UseItem(Item item, Terraria.Player player)
        {
            if (mode == 1)
            {
                Main.dayTime = false;
                Main.time = 0;
            }
            if (mode == 2)
            {
                Main.dayTime = false;
                Main.time = Main.nightLength - 1;
            }
            return null;
        }
        public override bool ConsumeItem(Item item, Terraria.Player player)
        {
            return false;
        }
    }
}
