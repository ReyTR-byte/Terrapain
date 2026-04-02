using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terrapain.Content;
using Terrapain.Content.Items.Weapons.MagicWeapons;
using Terrapain.Content.Items.Weapons.MeleeWeapons;

namespace Terrapain.Common.Global{
    public class AnimationsRework : GlobalItem
    {
        public override void UseAnimation(Item item, Terraria.Player player)
        {
            if (item.useStyle == ItemUseStyleID.Swing)
            {
                player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.MountedCenter.X));
            }
        }
        public override void UseItemFrame(Item item, Terraria.Player player)
        {
            if (item.useStyle == ItemUseStyleID.Shoot)
            {
                if (item.type != ModContent.ItemType<TheForestRipper>())
                {
                    float rotation = Convert.ToSingle(Math.Acos((Main.MouseWorld.X - player.MountedCenter.X) / (Main.MouseWorld - player.MountedCenter).Length()));
                    if ((Main.MouseWorld - player.MountedCenter).Y < 0)
                        rotation = 2 * MathF.PI - rotation;

                    if (Math.Abs(player.MountedCenter.X - Main.MouseWorld.X) > 6)
                    {
                        if (player.MountedCenter.X > Main.MouseWorld.X)
                            player.ChangeDir(-1);
                        else
                            player.ChangeDir(1);
                    }

                    player.itemRotation = MathHelper.WrapAngle(player.direction == 1 ? rotation : (float)Math.PI - rotation) * player.direction;
                    if (item.GetGlobalItem<TGlobalItem>().ShootRotation != 0)
                    {
                        player.itemLocation = player.MountedCenter + new Vector2(Functions.UnitVectorFromRotation(player.itemRotation).Y, -Functions.UnitVectorFromRotation(player.itemRotation).X) * 20 + new Vector2(6 * (float)Math.Sin(player.itemRotation) + (player.direction == 1 ? -4 * ((float)Math.Sin(player.itemRotation) + 1) : 4 * (1 - (float)Math.Sin(player.itemRotation))), -player.height / 2);
                        player.itemRotation += item.GetGlobalItem<TGlobalItem>().ShootRotation * player.direction;

                        //if (player.DirectionTo(Main.MouseWorld).Y > 1f / 6 * (float)Math.PI)
                        //{
                        //    player.bodyFrame.Y = 224;
                        //}
                        //else if (player.DirectionTo(Main.MouseWorld).Y > -1f / 6 * (float)Math.PI)
                        //{
                        //    player.bodyFrame.Y = 168;
                        //}
                        //else
                        //{
                        //    player.bodyFrame.Y = 112;
                        //}
                    }
                    if (item.type >= ItemID.Count && item.ModItem.HoldoutOrigin().HasValue)
                    {
                        Vector2 origin = item.ModItem.HoldoutOrigin().Value;
                        origin.Y *= player.direction;
                        player.itemLocation += origin.RotatedBy(player.itemRotation) * player.direction;
                    }
                }
                player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-90 * player.direction) - item.GetGlobalItem<TGlobalItem>().ShootRotation * player.direction);
            }
            if (item.useStyle == ItemUseStyleID.Swing)
            {
                if (item.DamageType != DamageClass.SummonMeleeSpeed)
                { 
                    player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-135 * player.direction)); 
                }
            }
        }
    }
}