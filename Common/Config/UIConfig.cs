using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace Terrapain.Common.Config
{
    public class UIConfig : ModConfig
    {
        public static UIConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public enum StaminaBarType
        {
            Thin,
            Width,
        }

        [DefaultValue(StaminaBarType.Width)]
        public StaminaBarType staminaBarType;

        public Color EmptyColor = Color.Red;

        public Color FullColor = Color.LimeGreen;

        [Range(-50f, 50f)]
        public Vector2 Offset;
    }
}
