using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using static Terrapain.Common.Global.TGlobalNPCs.EyeofCthulhu;

namespace Terrapain.Common.CameraModifiers
{
    public class SmoothMoovingCameraModifier : ICameraModifier
    {
        public Vector2 OriginalCameraPosition;
        public static Vector2 TargetPosition;
        public float TargetZoom;
        public float StartZoom;
        public int AimTime;
        public int TotalTime;
        public static int Timer;
        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }
        public void Update(ref CameraInfo cameraInfo)
        {
            cameraInfo.CameraPosition = OriginalCameraPosition + (TargetPosition - OriginalCameraPosition) * Functions.EasingInOut(AimTime, Timer);
            float Zoom = StartZoom + (TargetZoom - StartZoom) * Functions.EasingInOut(AimTime, Timer);

            if (TotalTime - Timer < AimTime)
            { 
                cameraInfo.CameraPosition = TargetPosition + (Main.screenPosition - TargetPosition) * Functions.EasingInOut(AimTime, AimTime - TotalTime + Timer);
                Zoom = TargetZoom + (StartZoom - TargetZoom) * Functions.EasingInOut(AimTime, AimTime - TotalTime + Timer);
            }
            if (Zoom != 0)
            ZoomSystem.Zoom = Vector2.One * Zoom;

            if (Timer >= TotalTime)
            {
                Finished = true;
            }
        }
    }
    public class ZoomSystem : ModSystem
    {
        public static Vector2? Zoom;
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (Zoom.HasValue)
                Transform.Zoom = Zoom.Value;
            Zoom = null;
        }
        public override void PostUpdateWorld()
        {
            SmoothMoovingCameraModifier.Timer++;
        }
    }
}
