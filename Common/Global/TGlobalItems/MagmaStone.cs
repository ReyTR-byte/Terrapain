using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content;
using Terrapain.Content.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.TGlobalItems
{
    public class MagmaStone : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //if (!entity.accessory) return false;

            //Terraria.Player player = new Terraria.Player();
            //player.armor[3] = entity;
            //player.UpdateEquips(0);
            //Functions.Chatic(player.magmaStone);

            //return player.magmaStone;
            return entity.type == ItemID.MagmaStone;
        }
    }
}
