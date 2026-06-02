using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Items.Ingredients;
using Terrapain.Content.Items.ItemDropRules;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.TGlobalItems
{
    internal class BossBags : GlobalItem
    {
        static int[] bossBags = [ItemID.EyeOfCthulhuBossBag, ItemID.KingSlimeBossBag];
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return bossBags.Contains(entity.type);
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type) {
                case ItemID.EyeOfCthulhuBossBag:
                    TerrapainDropRull.Parameters parameters = Terrapain.DefaultIngredientsDropParameters;
                    parameters.Boss = 1;
                    parameters.MinimumItemDropsCount = 15;
                    parameters.MaximumItemDropsCount = 20;
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<MassiveLensSharp>(), parameters));
                    parameters.VanillaDrop = true;
                    parameters.MinimumItemDropsCount = 25;
                    parameters.MaximumItemDropsCount = 35;
                    itemLoot.Add(new TerrapainDropRull(ItemID.CrimtaneOre, parameters));
                    itemLoot.Add(new TerrapainDropRull(ItemID.DemoniteOre, parameters));
                    parameters.MinimumItemDropsCount = 1;
                    parameters.MaximumItemDropsCount = 3;
                    itemLoot.Add(new TerrapainDropRull(ItemID.CrimsonSeeds, parameters));
                    itemLoot.Add(new TerrapainDropRull(ItemID.CorruptSeeds, parameters));
                    parameters = Terrapain.DefaultRaretiesDropParameters;
                    parameters.ChanceDenominator = 3;
                    itemLoot.Add(new TerrapainDropRull(ItemID.PinkGel, parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.SummonerWeapons.GlassKnife>(), parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.MeleeWeapons.GlassSword>(), parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.RangerWeapons.Reflector>(), parameters));
                    break;
                case ItemID.KingSlimeBossBag:
                    parameters = Terrapain.DefaultIngredientsDropParameters;
                    parameters.Boss = 0;
                    parameters.MinimumItemDropsCount = 15;
                    parameters.MaximumItemDropsCount = 20;
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<SuperDenseGel>(), parameters));
                    parameters.VanillaDrop = true;
                    parameters.MinimumItemDropsCount = 25;
                    parameters.MaximumItemDropsCount = 35;
                    itemLoot.Add(new TerrapainDropRull(ItemID.Gel, parameters));
                    parameters.MinimumItemDropsCount = 15;
                    parameters.MaximumItemDropsCount = 20;
                    parameters = Terrapain.DefaultRaretiesDropParameters;
                    parameters.ChanceDenominator = 3;
                    itemLoot.Add(new TerrapainDropRull(ItemID.PinkGel, parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.MagicWeapons.SlimeStuff>(), parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.MeleeWeapons.FireSword>(), parameters));
                    itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Content.Items.Weapons.RangerWeapons.SlimeBow>(), parameters));
                    break;
            }
        }
    }
}
