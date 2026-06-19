using Luminance.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global.UseStyles
{
    public class BowsOverride : GlobalItem
    {
        public int bowTime;
        public int projectile;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.BowOverride;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            if (player.itemAnimation == 1 || player.itemAnimation == 0)
            {
                return;
            }
            player.itemTime = player.itemAnimationMax - 1;
            if (player.itemAnimation == player.itemAnimationMax)
            {
                player.HeldItem.GetGlobalItem<BowsOverride>().bowTime = 0;
                player.itemTime = 0;
            }
            player.itemAnimation = player.itemAnimationMax - 1;
            float rotation = (Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))).ToRotation();
            player.ChangeDir((Main.MouseWorld - player.MountedCenter).X.NonZeroSign());
            Vector2 refOffset = Vector2.Zero;
            ItemLoader.HoldoutOrigin(player, ref refOffset);
            refOffset.X *= player.direction;
            refOffset.Y *= player.gravDir;
            refOffset.Y += TextureAssets.Item[item.type].Value.Height / 2f / (Main.itemAnimations[item.type]?.FrameCount ?? 1);
            Vector2 offset = TGlobalItem.basicOffset + refOffset * item.scale;
            offset.Y *= player.direction;
            float basicRotation = item.GetT().spriteRotation ?? 0;
            player.SetItemRotation(rotation + basicRotation * player.direction);
            player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
            player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
            player.bodyFrame.Y = player.bodyFrame.Height;
            player.HeldItem.GetGlobalItem<BowsOverride>().bowTime++;
            if (!player.controlUseItem)
            {
                player.itemTime = 0;
                player.itemAnimation = 1;
            }
        }
        public override void ModifyShootStats(Item item, Terraria.Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float multiply = (float)bowTime / item.useAnimation;
            velocity *= MathF.Min(multiply, 1.5f);
            damage = (int)(damage * MathF.Min(multiply, 5f));
            knockback *= MathF.Min(multiply, 3f);
            projectile = type;
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Terraria.Player player)
        {
            if(bowTime == 1)
            {
                return false;
            }
            return true;
        }
        UnifiedRandom random = new();
        public override bool Shoot(Item item, Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (bowTime >= item.useAnimation * 5)
            {
                switch (item.type)
                {
                    case ItemID.Tsunami:
                        Projectile.NewProjectile(source, position, velocity / 2, ProjectileID.Typhoon, (int)(damage * 0.8f), knockback, player.whoAmI);
                        break;
                    case ItemID.BeesKnees:
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(source, position, velocity / 2 + new Vector2(random.Next(-5, 6), random.Next(-5, 6)), ProjectileID.Bee, damage / 4, knockback, player.whoAmI);
                        }
                        break;
                }
            }
            
            if (bowTime == 1)
            {
                return false;
            }
            return true;
        }
    }
}
