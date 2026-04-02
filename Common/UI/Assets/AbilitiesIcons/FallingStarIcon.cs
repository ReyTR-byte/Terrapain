using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Common.UI.Assets.AbilitiesIcons
{
    public class FallingStarIcon : AbilityIcon
    {
        public override Vector2 DrawCenter => new Vector2(21, 19);
        public FallingStarIcon()
        {
            animationType = 1;
            animationSpeed = 6;
            frameCount = 4;
        }
    }
}
