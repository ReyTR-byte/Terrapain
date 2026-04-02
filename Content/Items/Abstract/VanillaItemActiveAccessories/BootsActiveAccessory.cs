using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.ChargeStrips;
using Terrapain.Content.Dashes;
using Terraria;
using Terraria.GameContent.LootSimulation.LootSimulatorConditionSetterTypes;
using Terraria.ID;

namespace Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories
{
    public class BootsActiveAccessory : VanillaItemActiveAccessory
    {
        public float velocityMultiplyer;
        public int duration;
        public float accelerationMultiplyer;
        public int infiniteFlightDuration;
        public float dashPriority;
        public int dashPenetrate;
        public bool hurtfull;

        public override string AbilityDescription => base.AbilityDescription + (infiniteFlightDuration > 0? "InfFly" : "");
        public BootsActiveAccessory(float VelocityMultiplyer, int Duration, int Reload, float AccelerationMultiplyer, int InfiniteFlightDuration = 0, float DashPriority = 2, int DashPenetrate = -1, bool Hurtfull = false)
        {
            velocityMultiplyer = VelocityMultiplyer;
            duration = Duration;
            AbilityReloadMax = Reload;
            accelerationMultiplyer = AccelerationMultiplyer;
            infiniteFlightDuration = InfiniteFlightDuration;
            dashPriority = DashPriority;
            dashPenetrate = DashPenetrate;
            hurtfull = Hurtfull;
            if (InfiniteFlightDuration != 0)
            {
                abilityChargeStrip = new DoubleAbilityChargeStrip(1 - (float)infiniteFlightDuration / duration);
            }
            abilityIcon = infiniteFlightDuration == 0? new BootIcon() : new BootIconInfiniteFly();
            DescriptionLinesCount = 1;
        }
        bool Using;
        public override void OnUseAbility(Player player, Item item)
        {
            Using = true;
            FloatAbilityReload = 0;
        }
        public override void UpdateAccessory(Player player, Item item)
        {
            if (Using)
            {
                if (infiniteFlightDuration > FloatAbilityReload / AbilityReloadMax * duration)
                {
                    player.wingTime += 1;
                }
                player.Custom().Dash = new ActiveAccessoryDash(item) { DashDuration = DashDuration, damageType = item.DamageType, DashPower = DashPower, priority = dashPriority, penetrate = dashPenetrate, hurtfull = hurtfull };
                player.accRunSpeed *= velocityMultiplyer;
                player.runAcceleration *= accelerationMultiplyer;
                player.Custom().bootsActiveAccessory = true;
            }
        }
        float FloatAbilityReload;
        public override void Countdown(Player player, Item item)
        {
            if (!Using)
            {
                base.Countdown(player, item);
            }
            else
            {
                FloatAbilityReload += AbilityReloadMax / (float)duration;
                AbilityReload = (int)FloatAbilityReload;
                if (AbilityReload >= AbilityReloadMax)
                {
                    AbilityReload = AbilityReloadMax;
                    Using = false;
                }
                if (DashReload > 0)
                {
                    DashReload--;
                }
            }
        }
        public override void ResetAbilities(string reason)
        {
            Using = false;
            base.ResetAbilities(reason);
        }
        public override void SetAbilityReload(Player player, Item item)
        {
            
        }
    }
}
