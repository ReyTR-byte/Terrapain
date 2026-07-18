using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class VanillaItemActiveAccessory
    {
        public bool AutoUse;
        public bool CanAutoUse;
        public int AbilityReload;
        public int AbilityReloadMax;
        public bool AbilityUnarmedOnly = true;
        public int DashReload;
        public int DashReloadMax;
        public int DashDuration;
        public float DashPower;
        public int DescriptionLinesCount;

        public bool HoldAbility;
        public int HoldConsumption = 5;
        public bool HoldCanUse;

        public virtual string AbilityDescription => activeAccessory != null? activeAccessory.AbilityDescription : $"Mods.Terrapain.AbilityDescription.{this.GetType().Name}";

        public ActiveAccessory activeAccessory = null;

        public AbilityFrame abilityFrame = new DefaultAbilityFrame();
        public ItemFrame itemFrame = new DefaultItemFrame();
        public AbilityIcon abilityIcon = null;
        public Bar abilityEmptyStrip = new DefaultAbilityBar();
        public BarFill abilityChargeStrip = new DefaultAbilityBarFill();
        public AbilityIcon dashIcon = new DashIcon();
        public Bar dashEmptyStrip = new DefaultAbilityBar();
        public BarFill dashChargeStrip = new DefaultDashBarFill();

        public virtual void UpdateAccessory(Player player, Item item) { }
        public virtual bool CanUseAbility(Player player, Item item)
        {
            bool? canUse = activeAccessory?.CanUseAbility(player);
            if (canUse.HasValue)
                return canUse.Value;

            if (player.dead)
            {
                return false;
            }
            if (HoldAbility)
            {
                return AbilityReload < AbilityReloadMax && HoldCanUse;
            }
            return AbilityReload == 0 && (player.GetModPlayer<TerrapainPlayer>().unarmed || !AbilityUnarmedOnly);
        }
        public virtual void SetAbilityReload(Player player, Item item)
        {
            if (activeAccessory?.SetAbilityReload(player)?? true)
            {
                if (HoldAbility)
                {
                    AbilityReload += HoldConsumption;
                }
                else
                {
                AbilityReload = AbilityReloadMax;
                }
            }
        }
        public virtual void TryUseAbilty(Player player, Item item, bool release)
        {
            if (activeAccessory?.OnTryUseAbilty(player)?? false)
                return;

            if (CanUseAbility(player, item))
            {
                if (HoldAbility)
                {
                    if (HoldAbility && !release && AbilityReloadMax - AbilityReload > HoldConsumption)
                    {
                        if (activeAccessory?.OnHoldAbility(player)?? true)
                            OnHoldAbility(player, item);
                    }
                    else
                    {
                        if (activeAccessory?.OnUseAbility(player)?? true)
                            OnUseAbility(player, item);
                        HoldCanUse = false;
                    }
                }
                else if (!release)
                {
                    if (activeAccessory?.OnUseAbility(player)?? true)
                        OnUseAbility(player, item);
                }
                SetAbilityReload(player, item);
            }
        }
        public virtual void OnUseAbility(Player player, Item item) { }
        public virtual void OnHoldAbility(Player player, Item item) { }
        public virtual float AbilityCharge()
        {
            if (AbilityReloadMax == 0)
            {
                return 1;    
            }
            float num = 1 - (float)AbilityReload / AbilityReloadMax;
            if (num < 0)
            {
                return 0;
            }
            if (num > 1)
            {
                return 1;
            }
            return num;
        }
        public virtual bool CanUseDash(Player player, bool[] Directions, Item item)
        {
            return DashReload == 0;
        }
        public virtual void TryUseDash(Player player, bool[] Directions, Item item)
        {
            if (activeAccessory != null)
                activeAccessory.OnTryUseDash(player, Directions);

            if (CanUseDash(player, Directions, item))
            {
                    if (activeAccessory?.OnUseDash(player, Directions)?? true)
                        OnUseDash(player, Directions, item);
            }
        }
        public virtual void OnUseDash(Player player, bool[] Directions, Item item) 
        {
            DashReload = DashReloadMax;
        }
        public virtual void Countdown(Player player, Item item)
        {
            if (DashReload > 0)
            {
                DashReload--;
            }
            if (AbilityReload > 0)
            {
                AbilityReload--;
            }
            else
            {
                if (AutoUse)
                {
                    TryUseAbilty(player, item, false);
                }
                HoldCanUse = true;
            }
        }
        public virtual void ResetAbilities(string reason)
        {
            //switch (reason)
            //{
            //    case "Dead":
                    DashReload = 0;
                    AbilityReload = 0;
            //        break;
            //}
        }
        public virtual float DashCharge()
        {
            if (DashReloadMax == 0)
            {
                return 1;
            }
            float num = 1 - (float)DashReload / DashReloadMax;
            if (num < 0)
            {
                return 0;
            }
            if (num > 1)
            {
                return 1;
            }
            return num;
        }
    }
}