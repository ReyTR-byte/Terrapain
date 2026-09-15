using Luminance.Common.Utilities;
using Terrapain.Content;
using Terrapain.Content.Projectiles.Ammo.Arrows;
using Terraria;
using Terraria.Audio;
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
        public float damageMultiply = 5;
        public float speedMultiply = 1.5f;
        public float knockbackMultiply = 3f;
        public float fullCharge = 5;
        public int projectile;
        public SoundStyle? sound;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.useStyle == TGlobalItem.BowOverride;
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.ModItem != null && entity.ModItem is IBowUseStyle)
            {
                (entity.ModItem as IBowUseStyle).SetBowDeffaults(this);
            }
            sound = entity.UseSound;
            entity.UseSound = null;
            fullCharge = MathF.Max(MathF.Max(damageMultiply, knockbackMultiply), speedMultiply);
        }
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if(player.HeldItem.TryGetGlobalItem<BowsOverride>(out var bow))
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
                float rotation = (Main.MouseWorld + player.velocity.GetInt() - (player.MountedCenter.GetInt() + TGlobalItem.GetHandOffset(player))).ToRotation();
                if (MathF.Abs((Main.MouseWorld - player.MountedCenter).X) > 6)
                {
                    player.ChangeDir((Main.MouseWorld - player.MountedCenter).X.NonZeroSign());
                }
                Vector2 refOffset = Vector2.Zero;
                ItemLoader.HoldoutOrigin(player, ref refOffset);
                refOffset.X *= player.direction;
                refOffset.Y *= player.gravDir;
                refOffset.Y += TextureAssets.Item[item.type].Value.Height / 2f / (Main.itemAnimations[item.type]?.FrameCount ?? 1);
                Vector2 offset = TGlobalItem.basicOffset + refOffset * item.scale;
                offset.Y *= player.direction;
                float basicRotation = item.GetT().spriteRotation ?? 0;
                player.SetItemRotation(rotation + basicRotation * player.direction);
                player.itemLocation = player.MountedCenter.GetInt() + TGlobalItem.GetHandOffset(player) + offset.RotatedBy(rotation);
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.ToItemRotation(rotation) - 0.5f * (float)Math.PI * player.direction);
                player.bodyFrame.Y = player.bodyFrame.Height;
                bow.bowTime++;
                if (!player.controlUseItem)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 1;
                }
            }
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            float multiply = (float)bowTime / item.useAnimation;
            velocity *= MathF.Min(multiply, speedMultiply);
            damage = (int)(damage * MathF.Min(multiply, damageMultiply));
            knockback *= MathF.Min(multiply, knockbackMultiply);
            projectile = type;
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if(bowTime == 1)
            {
                return false;
            }
            return true;
        }
        UnifiedRandom random = new();
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (bowTime == 1)
            {
                return false;
            }
            if (bowTime >= item.useAnimation * fullCharge)
            {
                if (item.ModItem != null && item.ModItem is IBowUseStyle)
                {
                    (item.ModItem as IBowUseStyle).FullPowerShoot(this, player, source, position, velocity, type, damage, knockback);
                }    
                switch (item.type)
                {
                    case ItemID.CopperBow:
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f), ModContent.ProjectileType<CopperArrow>(), damage / 3, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.2f), ModContent.ProjectileType<CopperArrow>(), damage / 3, knockback, player.whoAmI);
                        break;
                    case ItemID.TinBow:
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f), ModContent.ProjectileType<TinArrow>(), damage / 3, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.15f), ModContent.ProjectileType<TinArrow>(), damage / 3, knockback, player.whoAmI);
                        break;
                    case ItemID.IronBow:
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f), ModContent.ProjectileType<IronArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.15f), ModContent.ProjectileType<IronArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
                    case ItemID.LeadBow:
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(0.15f), ModContent.ProjectileType<LeadArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.15f), ModContent.ProjectileType<LeadArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
                    case ItemID.TungstenBow:
                        Vector2 offset = velocity.ToUnit().RotatedBy(MathF.PI / 2);
                        Projectile.NewProjectile(source, position + offset * 6, velocity, ModContent.ProjectileType<TungstenArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position - offset * 6, velocity, ModContent.ProjectileType<TungstenArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
                    case ItemID.SilverBow:
                        offset = velocity.ToUnit().RotatedBy(MathF.PI / 2);
                        Projectile.NewProjectile(source, position + offset * 6, velocity, ModContent.ProjectileType<SilverArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position - offset * 6, velocity, ModContent.ProjectileType<SilverArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
                    case ItemID.GoldBow:
                        offset = velocity.ToUnit().RotatedBy(MathF.PI / 2);
                        Projectile.NewProjectile(source, position + offset * 6, velocity, ModContent.ProjectileType<GoldenArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position - offset * 6, velocity, ModContent.ProjectileType<GoldenArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
                    case ItemID.PlatinumBow:
                        offset = velocity.ToUnit().RotatedBy(MathF.PI / 2);
                        Projectile.NewProjectile(source, position + offset * 6, velocity, ModContent.ProjectileType<PlatinumArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position - offset * 6, velocity, ModContent.ProjectileType<PlatinumArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position + offset * 12, velocity, ModContent.ProjectileType<PlatinumArrow>(), damage / 2, knockback, player.whoAmI);
                        Projectile.NewProjectile(source, position - offset * 12, velocity, ModContent.ProjectileType<PlatinumArrow>(), damage / 2, knockback, player.whoAmI);
                        break;
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
            if (sound.HasValue)
            {
                SoundEngine.PlaySound(sound, player.position);
            }
            return true;
        }
    }
    public interface IBowUseStyle
    {
        public virtual void SetBowDeffaults(BowsOverride bowsOverride) { }
        public virtual void FullPowerShoot(BowsOverride bowsOverride, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { }
    }
}
