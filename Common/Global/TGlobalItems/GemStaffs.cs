using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static AssGen.Assets;
using static System.Net.Mime.MediaTypeNames;

namespace Terrapain.Common.Global.TGlobalItems
{
    public class GemStaffs : GlobalItem
    {
        static UnifiedRandom random = new UnifiedRandom();
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.AmethystStaff || entity.type == ItemID.RubyStaff || entity.type == ItemID.EmeraldStaff || entity.type == ItemID.SapphireStaff || entity.type == ItemID.DiamondStaff || entity.type == ItemID.AmberStaff || entity.type == ItemID.TopazStaff;
        }
        public override bool Shoot(Item item, Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (item.type)
            {
                case ItemID.AmethystStaff:
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.075), ModContent.ProjectileType<AmethystSharp>(), damage, knockback, player.whoAmI);
                    break;
                case ItemID.RubyStaff:
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3), ModContent.ProjectileType<RubySharp>(), damage, knockback, player.whoAmI);
                    break;
                case ItemID.EmeraldStaff:
                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.02), ModContent.ProjectileType<EmeraldSharp>(), damage, knockback, player.whoAmI);
                    break;
                case ItemID.SapphireStaff:
                    int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.02), ModContent.ProjectileType<SapphireSharp>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<SapphireStaff>(), 0, 0, player.whoAmI, proj);
                    break;
                case ItemID.AmberStaff:
                    for (int i = 0; i < 7; i++)
                        Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.3) * random.NextFloat(0.8f, 1), ModContent.ProjectileType<DesertGore>(), damage, knockback, player.whoAmI);
                    break;
                case ItemID.DiamondStaff:
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DiamondStaffLaser>(), damage, knockback, player.whoAmI, -1, -1, 1);
                    break;
            }
            return false;
        }
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.AmethystStaff:
                    entity.useAnimation = 90;
                    entity.useTime = 90;
                    entity.shootSpeed = 20;
                    entity.damage = 40;
                    entity.mana = 15;
                    break;
                case ItemID.RubyStaff:
                    entity.shootSpeed = 9;
                    entity.useAnimation = 9;
                    entity.useTime = 10;
                    entity.mana = 3;
                    break;
                case ItemID.EmeraldStaff:
                    entity.useAnimation = 20;
                    entity.useTime = 20;
                    entity.shootSpeed = 13;
                    break;
                case ItemID.SapphireStaff:
                    entity.shootSpeed = 10;
                    break;
                case ItemID.AmberStaff:
                    entity.shootSpeed = 20;
                    break;
            }
        }
    }
}
