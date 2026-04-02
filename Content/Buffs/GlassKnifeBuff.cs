using Terraria;
using Terraria.ModLoader;
using Terrapain.Content.Projectiles.Minions;

namespace Terrapain.Content.Buffs
{
    public class GlassKnifeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GlassKnife>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                return;
            }

            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}