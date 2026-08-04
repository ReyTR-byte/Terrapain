using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.TGlobalItems
{
    public class EvilBossesSummon : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.WormFood || entity.type == ItemID.BloodySpine;
        }
        public override void SetDefaults(Item entity)
        {
            entity.consumable = false;
            entity.maxStack = 1;
        }
        public override bool CanUseItem(Item item, Terraria.Player player)
        {
            return WorldDifficultySystem.clasic;
        }
    }
}
