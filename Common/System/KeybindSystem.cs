using Steamworks;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Common.System
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind DashKeybind { get; private set; }
        public static ModKeybind RealiseHookedNPC { get; private set; }
        public static ModKeybind ActiveAccesory1 { get; private set; }
        public static ModKeybind ActiveAccesory2 { get; private set; }
        public static ModKeybind ActiveAccesory3 { get; private set; }
        public static ModKeybind ActiveAccesory4 { get; private set; }
        public static ModKeybind ActiveAccesory5 { get; private set; }
        public static ModKeybind ActiveAccesory6 { get; private set; }
        public static ModKeybind ActiveAccesory7 { get; private set; }
        public static ModKeybind[] ActiveAccesories 
        {
            get => 
            [
                ActiveAccesory1,
                ActiveAccesory2,
                ActiveAccesory3,
                ActiveAccesory4,
                ActiveAccesory5,
                ActiveAccesory6,
                ActiveAccesory7,
            ];
        }
        public override void Load()
        {
            DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", Microsoft.Xna.Framework.Input.Keys.F);
            RealiseHookedNPC = KeybindLoader.RegisterKeybind(Mod, "RealiseHookedNPC", Microsoft.Xna.Framework.Input.Keys.R);
            ActiveAccesory1 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory1", Microsoft.Xna.Framework.Input.Keys.O);
            ActiveAccesory2 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory2", Microsoft.Xna.Framework.Input.Keys.P);
            ActiveAccesory3 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory3", Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets);
            ActiveAccesory4 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory4", Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets);
            ActiveAccesory5 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory5", Microsoft.Xna.Framework.Input.Keys.L);
            ActiveAccesory6 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory6", Microsoft.Xna.Framework.Input.Keys.OemSemicolon);
            ActiveAccesory7 = KeybindLoader.RegisterKeybind(Mod, "ActiveAccesory7", Microsoft.Xna.Framework.Input.Keys.OemQuotes);
        }
        public override void Unload()
        {
            DashKeybind = null;
            RealiseHookedNPC = null;
            ActiveAccesory1 = null;
            ActiveAccesory2 = null; 
            ActiveAccesory3 = null;
            ActiveAccesory4 = null;
            ActiveAccesory5 = null;
            ActiveAccesory6 = null;
            ActiveAccesory7 = null;
        }
    }
}