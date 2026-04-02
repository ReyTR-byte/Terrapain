using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace Terrapain.Common.UI
{
    public static class TerrapainUIManager
    {
        public struct TerrapainUserInterface
        {
            public UserInterface UserInterface;
            public TerrapainUI TerrapainUI;
            public TerrapainUserInterface(TerrapainUI terrapainUI)
            {
                UserInterface = new();
                TerrapainUI = terrapainUI;
            }
        }
        public static Dictionary<string, TerrapainUserInterface> UserInterfaces = [];

        private static GameTime LastUpdateUIGameTime { get; set; }

        /*public static Asset<Texture2D> CheckMark { get; private set; }

        public static Asset<Texture2D> CheckBox { get; private set; }

        public static Asset<Texture2D> Cross { get; private set; }

        public static Asset<Texture2D> SoulTogglerButtonTexture { get; private set; }

        public static Asset<Texture2D> SoulTogglerButton_MouseOverTexture { get; private set; }

        public static Asset<Texture2D> PresetButtonOutline { get; private set; }

        public static Asset<Texture2D> PresetOffButton { get; private set; }

        public static Asset<Texture2D> PresetOnButton { get; private set; }

        public static Asset<Texture2D> PresetMinimalButton { get; private set; }

        public static Asset<Texture2D> PresetCustomButton { get; private set; }

        public static Asset<Texture2D> ReloadButtonTexture { get; private set; }
        public static Asset<Texture2D> DisplayAllButtonTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantAuraTexture { get; private set; }

        public static Asset<Texture2D> OncomingMutantntTexture { get; private set; }

        public static Asset<Texture2D> CooldownBarTexture { get; private set; }

        public static Asset<Texture2D> CooldownBarFillTexture { get; private set; }*/
        public static void Register(TerrapainUI ui)
        {
            UserInterfaces.Add(ui.GetType().Name, new(ui));
        }
        public static TerrapainUserInterface GetFromDict(TerrapainUI ui) => UserInterfaces[ui.GetType().Name];
        public static TerrapainUserInterface GetFromDict<T>() where T : TerrapainUI => UserInterfaces[typeof(T).Name];
        public static T Get<T>() where T : TerrapainUI => GetFromDict<T>().TerrapainUI as T;
        public static void Open<T>() where T : TerrapainUI
        {
            if (IsOpen<T>())
                return;
            var ui = GetFromDict<T>();
            ui.UserInterface.SetState(ui.TerrapainUI);
            ui.TerrapainUI.OnOpen();
            if (ui.TerrapainUI.MenuToggleSound)
                SoundEngine.PlaySound(SoundID.MenuOpen);
        }
        public static void Open(TerrapainUI terrapainUI)
        {
            if (IsOpen(terrapainUI))
                return;
            var ui = GetFromDict(terrapainUI);
            ui.UserInterface.SetState(ui.TerrapainUI);
            ui.TerrapainUI.OnOpen();
            if (ui.TerrapainUI.MenuToggleSound)
                SoundEngine.PlaySound(SoundID.MenuOpen);
        }
        public static void Close<T>() where T : TerrapainUI
        {
            if (!IsOpen<T>())
                return;
            var ui = GetFromDict<T>();
            ui.UserInterface.SetState(null);
            ui.TerrapainUI.OnClose();
            if (ui.TerrapainUI.MenuToggleSound)
                SoundEngine.PlaySound(SoundID.MenuClose);
        }
        public static void Close(TerrapainUI terrapainUI)
        {
            if (!IsOpen(terrapainUI))
                return;
            var ui = GetFromDict(terrapainUI);
            ui.UserInterface.SetState(null);
            ui.TerrapainUI.OnClose();
            if (ui.TerrapainUI.MenuToggleSound)
                SoundEngine.PlaySound(SoundID.MenuClose);
        }
        public static void Toggle<T>() where T: TerrapainUI
        {
            if (!IsOpen<T>())
                Open<T>();
            else
                Close<T>();
        }
        public static void Toggle(TerrapainUI terrapainUI)
        {
            if (!IsOpen(terrapainUI))
                Open(terrapainUI);
            else
                Close(terrapainUI);
        }
        public static bool IsOpen<T>() where T : TerrapainUI
        {
            var ui = GetFromDict<T>();
            return ui.UserInterface?.CurrentState != null;
        }
        public static bool IsOpen(TerrapainUI terrapainUI)
        {
            var ui = GetFromDict(terrapainUI);
            return ui.UserInterface?.CurrentState != null;
        }

        public static void LoadUI()
        {
            if (!Main.dedServ)
            {
                // Load textures
                /*CheckMark = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckMark", AssetRequestMode.ImmediateLoad);
                CheckBox = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckBox", AssetRequestMode.ImmediateLoad);
                Cross = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/Cross", AssetRequestMode.ImmediateLoad);
                SoulTogglerButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle", AssetRequestMode.ImmediateLoad);
                SoulTogglerButton_MouseOverTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle_MouseOver", AssetRequestMode.ImmediateLoad);
                PresetButtonOutline = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOutline", AssetRequestMode.ImmediateLoad);
                PresetOffButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOff", AssetRequestMode.ImmediateLoad);
                PresetOnButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOn", AssetRequestMode.ImmediateLoad);
                PresetMinimalButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetMinimal", AssetRequestMode.ImmediateLoad);
                PresetCustomButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetCustom", AssetRequestMode.ImmediateLoad);
                DisplayAllButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/DisplayAllButton", AssetRequestMode.ImmediateLoad);
                ReloadButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ReloadButton", AssetRequestMode.ImmediateLoad);
                OncomingMutantTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutant", AssetRequestMode.ImmediateLoad);
                OncomingMutantAuraTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantAura", AssetRequestMode.ImmediateLoad);
                OncomingMutantntTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantnt", AssetRequestMode.ImmediateLoad);
                CooldownBarTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBar", AssetRequestMode.ImmediateLoad);
                CooldownBarFillTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CooldownBarFill", AssetRequestMode.ImmediateLoad);*/

                foreach (var ui in UserInterfaces)
                {
                    ui.Value.TerrapainUI.OnLoad();
                    ui.Value.TerrapainUI.Activate();
                }
                    
            }
        }
        public static void UpdateUI(GameTime gameTime)
        {
            LastUpdateUIGameTime = gameTime;

            foreach (var ui in UserInterfaces)
            {
                ui.Value.TerrapainUI.UpdateUI();
                if (ui.Value.UserInterface?.CurrentState != null)
                    ui.Value.UserInterface.Update(gameTime);
            }
        }

        public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
            if (index != -1)
            {
                foreach (var ui in UserInterfaces)
                {
                    int insertIndex = ui.Value.TerrapainUI.InterfaceIndex(layers, index);
                    var userInterface = ui.Value.UserInterface;
                    string name = ui.Value.TerrapainUI.InterfaceLayerName;
                    layers.Insert(index - 1, new LegacyGameInterfaceLayer(name, delegate
                    {
                        if (LastUpdateUIGameTime != null && userInterface?.CurrentState != null)
                            userInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

                        return true;
                    }, InterfaceScaleType.UI));
                }
            }
            
            layers.Insert(0, new LegacyGameInterfaceLayer("Fargo: Title Links", delegate
            {
                if (!WorldGen.generatingWorld && !WorldGen.drunkWorldGen && Main.menuMode == 0)
                {
                    float upBump = 0;
                    byte b = (byte)((255 + Main.tileColor.R * 2) / 3);
                    Color color = new Color(b, b, b, 255);
                    Terrapain.DrawTitleLinks(color, upBump);
                    upBump += 32f;
                    
                }
                return true;
                  
            }, InterfaceScaleType.None));          
        }
    }
}