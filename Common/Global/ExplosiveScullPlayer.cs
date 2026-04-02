using Terrapain.Common.Player;
using Terrapain.Content;
using Terrapain.Content.Buffs;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class ShootBonusProjectiles : GlobalItem
    {
        public override bool? UseItem(Item item, Terraria.Player player)
        {
            if (player.GetModPlayer<TerrapainPlayer>().ExplosiveSkull && item.DamageType == DamageClass.Ranged && player.GetModPlayer<TerrapainPlayer>().ExplosiveSkullReload == 0)
            {
                player.GetModPlayer<TerrapainPlayer>().ExplosiveSkullReload = 120;
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, (Main.MouseWorld - player.Center) / (Main.MouseWorld - player.Center).Length() * 8, ModContent.ProjectileType<ExplosiveSkull>(), player.GetModPlayer<TerrapainPlayer>().ExplosiveSkullDamage, 5f, player.whoAmI);
            }
            if (player.Custom().StarFuryBrasslet && item.DamageType == DamageClass.Magic && player.Custom().StarFuryBrassletReload == 0)
            {
                player.Custom().StarFuryBrassletReload = 10;
                Vector2 pos = player.Center + new Vector2(player.Custom().playerRandom.Next(-30, 31), player.Custom().playerRandom.Next(-30, 31));
                Projectile.NewProjectile(player.GetSource_FromThis(), pos, pos.DirectionTo(Main.MouseWorld) * 16, ProjectileID.StarCannonStar, 15, 5f, player.whoAmI);
            }
            return base.UseItem(item, player);
        }
    }
}