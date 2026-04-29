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
    internal class MassiveSwing : GlobalItem
    {
        public float targetRotation;
        public float angleBetween;
        public float startRotation;
        public float finishRotation;
        public float rotation;
        public int hitDir;
        public bool useAgain;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.MassiveSwing;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            if (player.HeldItem.type != ItemID.None)
            {
                MassiveSwing ms = player.HeldItem.GetGlobalItem<MassiveSwing>();
                player.attackCD = 0;
                ms.rotation = 0;
                if (player.itemAnimation == player.itemAnimationMax)
                {
                    for (int i = 0; i < player.HeldItem.GetT().hitList.Length; i++)
                    {
                        player.HeldItem.GetT().hitList[i] = false;
                    }
                    //Chatic("use item");
                    ms.rotation = AngleFromVector(Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player)));
                    if ((player.MountedCenter + TGlobalItem.GetHandOffset(player)).X > Main.MouseWorld.X)
                        player.ChangeDir(-1);
                    else
                        player.ChangeDir(1);
                    ms.targetRotation = -player.direction * MathF.PI / 2 + ms.rotation;
                    //Chatic("startAnimate");
                    if (!ms.useAgain)
                        ms.startRotation = MathF.PI / 2;
                    ms.useAgain = false;
                    Vector2 v1 = ms.startRotation.ToRotationVector2();
                    Vector2 v2 = ms.targetRotation.ToRotationVector2();
                    float btw1 = NormalizeRotation(AngleBetweenVectors(v1, v2));
                    float btw2 = NormalizeRotation(AngleBetweenVectors(v2, v1));
                    //if (btw1 > btw2)
                    //{
                    //    angleBetween = btw2;
                    //    internalAnimationTime = (int)(angleBetween / MathF.PI * player.itemAnimationMax / 4 * 3 + player.itemAnimationMax / 4f);
                    //    internalAnimationTimeMax = internalAnimationTime;
                    //    hitDir = 1;
                    //}
                    //else if (btw2 > btw1)
                    //{
                    //    angleBetween = btw1;
                    //    internalAnimationTime = (int)(angleBetween / MathF.PI * player.itemAnimationMax / 4 * 3 + player.itemAnimationMax / 4f);
                    //    internalAnimationTimeMax = internalAnimationTime;
                    //    hitDir = -1;
                    //}
                    if (v1.Y > v2.Y)
                    {
                        if (/*v2.X > 0 || (v2.X == 0 && */player.direction == 1)
                        {
                            ms.angleBetween = NormalizeRotation(AngleBetweenVectors(v1, v2));
                            player.itemAnimation = (int)(ms.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            ms.hitDir = -1;
                        }
                        else
                        {
                            ms.angleBetween = NormalizeRotation(AngleBetweenVectors(v2, v1));
                            player.itemAnimation = (int)(ms.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            ms.hitDir = 1;
                        }
                    }
                    else
                    {
                        if (/*v2.X > 0 || (v2.X == 0 && */player.direction == 1)
                        {
                            ms.angleBetween = NormalizeRotation(AngleBetweenVectors(v2, v1));
                            player.itemAnimation = (int)(ms.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            ms.hitDir = 1;
                        }
                        else
                        {
                            ms.angleBetween = NormalizeRotation(AngleBetweenVectors(v1, v2));
                            player.itemAnimation = (int)(ms.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            ms.hitDir = -1;
                        }
                    }
                }
                player.itemAnimation -= 1;
                player.itemAnimationMax -= 1;
                if (player.itemAnimation > player.HeldItem.useAnimation / 4)
                {
                    ms.rotation = ms.startRotation + ms.angleBetween * EasingInOut(player.itemAnimationMax - player.HeldItem.useAnimation / 4, player.itemAnimationMax - player.itemAnimation) * ms.hitDir;
                }
                else
                {
                    ms.rotation = ms.targetRotation + MathF.PI * EasingIn(player.HeldItem.useAnimation / 4, player.HeldItem.useAnimation / 4 - player.itemAnimation) * player.direction;
                    if (player.itemAnimation == 0 && Main.mouseLeft)
                    {
                        ms.startRotation = ms.rotation;
                        ms.useAgain = true;
                    }
                }
                player.itemAnimation += 1;
                player.itemAnimationMax += 1;
                Vector2 offset = TGlobalItem.basicOffset;
                Vector2? _offset = item.ModItem?.HoldoutOffset();
                if (_offset.HasValue)
                {
                    offset += _offset.Value;
                }
                offset.Y *= player.direction;
                float basicRotation = MathF.PI / 4;
                if (item.GetT().spriteRotation.HasValue)
                {
                    basicRotation = item.GetT().spriteRotation.Value;
                }
                player.SetItemRotation(ms.rotation + basicRotation * player.direction);
                player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(ms.rotation);
                player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(ms.rotation) - 0.5f * (float)Math.PI * player.direction);
                player.bodyFrame.Y = player.bodyFrame.Height;
            }
        }
        public override void UseItemHitbox(Item item, Terraria.Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            MassiveSwing ms = item.GetGlobalItem<MassiveSwing>();
            //Chatic(player.meleeNPCHitCooldown.Length, player.meleeNPCHitCooldown[0], player.meleeNPCHitCooldown[1], player.meleeNPCHitCooldown[2], player.meleeNPCHitCooldown[3]);
            if (player.itemAnimation > item.useAnimation / 4)
            {
                noHitbox = true;
                return;
            }
            Vector2 offset = Vector2.Zero;
            Vector2? _offset = item.ModItem?.HoldoutOffset();
            if (_offset.HasValue)
            {
                offset = _offset.Value.RotatedBy(ms.rotation * player.direction);
                offset.X *= player.direction;
            }
            float basicRotation = MathF.PI / 4;
            if (item.GetT().spriteRotation.HasValue)
            {
                basicRotation = item.GetT().spriteRotation.Value;
            }
            Vector2 position = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset + TGlobalItem.basicOffset.RotatedBy(ms.rotation);
            float lenth = GetLengthInHitBox(hitbox, basicRotation);
            Vector2 end = position + ms.rotation.ToRotationVector2() * lenth;
            hitbox = GetRectangle(position, end);
        }
        public override void OnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.attackCD = 0;
        }
        public override bool? CanHitNPC(Item item, Terraria.Player player, NPC target)
        {
            return item.GetT().hitList[target.whoAmI]? false : null;
        }
    }
}
