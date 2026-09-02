using Luminance.Common.Utilities;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Items.Accessories.ActiveAccessories.VanillaItemActiveAccessories
{
    public class FledglingWings : VanillaItemActiveAccessory
    {
        public override VanillaItemRework GetNewInstance(Item item)
        {
            return new FledglingWings();
        }
        public override int[] Items => [ItemID.CreativeWings];
        UnifiedRandom random = new UnifiedRandom();
        public override void ModSetDefaults(Item item)
        {
            item.damage = 25;
            item.DamageType = ModContent.GetInstance<Unarmed>();
            item.knockBack = 3;
            activeAccessory = new ActiveAccessory(this);
            activeAccessory.AbilityReloadMax = 75;
            activeAccessory.AbilityUnarmedOnly = true;
            activeAccessory.abilityIcon = new FledglingWingsAbilityIcon();
            item.GetT().activeAccessory = true;
            item.GetT().ActiveAccessory = activeAccessory;
            DescriptionLinesCount = 1;
        }
        public override void OnUseAbility(Player player, Item item)
        {
            int count = 5;
            float angle = MathF.PI * 0.75f / count;
            float startAngle = (player.direction == 1? MathF.PI : 0) - angle * count / 2;
            float speed = 5;
            for (int i = 0; i < count; i++)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy(startAngle + angle * i) * speed, ModContent.ProjectileType<Nugget>(), (int)player.GetTotalDamage<Unarmed>().ApplyTo(item.damage), player.GetTotalKnockback<Unarmed>().ApplyTo(item.knockBack), player.whoAmI);
            }
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (activeAccessory.AbilityCharge() < 0.33f)
            {
                player.wingTime = 0;
            }
        }
        
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach(TooltipLine line in tooltips)
            {
                if ((line.Name == "Damage" || line.Name == "Knockback") && !Main.player[Main.myPlayer].GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    line.Text = "";
                }
            }
        } 
    }
}