using Microsoft.Xna.Framework;
using Terrapain.Common.Player;
using Terrapain.Content.Buffs;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AcidCobwebHood : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

            // If your head equipment should draw hair while drawn, use one of the following:
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 26; // Width of the item
            Item.height = 24; // Height of the item
            Item.value = Item.sellPrice(gold: 3); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 6; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            if (body.type == ModContent.ItemType<AcidCobwebChestplate>() && legs.type == ModContent.ItemType<AcidCobwebLeggings>())
            {
                return true;
            }
            return false;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) *= 1.075f;
        }
        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Terrapain.SetBonus.AcidCobwebHood");
            player.GetModPlayer<TerrapainPlayer>().AcidCobwebBonus = true;
            player.manaCost *= 0.95f;
            player.manaRegen += 3;
            player.statManaMax2 += 60;
            player.GetDamage(DamageClass.Magic) *= 1.05f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<ScorspiderCobweb>(11);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}