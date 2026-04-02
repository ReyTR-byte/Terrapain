using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class UnarmedItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        //public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        //{
        //    TooltipLine line = new TooltipLine(Mod, "Unarmed", Language.GetTextValue("Mods.Terrapain.Unarmed.SetBonus.CactusArmor"));
        //    tooltips.Add();
        //}
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (head.type == ItemID.CactusHelmet && body.type == ItemID.CactusBreastplate && legs.type == ItemID.CactusLeggings)
            {
                return "cactus";
            }
            else return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Terraria.Player player, string set)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                if (set == "cactus")
                {
                    player.AddBuff(BuffID.Thorns, 2);
                }
                player.GetModPlayer<TerrapainPlayer>().CactusSet = true;
                player.setBonus += " | " + Language.GetTextValue("Mods.Terrapain.Unarmed.SetBonus.CactusArmor");
            }
        }
    }
}
