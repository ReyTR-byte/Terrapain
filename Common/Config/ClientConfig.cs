using System.ComponentModel;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria.ModLoader.Config;

namespace Terrapain.Common.Config
{
    public class ClientConfig : ModConfig
    {
        public static ClientConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        
        [DefaultValue(true)]
        public bool UseShaders;

        [DefaultValue(false)]
        public bool drawHitbox;

        [DefaultValue(true)]
        public bool Tips;

        [DefaultValue(12)]
        public int LimboSpeed;

        [Range(1f, 2.5f)]
        [Increment(0.1f)]
        [DefaultValue(2.5f)]
        public float CutsceneCameraZoom;

        [DefaultValue(true)]
        public bool CutsceneHideUI;

        [DefaultValue(false)]
        public bool SkipDifficultyChangeAnimation;

        public Color hitboxColor;
    }
}