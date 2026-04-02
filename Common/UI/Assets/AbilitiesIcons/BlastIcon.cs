using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Common.UI.Assets.AbilitiesIcons
{
    public class BlastIcon : AbilityIcon
    {
        public override Vector2 DrawCenter => new Vector2(13, 11);
        public BlastIcon()
        {
            animationType = 2;
            animationSpeed = 1;
            frameCount = 14;
        }
    }
}
