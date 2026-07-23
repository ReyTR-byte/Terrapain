using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terrapain.Terrapain;

namespace Terrapain.Common.System
{
    public class TunderSystem : ModSystem
    {
        public static LightningDrawInfo[] lightnings = new LightningDrawInfo[16];
        public static int NewLightning(LightningDrawInfo lightninDrawInfo)
        {
            for (int i = 0; i < lightnings.Length; i++)
            { 
                if (!lightnings[i].active)
                {
                    lightnings[i] = lightninDrawInfo;
                    return i;
                }
            }
            return -1;
        }
        public override void PreUpdateProjectiles()
        {
            for (int i = 0; i < lightnings.Length; i++)
            {
                LightningDrawInfo light = lightnings[i];
                if (light.active)
                {
                    if (light.playSound && light.sound.HasValue)
                    {
                        SoundEngine.PlaySound(light.sound.Value, light.end);
                        light.playSound = false;
                    }
                    light.progress += light.speed;
                    float prog = 0;
                    float prog1 = 0;
                    int j = 0;
                    while (prog < light.progress && j < light.Count)
                    {
                        Vector2 pos = light.parts[j].start + prog1 * light.parts[j].start.DirectionTo(light.parts[j].end);
                        prog1 += 20;
                        prog += 20;
                        if (prog1 > light.parts[j].Length)
                        {
                            prog1 -= light.parts[j].Length;
                            j++;
                        }

                        Lighting.AddLight(pos, light.color.ToVector3() * 1.5f);
                    }
                    if (light.progress >= light.TotalLength)
                    {
                        light.timeLeft--;
                        if (light.timeLeft <= 0)
                        {
                            light.active = false;
                        }
                    }
                    lightnings[i] = light;
                }
            }
        }
        public override void PostDrawTiles()
        {
            foreach(var light in lightnings)
            {
                if (light.active)
                {
                    Graphics.DrawLightning(light, 0, light.progress);
                }
            }
        }
    }
}
