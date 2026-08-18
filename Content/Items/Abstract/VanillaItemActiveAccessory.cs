using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;
using Terraria.DataStructures;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class VanillaItemActiveAccessory : VanillaItemRework, IActiveAccessory
    {
        public virtual bool? CanUseAbility(Player player, Item item) { return null; }
        public virtual bool SetAbilityReload(Player player, Item item) { return true; }
        public virtual bool OnTryUseAbilty(Player player, Item item) { return false; }
        public virtual void OnUseAbility(Player player, Item item) { }
        public virtual void OnHoldAbility(Player player, Item item) { }
        public virtual bool? CanUseDash(Player player, Item item, bool[] Directions) { return null; }
        public virtual void OnTryUseDash(Player player, Item item, bool[] Directions) { }
        public virtual bool OnUseDash(Player player, Item item, bool[] Directions) { return true; }
        public virtual bool Countdown(Player player, Item item) { return true; }
        public virtual bool ResetAbilities(Player player, Item item, string reason) { return true; }
        public virtual string AbilityDescription => null;

        public ActiveAccessory activeAccessory;
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
    }
}