using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class BombsRework : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Dynamite || entity.type == ItemID.StickyDynamite || entity.type == ItemID.Dynamite || entity.type == ItemID.Bomb || entity.type == ItemID.StickyBomb;
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.Dynamite] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.StickyDynamite] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.Bomb] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.StickyBomb] = true;

        }
        public override bool Shoot(Item item, Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.type == ItemID.Dynamite || item.type == ItemID.Bomb || item.type == ItemID.StickyBomb || item.type == ItemID.StickyDynamite)
            {
                velocity *= 3f;
                if (player.altFunctionUse == 2)
                {
                    player.itemTime = 15;
                    player.itemAnimation = 15;
                }
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
                item.stack--;
                return false;
            }
            return true;
        }
        public override bool AltFunctionUse(Item item, Terraria.Player player)
        {
            return item.type == ItemID.Dynamite || item.type == ItemID.Bomb || item.type == ItemID.StickyBomb || item.type == ItemID.StickyDynamite;
        }
    }
}