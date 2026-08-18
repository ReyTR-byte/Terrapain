using Steamworks;
using Terrapain.Common.Global;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class ModItemActiveAccessory : ModItem, IActiveAccessory
    {
        public bool AutoUse
        {
            get => activeAccessory.AutoUse;
            set => activeAccessory.AutoUse = value;
        }
        public bool CanAutoUse
        {
            get => activeAccessory.CanAutoUse;
            set => activeAccessory.CanAutoUse = value;
        }
        public int DescriptionLinesCount
        {
            get => activeAccessory.DescriptionLinesCount;
            set => activeAccessory.DescriptionLinesCount = value;
        }
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
        public ActiveAccessory activeAccessory;
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
        public bool? CanUseAbility(Player player, Item item)
        {
            return CanUseAbility(player);
        }
        public virtual bool? CanUseAbility(Player player) { return null; }
        public bool SetAbilityReload(Player player, Item item)
        {
            return SetAbilityReload(player);
        }
        public virtual bool SetAbilityReload(Player player) { return true; }
        public bool OnTryUseAbilty(Player player, Item item)
        {
            return OnTryUseAbilty(player);
        }
        public virtual bool OnTryUseAbilty(Player player) { return false; }
        public void OnUseAbility(Player player, Item item)
        {
            OnUseAbility(player);
        }
        public virtual bool OnUseAbility(Player player) { return true; }
        public void OnHoldAbility(Player player, Item item)
        {
            OnHoldAbility(player);
        }
        public virtual bool OnHoldAbility(Player player) { return true; }
        public bool? CanUseDash(Player player, Item item, bool[] Directions)
        {
            return CanUseDash(player, Directions);
        }
        public virtual bool? CanUseDash(Player player, bool[] Directions) { return null; }
        public void OnTryUseDash(Player player, Item item, bool[] Directions)
        {
            OnTryUseDash(player, Directions);
        }
        public virtual void OnTryUseDash(Player player, bool[] Directions) { }
        public bool OnUseDash(Player player, Item item, bool[] Directions)
        {
            return OnUseDash(player, Directions);
        }
        public virtual bool OnUseDash(Player player, bool[] Directions) { return true; }
        public bool Countdown(Player player, Item item)
        {
            return Countdown(player);
        }
        public virtual bool Countdown(Player player) { return true; }
        public bool ResetAbilities(Player player, Item item, string reason)
        {
            return ResetAbilities(player, reason);
        }
        public virtual bool ResetAbilities(Player player, string reason) { return true; }
        public virtual void ModUpdateAccessory(Player player, bool hideVisual) { }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModUpdateAccessory(player, hideVisual);
            activeAccessory.activeAccessory = this;
            Item.GetGlobalItem<TGlobalItem>().ActiveAccessory = activeAccessory;
        }
        public virtual void ModSetDefaults() { }
        public override void SetDefaults()
        {
            Item.accessory = true;
            activeAccessory = new ActiveAccessory(this);
            ModSetDefaults();
            activeAccessory.activeAccessory = this;
            Item.GetGlobalItem<TGlobalItem>().ActiveAccessory = activeAccessory;
        }
    }
}