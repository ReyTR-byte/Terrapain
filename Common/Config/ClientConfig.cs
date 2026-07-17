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

        [Range(1f, 2.5f)]
        [Increment(0.1f)]
        [DefaultValue(2.5f)]
        public float CutsceneCameraZoom;

        [DefaultValue(true)]
        public bool CutsceneHideUI;

        [DefaultValue(false)]
        public bool SkipDifficultyChangeAnimation;

        [DefaultValue(true)]
        public bool Tips;

        [DefaultValue(1.5f)]
        [Range(0f, 5f)]
        public float UnarmedMouseActiveTime;

        [DefaultValue(false)]
        public bool UnarmedMouseAlwaysActive;

        [DefaultValue(false)]
        public bool drawHitbox;

        public Color hitboxColor;

        [DefaultValue(12)]
        public int LimboSpeed;
    }
}