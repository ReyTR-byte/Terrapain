using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Common.Global.UseStyles;
using Terrapain.Content.Projectiles.Ammo.Bouquet;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
    public class Gardeneer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;

            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 15;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = TGlobalItem.BowOverride;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 11;
            Item.value = 50000;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<BouquetArrow>();
                velocity += player.velocity;
            }
            velocity = velocity.RotatedByRandom(0.02);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Item.GetGlobalItem<BowsOverride>().bowTime >= Item.useAnimation * 5)
            {
                foreach (int p in BouquetArrow.petals)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.1), p, damage / 2, knockback / 2, player.whoAmI);
                }
            }
            return true;
        }
    }
}
