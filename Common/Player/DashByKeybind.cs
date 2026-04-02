using Terrapain.Content;
using Terraria.ModLoader;

namespace Terrapain.Common.Player
{
    public class DashByKeybind : ModPlayer
    {
        public override void ResetEffects()
        {
            if (System.KeybindSystem.DashKeybind.Current)
            {
                if (Player.controlLeft)
                {
                    Player.doubleTapCardinalTimer[3] = 1;
                    Player.releaseLeft = true;
                }
                if (Player.controlRight)
                {
                    Player.doubleTapCardinalTimer[2] = 1;
                    Player.releaseRight = true;
                }
                if (Player.controlUp)
                {
                    Player.doubleTapCardinalTimer[1] = 1;
                    Player.releaseUp = true;
                }
                if (Player.controlDown)
                {
                    Player.doubleTapCardinalTimer[0] = 1;
                    Player.releaseDown = true;
                }
            }
        }
        public override void PreUpdate()
        {
            if (System.KeybindSystem.DashKeybind.Current)
            {
                if (Player.controlLeft)
                {
                    Player.doubleTapCardinalTimer[3] = 1;
                    Player.releaseLeft = true;
                }
                if (Player.controlRight)
                {
                    Player.doubleTapCardinalTimer[2] = 1;
                    Player.releaseRight = true;
                }
                if (Player.controlUp)
                {
                    Player.doubleTapCardinalTimer[1] = 1;
                    Player.releaseUp = true;
                }
                if (Player.controlDown)
                {
                    Player.doubleTapCardinalTimer[0] = 1;
                    Player.releaseDown = true;
                }
            }
        }
        public override void PostUpdate()
        {
            if (System.KeybindSystem.DashKeybind.Current)
            {
                if (Player.controlLeft)
                {
                    Player.doubleTapCardinalTimer[3] = 1;
                    Player.releaseLeft = true;
                }
                if (Player.controlRight)
                {
                    Player.doubleTapCardinalTimer[2] = 1;
                    Player.releaseRight = true;
                }
                if (Player.controlUp)
                {
                    Player.doubleTapCardinalTimer[1] = 1;
                    Player.releaseUp = true;
                }
                if (Player.controlDown)
                {
                    Player.doubleTapCardinalTimer[0] = 1;
                    Player.releaseDown = true;
                }

            }
        }
        public override void PreUpdateMovement()
        {
            if (System.KeybindSystem.DashKeybind.Current)
            {
                if (Player.controlLeft)
                {
                    Player.doubleTapCardinalTimer[3] = 1;
                    Player.releaseLeft = true;
                }
                if (Player.controlRight)
                {
                    Player.doubleTapCardinalTimer[2] = 1;
                    Player.releaseRight = true;
                }
                if (Player.controlUp)
                {
                    Player.doubleTapCardinalTimer[1] = 1;
                    Player.releaseUp = true;
                }
                if (Player.controlDown)
                {
                    Player.doubleTapCardinalTimer[0] = 1;
                    Player.releaseDown = true;
                }

            }
        }
    }
}