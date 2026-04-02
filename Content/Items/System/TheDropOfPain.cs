using Terrapain.Common.System;
using Terrapain.Content.NPCs;
using Terrapain.Content.NPCs.Bosses;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.System
{
    public class TheDropOfPain : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips[1].Text = NetworkText.FromKey("Mods.Terrapain.NetworkText.TheDropOfPainTooltip" + WorldDifficultySystem.TerrapainDifficulty.ToString()).ToString();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(ModContent.NPCType<Torture>()) && !NPC.AnyNPCs(ModContent.NPCType<Suicide>()))
                {
                    int torture = NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<Torture>(), 0, player.whoAmI);
                    int suicide = NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<Suicide>(), 0, player.whoAmI, torture);
                    Main.npc[torture].ai[1] = suicide;
                    return true;
                }
                else
                {
                    foreach (var npc in Main.npc)
                    {
                        if ((npc.type == ModContent.NPCType<Torture>() || npc.type == ModContent.NPCType<Suicide>()) && npc.ai[2] == 1)
                        {
                            npc.life = 0;
                        }
                    }
                }
            }
            return true;
        }
    }
}