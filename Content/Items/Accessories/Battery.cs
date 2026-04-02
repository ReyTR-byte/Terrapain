using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.Buffs;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Stimulators;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories
{
    public class Battery : ActiveAccessory
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 4));
        }
        public override void ModSetDefaults()
        {
            Item.width = 38;
            Item.height = 50;
            Item.GetT().activeAccessory = true;
            AbilityReloadMax = 300;
            AbilityUnarmedOnly = false;
            abilityIcon = new LightningIcon();
        }
        public override bool? CanUseAbility(Player player)
        {
            return AbilityReloadMax - AbilityReload >= 45;
        }
        public override bool SetAbilityReload(Player player)
        {
            return false;
        }
        public override bool OnUseAbility(Player player)
        {
            player.AddBuff(ModContent.BuffType<Shocked>(), AbilityReloadMax - AbilityReload);
            return false;
        }
        public override void ModUpdateAccessory(Player player, bool hideVisual)
        {
            if (player.Custom().unarmed)
            {
                DescriptionLinesCount = 2;
            }
            else
            {
                DescriptionLinesCount = 1;
            }
            if (player.HasBuff(ModContent.BuffType<Shocked>()))
            {
                AbilityReload += 2;
            }
            player.Custom().stimulator = new ShockerStimulator(Item);
        }
    }
}
