using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terrapain.Common.Player;
using Terrapain.Content.Dashes;
using Terrapain.Content.Projectiles.Enemies;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories
{
    public class EoCShield : VanillaItemActiveAccessory
    {
        public EoCShield()
        {
            DescriptionLinesCount = 1;
        }
        public override void OnUseDash(Player player, bool[] Directions, Item item)
        {
            base.OnUseDash(player, Directions, item);
            if (Directions[2] && Directions[3])
            {
                DashReload = 0;
                return;
            }
            if (player.GetModPlayer<TerrapainPlayer>().unarmed)
            {
                if (Directions[2])
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitX * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                    return;
                }
                if (Directions[3])
                {
                    int proj = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, -Vector2.UnitX * 5, ModContent.ProjectileType<DemonicEyeLazer>(), item.damage, item.knockBack);
                    Main.projectile[proj].DamageType = item.DamageType;
                    return;
                }
            }
        }
        public override void UpdateAccessory(Player player, Item item)
        {
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
            if (!CanUseDash(player, Directions, item))
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
