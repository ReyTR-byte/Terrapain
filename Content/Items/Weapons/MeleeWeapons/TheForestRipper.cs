using System;
using System.Timers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terrapain.Content.Buffs;
using Terrapain.Common.Player;
using Terrapain.Content.Projectiles.Friendly;
using Terrapain.Common.Global;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{

    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
    public class TheForestRipper : ModItem
    {
        int bonusArcons = 0;
        int maxBonusArcons = 3;
        int timer = 60;

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = TGlobalItem.LightSwing;
            Item.shoot = ModContent.ProjectileType<Acorn>();
            Item.knockBack = 3;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.GetT().basicRotation = 0;
            Item.GetT().DrawOrigin = new Vector2(2, 10);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (bonusArcons == 0)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Item.whoAmI);
            }
            else
            {
                for (int i = 0; i <= bonusArcons; i++)
                {
                    Vector2 newVelocity = Functions.Rotate(velocity, 5f - 10f * ((float)i / (float)maxBonusArcons));
                    Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI, Item.whoAmI);
                }
            }
            return false;
        }
        public override void UpdateInventory(Player player)
        {
            timer--;
            if (timer <= 0 && bonusArcons > 0)
            {
                bonusArcons--;
                timer = 60;
            }
            if (player.GetModPlayer<TerrapainPlayer>().bonusArcons)
            {
                if (bonusArcons < maxBonusArcons)
                {
                    bonusArcons++;
                }
                timer = 60;
                player.GetModPlayer<TerrapainPlayer>().bonusArcons = false;
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.GetModPlayer<TerrapainPlayer>().bonusArcons = true;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(RecipeGroupID.Wood, 200);
            recipe.AddIngredient(ItemID.Acorn, 50);
            recipe.AddIngredient(ItemID.Apple, 5);
            recipe.AddIngredient(ItemID.Peach, 5);
            recipe.AddIngredient(ItemID.Apricot, 5);
            recipe.AddIngredient(ItemID.Lemon, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }
//    public class TheForestRipperSword : ModProjectile
//    {
//        int lifeTime;
//        int direction;
//        Player owner;

//        public override void SetStaticDefaults()
//        {
//            Main.projFrames[Projectile.type] = 2;
//        }

//        public override void SetDefaults()
//        {
//            Projectile.width = 40;
//            Projectile.height = 40;

//            Projectile.friendly = true;
//            Projectile.hostile = false;
//            Projectile.DamageType = DamageClass.Melee;

//            Projectile.tileCollide = false;
//            Projectile.penetrate = -1;
//            Projectile.timeLeft = 9999;
//            Projectile.alpha = 255;
//        }

//        public override void OnSpawn(IEntitySource source)
//        {
//            Projectile.velocity.Normalize();

//            direction = Projectile.velocity.X > 0 ? 1 : -1;

//            float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
//            if (Projectile.velocity.Y < 0)
//                angel = 2 * Convert.ToSingle(Math.PI) - angel;
//            Projectile.rotation = angel + Convert.ToSingle(-0.6 * direction * Math.PI);

//            owner = Main.player[Projectile.owner];

//            Projectile.timeLeft = owner.itemTime;
//            lifeTime = Projectile.timeLeft;

//            Projectile.velocity = Vector2.Zero;
//            if (direction == -1)
//                Projectile.frame = 1;
//        }

//        List<int> oldTargets = new List<int>();

//        public override bool? CanHitNPC(NPC target)
//        {
//            if (Functions.Collision(Main.player[Projectile.owner].Center, Projectile.Center,Projectile.width / 2, target.position, target.width, target.height))
//            {
//                foreach (var id in oldTargets)
//                {
//                    if (id == target.whoAmI)
//                    {
//                        return false;
//                    }
//                }
//                oldTargets.Add(target.whoAmI);
//                return null;
//            }
//            return false;
//        }

//        public override void AI()
//        {
//            if (Projectile.timeLeft % 3 == 0)
//            {
//                Dust.NewDust(Projectile.position, Projectile.height, Projectile.width, DustID.Grass);
//            }
//            Projectile.rotation += 1.2f * (float)Math.PI / lifeTime * direction;

//            Projectile.position.Y = owner.Center.Y - 35 * -(float)Math.Sin(Projectile.rotation) - Projectile.height / 2;
//            Projectile.position.X = owner.Center.X - 35 * -(float)Math.Cos(Projectile.rotation) - Projectile.width / 2;

//            if (owner.dead)
//                Projectile.Kill();

//            if (Projectile.timeLeft < 9000)
//            {
//                owner.ChangeDir(direction);
//                owner.heldProj = Projectile.whoAmI;
//                owner.itemAnimation = Projectile.timeLeft;
//                owner.itemTime = Projectile.timeLeft;
//                owner.itemRotation = MathHelper.WrapAngle(direction == 1? Projectile.rotation : (float)Math.PI - Projectile.rotation) * direction;
//            }
//        }
//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            owner.GetModPlayer<TerrapainPlayer>().bonusArcons = true;
//        }
//    }
}