using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Content;
using Terrapain.Content.Items.Weapons.MagicWeapons;
using Terrapain.Content.Items.Weapons.MeleeWeapons;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.UseStyles{
    public class ShootRework : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            entity.useStyle = TGlobalItem.ShootOverride;
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == ItemUseStyleID.Shoot && entity.type != ItemID.DiamondStaff;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            float rotation = (Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))).ToRotation();
            player.ChangeDir((Main.MouseWorld - player.MountedCenter).X.NonZeroSign());
            Vector2 refOffset = Vector2.Zero;
            ItemLoader.HoldoutOrigin(player, ref refOffset);
            refOffset.X *= player.direction;
            refOffset.Y *= player.gravDir;
            refOffset.Y += TextureAssets.Item[item.type].Value.Height / 2f / (Main.itemAnimations[item.type]?.FrameCount?? 1);
            Vector2 offset = TGlobalItem.basicOffset + refOffset * item.scale;
            offset.Y *= player.direction;
            float basicRotation = item.GetT().spriteRotation?? 0;
            player.SetItemRotation(rotation + basicRotation * player.direction);
            player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
            player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
            player.bodyFrame.Y = player.bodyFrame.Height;
        }
    }
}