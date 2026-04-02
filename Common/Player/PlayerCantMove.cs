using Luminance.Common.Easings;
using Microsoft.Xna.Framework;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Player
{
	public class PlayerCantMove : ModPlayer
	{
		public bool cantMove;
		Vector2 preUpdatePos;
		public override void PreUpdate()
		{
			if (!cantMove)
				preUpdatePos = Player.position;
			else
			{
				for (int i = 0; i < 4; i++)
				{
					Player.doubleTapCardinalTimer[i] = 60;
				}
				Player.releaseDown = false;
				Player.releaseLeft = false;
				Player.releaseRight = false;
				Player.releaseUp = false;
				Player.controlLeft = false;
				Player.controlRight = false;
				Player.controlDown = false;
				Player.controlUp = false;
				Player.controlUseItem = false;
				Player.controlUseTile = false;
				Player.controlHook = false;
				Player.controlMount = false;
			}
		}
		public override void ResetEffects()
		{
			if (cantMove)
			{
				for (int i = 0; i < 4; i++)
				{
					Player.doubleTapCardinalTimer[i] = 60;
				}
				Player.releaseDown = false;
				Player.releaseLeft = false;
				Player.releaseRight = false;
				Player.releaseUp = false;
				Player.controlLeft = false;
				Player.controlRight = false;
				Player.controlDown = false;
				Player.controlUp = false;
				Player.controlUseItem = false;
				Player.controlUseTile = false;
				Player.controlHook = false;
				Player.controlMount = false;
			}
			cantMove = false;
		}
		public override void PreUpdateMovement()
		{
			if (cantMove)
			{
				Player.velocity = Vector2.Zero;
				for (int i = 0; i < 4; i++)
				{
					Player.doubleTapCardinalTimer[i] = 60;
				}
				Player.releaseDown = false;
				Player.releaseLeft = false;
				Player.releaseRight = false;
				Player.releaseUp = false;
				Player.controlLeft = false;
				Player.controlRight = false;
				Player.controlDown = false;
				Player.controlUp = false;
				Player.controlUseItem = false;
				Player.controlUseTile = false;
				Player.controlHook = false;
				Player.controlMount = false;
			}
		}

        public override void PostUpdate()
		{
			if (cantMove)
			{
				Player.velocity = Vector2.Zero;
				Player.position = preUpdatePos;
			}
        }
	}
	public class IPlayerCantMove : GlobalItem
	{
		public override void UpdateAccessory(Item item, Terraria.Player player, bool hideVisual)
		{
			if (player.GetModPlayer<PlayerCantMove>().cantMove)
			{
				for (int i = 0; i < 4; i++)
				{
					player.doubleTapCardinalTimer[i] = 60;
				}
				player.releaseDown = false;
				player.releaseLeft = false;
				player.releaseRight = false;
				player.releaseUp = false;
				player.controlLeft = false;
				player.controlRight = false;
				player.controlDown = false;
				player.controlUp = false;
				player.controlUseItem = false;
				player.controlUseTile = false;
				player.controlHook = false;
				player.controlMount = false;
			}
		}
	}
}