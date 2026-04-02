using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Tiles
{
    public class HardenedDemoniteBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(150, 21, 150), CreateMapEntryName());

            DustType = DustID.Demonite;

            HitSound = SoundID.Tink;
            MineResist = 4f;
            MinPick = 65;
        }
    }
}
