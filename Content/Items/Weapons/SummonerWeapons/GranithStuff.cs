using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Content.Projectiles.Minions;

namespace Terrapain.Content.Items.Weapons.SummonerWeapons
{
    public class GranithStuff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;

            Item.UseSound = SoundID.Item8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;

            Item.DamageType = DamageClass.Summon;
            Item.damage = 10;
            Item.mana = 20;
            Item.knockBack = 6;
            Item.shoot = ModContent.ProjectileType<GranithStuffProjectile>();
            Item.value = Item.buyPrice(gold: 8);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = Vector2.Zero;
            position = Main.MouseWorld;
            damage *= 2;
        }
        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(-4, 0);
        }
    }
}
