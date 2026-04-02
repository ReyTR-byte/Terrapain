using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.ChargeStrips;
using Terrapain.Common.UI.Assets.EmptyStrips;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;

namespace Terrapain.Content.Dashes
{
    public interface IDashSource
    {
        public AbilityIcon dashIcon { get; set; }
        public AbilityFrame abilityFrame { get; set; }
        public ItemFrame itemFrame { get; set; }
        public ChargeStrip chargeStrip { get; set; }
        public EmptyStrip emptyStrip { get; set; }
        public int reloadMax { get; set; }
        public int reload { get; set; }
        public Item TryGetDrawItem();
        public Item TryGetDashItem();
        public bool CanUse(Player player, bool[] directions);
        public void OnUse(Player player, bool[] directions);
    }
}
