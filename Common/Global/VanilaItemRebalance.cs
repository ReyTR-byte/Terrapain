using Terrapain.Common.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class VanillaItemRebalance : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ItemID.FlinxFurCoat)
            {
                entity.defense = 3;
            }
        }
        public override void UpdateAccessory(Item item, Terraria.Player player, bool hideVisual)
        {
            if (item.type == ItemID.TerrasparkBoots && WorldDifficultySystem.TerrapainDifficulty > 0)
            {
                player.accRunSpeed += 0.2f;
                player.runAcceleration += 0.2f;
                player.runSlowdown += 0.4f;
            }
        }
    }
}