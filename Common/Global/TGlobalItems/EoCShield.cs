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
    public class EoCShield : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.EoCShield;
        }
        public override void SetDefaults(Item entity)
        {
            entity.GetGlobalItem<TGlobalItem>().dashAccessory = true;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.EoCShield();
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower = 15;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashDuration = 15;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReloadMax = 60;
            entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityReloadMax = 150;
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.abilityIcon = new SuperDashIcon();
        }
        public override void OnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                if (player.GetModPlayer<PlayerMovement>().DashPower > item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower)
                {
                    item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityReload = 30;
                }
                item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReload = 30;
            }
        }
        public override void UpdateInventory(Item item, Terraria.Player player)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                item.DamageType = ModContent.GetInstance<Unarmed>();
            }
            else
            {
                item.DamageType = DamageClass.Melee;
            }
        }
        public override void UpdateAccessory(Item item, Terraria.Player player, bool hideVisual)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                item.DamageType = ModContent.GetInstance<Unarmed>();
            }
            else
            {
                item.DamageType = DamageClass.Melee;
            }
        }
    }
}
