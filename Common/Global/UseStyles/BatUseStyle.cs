using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.UseStyles
{
    public class BatUseStyle : GlobalItem
    {
        public float targetRotation;
        public float angleBetween;
        public float startRotation;
        public float finishRotation;
        public float rotation;
        public float scale;
        public int hitDir;
        public bool useAgain;
        public bool modifiedKnockBack;
        public float defaultScale;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.BatUseStyle;
        }
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            if (player.HeldItem.type != ItemID.None)
            {
                BatUseStyle bus = player.HeldItem.GetGlobalItem<BatUseStyle>();
                player.attackCD = 0;
                bus.rotation = 0;
                if (player.itemAnimation == player.itemAnimationMax)
                {
                    bus.modifiedKnockBack = true;
                    bus.defaultScale = item.scale;
                    bus.scale = 1;
                    for (int i = 0; i < player.HeldItem.GetT().hitList.Length; i++)
                    {
                        player.HeldItem.GetT().hitList[i] = false;
                    }
                    bus.rotation = AngleFromVector(Main.MouseWorld - (player.MountedCenter + TGlobalItem.GetHandOffset(player)));
                    if ((player.MountedCenter + TGlobalItem.GetHandOffset(player)).X > Main.MouseWorld.X)
                        player.ChangeDir(-1);
                    else
                        player.ChangeDir(1);
                    bus.targetRotation = -player.direction * MathF.PI / 2 + bus.rotation;
                    if (!bus.useAgain)
                        bus.startRotation = MathF.PI / 2;
                    bus.useAgain = false;
                    Vector2 v1 = bus.startRotation.ToRotationVector2();
                    Vector2 v2 = bus.targetRotation.ToRotationVector2();
                    float btw1 = NormalizeRotation(AngleBetweenVectors(v1, v2));
                    float btw2 = NormalizeRotation(AngleBetweenVectors(v2, v1));
                    if (v1.Y > v2.Y)
                    {
                        if (player.direction == 1)
                        {
                            bus.angleBetween = NormalizeRotation(AngleBetweenVectors(v1, v2));
                            player.itemAnimation = (int)(bus.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            bus.hitDir = -1;
                        }
                        else
                        {
                            bus.angleBetween = NormalizeRotation(AngleBetweenVectors(v2, v1));
                            player.itemAnimation = (int)(bus.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            bus.hitDir = 1;
                        }
                    }
                    else
                    {
                        if (player.direction == 1)
                        {
                            bus.angleBetween = NormalizeRotation(AngleBetweenVectors(v2, v1));
                            player.itemAnimation = (int)(bus.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            bus.hitDir = 1;
                        }
                        else
                        {
                            bus.angleBetween = NormalizeRotation(AngleBetweenVectors(v1, v2));
                            player.itemAnimation = (int)(bus.angleBetween / MathF.PI * player.HeldItem.useAnimation / 4 * 3 + player.HeldItem.useAnimation / 4f) + 1;
                            player.itemAnimationMax = player.itemAnimation;
                            bus.hitDir = -1;
                        }
                    }
                }
                player.itemAnimation -= 1;
                player.itemAnimationMax -= 1;
                if (player.itemAnimation > player.HeldItem.useAnimation / 4)
                {
                    bus.rotation = bus.startRotation + bus.angleBetween * EasingInOut(player.itemAnimationMax - player.HeldItem.useAnimation / 4, player.itemAnimationMax - player.itemAnimation) * bus.hitDir;
                }
                else
                {
                    bus.rotation = bus.targetRotation + MathF.PI * EasingIn(player.HeldItem.useAnimation / 4, player.HeldItem.useAnimation / 4 - player.itemAnimation) * player.direction;
                    if (player.itemAnimation == player.HeldItem.useAnimation / 4 && Main.mouseRight)
                    {
                        bus.scale += 0.05f;
                        if (bus.scale >= 1.5f)
                        {
                            bus.scale = 1.5f;
                        }
                        player.itemAnimation += 1;
                    }
                    item.scale = bus.scale * bus.defaultScale;
                    if (player.itemAnimation == 0 && (Main.mouseLeft || Main.mouseRight))
                    {
                        bus.startRotation = bus.rotation;
                        bus.useAgain = true;
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
                player.SetItemRotation(bus.rotation + basicRotation * player.direction);
                player.itemLocation = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(bus.rotation);
                player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.ToItemRotation(bus.rotation) - 0.5f * (float)Math.PI * player.direction);
                player.bodyFrame.Y = player.bodyFrame.Height;
            }
        }
        public override void UseItemHitbox(Item item, Terraria.Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            BatUseStyle bus = item.GetGlobalItem<BatUseStyle>();
            if (player.itemAnimation >= item.useAnimation / 4)
            {
                noHitbox = true;
                return;
            }
            Vector2 offset = Vector2.Zero;
            Vector2? _offset = item.ModItem?.HoldoutOffset();
            if (_offset.HasValue)
            {
                offset = _offset.Value.RotatedBy(bus.rotation * player.direction);
                offset.X *= player.direction;
            }
            float basicRotation = MathF.PI / 4;
            if (item.GetT().spriteRotation.HasValue)
            {
                basicRotation = item.GetT().spriteRotation.Value;
            }
            Vector2 position = player.MountedCenter + TGlobalItem.GetHandOffset(player) + offset + TGlobalItem.basicOffset.RotatedBy(bus.rotation);
            float lenth = GetLengthInHitBox(hitbox, basicRotation) * bus.scale;
            Vector2 end = position + bus.rotation.ToRotationVector2() * lenth;
            hitbox = GetRectangle(position, end);
        }
        public override void OnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HeldItem.GetGlobalItem<BatUseStyle>().modifiedKnockBack && player.HeldItem.GetGlobalItem<BatUseStyle>().scale >= 1.5f && target.knockBackResist != 0)
            {
                player.HeldItem.GetGlobalItem<BatUseStyle>().modifiedKnockBack = false;
                target.velocity += target.DirectionFrom(player.MountedCenter) * item.knockBack * 2f;
                target.GetT().fallThroughtPlatforms = true;
                target.GetT().oldFallThroughtPlatforms = true;
                Projectile.NewProjectile(item.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<BatProjectile>(), item.damage, item.knockBack, player.whoAmI, target.whoAmI);
            }
            player.attackCD = 0;
        }
        public override void OnHitPvp(Item item, Terraria.Player player, Terraria.Player target, Terraria.Player.HurtInfo hurtInfo)
        {
            if (player.HeldItem.GetGlobalItem<BatUseStyle>().modifiedKnockBack && player.HeldItem.GetGlobalItem<BatUseStyle>().scale >= 1.5f)
            {
                modifiedKnockBack = false;
                target.velocity += target.DirectionFrom(player.MountedCenter) * item.knockBack * 1.5f;
                Projectile.NewProjectile(item.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<BatProjectile>(), item.damage, item.knockBack, player.whoAmI, -target.whoAmI);
            }
        }
        public override void ModifyHitNPC(Item item, Terraria.Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.HeldItem.GetGlobalItem<BatUseStyle>().modifiedKnockBack && player.HeldItem.GetGlobalItem<BatUseStyle>().scale >= 1.5f)
            {
                modifiers.DisableKnockback();
            }
            else
            {
                modifiers.Knockback *= player.HeldItem.GetGlobalItem<BatUseStyle>().scale;
            }
        }
        public override void ModifyHitPvp(Item item, Terraria.Player player, Terraria.Player target, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (player.HeldItem.GetGlobalItem<BatUseStyle>().modifiedKnockBack && player.HeldItem.GetGlobalItem<BatUseStyle>().scale >= 1.5f)
            {
                modifiers.Knockback *= 0;
            }
            else
            {
                modifiers.Knockback *= player.HeldItem.GetGlobalItem<BatUseStyle>().scale;
            }
        }
        public override bool? CanHitNPC(Item item, Terraria.Player player, NPC target)
        {
            return item.GetT().hitList[target.whoAmI] ? false : null;
        }
    }
}
