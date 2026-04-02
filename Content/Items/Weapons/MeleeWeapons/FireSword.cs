using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luminance.Core.Graphics;
using Terraria.Utilities;
using System.Data;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles.Friendly;
using Terrapain.Common.Global;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
    public class FireSword : ModItem
    {
        int timer;
        bool fire => timer >= 300;
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 76;
            Item.height = 106;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = TGlobalItem.NormalSwing;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FireBall>();
            Item.shootSpeed = 8;
            Item.value = Item.buyPrice(gold: 6);
        }
        UnifiedRandom ur = new UnifiedRandom();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2){
                if (fire)
                {
                    Projectile.NewProjectile(source, position,velocity, type, damage, knockback / 3, player.whoAmI, 2, 1, 0);
                }
            }
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 600);
            timer += 60;
            if (timer > 1000)
            {
                timer = 1000;
            }
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (timer > 0)
            {
                timer--;
            }
            Item.color = new Color(255, 0, 0, (float)timer / 1500);
        }
        public override void UpdateInventory(Player player)
        {
            if (timer > 0)
            {
                timer--;
            }
            Item.color = new Color(255, 0, 0, (float)timer / 1500);
        }
        //public override void UseItemFrame(Player player)
        //{
        //    player.itemLocation -= new Vector2((float)Math.Cos(Functions.AngleFromVector(player.itemLocation - player.Center)) * 12, (float)Math.Sin(Functions.AngleFromVector(player.itemLocation - player.Center)) * 12);
        //    if (fire)
        //    {
        //        Vector2 vector = Functions.UnitVectorFromRotation(player.itemRotation - (float)Math.PI * 0.5f * player.direction - 0.45f * player.direction + (Functions.AngleFromVector(new Vector2(76,106)) * player.direction));
        //        vector.X *= player.direction;
        //        vector.Y *= player.direction;
        //        for (int i = 0; i < 20; i++)
        //        {
        //            Dust.NewDust(player.Center + vector * (40 / new Vector2(player.itemHeight,player.itemWidth).Length() + ur.NextFloat() * (1 - 40 / new Vector2(player.itemHeight,player.itemWidth).Length())) * new Vector2(player.itemHeight,player.itemWidth).Length(), 0, 0, DustID.Torch, Scale: 2);
        //        }
        //        Lighting.AddLight(player.Center + vector * ur.NextFloat() * new Vector2(player.itemHeight,player.itemWidth).Length(), 2, 2, 2);
        //    }
        //}
        public override Vector2? HoldoutOffset()
        {
            return Vector2.UnitX * -4;
        }
        
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 15);
            recipe.AddIngredient(ModContent.ItemType<SuperDenseGel>(), 12);
            recipe.AddIngredient(ItemID.Fireblossom, 6);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}