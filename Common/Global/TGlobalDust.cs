using Microsoft.Xna.Framework;
using Terrapain.Content;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class TGlobalDust : ModSystem
    {
        public static Vector3[] dustLights = new Vector3[Main.dust.Length];
        public override void PostUpdateDusts()
        {
            for (int i = 0; i < Main.dust.Length; i++)
            {
                if (Main.dust[i].active && dustLights[i] != Vector3.Zero)
                {
                    Lighting.AddLight(Main.dust[i].position, dustLights[i].X, dustLights[i].Y, dustLights[i].Z);
                }
                if (dustLights[i] != Vector3.Zero && !Main.dust[i].active)
                {
                    dustLights[i] = Vector3.Zero;
                }
            }
        }
    }
}
