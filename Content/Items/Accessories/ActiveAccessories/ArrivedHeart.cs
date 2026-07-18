using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories.ActiveAccessories
{
    public class ArrivedHeart : ActiveAccessory
    {
        public override void ModSetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.GetT().activeAccessory = true;
            AbilityReloadMax = 500;
            AbilityUnarmedOnly = false;
            Item.damage = 15;
            Item.DamageType = ModContent.GetInstance<Unarmed>();
            Item.knockBack = 0.5f;
            AutoUse = true;
            CanAutoUse = true;
            //abilityIcon = new LightningIcon();
        }
        public override bool OnUseAbility(Player player)
        {
            Vector2 vector = (player.velocity == Vector2.Zero? new Vector2(-player.direction, 0) : -player.velocity.Normalized()) * 75;
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center - player.velocity + vector, Vector2.Zero, ModContent.ProjectileType<ArrivedHeartProjectile>(), (int)player.GetTotalDamage<Unarmed>().ApplyTo(Item.damage), Item.knockBack, player.whoAmI);
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center - player.velocity + vector.RotatedBy(MathF.PI * 0.25f), Vector2.Zero, ModContent.ProjectileType<ArrivedHeartProjectile>(), (int)player.GetTotalDamage<Unarmed>().ApplyTo(Item.damage), Item.knockBack, player.whoAmI);
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center - player.velocity + vector.RotatedBy(MathF.PI * -0.25f), Vector2.Zero, ModContent.ProjectileType<ArrivedHeartProjectile>(), (int)player.GetTotalDamage<Unarmed>().ApplyTo(Item.damage), Item.knockBack, player.whoAmI);
            return false;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (AbilityReload > 0)
                AbilityReload -= MathF.Abs(player.velocity.X) < 3? player.Custom().oldLifeRegen : player.Custom().oldLifeRegen / 10;
            else
                AbilityReload = 0;
            if (AbilityReload > AbilityReloadMax)
                AbilityReload = AbilityReloadMax;
        }
        public override bool? CanUseAbility(Player player)
        {
            if (!AutoUse)
            {
                return null;
            }
            else
            {
                return SearchForTarget(player)? null : false;
            }
        }
        bool SearchForTarget(Player player)
        {
            Vector2 targetCenter = player.position;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy)
                {
                    float between = Vector2.Distance(npc.Center, player.Center);
                    bool inRange = between < 700;
                    bool lineOfSight = Functions.CanHit(player.Center, npc.position, npc.width, npc.height);

                    if (inRange && lineOfSight)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
