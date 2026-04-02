using Luminance.Common.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization.Formatters;
using Terrapain.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.UseStyles
{
    internal class NormalSwing : GlobalItem
    {
        public float rotation;
        public int hitDir;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.NormalSwing;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            if (player.HeldItem.type != ItemID.None)
            {
                NormalSwing ns = player.HeldItem.GetGlobalItem<NormalSwing>();
                player.attackCD = 0;
                rotation = 0;
                if (player.itemAnimation == player.itemAnimationMax)
                {
                    for (int i = 0; i < player.HeldItem.GetT().hitList.Length; i++)
                    {
                        player.HeldItem.GetT().hitList[i] = false;
                    }
                    if ((player.MountedCenter + TGlobalItem.GetHandOffset(player)).X > Main.MouseWorld.X)
                        player.ChangeDir(-1);
                    else
                        player.ChangeDir(1);
                    ns.hitDir *= -1;
                    if (ns.hitDir == 0)
                    {
                        ns.hitDir = player.direction;
                    }
                }
                rotation = AngleFromVector(Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player))) - MathF.PI / 2 * ns.hitDir;
                rotation += EasingIn(player.itemAnimationMax, player.itemAnimationMax - player.itemAnimation) * MathF.PI * ns.hitDir;
                Vector2 offset = TGlobalItem.basicOffset;
                Vector2? _offset = item.ModItem?.HoldoutOffset();
                if (_offset.HasValue)
                {
                    offset += _offset.Value;
                }
                offset.Y *= player.direction;
                float basicRotation = MathF.PI / 4;
                if (item.GetT().basicRotation.HasValue)
                {
                    basicRotation = item.GetT().basicRotation.Value;
                }
                player.HeldItem.GetT().drawDir = ns.hitDir * player.direction;
                player.SetItemRotation(rotation + basicRotation * player.direction);
                player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
                player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
                player.bodyFrame.Y = player.bodyFrame.Height;
                player.HeldItem.GetGlobalItem<NormalSwing>().rotation = rotation;
            }
        }
        public override void UseItemHitbox(Item item, Terraria.Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            NormalSwing ns = item.GetGlobalItem <NormalSwing>();
            Vector2 offset = Vector2.Zero;
            Vector2? _offset = item.ModItem?.HoldoutOffset();
            if (_offset.HasValue)
            {
                offset = _offset.Value.RotatedBy(rotation * player.direction);
                offset.X *= player.direction;
            }
            float basicRotation = MathF.PI / 4;
            if (item.GetT().basicRotation.HasValue)
            {
                basicRotation = item.GetT().basicRotation.Value;
            }
            Vector2 position = player.MountedCenter + TGlobalItem.GetHandOffset(player) + /*offset +*/ TGlobalItem.basicOffset.RotatedBy(rotation);
            float lenth = GetLengthInHitBox(hitbox, basicRotation);
            Vector2 end = position + ns.rotation.ToRotationVector2() * lenth;
            hitbox = GetRectangle(position, end);
        }
        public override void OnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.attackCD = 0;
        }
        public override bool? CanHitNPC(Item item, Terraria.Player player, NPC target)
        {
            return item.GetT().hitList[target.whoAmI] ? false : null;
        }
    }
}

