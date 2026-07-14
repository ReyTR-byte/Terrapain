using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;
using Terraria.DataStructures;

namespace Terrapain.Content.Dashes
{
    public class ActiveAccessoryDashSource : IDashSource
    {
        Item sourceItem;
        public ActiveAccessoryDashSource(Item item)
        {
            sourceItem = item;
        }

        public int reloadMax { get => sourceItem.GetT().ActiveAccessoryVanillaItem.DashReloadMax; set => sourceItem.GetT().ActiveAccessoryVanillaItem.DashReloadMax = value; }
        public int reload { get => sourceItem.GetT().ActiveAccessoryVanillaItem.DashReload; set => sourceItem.GetT().ActiveAccessoryVanillaItem.DashReload = value; }
        public AbilityIcon dashIcon { get => sourceItem.GetT().ActiveAccessoryVanillaItem.dashIcon; set => sourceItem.GetT().ActiveAccessoryVanillaItem.dashIcon = value; }
        public AbilityFrame abilityFrame { get => sourceItem.GetT().ActiveAccessoryVanillaItem.abilityFrame; set => sourceItem.GetT().ActiveAccessoryVanillaItem.abilityFrame = value; }
        public ItemFrame itemFrame { get => sourceItem.GetT().ActiveAccessoryVanillaItem.itemFrame; set => sourceItem.GetT().ActiveAccessoryVanillaItem.itemFrame = value; }
        public BarFill chargeStrip { get => sourceItem.GetT().ActiveAccessoryVanillaItem.dashChargeStrip; set => sourceItem.GetT().ActiveAccessoryVanillaItem.dashChargeStrip = value; }
        public Bar emptyStrip { get => sourceItem.GetT().ActiveAccessoryVanillaItem.dashEmptyStrip; set => sourceItem.GetT().ActiveAccessoryVanillaItem.dashEmptyStrip = value; }

        public bool CanUse(Player player, bool[] directions)
        {
            return sourceItem.GetT().ActiveAccessoryVanillaItem.CanUseDash(player, directions, sourceItem);
        }

        public void OnUse(Player player, bool[] directions)
        {
            sourceItem.GetT().ActiveAccessoryVanillaItem.OnUseDash(player, directions, sourceItem);
        }

        public Item TryGetDashItem()
        {
            return sourceItem;
        }

        public Item TryGetDrawItem()
        {
            return sourceItem;
        }
    }
}
