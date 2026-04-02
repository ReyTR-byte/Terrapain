using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terrapain.Content.DamageClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Player
{
    public class UnarmedBuffEffects : ModPlayer
    {
        public override void PostUpdateBuffs()
        {
            if (Player.GetModPlayer<TerrapainPlayer>().unarmed && (Player.HasBuff(BuffID.Thorns) || Player.GetModPlayer<TerrapainPlayer>().CactusSet) && Player.GetModPlayer<TerrapainPlayer>().ThornsSpikeReload == 0 && Functions.DistanceToClosestNPC(Player) < 150 && Functions.DistanceToClosestNPC(Player) != -1)
            {
                UnifiedRandom random = new UnifiedRandom();
                for (int i = 0; i < 8; i++)
                {
                    int proj = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Functions.UnitVectorFromRotation(random.NextFloat() * 2 * MathF.PI) * (4 + 2 * random.NextFloat()), ProjectileID.RollingCactusSpike, (int)Player.GetDamage<Unarmed>().ApplyTo(8), 1, Player.whoAmI);
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].hostile = false;
                }
                Player.GetModPlayer<TerrapainPlayer>().ThornsSpikeReload = 24;
            }
        }
    }
}
