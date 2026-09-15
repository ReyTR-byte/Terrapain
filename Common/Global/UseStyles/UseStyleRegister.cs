using Terraria.ModLoader;

namespace Terrapain.Common.Global.UseStyles
{
    public class UseStyleRegister : ILoadable
    {
        public void Load(Mod mod)
        {
            TGlobalItem.SharperUseStyle = ItemLoader.RegisterUseStyle(mod, "SharperUseStyle");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.SharperUseStyle);
            TGlobalItem.MassiveSwing = ItemLoader.RegisterUseStyle(mod, "MassiveSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.MassiveSwing);
            TGlobalItem.NormalSwing = ItemLoader.RegisterUseStyle(mod, "NormalSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.NormalSwing);
            TGlobalItem.LightSwing = ItemLoader.RegisterUseStyle(mod, "LightSwing");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.LightSwing);
            TGlobalItem.BatUseStyle = ItemLoader.RegisterUseStyle(mod, "Bat");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.BatUseStyle);
            TGlobalItem.LaserUseStyle = ItemLoader.RegisterUseStyle(mod, "Laser");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.LaserUseStyle);
            TGlobalItem.ShootOverride = ItemLoader.RegisterUseStyle(mod, "ShootOverride");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.ShootOverride);
            TGlobalItem.BowOverride = ItemLoader.RegisterUseStyle(mod, "BowOverride");
            TGlobalItem.UseDrawOverride.Add(TGlobalItem.BowOverride);
        }

        public void Unload()
        {
        }
    }
}
