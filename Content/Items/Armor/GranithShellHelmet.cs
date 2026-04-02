using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Buffs;
using Microsoft.Xna.Framework;
using Terrapain.Common.Player;
using Terraria.Localization;
using Terrapain.Content.Projectiles.Minions;

namespace Terrapain.Content.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class GranithShellHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 26; // Width of the item
			Item.height = 26; // Height of the item
			Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
			Item.value = Item.sellPrice(gold:3); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 8; // The amount of defense the item will give when equipped
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			if (body.type == ModContent.ItemType<GranithShellChestplate>() && legs.type == ModContent.ItemType<GranithShellLeggings>())
			{
				return true;
			}
			return false;
		}
        public override void UpdateEquip(Player player)
        {
			player.GetDamage(DamageClass.Melee) += 0.5f - (player.statLife / player.statLifeMax2) * 0.5f; 
        }

		int proj;
		Projectile Proj => Main.projectile[proj];
		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.Terrapain.SetBonus.GranithShellHelmet");
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ScorspiderSting>()] == 0 && !player.GetModPlayer<PlayerOrganismOverload>().RemovedBuffs.Contains(ModContent.BuffType<ScorspiderStingBuff>()))
			{
				proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<ScorspiderSting>(), Item.damage, Item.knockBack, player.whoAmI);
			}
			else
            {
                try
                {
                    Proj.timeLeft = 2;
                }
				catch {}
            }
			player.GetDamage(DamageClass.Melee) += 0.05f;
			player.AddBuff(ModContent.BuffType<ScorspiderStingBuff>(), 3);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<ScorspiderShellShard>(9);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}