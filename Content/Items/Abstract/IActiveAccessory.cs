using Terraria;

namespace Terrapain.Content.Items.Abstract
{
    public interface IActiveAccessory
    {
        public virtual string AbilityDescription => null;
        public abstract bool? CanUseAbility(Player player, Item item);
        public abstract bool SetAbilityReload(Player player, Item item);
        public abstract bool OnTryUseAbilty(Player player, Item item);
        public abstract void OnUseAbility(Player player, Item item);
        public abstract void OnHoldAbility(Player player, Item item);
        public abstract bool? CanUseDash(Player player, Item item, bool[] Directions);
        public abstract void OnTryUseDash(Player player, Item item, bool[] Directions);
        public abstract bool OnUseDash(Player player, Item item, bool[] Directions);
        public abstract bool Countdown(Player player, Item item);
        public abstract bool ResetAbilities(Player player, Item item, string reason);
    }
}