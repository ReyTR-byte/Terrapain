using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.MagicWeapons
{
    public class JungleStuff : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic; // Makes the damage register as magic. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type.
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot; // Makes the player use a 'Shoot' use style for the Item.
            Item.noMelee = true; // Makes the item not do damage with it's melee hitbox.
            Item.knockBack = 6;
            Item.value = 50000;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item17;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<JungleSeed>(); // Shoot a black bolt, also known as the projectile shot from the onyx blaster.
            Item.shootSpeed = 13; // How fast the item shoots the projectile.
            Item.crit = 32; // The percent chance at hitting an enemy with a crit, plus the default amount of 4.
            Item.mana = 11; // This is how much mana the item uses.
            Item.value = Item.buyPrice(gold: 8);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.05f * (float)Math.PI);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        //		public override void AddRecipes() {
        //			CreateRecipe()
        //				.AddIngredient<ExampleItem>()
        //				.AddTile<Tiles.Furniture.ExampleWorkbench>()
        //				.Register();
        //		}
    }
}
