using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles.Friendly;
using Terrapain.Content.Projectiles.Minions;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{

    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
    public class GlassSword : ModItem
    {
        int charge = 0;

        Projectile projectile;
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = 20;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GlassSwordProjectile>();
            Item.shootSpeed = 300;
            Item.noMelee = true;
            Item.value = Item.buyPrice(gold: 7);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, player.Center + velocity / velocity.Length() * 25, velocity / Item.useTime, Item.shoot, damage, knockback, player.whoAmI, Item.useTime, charge);
            projectile = Main.projectile[proj];
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GlassSwordProjectile>()] > 0)
            {
                charge = projectile.frame;

                if (player.HeldItem.type == Item.type)
                {
                    if (Main.mouseRight)
                    {
                        projectile.ai[2] = 1;
                        charge -= 1;
                        if (charge < 0)
                        {
                            charge = 0;
                        }
                    }       
                }
            }
        }
        
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<GlassSwordProjectile>()] < 1;
        }

        public override void UseItemFrame(Player player)
        {
            player.bodyFrame.X = 0;
            player.bodyFrame.Width = 40;
            player.bodyFrame.Height = 56;
            if (player.itemTime > player.itemAnimationMax * 0.66f)
            {
                player.bodyFrame.Y = 56;
            }
            else if (player.itemTime > player.itemAnimationMax * 0.33f)
            {
                player.bodyFrame.Y = 112;
            }
            else
            {
                player.bodyFrame.Y = 168;
            }
        }
        
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MassiveLensSharp>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
    }
    public class GlassSwordProjectile : ModProjectile
    {
        private int useTime
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private int charge
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public bool turningBack
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value? 1 : 2;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.height = 34;
            Projectile.width = 34;

            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 99999999;
        }

        int timer;
        bool returning;
        float projectileRotationSpeed;
        bool canCharge = true;
        float speed;
        float maxSpeed;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = charge;
            projectileRotationSpeed = 4 * (float)Math.PI / useTime;
        }

        public override void AI()
        {
            Projectile.rotation += projectileRotationSpeed;
            Vector2 direction = Main.player[Projectile.owner].Center - Projectile.Center;
            
            if (!turningBack)
            {
                if (useTime != 0 && direction != Vector2.Zero)
                    Projectile.velocity += (direction / direction.Length() * 150f / (float)(useTime * useTime)) * direction.Length() / 80;
                if (direction.Length() > 300)
                    Projectile.velocity = (direction / direction.Length() * 150f / (float)(useTime * useTime)) * direction.Length() / 5;
            }
            else
            {
                Projectile.tileCollide = false;
                direction.Normalize();
                maxSpeed = 300f / useTime;
                speed = Projectile.velocity.Length();
                if (speed >= maxSpeed)
                {
                    speed -= 15f / useTime;
                    if (speed <= maxSpeed)
                    {
                        speed = maxSpeed;
                    }
                }
                else
                {
                    speed += 15f / useTime;
                    if (speed >= maxSpeed)
                    {
                        speed = maxSpeed;
                    }
                }
                Projectile.velocity = direction * speed;
            }
            if (timer >= useTime && useTime != 0)
                returning = true;

            if (returning && (Main.player[Projectile.owner].Center - Projectile.Center).Length() <= 25)
                Projectile.Kill();

            Projectile.light = (float)charge / (float)3;
            
            timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (canCharge)
            {
                canCharge = false;
                if (charge < 8)
                {
                    Projectile.frame++;
                    charge++;
                }
                else
                {
                    Projectile.frame = 0;
                    charge = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2((float)Math.Cos(0.3 * Math.PI + 0.1 * i * Math.PI), -(float)Math.Sin(0.3 * Math.PI + 0.1 * i * Math.PI)) * 5;
                        Projectile.NewProjectile(target.GetSource_FromAI(), target.Center, velocity, ModContent.ProjectileType<ChargedBlood>(), Projectile.damage, 0.1f, Projectile.owner);
                    }
                    Projectile.NewProjectile(target.GetSource_FromAI(), target.Center, Vector2.One, ModContent.ProjectileType<PetEye>(), Projectile.damage, 0.1f, Projectile.owner);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            useTime *= -1;
			// If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
			// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}
        
    }
}