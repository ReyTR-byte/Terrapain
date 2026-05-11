using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.UseStyles
{
    public class UseStyleRegister : GlobalItem
    {
        public override void Load()
        {
            TGlobalItem.SharperUseStyle = ItemLoader.RegisterUseStyle(Mod, "SharperUseStyle");
            TGlobalItem.MassiveSwing = ItemLoader.RegisterUseStyle(Mod, "MassiveSwing");
            TGlobalItem.NormalSwing = ItemLoader.RegisterUseStyle(Mod, "NormalSwing");
            TGlobalItem.UseModDrawStyles.Add(TGlobalItem.NormalSwing);
            TGlobalItem.LightSwing = ItemLoader.RegisterUseStyle(Mod, "LightSwing");
            TGlobalItem.UseModDrawStyles.Add(TGlobalItem.LightSwing);
            TGlobalItem.BatUseStyle = ItemLoader.RegisterUseStyle(Mod, "Bat");
            TGlobalItem.LaserUseStyle = ItemLoader.RegisterUseStyle(Mod, "Laser");
            TGlobalItem.ShootOverride = ItemLoader.RegisterUseStyle(Mod, "ShootOverride");
        }
    }
}
