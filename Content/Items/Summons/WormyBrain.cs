using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Summons
{
    public class WormyBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 36;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Roar;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(NPCID.BrainofCthulhu);
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.BrainofCthulhu);
            return true;
        }
    }
}
