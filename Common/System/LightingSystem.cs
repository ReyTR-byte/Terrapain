using ILGPU;
using ILGPU.Algorithms;
using ILGPU.IR;
using ILGPU.IR.Analyses;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Thai;

namespace Terrapain.Common.System
{
    public class LightingSystem : ModSystem
    {
        Vector3[,] map = new Vector3[0,0];
        Point offcet = new Point();
        Action<AcceleratorStream, Index2D, ArrayView<Light>, Vector2, ArrayView2D<Vector3, Stride2D.DenseY>> kernel;
        Accelerator accelerator;
        Context context;
        public override void Load()
        {
            On_LightingEngine.GetColor += Lightning;
            On_LegacyLighting.GetColor += Lightning;
            On_LightingEngine.ProcessArea += ProcessArea;
            On_LegacyLighting.ProcessArea += ProcessArea;
            On_LightingEngine.Clear += Clear;
            On_LegacyLighting.Clear += Clear;
            context = Context.CreateDefault();
            accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);
            kernel = accelerator.LoadAutoGroupedKernel((Index2D uv, ArrayView<Light> input, Vector2 worldPos, ArrayView2D<Vector3, Stride2D.DenseY> output) =>
                {
                    Vector3 value = new Vector3();
                    Vector2 This = worldPos;
                    This.X += uv.X;
                    This.Y += uv.Y;
                    for (int i = 0; i < input.Length; i++)
                    {
                        float dist = MathF.Sqrt((This.X - input[i].LightPos.X) * (This.X - input[i].LightPos.X) + (This.Y - input[i].LightPos.Y) * (This.Y - input[i].LightPos.Y)) / 15;
                        value.X += MathF.Max(input[i].light.X - dist, 0);
                        value.Y += MathF.Max(input[i].light.Y - dist, 0);
                        value.Z += MathF.Max(input[i].light.Z - dist, 0);
                    }
                    output[uv] = value;
                });
        }


        public override void Unload()
        {
            On_LightingEngine.GetColor -= Lightning;
            On_LegacyLighting.GetColor -= Lightning;
            On_LightingEngine.ProcessArea -= ProcessArea;
            On_LegacyLighting.ProcessArea -= ProcessArea;
            On_LightingEngine.Clear -= Clear;
            On_LegacyLighting.Clear -= Clear;
            accelerator.Dispose();
            context.Dispose();
        }

        private void Clear(On_LegacyLighting.orig_Clear orig, LegacyLighting self)
        {
            orig(self);
            lights = new List<Light>();
        }

        private void Clear(On_LightingEngine.orig_Clear orig, LightingEngine self)
        {
            orig(self);
            lights = new List<Light>();
        }

        private void ProcessArea(On_LegacyLighting.orig_ProcessArea orig, LegacyLighting self, Rectangle area)
        {
            orig(self, area);
            if (!Main.gamePaused && !Main.gameInactive)
            {
                map = new Vector3[area.Width, area.Height];
                offcet = area.Location;
                if (lights.Count > 0)
                {
                    var input = accelerator.Allocate1D(lights.ToArray());
                    var output = accelerator.Allocate2DDenseY<Vector3>(new Index2D(area.Width, area.Height));
                    kernel(accelerator.DefaultStream, (Index2D)output.Extent, input.View, area.Location.ToVector2(), output.View);
                    accelerator.Synchronize();
                    map = output.View.GetAsArray2D();
                }
                lights = new List<Light>();
            }
        }

        private void ProcessArea(On_LightingEngine.orig_ProcessArea orig, LightingEngine self, Rectangle area)
        {
            orig(self, area);
            if (!Main.gamePaused && !Main.gameInactive)
            { 
                map = new Vector3[area.Width, area.Height];
                offcet = area.Location;
                if (lights.Count > 0)
                {
                    var input = accelerator.Allocate1D(lights.ToArray());
                    var output = accelerator.Allocate2DDenseY<Vector3>(new Index2D(area.Width, area.Height));
                    kernel(accelerator.DefaultStream, (Index2D)output.Extent, input.View, area.Location.ToVector2(), output.View);
                    accelerator.Synchronize();
                    map = output.View.GetAsArray2D();
                }
                lights = new List<Light>(); 
            }
        }

        private Vector3 Lightning(On_LegacyLighting.orig_GetColor orig, LegacyLighting self, int x, int y)
        {
            Vector3 value = orig(self, x, y);
            if (x >= offcet.X && x < offcet.X + map.GetLength(0) && y >= offcet.Y && y < offcet.Y + map.GetLength(1))
                value += map[x - offcet.X, y - offcet.Y];
            return value;
        }

        private Vector3 Lightning(On_LightingEngine.orig_GetColor orig, LightingEngine self, int x, int y)
        {
            Vector3 value = orig(self, x, y);
            if (x >= offcet.X && x < offcet.X + map.GetLength(0) && y >= offcet.Y && y < offcet.Y + map.GetLength(1))
                value += map[x - offcet.X, y - offcet.Y];
            return value;
        }
        public struct Light
        {
            public Point LightPos;
            public Vector3 light;
            public Light(Vector3 light, Point lightPos)
            {
                LightPos = lightPos;
                this.light = light;
            }
        }
        public static List<Light> lights = new List<Light>();
        public static void AddLight(Point pos, Vector3 light) 
        {
            lights.Add(new Light(light, pos)); 
        }
    }
}
