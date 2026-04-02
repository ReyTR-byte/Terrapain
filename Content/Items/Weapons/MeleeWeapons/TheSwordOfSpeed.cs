using System;
using System.Timers;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Common.Global;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class TheSwordOfSpeed : ModItem
	{
		public int speed = 20;
		public int basespeed = 20;
		public int maxspeed = 5;
		public int timer;
		public int timerrestart = 600;
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 66;
			Item.height = 66;
			Item.useTime = speed;
			Item.useAnimation = speed;
			Item.useStyle = TGlobalItem.LightSwing;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 6);
        }

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (speed > maxspeed)
			{
				speed -= 1;
				Item.useTime = speed;
				Item.useAnimation = speed;
			}
			timer = timerrestart;
		}
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
		{
			if (Main.myPlayer == target.whoAmI)
            {
                while (true)
                {
                    
                }
            }
        }
		public override void UpdateInventory(Player player)
		{
			timer -= 1;
			if (timer <= 0)
			{
				speed = basespeed;
				Item.useTime = speed;
				Item.useAnimation = speed;
			}
			if (player.dead)
            {
				speed = basespeed;
            }
        }
		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			speed = basespeed;
			Item.useTime = speed;
			Item.useAnimation = speed;
        }
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 12);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		UnifiedRandom rand = new UnifiedRandom();
		public override bool? UseItem(Player player)
		{
			if (rand.Next(speed) < 5)
			{
				Dust.NewDust(player.Center + new Vector2((float)Math.Cos(player.itemRotation) * player.direction, (float)Math.Sin(player.itemRotation)) * rand.NextFloat() * new Vector2(Item.width, Item.height).Length(), 0, 0, ModContent.DustType<TheSwordOfSpeedDust>());
			}
			return null;
		}
	}
}
