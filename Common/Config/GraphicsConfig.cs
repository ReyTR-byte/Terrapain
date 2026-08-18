using System.ComponentModel;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria.ModLoader.Config;

namespace Terrapain.Common.Config
{
    public class GraphicsConfig : ModConfig
    {
        public static GraphicsConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        
        public enum GraphicsLevel
        {
            Potato,
            Low,
            Medium,
            High,
            Ultra,
        }

        [DefaultValue(GraphicsLevel.Ultra)]
        public GraphicsLevel shaders;
        [DefaultValue(GraphicsLevel.Ultra)]
        public GraphicsLevel filters;
        [DefaultValue(true)]
        public bool smoothing;
    }
}