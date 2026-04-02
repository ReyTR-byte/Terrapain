using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.TGlobalItems
{
    public class Boots : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.HermesBoots || entity.type == ItemID.SailfishBoots || entity.type == ItemID.SandBoots || entity.type == ItemID.FlurryBoots || entity.type == ItemID.SpectreBoots || entity.type == ItemID.LightningBoots || entity.type == ItemID.FrostsparkBoots || entity.type == ItemID.TerrasparkBoots; 
        }
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.HermesBoots:
                case ItemID.SailfishBoots:
                case ItemID.SandBoots:
                case ItemID.FlurryBoots:
                case ItemID.SpectreBoots:
                    entity.GetGlobalItem<TGlobalItem>().dashAccessory = true;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.BootsActiveAccessory(1.5f, 900, 2400, 1.3f, DashPriority: 0.5f);
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower = 17;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashDuration = 15;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReloadMax = 60;
                    entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
                    break;
                case ItemID.LightningBoots:
                    entity.GetGlobalItem<TGlobalItem>().dashAccessory = true;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.BootsActiveAccessory(1.7f, 950, 2400, 1.5f, 350, 0.7f);
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower = 17.5f;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashDuration = 16;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReloadMax = 60;
                    entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
                    break;
                case ItemID.FrostsparkBoots:
                    entity.damage = 30;
                    entity.DamageType = ModContent.GetInstance<Unarmed>();
                    entity.GetGlobalItem<TGlobalItem>().dashAccessory = true;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.BootsActiveAccessory(1.7f, 950, 2400, 1.5f, 400, 1.5f, 1, true);
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower = 17.5f;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashDuration = 16;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReloadMax = 60;
                    entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
                    break;
                case ItemID.TerrasparkBoots:
                    entity.damage = 40;
                    entity.DamageType = ModContent.GetInstance<Unarmed>();
                    entity.GetGlobalItem<TGlobalItem>().dashAccessory = true;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = new Content.Items.Abstract.VanillaItemActiveAccessories.BootsActiveAccessory(1.8f, 1000, 2400, 1.6f, 500, 1.6f, 2, true);
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashPower = 18;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashDuration = 16;
                    entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.DashReloadMax = 60;
                    entity.GetGlobalItem<TGlobalItem>().activeAccessory = true;
                    break;
            }
            entity.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityUnarmedOnly = false;

        }
    }
}
