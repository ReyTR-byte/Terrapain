using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terrapain.Common.Player;
using Microsoft.Xna.Framework;

namespace Terrapain.Content.Items.Accessories
{
    public class ExplosiveSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 2));
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.accessory = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 40;
            Item.value = Item.buyPrice(0, 4, 30, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TerrapainPlayer>().ExplosiveSkull = true;
            player.GetModPlayer<TerrapainPlayer>().ExplosiveSkullDamage = Item.damage;
        }
        public override void UpdateVisibleAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual && player.GetModPlayer<TerrapainPlayer>().ExplosiveSkullReload == 0)
            {
                bool haveProj = false;
                int proj = -1;
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.type == ModContent.ProjectileType<Content.Projectiles.Friendly.ExplosiveSkull>() && p.owner == player.whoAmI)
                    {
                        haveProj = true;
                        if (p.ai[0] == 1)
                            proj = p.whoAmI;
                    }
                }
                if (!haveProj)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.Friendly.ExplosiveSkull>(), 0, 0, player.whoAmI, 1);
                }
                else
                {
                    if (proj != -1)
                    {
                        Main.projectile[proj].timeLeft = 2;
                    }
                }
            }
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(nameof(ItemID.DemoniteBar), 9);
            recipe.AddIngredient(ItemID.Bomb, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}