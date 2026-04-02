using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.MagicWeapons
{
    public class FireStuff : ModItem
    {
        int fireball = 0;
        int fireballID;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 40;

            Item.damage = 10;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 5;
            Item.shoot = ModContent.ProjectileType<FireBall>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item20;
            Item.GetGlobalItem<TGlobalItem>().ShootRotation = 0.25f * (float)Math.PI;
            Item.value = Item.buyPrice(gold: 6);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (fireball == 0)
                    fireballID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, fireball, 0, 1);
                else if (HasFireball(player))
                    Main.projectile[fireballID].ai[0] = fireball;
                if (fireball < 4)
                    fireball++;
            }
            else
            {
                player.statMana += 1;
                fireballID = Projectile.NewProjectile(source, position, velocity, type, (int)((float)damage * 1.2f) , knockback, player.whoAmI, 0, 1, 0);
            }
            return false;
        }
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.type == Item.type)
            {
                if ((!Main.mouseLeft || Main.mouseRight) && (fireball > 1 || player.itemTime == 1) && player.altFunctionUse != 2 || player.itemTime == 0)
                {
                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    fireball = 0;
                    if (HasFireball(player))
                        Main.projectile[fireballID].ai[1] = 1;
                }       
            }
            else
            {
                if (HasFireball(player))
                    Main.projectile[fireballID].ai[1] = 1;
            }
        }
        public override void UseItemFrame(Player player)
        {
            if (!HasFireball(player) && fireball > 0)
            {
                fireball = 0;
            }
        }
        bool HasFireball(Player player)
        {
            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<FireBall>() && proj.ai[1] != 1 && proj.owner == player.whoAmI)
                {
                    return true;
                }
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 9);
            recipe.AddIngredient(ItemID.Fireblossom, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }
}