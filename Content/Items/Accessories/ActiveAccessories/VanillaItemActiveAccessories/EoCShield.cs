using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories.ActiveAccessories.VanillaItemActiveAccessories
{
    public class EoCShield : VanillaItemActiveAccessory
    {
        public override VanillaItemRework GetNewInstance(Item item)
        {
            return new EoCShield();
        }
        public override void ModSetDefaults(Item entity)
        {
            entity.GetT().dashAccessory = true;
            DashPower = 15;
            DashDuration = 15;
            DashReloadMax = 60;
            AbilityReloadMax = 150;
            abilityIcon = new SuperDashIcon();
            entity.GetT().activeAccessory = true;
            activeAccessory = new ActiveAccessory(this);
            entity.GetT().ActiveAccessory = activeAccessory;
            DescriptionLinesCount = 1;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                if (player.GetModPlayer<PlayerMovement>().DashPower > activeAccessory.DashPower)
                {
                    activeAccessory.AbilityReload = 30;
                }
                activeAccessory.DashReload = 30;
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                item.DamageType = ModContent.GetInstance<Unarmed>();
            }
            else
            {
                item.DamageType = DamageClass.Melee;
            }
        }
        public override bool OnUseDash(Player player, Item item, bool[] Directions)
        {
            if (Directions[2] && Directions[3])
            {
                DashReload = 0;
                return false;
            }
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                if (Directions[2])
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                    return false;
                }
                if (Directions[3])
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, -Vector2.UnitX * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                    return false;
                }
            }
            return false;
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                item.DamageType = ModContent.GetInstance<Unarmed>();
            }
            else
            {
                item.DamageType = DamageClass.Melee;
            }
            player.Custom().Dash = new ActiveAccessoryDash(item) { damageType = item.DamageType, immune = 10, DashDuration = DashDuration, DashPower = DashPower, penetrate = 1, priority = 1, hurtfull = true };
        }
        public override void OnUseAbility(Player player, Item item)
        {
            bool[] Directions = { player.controlDown, player.controlUp, player.controlRight, player.controlLeft };
            if (!Directions[0] && !Directions[1] && !Directions[2] && !Directions[3])
            {
                AbilityReload = 0;
                return; 
            }
            if (!activeAccessory.CanUseDash(player, Directions, item))
            {
                AbilityReload = 0;
                return;
            }
            if (Directions[2] && Directions[3])
            {
                AbilityReload = 0;
                return;
            }
            if (Directions[0] && Directions[1])
            {
                AbilityReload = 0;
                return;
            }
            if (Directions[2])
            {
                float angle = 0; 
                if (Directions[0])
                {
                    angle = MathF.PI * 0.25f;
                }
                if (Directions[1])
                {
                    angle = -MathF.PI * 0.25f;
                }
                DashReload = 60;
                NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = 1 };
                player.GetModPlayer<PlayerMovement>().Dash(item, DashPower * 1.5f, angle, DashDuration, 1, 10, modifiers);
                if (player.GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy(angle) * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                }
                return;
            }
            else if (Directions[3])
            {
                float angle = 0;
                if (Directions[0])
                {
                    angle = -MathF.PI * 0.25f;
                }
                if (Directions[1])
                {
                    angle = MathF.PI * 0.25f;
                }
                DashReload = 60;
                NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = -1 };
                player.GetModPlayer<PlayerMovement>().Dash(item, DashPower * 1.5f, MathF.PI + angle, (int)(DashDuration * 1.5f), 1, 10, modifiers);
                if (player.GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy(MathF.PI + angle) * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                }
                return;
            }
            else if (Directions[0])
            {
                DashReload = 60;
                NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = -1 };
                player.GetModPlayer<PlayerMovement>().Dash(item, DashPower * 1.5f, MathF.PI * 0.5f, (int)(DashDuration * 1.5f), 1, 10, modifiers);
                if (player.GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy(MathF.PI * 0.5f) * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                }
                return;
            }
            else if (Directions[1])
            {
                DashReload = 60;
                NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = -1 };
                player.GetModPlayer<PlayerMovement>().Dash(item, DashPower * 1.5f, MathF.PI * 1.5f, (int)(DashDuration * 1.5f), 1, 10, modifiers);
                if (player.GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX.RotatedBy(MathF.PI * 1.5f) * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                }
                return;
            }
            DashReload = 0;
        }
    }
}
