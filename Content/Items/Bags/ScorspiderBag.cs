using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terrapain.Common.System;
using Terrapain.Content.Items.ItemDropRules;
using Terraria.IO;

namespace Terrapain.Content.Items.Bags
{
    public class ScorspiderBag : ModItem
    {
        public override void SetStaticDefaults() {

			ItemID.Sets.BossBag[Type] = true; // This set is one that every boss bag should have, it, for example, lets our boss bag drop dev armor..
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true; // ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags.

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			Item item = new Item(ItemID.KingSlimeBossBag);

			TooltipLine ttl = new TooltipLine(Mod, "Tooltip0", item.ToolTip.GetLine(0));
			for (int i = 0; i < tooltips.Count; i++)
			{
				if (tooltips[i].Name == "Tooltip0")
				{
					tooltips[i] = ttl;
				}
			}

            item.active = false;
        }
		public override void SetDefaults() {
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) 
		{
			TerrapainDropRull.Parameters parameters = Terrapain.DefaultIngredientsDropParameters;
			parameters.Boss = 2;
			parameters.MinimumItemDropsCount = 15;
			parameters.MaximumItemDropsCount = 20;
			itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Ingredients.ScorspiderShellShard>(), parameters));
			itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Ingredients.ScorspiderCobweb>(), parameters));

			parameters = Terrapain.DefaultRaretiesDropParameters;
			parameters.ChanceDenominator = 3;
			parameters.Boss = 2;
			itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Tools.ScorspiderHook>(), parameters));
			itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Accessories.ScorspiderHeartAccesory>(), parameters));
			itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.MagicWeapons.GranithBook>(), parameters));
            itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.MeleeWeapons.Sharper>(), parameters));
            itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.RangerWeapons.PizdetsKrutayaPushka>(), parameters));
            itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.SummonerWeapons.GranithStuff>(), parameters));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<ScorspiderBody>()));
		}

		// Below is code for the visuals

		public override Color? GetAlpha(Color lightColor) {
			// Makes sure the dropped bag is always visible
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}

		public override void PostUpdate() {
			// Spawn some light and dust when dropped in the world
			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

			if (Item.timeSinceItemSpawned % 12 == 0) {
				Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

				// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
				Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
				float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
				Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

				Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
				dust.scale = 0.5f;
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
				dust.noLight = true;
				dust.alpha = 0;
			}
		}
        public override void OnConsumeItem(Player player)
        {
			if (BossDownedSystem.bossBagsSuicide[2] > 0)
            {
                BossDownedSystem.bossBagsSuicide[2]--;
            }
			else if (BossDownedSystem.bossBagsTorture[2] > 0)
            {
                BossDownedSystem.bossBagsTorture[2]--;
            }
        }

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            if (BossDownedSystem.bossBagsSuicide[2] > 0)
            {
				Lighting.AddLight(Item.Center, 3, 0, 0);
			}
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[Item.type].Value;

			Rectangle frame;

			if (Main.itemAnimations[Item.type] != null) {
				// In case this item is animated, this picks the correct frame
				frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else {
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f) {
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Color color = new Color(90, 70, 255, 50);
                if (BossDownedSystem.bossBagsSuicide[2] > 0)
                {
					color.B = color.R;
					color.R = 255;
                    color.G = (byte)(color.G / 15f);
                    color.B = (byte)(color.B / 15f);
                }

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame,  color, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f) 
			{
				float radians = (i + timer) * MathHelper.TwoPi;
                Color color = new Color(140, 120, 255, 77);
                if (BossDownedSystem.bossBagsSuicide[2] > 0)
                {
                    color.B = color.R;
                    color.R = 255;
					color.G = (byte)(color.G / 15f);
                    color.B = (byte)(color.B / 15f);
                }

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, color, rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
    }
}