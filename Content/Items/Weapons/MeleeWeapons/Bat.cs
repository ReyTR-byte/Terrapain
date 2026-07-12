using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{
    public class Bat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.knockBack = 15;
            Item.damage = 25;
            Item.useAnimation = 50;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = TGlobalItem.BatUseStyle;
            Item.value = Item.buyPrice(gold: 4, silver: 50);
            Item.GetT().StaminaUsage = 3f;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
