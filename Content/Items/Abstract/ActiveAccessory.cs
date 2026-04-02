using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class ActiveAccessory : ModItem
    {
        public int DescriptionLinesCount
        {
            get => activeAccessory.DescriptionLinesCount;
            set => activeAccessory.DescriptionLinesCount = value;
        }
        public virtual string AbilityDescription => $"Mods.Terrapain.AbilityDescription.{this.GetType().Name}";
        public int AbilityReload 
        {
            get => activeAccessory.AbilityReload;
            set => activeAccessory.AbilityReload = value;   
        }
        public int AbilityReloadMax
        {
            get => activeAccessory.AbilityReloadMax;
            set => activeAccessory.AbilityReloadMax = value;
        }
        public bool AbilityUnarmedOnly
        {
            get => activeAccessory.AbilityUnarmedOnly;
            set => activeAccessory.AbilityUnarmedOnly = value;
        }
        public int DashReload
        {
            get => activeAccessory.DashReload;
            set => activeAccessory.DashReload = value;
        }
        public int DashReloadMax
        {
            get => activeAccessory.DashReloadMax;
            set => activeAccessory.DashReloadMax = value;
        }
        public int DashDuration
        {
            get => activeAccessory.DashDuration;
            set => activeAccessory.DashDuration = value;
        }
        public float DashPower
        {
            get => activeAccessory.DashPower;
            set => activeAccessory.DashPower = value;
        }
        public VanillaItemActiveAccessory activeAccessory;
        public AbilityIcon abilityIcon
        {
            get => activeAccessory.abilityIcon;
            set => activeAccessory.abilityIcon = value;
        }
        public AbilityIcon dashIcon
        {
            get => activeAccessory.dashIcon;
            set => activeAccessory.dashIcon = value;
        }

        public virtual bool? CanUseAbility(Player player) { return null; }
        public virtual bool SetAbilityReload(Player player) { return true; }
        public virtual void OnTryUseAbilty(Player player) { }
        public virtual bool OnUseAbility(Player player) { return true; }
        public virtual bool? CanUseDash(Player player, bool[] Directions) { return null; }
        public virtual void OnTryUseDash(Player player, bool[] Directions) { }
        public virtual bool OnUseDash(Player player, bool[] Directions) { return true; }
        public virtual void ModUpdateAccessory(Player player, bool hideVisual) { }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModUpdateAccessory(player, hideVisual);
            activeAccessory.activeAccessory = this;
            Item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = activeAccessory;
        }
        public virtual void ModSetDefaults() { }
        public override void SetDefaults()
        {
            Item.accessory = true;
            activeAccessory = new DefaultActiveAccessory();
            ModSetDefaults();
            activeAccessory.activeAccessory = this;
            Item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem = activeAccessory;
        }
    }
}