using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;

namespace Terrapain.Common.UI
{
    public class UIManagerSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            TerrapainUIManager.UpdateUI(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            TerrapainUIManager.ModifyInterfaceLayers(layers);
        }
    }
}
