using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories.ActiveAccessories.VanillaItemActiveAccessories
{
    public class BootsActiveAccessory : VanillaItemActiveAccessory
    {
        public override VanillaItemRework GetNewInstance(Item item)
        {
            return new BootsActiveAccessory();
        }
        public override int[] Items => [ 
            ItemID.TerrasparkBoots, 
            ItemID.FrostsparkBoots, 
            ItemID.LightningBoots, 
            ItemID.SpectreBoots, 
            ItemID.FlurryBoots, 
            ItemID.SandBoots, 
            ItemID.SailfishBoots, 
            ItemID.HermesBoots 
            ];
        public override void ModSetDefaults(Item entity)
        {
            entity.GetT().dashAccessory = true;
            entity.GetT().activeAccessory = true;
            entity.GetT().ActiveAccessory = new ActiveAccessory(this);
            activeAccessory = entity.GetT().ActiveAccessory;
            dashPenetrate = -1;
            hurtfull = false;
            switch (entity.type)
            {
                case ItemID.HermesBoots:
                case ItemID.SailfishBoots:
                case ItemID.SandBoots:
                case ItemID.FlurryBoots:
                case ItemID.SpectreBoots:
                    velocityMultiplyer = 1.5f;
                    duration = 900;
                    AbilityReloadMax = 2400;
                    accelerationMultiplyer = 1.3f;
                    dashPriority = 0.5f;

                    DashPower = 17;
                    DashDuration = 15;
                    DashReloadMax = 60;
                    break;
                case ItemID.LightningBoots:
                    velocityMultiplyer = 1.7f;
                    duration = 950;
                    AbilityReloadMax = 2400;
                    accelerationMultiplyer = 1.5f;
                    infiniteFlightDuration = 350;
                    dashPriority = 0.7f;

                    DashPower = 17.5f;
                    DashDuration = 16;
                    DashReloadMax = 60;
                    break;
                case ItemID.FrostsparkBoots:
                    entity.damage = 30;
                    entity.DamageType = ModContent.GetInstance<Unarmed>();
                    velocityMultiplyer = 1.7f;
                    duration = 950;
                    AbilityReloadMax = 2400;
                    accelerationMultiplyer = 1.5f;
                    infiniteFlightDuration = 400;
                    dashPriority = 1.5f;
                    dashPenetrate = 1;
                    hurtfull = true;

                    DashPower = 17.5f;
                    DashDuration = 16;
                    DashReloadMax = 60;
                    break;
                case ItemID.TerrasparkBoots:
                    entity.damage = 40;
                    entity.DamageType = ModContent.GetInstance<Unarmed>();
                    velocityMultiplyer = 1.8f;
                    duration = 1000;
                    AbilityReloadMax = 2400;
                    accelerationMultiplyer = 1.6f;
                    infiniteFlightDuration = 500;
                    dashPriority = 1.6f;
                    dashPenetrate = 2;
                    hurtfull = true;

                    DashPower = 18;
                    DashDuration = 16;
                    DashReloadMax = 60;
                    break;
            }
            AbilityUnarmedOnly = false;
            if (infiniteFlightDuration != 0)
            {
                activeAccessory.abilityChargeBar = new DoubleAbilityBarFill(1 - (float)infiniteFlightDuration / duration);
            }
            abilityIcon = infiniteFlightDuration == 0? new BootIcon() : new BootIconInfiniteFly();
            DescriptionLinesCount = 1;
        }
        public float velocityMultiplyer;
        public int duration;
        public float accelerationMultiplyer;
        public float curentMaxVelocity;
        public int infiniteFlightDuration;
        public float dashPriority;
        public int dashPenetrate;
        public bool hurtfull;

        public override bool SetAbilityReload(Player player, Item item)
        {
            return false;
        }
        public override string AbilityDescription => $"Mods.Terrapain.AbilityDescription.{this.GetType().Name}" + (infiniteFlightDuration > 0? "InfFly" : "");
        // public BootsActiveAccessory(float VelocityMultiplyer, int Duration, int Reload, float AccelerationMultiplyer, int InfiniteFlightDuration = 0, float DashPriority = 2, int DashPenetrate = -1, bool Hurtfull = false)
        // {
        //     velocityMultiplyer = VelocityMultiplyer;
        //     duration = Duration;
        //     AbilityReloadMax = Reload;
        //     accelerationMultiplyer = AccelerationMultiplyer;
        //     infiniteFlightDuration = InfiniteFlightDuration;
        //     dashPriority = DashPriority;
        //     dashPenetrate = DashPenetrate;
        //     hurtfull = Hurtfull;
        //     if (InfiniteFlightDuration != 0)
        //     {
        //         abilityChargeStrip = new DoubleAbilityBarFill(1 - (float)infiniteFlightDuration / duration);
        //     }
        //     abilityIcon = infiniteFlightDuration == 0? new BootIcon() : new BootIconInfiniteFly();
        //     DescriptionLinesCount = 1;
        // }
        bool Using;
        public override void OnUseAbility(Player player, Item item)
        {
            Using = true;
            FloatAbilityReload = 0;
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            AbilityReloadMax = 2400;
            if (Using)
            {
                if (infiniteFlightDuration > FloatAbilityReload / AbilityReloadMax * duration)
                {
                    player.wingTime += 1;
                }
                if (MathF.Abs(player.velocity.X) + 0.5f > player.accRunSpeed)
                {
                    player.accRunSpeed = MathHelper.Clamp(curentMaxVelocity + 0.02f, player.accRunSpeed, player.accRunSpeed * velocityMultiplyer);
                    curentMaxVelocity = player.accRunSpeed;
                    player.runAcceleration *= accelerationMultiplyer;
                    player.Custom().bootsActiveAccessory = true;
                }
                else
                {
                    curentMaxVelocity = 0;
                }
                player.Custom().Dash = new ActiveAccessoryDash(item) { DashDuration = DashDuration, damageType = item.DamageType, DashPower = DashPower, priority = dashPriority, penetrate = dashPenetrate, hurtfull = hurtfull };
            }
        }
        float FloatAbilityReload;
        public override bool Countdown(Player player, Item item)
        {
            if (!Using)
            {
                return true;
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
            return false;
        }
        public override bool ResetAbilities(Player player, Item item, string reason)
        {
            Using = false;
            return true;
        }
    }
}
