using Luminance.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global.UseStyles
{
    public class SharperUseStyle : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.SharperUseStyle;
        }
        public override bool InstancePerEntity => true;
        int timer;
        public int hitTimer;
        int swingDir;
        bool resetTimer;
        public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
        {
            bool swing = false;
            if (player.itemAnimation == player.itemAnimationMax && Main.mouseLeft)
            {
                if (resetTimer)
                {
                    timer = 0;
                    resetTimer = false;
                }
                if (player.velocity == Vector2.Zero)
                    player.itemRotation = Functions.AngleFromVector(Vector2.UnitX * -1);
                else
                    player.itemRotation = (-player.velocity * player.direction).ToRotation();

                if (player.velocity.X != 0)
                    player.ChangeDir(player.velocity.X.NonZeroSign());
                //player.itemTime = player.itemAnimationMax + 1;
                player.itemAnimation = player.itemAnimationMax + 1;
                timer++;
            }
            else
            {
                swing = true;
                if (player.itemAnimation == player.itemAnimationMax)
                {
                    player.HeldItem.GetGlobalItem<SharperUseStyle>().hitTimer = timer;
                    swingDir = 1;
                    if (player.velocity.Length() != 0)
                    {
                        swingDir = (player.velocity.Y / player.velocity.Length() + 0.8f).NonZeroSign();
                    }
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                }
                player.itemRotation += MathF.PI / player.itemAnimationMax * player.direction * swingDir;
                resetTimer = true;
            }
            Vector2 offset = item.ModItem.HoldoutOffset().Value.RotatedBy(player.itemRotation * player.direction);
            offset.X *= player.direction;
            player.itemLocation = player.MountedCenter.GetInt() + TGlobalItem.GetHandOffset(player) + offset;
            player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.itemRotation - 0.5f * (float)Math.PI * player.direction);
            player.bodyFrame.Y = player.bodyFrame.Height;

            UnifiedRandom random = new UnifiedRandom();

            if (item.GetT().dust != -1 && random.Next(2) == 0 && timer >= 100)
            {
                bool oldNormalHitbox = normalHitbox;
                Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
                bool noHitbox = false;
                normalHitbox = true;
                UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
                normalHitbox = oldNormalHitbox;
                Vector2 aditiveVelocity = Vector2.Zero;
                if (swing)
                {
                    Vector2 center = miniHitbox.Location.ToVector2() + miniHitbox.Size() / 2;
                    Vector2 aboutPlayer = center - player.MountedCenter - TGlobalItem.GetHandOffset(player);
                    aditiveVelocity = aboutPlayer.RotatedBy((float)Math.PI / player.itemAnimationMax * player.direction * swingDir) - aboutPlayer;
                }
                int d = Dust.NewDust(miniHitbox.Location.ToVector2(), miniHitbox.Width, miniHitbox.Height, item.GetT().dust, player.velocity.X + aditiveVelocity.X, player.velocity.Y + aditiveVelocity.Y);
                TGlobalDust.dustLights[d] = item.GetT().dustLight;
            }
        }
        List<int> dusts = new List<int>();
        int _Dust = -1;
        bool normalHitbox = true;
        public override bool? CanHitNPC(Item item, Terraria.Player player, NPC target)
        {
            Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
            bool noHitbox = false;
            normalHitbox = true;
            UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
            normalHitbox = false;
            if (noHitbox)
            {
                return false;
            }
            if (Functions.RectangleColision(target.Hitbox, miniHitbox))
            {
                return null;
            }
            Vector2 rotatin = Functions.UnitVectorFromRotation(player.itemRotation) * player.direction;
            if (Functions.Collision(player.MountedCenter + TGlobalItem.GetHandOffset(player), player.MountedCenter + TGlobalItem.GetHandOffset(player) + rotatin * item.ModItem.HoldoutOffset().Value.X, item.width, target.position, target.width, target.height))
            {
                return null;
            }
            return false;
        }
        public override bool CanHitPvp(Item item, Terraria.Player player, Terraria.Player target)
        {
            Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
            bool noHitbox = false;
            normalHitbox = true;
            UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
            normalHitbox = false;
            if (noHitbox)
            {
                return false;
            }
            if (Functions.RectangleColision(target.Hitbox, miniHitbox))
            {
                return true;
            }
            Vector2 rotatin = Functions.UnitVectorFromRotation(player.itemRotation) * player.direction;
            if (Functions.Collision(player.MountedCenter + TGlobalItem.GetHandOffset(player), player.MountedCenter + TGlobalItem.GetHandOffset(player) + rotatin * item.ModItem.HoldoutOffset().Value.X, item.width, target.position, target.width, target.height))
            {
                return true;
            }
            return false;
        }
        public override void ModifyHitNPC(Item item, Terraria.Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item.GetGlobalItem<SharperUseStyle>().hitTimer < item.useTime * 10)
            {
                //Functions.Chatic(item.GetGlobalItem<TGlobalItem>().hitTimer, item.useTime * 10, item.useTime);
                modifiers.TargetDamageMultiplier *= 0.5f;
                return;
            }
            modifiers.TargetDamageMultiplier *= player.velocity.Length() * 2f + 1;
        }
        public override void ModifyHitPvp(Item item, Terraria.Player player, Terraria.Player target, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (item.GetGlobalItem<SharperUseStyle>().hitTimer < item.useTime * 10)
            {
                modifiers.FinalDamage *= 0.5f;
                return;
            }
            modifiers.FinalDamage *= player.velocity.Length() * 2f + 1;
        }
        public override void UseItemHitbox(Item item, Terraria.Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (player.itemAnimation == player.itemAnimationMax + 1 || player.itemAnimation == player.itemAnimationMax)
            {
                noHitbox = true;
            }
            if (normalHitbox)
            {
                Vector2 location = player.itemLocation;
                Vector2 offset = new Vector2(hitbox.Width - hitbox.Height / 2, -hitbox.Height / 2 * player.direction);
                location += offset.RotatedBy(player.itemRotation) * player.direction;
                hitbox.Width = hitbox.Height;
                hitbox.Location = (location - hitbox.Size() / 2).ToPoint();
            }
            else
            {
                Vector2 location = player.itemLocation;
                Vector2 offset = new Vector2(hitbox.Width - hitbox.Height / 2, -hitbox.Height / 2 * player.direction);
                location += offset.RotatedBy(player.itemRotation) * player.direction;
                hitbox.Width = hitbox.Height;
                hitbox.Location = (location - hitbox.Size() / 2).ToPoint();
                if (hitbox.X > player.MountedCenter.X + TGlobalItem.GetHandOffset(player).X)
                {
                    hitbox.Width = hitbox.X + hitbox.Width - (int)(player.MountedCenter.X + TGlobalItem.GetHandOffset(player).X);
                    hitbox.X = (int)(player.MountedCenter.X + TGlobalItem.GetHandOffset(player).X);
                }
                if (hitbox.X + hitbox.Width < player.MountedCenter.X)
                {
                    hitbox.Width = (int)(player.MountedCenter.X + TGlobalItem.GetHandOffset(player).X) - hitbox.X;
                }
                if (hitbox.Y > player.MountedCenter.Y + TGlobalItem.GetHandOffset(player).Y)
                {
                    hitbox.Height = hitbox.X + hitbox.Height - (int)(player.MountedCenter.Y + TGlobalItem.GetHandOffset(player).Y);
                    hitbox.Y = (int)(player.MountedCenter.Y + TGlobalItem.GetHandOffset(player).Y);
                }
                if (hitbox.Y + hitbox.Height < player.MountedCenter.Y)
                {
                    hitbox.Height = (int)(player.MountedCenter.Y + TGlobalItem.GetHandOffset(player).X) - hitbox.Y;
                }
                normalHitbox = true;
            }
        }
    }
}
