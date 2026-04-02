using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.ChargeStrips;
using Terrapain.Common.UI.Assets.EmptyStrips;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class VanillaItemActiveAccessory
    {
        public int AbilityReload;
        public int AbilityReloadMax;
        public bool AbilityUnarmedOnly = true;
        public int DashReload;
        public int DashReloadMax;
        public int DashDuration;
        public float DashPower;
        public int DescriptionLinesCount;
        public virtual string AbilityDescription => activeAccessory != null? activeAccessory.AbilityDescription : $"Mods.Terrapain.AbilityDescription.{this.GetType().Name}";

        public ActiveAccessory activeAccessory = null;

        public AbilityFrame abilityFrame = new DefaultAbilityFrame();
        public ItemFrame itemFrame = new DefaultItemFrame();
        public AbilityIcon abilityIcon = null;
        public EmptyStrip abilityEmptyStrip = new DefaultEmptyStrip();
        public ChargeStrip abilityChargeStrip = new DefaultAbilityChargeStrip();
        public AbilityIcon dashIcon = new DashIcon();
        public EmptyStrip dashEmptyStrip = new DefaultEmptyStrip();
        public ChargeStrip dashChargeStrip = new DefaultDashChargeStrip();

        public virtual void UpdateAccessory(Player player, Item item) { }
        public virtual bool CanUseAbility(Player player, Item item)
        {
            bool? canUse = activeAccessory?.CanUseAbility(player);
            if (canUse.HasValue)
                return canUse.Value;

            return AbilityReload == 0 && (player.GetModPlayer<TerrapainPlayer>().unarmed || !AbilityUnarmedOnly);
        }
        public virtual void SetAbilityReload(Player player, Item item)
        {
            if (activeAccessory != null) {
                if (activeAccessory.SetAbilityReload(player))
                    AbilityReload = AbilityReloadMax;
            }
            else
                AbilityReload = AbilityReloadMax;
        }
        public virtual void TryUseAbilty(Player player, Item item)
        {
            if (activeAccessory != null)
                activeAccessory.OnTryUseAbilty(player);

            if (CanUseAbility(player, item))
            {
                SetAbilityReload(player, item);
                if (activeAccessory != null)
                {
                    if (activeAccessory.OnUseAbility(player))
                        OnUseAbility(player, item);
                }
                else
                    OnUseAbility(player, item);

            }
        }
        public virtual void OnUseAbility(Player player, Item item) { }
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
                DashReload = DashReloadMax;
                if (activeAccessory != null)
                {
                    if (activeAccessory.OnUseDash(player, Directions))
                        OnUseDash(player, Directions, item);
                }
                else
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