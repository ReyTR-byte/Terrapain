using Microsoft.Xna.Framework;
using Terrapain.Content;
using Terrapain.Content.Items.Weapons.MagicWeapons;
using Terrapain.Content.Items.Weapons.MeleeWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.UseStyles{
    public class ShootRework : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == ItemUseStyleID.Shoot && entity.type != ItemID.DiamondStaff;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            player.attackCD = 0;
            float rotation = (Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))).ToRotation();
            Vector2 offset = TGlobalItem.basicOffset + (item.ModItem?.HoldoutOffset()?? Vector2.Zero);
            offset.Y *= player.direction;
            float basicRotation = item.GetT().spriteRotation?? 0;
            player.SetItemRotation(rotation + basicRotation * player.direction);
            player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
            player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
            player.bodyFrame.Y = player.bodyFrame.Height;
        }
    }
}