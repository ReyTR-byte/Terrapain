using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using static Terrapain.Terrapain;

namespace Terrapain.Common.System
{
    public class ShaderSystem : ModSystem
    {
        public static ManagedRenderTarget target;
        public static bool drawScorspiderBorders;
        public static float BottomOfScorspiderWalls;
        public static int ScorspiderTimer;
        public static bool drawScorspiderAura;
        public static int ScorspiderAuraTimer;
        public static Vector2 ScorspiderPosition;
        public static float AuraRadius;
    }
}