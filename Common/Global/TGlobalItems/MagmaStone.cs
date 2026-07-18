using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Player;
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
        public override void SetDefaults(Item entity)
        {
            entity.damage = 40;
            entity.DamageType = ModContent.GetInstance<Unarmed>();
            entity.knockBack = 3;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.MagmaStone();
            entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityReloadMax = 400;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityUnarmedOnly = true;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.abilityIcon = new BlastIcon();
            entity.GetT().ActiveAccessoryVanillaItem.HoldAbility = true;
            entity.GetT().ActiveAccessoryVanillaItem.HoldConsumption = 2;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if ((line.Name == "Damage" || line.Name == "Knockback") && !Main.player[Main.myPlayer].GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    line.Text = "";
                }
            }
        }
    }
}
