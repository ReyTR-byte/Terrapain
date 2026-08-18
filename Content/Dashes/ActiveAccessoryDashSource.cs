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

        public int reloadMax { get => sourceItem.GetT().ActiveAccessory.DashReloadMax; set => sourceItem.GetT().ActiveAccessory.DashReloadMax = value; }
        public int reload { get => sourceItem.GetT().ActiveAccessory.DashReload; set => sourceItem.GetT().ActiveAccessory.DashReload = value; }
        public AbilityIcon dashIcon { get => sourceItem.GetT().ActiveAccessory.dashIcon; set => sourceItem.GetT().ActiveAccessory.dashIcon = value; }
        public AbilityFrame abilityFrame { get => sourceItem.GetT().ActiveAccessory.abilityFrame; set => sourceItem.GetT().ActiveAccessory.abilityFrame = value; }
        public ItemFrame itemFrame { get => sourceItem.GetT().ActiveAccessory.itemFrame; set => sourceItem.GetT().ActiveAccessory.itemFrame = value; }
        public BarFill chargeStrip { get => sourceItem.GetT().ActiveAccessory.dashChargeStrip; set => sourceItem.GetT().ActiveAccessory.dashChargeStrip = value; }
        public Bar emptyStrip { get => sourceItem.GetT().ActiveAccessory.dashEmptyStrip; set => sourceItem.GetT().ActiveAccessory.dashEmptyStrip = value; }

        public bool CanUse(Player player, bool[] directions)
        {
            return sourceItem.GetT().ActiveAccessory.CanUseDash(player, directions, sourceItem);
        }

        public void OnUse(Player player, bool[] directions)
        {
            sourceItem.GetT().ActiveAccessory.OnUseDash(player, directions, sourceItem);
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
