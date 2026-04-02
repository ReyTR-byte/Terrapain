using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Common.Data;
using Terraria.Utilities;
using Humanizer;

namespace Terrapain.Content.Tiles
{
	public class DeepniteOre : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			AddMapEntry(new Microsoft.Xna.Framework.Color(198, 198, 198), CreateMapEntryName());

			DustType = 84;

			HitSound = SoundID.Tink;
			MineResist = 4f;
			MinPick = 210;
		}
		UnifiedRandom ur = new UnifiedRandom();
		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			if ((((4 % i < 2 && j % 2 == 0) || (4 % i > 1 && j % 2 == 1)) && ((Main.tile[i - 1, j].TileType == ModContent.TileType<DeepniteOre>() 
			&& Main.tile[i - 1, j].TileFrameX == Main.tile[i, j].TileFrameX 
			&& Main.tile[i - 1, j].TileFrameY == Main.tile[i, j].TileFrameY) 
			|| (Main.tile[i, j - 1].TileType == ModContent.TileType<DeepniteOre>() 
			&& Main.tile[i, j - 1].TileFrameX == Main.tile[i, j].TileFrameX 
			&& Main.tile[i, j - 1].TileFrameY == Main.tile[i, j].TileFrameY))) 
			|| (((4 % i > 1 && j % 2 == 0) || (4 % i < 2 && j % 2 == 1)) && ((Main.tile[i + 1, j - 1].TileType == ModContent.TileType<DeepniteOre>() 
			&& Main.tile[i + 1, j - 1].TileFrameX == Main.tile[i, j].TileFrameX 
			&& Main.tile[i + 1, j - 1].TileFrameY == Main.tile[i, j].TileFrameY) 
			|| (Main.tile[i - 1, j - 1].TileType == ModContent.TileType<DeepniteOre>() 
			&& Main.tile[i - 1, j - 1].TileFrameX == Main.tile[i, j].TileFrameX 
			&& Main.tile[i - 1, j - 1].TileFrameY == Main.tile[i, j].TileFrameY))))
			{
				int B = -1;
				for (int a = 0; a < 57; a++)
				{
					for (int b = 0; b < 3; b++)
					{
						if (TileFrames.TileFrame[a, b, 0] == Main.tile[i, j].TileFrameX && TileFrames.TileFrame[a, b, 1] == Main.tile[i, j].TileFrameY)
						{
							B = b;
							break;
						}
					}
					if (B != -1)
					{
						Main.tile[i, j].TileFrameX = (short)TileFrames.TileFrame[a, (B + ur.Next(1, 3)) % 3, 0];
						Main.tile[i, j].TileFrameY = (short)TileFrames.TileFrame[a, (B + ur.Next(1, 3)) % 3, 1];
						break;
					}
				}
			}
		}
        public override void PlaceInWorld(int i, int j, Item item)
		{
			//Functions.Chatic($"{Main.tile[i, j].TileFrameX}, {Main.tile[i, j].TileFrameY}");
        }
	}
}