using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.UseStyles
{
    public class LaserUseStyle : GlobalItem
    {
        float rotationSpeed;
        float angularAxeleration = 0.02f;
        float rotation;
        float MaxRotationSpeed = 0.1f;
        bool New = true;
        bool Old = true;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.LaserUseStyle || entity.type == ItemID.DiamondStaff;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            LaserUseStyle ls = player.HeldItem.GetGlobalItem<LaserUseStyle>();
            if ((player.MountedCenter + TGlobalItem.GetHandOffset(player)).X > Main.MouseWorld.X)
                player.ChangeDir(-1);
            else
                player.ChangeDir(1);
            float rotation;
            float rotationSpeed = ls.rotationSpeed;
            float targetRotation = (Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))).ToRotation();
            if (ls.New)
            {
                rotation = (Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))).ToRotation();
                rotationSpeed = 0; 
            }
            else
            {
                rotation = ls.rotation;
            }
            float positiveRotation = targetRotation - rotation;
            positiveRotation = Functions.NormalizeRotation(positiveRotation);
            float negativeRotation = rotation - targetRotation;
            negativeRotation = Functions.NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                rotationSpeed -= angularAxeleration;
                rotationSpeed = MathF.Min(rotationSpeed, MaxRotationSpeed * MathF.Max(1, negativeRotation * 2f));
                if (negativeRotation < -rotationSpeed)
                {
                    rotation = targetRotation;
                    rotationSpeed = 0;
                }
            }
            else
            {
                rotationSpeed += angularAxeleration;
                rotationSpeed = MathF.Min(rotationSpeed, MaxRotationSpeed * MathF.Max(1, positiveRotation * 2f));
                if (positiveRotation < rotationSpeed)
                {
                    rotation = targetRotation;
                    rotationSpeed = 0;
                }
            }
            rotation += rotationSpeed;
            Vector2 offset = TGlobalItem.basicOffset + (item.ModItem?.HoldoutOffset() ?? Vector2.Zero);
            offset.Y *= player.direction;
            float basicRotation = player.HeldItem.GetT().spriteRotation ?? 0;
            player.SetItemRotation(rotation + basicRotation * player.direction);
            player.itemLocation = player.MountedCenter.GetInt() + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
            player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
            player.bodyFrame.Y = player.bodyFrame.Height;
            player.HeldItem.GetGlobalItem<LaserUseStyle>().rotation = rotation;
            player.HeldItem.GetGlobalItem<LaserUseStyle>().rotationSpeed = rotationSpeed;
            player.HeldItem.GetGlobalItem<LaserUseStyle>().New = false;
        }
        public override void UpdateInventory(Item item, Terraria.Player player)
        {
            if (!player.ItemAnimationActive)
            {
                New = true;
            }
        }
    }
}
