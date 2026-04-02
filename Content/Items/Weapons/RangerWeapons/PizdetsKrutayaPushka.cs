using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
    public class PizdetsKrutayaPushka : ModItem
    {
        public override void SetDefaults() 
        {
            Item.width = 58;
            Item.height = 24;

            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item14;
            Item.shoot = ModContent.ProjectileType<GranithBall>();
            Item.shootSpeed = 22;
            Item.knockBack = 2;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.value = Item.buyPrice(gold: 8);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.09f), type, damage, knockback);
            }
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.07f) * 0.8f, ModContent.ProjectileType<BiggerGranithBall>(), (int)(damage * 1.2f), (int)(knockback * 1.2f));
            }
            return false;
        }
    }
}
