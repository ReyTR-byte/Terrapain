using Terraria;
using Terraria.ModLoader;
using Terrapain.Content.Projectiles;
using Microsoft.Xna.Framework;

namespace Terrapain.Content.Buffs
{
    public class ScorspiderStingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}