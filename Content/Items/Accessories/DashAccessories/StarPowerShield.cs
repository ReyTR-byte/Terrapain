using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories;
using Terrapain.Content.Items.Ingredients;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Items.Accessories.DashAccessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class StarPowerShield : ActiveAccessory
    {
        public override void ModSetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.damage = 50;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;

            Item.defense = 2;
            Item.GetGlobalItem<TGlobalItem>().dashAccessory = true;
            activeAccessory = new ClasicDashAccessory();
        }

        public override void ModUpdateAccessory(Player player, bool hideVisual)
        {
            bool hurtful = player.Custom().unarmed;
            int immune = player.Custom().unarmed? 10 : 0;
            player.GetDamage<Unarmed>() += 0.1f;
            player.Custom().Dash = new ActiveAccessoryDash(Item) { DashPower = DashPower, DashDuration = DashDuration, damageType = Item.DamageType, hurtfull = hurtful, immune = immune, penetrate = 1};
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<WoodenShield>();
            recipe.AddIngredient<StarPowerAlloy>(8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
