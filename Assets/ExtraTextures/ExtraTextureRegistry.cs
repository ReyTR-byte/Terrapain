using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Terrapain.Assets.Extratextures
{
    public class ExtraTextureRegistry
    {
        public static Asset<Texture2D> BlackPixel => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/BlackPixel");
        public static Asset<Texture2D> WhitePixel => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel");
        public static Asset<Texture2D> Glow => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/GlowTexture");
        public static Asset<Texture2D> Aura => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/Aura");
        public static Asset<Texture2D> Portal => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/Portal");
        public static Asset<Texture2D> Lighting => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/Lighting");
        public static Asset<Texture2D> EyeofCthulhuCloneDahs1 => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/EyeofCthulhuCloneDashTexture1");
        public static Asset<Texture2D> EyeofCthulhuCloneDahs2 => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/EyeofCthulhuCloneDashTexture2");
        public static Asset<Texture2D> CubedGradient10 => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/1-XCubedGradient10Pixel");
        public static Asset<Texture2D> CubedGradient10Mirrored => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/1-XCubedGradient10PixelMirrored");
        public static Asset<Texture2D> Triangle => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/ShaderTextures/Triangle");
    }
}