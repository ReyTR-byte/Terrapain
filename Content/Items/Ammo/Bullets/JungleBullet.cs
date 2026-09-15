using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Ammo.Bullets
{
    public class JungleBullet : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.Ammo.Bullets.JungleBullet>();
            Item.shootSpeed = 5f;
            Item.ammo = AmmoID.Bullet;
            Item.value = Item.buyPrice(0, 0, 0, 2);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(99);
            recipe.AddIngredient(ItemID.JungleSpores);
            recipe.AddIngredient(ItemID.MusketBall, 99);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
