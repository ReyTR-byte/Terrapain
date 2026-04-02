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
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.EyeOfCthulhuBossBag;
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


                    //parameters = Terrapain.DefaultRaretiesDropParameters;
                    //parameters.ChanceDenominator = 3;
                    //parameters.Boss = 1;
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Tools.ScorspiderHook>(), parameters));
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Accessories.ScorspiderHeartAccesory>(), parameters));
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.MagicWeapons.GranithBook>(), parameters));
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.MeleeWeapons.Sharper>(), parameters));
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.RangerWeapons.PizdetsKrutayaPushka>(), parameters));
                    //itemLoot.Add(new TerrapainDropRull(ModContent.ItemType<Weapons.SummonerWeapons.GranithStuff>(), parameters));
                    break;
            }
        }
    }
}
