using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Item.useStyle = ItemUseStyleID.Shoot;
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
    }
}
