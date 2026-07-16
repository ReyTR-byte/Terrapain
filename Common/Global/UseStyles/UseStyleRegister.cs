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
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.SharperUseStyle);
            TGlobalItem.MassiveSwing = ItemLoader.RegisterUseStyle(Mod, "MassiveSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.MassiveSwing);
            TGlobalItem.NormalSwing = ItemLoader.RegisterUseStyle(Mod, "NormalSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.NormalSwing);
            TGlobalItem.LightSwing = ItemLoader.RegisterUseStyle(Mod, "LightSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.LightSwing);
            TGlobalItem.BatUseStyle = ItemLoader.RegisterUseStyle(Mod, "Bat");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.BatUseStyle);
            TGlobalItem.LaserUseStyle = ItemLoader.RegisterUseStyle(Mod, "Laser");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.LaserUseStyle);
            TGlobalItem.ShootOverride = ItemLoader.RegisterUseStyle(Mod, "ShootOverride");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.ShootOverride);
            TGlobalItem.BowOverride = ItemLoader.RegisterUseStyle(Mod, "BowOverride");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.BowOverride);
        }
    }
}
