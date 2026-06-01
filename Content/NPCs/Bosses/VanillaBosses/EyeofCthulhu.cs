using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.CameraModifiers;
using Terrapain.Common.Config;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content;
using Terrapain.Content.Dusts;
using Terrapain.Content.Groups;
using Terrapain.Content.Items.DropRulls;
using Terrapain.Content.Projectiles.Enemies;
using Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses
{
    public class EyeofCthulhu : NPCBehaviour
    {
        public struct DrawLaser
        {
            public Vector2 start;
            public Vector2 end;
            public float width;
            public Color color;
        }
        List<DrawLaser> Lasers = new List<DrawLaser>();
        public struct LensHolder
        {
            bool Flying;
            bool Ghost;
            Vector2 Position;
            Vector2 Velocity;
            const float maxVelocity = 15;
            float rotation;
            bool Top;
            int FrameTimer;
            int frame;
            int Timer;


            public LensHolder(NPC npc, bool top)
            {
                Top = top;
                Position = npc.Center;
                Velocity = Vector2.UnitY.RotatedBy(rotation) * 3;
                Timer = 60;
                Flying = true;
            }
            public void Update(ref Lens lens)
            {
                FrameTimer--;
                if (FrameTimer <= 0)
                {
                    frame++;
                    if (frame >= 4 || (!Ghost && frame >= 2))
                    {
                        frame = 0;
                    }
                    FrameTimer = 12;
                }
                if (!Flying)
                {
                    Position = (Top ? lens.Top : lens.Bottom);
                    rotation = lens.rotation;
                    return;
                }
                Vector2 vectorToTargetPosition = (Top? lens.Top: lens.Bottom) - Position;
                Velocity += vectorToTargetPosition * 1.2f;
                float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Velocity);
                positiveRotation = NormalizeRotation(positiveRotation);
                float negativeRotation = AngleBetweenVectors(Velocity, vectorToTargetPosition);
                negativeRotation = NormalizeRotation(negativeRotation);
                if (positiveRotation > negativeRotation)
                {
                    Velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                }
                else
                {
                    Velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                }
                if (Velocity.Length() > maxVelocity)
                {
                    Velocity = Velocity.ToUnit() * maxVelocity;
                }
                rotation = Velocity.ToRotation();
                Timer--;
                if (Timer<= 0)
                {
                    lens.Holders++;
                    Flying = false;
                }
                Position += Velocity;
            }
            public Texture2D GetTexture()
            {
                Main.instance.LoadNPC(NPCID.ServantofCthulhu);
                return TextureAssets.Npc[NPCID.ServantofCthulhu].Value;
            }
            public Rectangle GetFrame(Texture2D texture)
            {
                return new Rectangle(0, texture.Height / 2 * frame, texture.Width, texture.Height / 2);
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                Texture2D texture = GetTexture();
                spriteBatch.Draw(texture, Position - Main.screenPosition, GetFrame(texture), Lighting.GetColor(Position.ToTileCoordinates()), rotation - (Ghost? 0 : MathF.PI / 2), new Vector2(10, 20), 1, SpriteEffects.None, 0);
            }
        }
        LensHolder[] holders = new LensHolder[2];
        byte holdersCount = 0;
        public struct Lens
        {
            public bool active;
            public bool freeFall;
            public float angularVelocity;
            public Vector2 velocity;
            public byte Holders;
            public float targetRotation;
            public float maxVelocity = 20;
            public byte Style;
            public Vector2 RealFocus = Vector2.Zero;

            public Vector2 Center;
            public float warp;
            public Vector2 Scale => new Vector2(warp, 1 / warp);
            public float width => Scale.X * 36;
            public float height => Scale.Y * 80;
            public Vector2 Top => Center + (Vector2.UnitY * height / 2).RotatedBy(rotation);
            public Vector2 Bottom => Center - (Vector2.UnitY * height / 2).RotatedBy(rotation);
            public float rotation;
            public Texture2D Texture => ModContent.Request<Texture2D>("Terrapain/Assets/ExtraTextures/Lens").Value;
            public float baseOpticPower => (float)1/80;
            public float opticPower => baseOpticPower * warp;
            public Vector2 focus1 => Center + Vector2.UnitX.RotatedBy(rotation) / opticPower;
            public Vector2 focus2 => Center - Vector2.UnitX.RotatedBy(rotation) / opticPower;
            public Lens()
            {
                warp = 1;
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                Color color = Lighting.GetColor(Center.ToTileCoordinates());
                spriteBatch.Draw(Texture, Center - Main.screenPosition, null, color, rotation, Texture.Size() / 2, Scale, SpriteEffects.None, 0);
            }
            public void Update(NPC npc)
            {
                if (Holders == 2)
                {
                    if (RealFocus != Vector2.Zero)
                    {
                        float dist = npc.Distance(RealFocus);
                        float distToPlayer = npc.Distance(npc.GetT().Target.Center);
                        if (dist>distToPlayer)
                        {
                            warp += 0.00005f * MathF.Abs(dist - distToPlayer) * warp * warp;
                        }
                        else
                        {
                            warp -= 0.00005f * MathF.Abs(dist - distToPlayer) * warp * warp;
                        }
                        if (warp < 0.53f)
                        {
                            warp = 0.53f;
                        }
                        if (warp > 2f)
                        {
                            warp = 2f;
                        }
                    }
                    float maxVelocityMultyplier = 1;
                    Vector2 targetPosition; 
                    if (Style == 0)
                    {   
                        targetPosition = npc.Center + Vector2.UnitX.RotatedBy(AngleFromVector(npc.GetT().Target.Center - npc.Center)) * 180; 
                        targetRotation = AngleFromVector(npc.GetT().Target.Center - npc.Center);
                    }
                    else
                    {
                        targetPosition = npc.Center + Vector2.UnitX.RotatedBy(npc.rotation + MathF.PI / 2) * 180;
                        targetRotation = npc.rotation + MathF.PI / 2;
                        if (Style == 2)
                        {
                            rotation = targetRotation;
                        }
                    }
                    if (Style == 1 || Style == 0)
                        velocity += Center.DirectionTo(targetPosition) * 1.2f;
                    else
                    {
                        velocity = Center.DirectionTo(targetPosition) * (velocity.Length() + 1);
                        if (velocity.Length() > Center.Distance(targetPosition))
                        {
                            velocity = velocity.Normalized() * Center.Distance(targetPosition);
                        }
                    }
                    if (Center.Distance(targetPosition) < 50)
                    {
                        if (Style == 1 || Style == 0)
                            maxVelocityMultyplier = 1 - (50 - Center.Distance(targetPosition)) / 50;
                        if (Style == 0)
                            maxVelocityMultyplier *= maxVelocityMultyplier;
                    }
                    if (Center.Distance(targetPosition) > 500)
                    {
                        maxVelocityMultyplier = (Center.Distance(targetPosition)) / 500;
                    }
                    if (Style == 1 || Style == 0)
                    {
                        float positiveRotation = AngleBetweenVectors(targetPosition - Center, velocity);
                        positiveRotation = NormalizeRotation(positiveRotation);
                        float negativeRotation = AngleBetweenVectors(velocity, targetPosition - Center);
                        negativeRotation = NormalizeRotation(negativeRotation);
                        if (positiveRotation > negativeRotation)
                        {
                            velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                        }
                        else
                        {
                            velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                        }
                        AngularAcceleration(ref angularVelocity, 0.03f, 0.3f, targetRotation, ref rotation, true);
                    }
                    if (velocity.Length() > maxVelocity * maxVelocityMultyplier)
                    {
                        velocity = velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                    }
                }
                else if (freeFall)
                {
                    velocity += Vector2.UnitY * 0.3f;
                    if (MathF.Abs(NormalizeRotation(ref rotation)) > MathF.PI * 0.5f)
                    {
                        if (rotation > 0)
                        {
                            if (angularVelocity * angularVelocity * 5 > rotation)
                            {
                                angularVelocity -= 0.1f;
                            }
                            else
                            {
                                angularVelocity += 0.1f;
                            }
                        }
                        else
                        {
                            if (angularVelocity * angularVelocity * 5 > -rotation)
                            {
                                angularVelocity += 0.1f;
                            }
                            else
                            {
                                angularVelocity -= 0.1f;
                            }
                        }
                    }
                    else
                    {
                        if (rotation > 0)
                        {
                            if (angularVelocity * angularVelocity * 5 > (MathF.PI - rotation))
                            {
                                angularVelocity += 0.1f;
                            }
                            else
                            {
                                angularVelocity -= 0.1f;
                            }
                        }
                        else
                        {
                            if (angularVelocity * angularVelocity * 5 > -(MathF.PI - rotation))
                            {
                                angularVelocity -= 0.1f;
                            }
                            else
                            {
                                angularVelocity += 0.1f;
                            }
                        }
                    }
                    rotation += angularVelocity;
                }
                Center += velocity;
            }
        }
        public Lens lens = new Lens();
        public struct _2To3PhaseTransitionAnimation
        {
            public bool Active;
            public int Time;
            public Vector2 StartCameraPosition;
            public float StartCameraZoom;
            public Vector2 EndCameraPosition;
            public float EndCameraZoom;
            public const int CameraTimeEnd = 100;
            public const int Phase3TimeStart = 250;
            public const int RotationStart = 20;
            public float StartRotation;
            public float EndRotation1;
            public const int RotationTime1End = RotationStart + 300;
            public float EndRotation2;
            public const int RotationTime2End = RotationTime1End + 25;
            public float EndRotation3;
            public const int RotationTime3End = RotationTime2End + 25;
            public const int CoughTimeStart = RotationTime3End + 30;
            public Vector2 EoCPositionStart;
            public Vector2 EoCPositionEnd;
            public const int Cough1TimeEnd = CoughTimeStart + 16;
            public Vector2 LensPosition1;
            public const int Cough2TimeEnd = Cough1TimeEnd + 17;
            public Vector2 LensPosition2;
            public const int Cough3TimeEnd = Cough2TimeEnd + 16;
            public Vector2 LensPosition3;
            public const int Cough4TimeEnd = Cough3TimeEnd + 15;
            public Vector2 LensPosition4;
            public const int Holder1SpawnTime = Cough4TimeEnd + 10;
            public const int Holder2SpawnTime = Cough4TimeEnd + 25;
            public const int Rotation4TimeStart = Cough4TimeEnd + 35;
            public float EndRotation4;
            public const int Rotation4TimeEnd = Rotation4TimeStart + 135;
            public const int LaserTimeStart = Rotation4TimeEnd + 125;
            public Vector2 ChargingPositionEnd;
            public const int ChargingTimeEnd = LaserTimeStart + 180;
            public bool[] GoreActive;
            public bool[] GorePhisics;
            public Vector2[] GorePosition;
            public Vector2[] GoreDrawSource;
            public Vector2[] GoreVelocity;
            public float[] GoreRotation;
            public float[] GoreAngularVelocity;
            public Vector2 LensPosition;
            public bool? saveUI;
            public Texture2D[] Gores => [
                Gore1,
                Gore2,
                Gore3,
                Gore4,
                Gore5,
                ];
            public Texture2D Gore1 => ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/Gores/EyeofCthulhu_Gore1").Value;
            public Texture2D Gore2 => ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/Gores/EyeofCthulhu_Gore2").Value;
            public Texture2D Gore3 => ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/Gores/EyeofCthulhu_Gore3").Value;
            public Texture2D Gore4 => ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/Gores/EyeofCthulhu_Gore4").Value;
            public Texture2D Gore5 => ModContent.Request<Texture2D>("Terrapain/Content/NPCs/Bosses/VanillaBosses/Gores/EyeofCthulhu_Gore5").Value;
            public _2To3PhaseTransitionAnimation(NPC npc)
            {
                StartCameraPosition = Main.screenPosition;
                EndCameraPosition = npc.Center - Main.ScreenSize.ToVector2() * 0.5f;
                StartCameraZoom = Main.GameZoomTarget;
                EndCameraZoom = 2.5f;
                EoCPositionStart = npc.Center;
                EoCPositionEnd = npc.Center + Vector2.UnitY.RotatedBy(-MathF.PI / 16) * 100;
                StartRotation = npc.rotation;
                EndRotation1 = npc.rotation + 10 * MathF.PI - NormalizeRotation(npc.rotation) - MathF.PI * 0.25f;
                EndRotation2 = EndRotation1 - MathF.PI;
                EndRotation3 = EndRotation2 + 1.25f * MathF.PI - MathF.PI / 16;
                EndRotation4 = EndRotation3 - 0.5f * MathF.PI + MathF.PI / 16;
                ChargingPositionEnd = npc.Center - Vector2.UnitX * 250;
                GoreActive = new bool[5];
                GorePhisics = new bool[5];
                GorePosition = new Vector2[5];
                GoreDrawSource = new Vector2[5];
                GoreVelocity = new Vector2[5];
                GoreRotation = new float[5];
                GoreAngularVelocity = new float[5];
                LensPosition = Vector2.Zero;
            }
        }
        _2To3PhaseTransitionAnimation anim;
        public Vector2 drawCenter;
        public void UpdateAnimation(NPC npc)
        {
            npc.ai[0] = -201;
            npc.velocity = Vector2.Zero;
            if (anim.Time == 0)
            {
                Main.instance.CameraModifiers.Add(new SmoothMoovingCameraModifier() { AimTime = _2To3PhaseTransitionAnimation.CameraTimeEnd, OriginalCameraPosition = Main.screenPosition, TotalTime = _2To3PhaseTransitionAnimation.ChargingTimeEnd, StartZoom = Main.GameZoomTarget, TargetZoom = ClientConfig.Instance.CutsceneCameraZoom, hideUI = ClientConfig.Instance.CutsceneHideUI });
                SmoothMoovingCameraModifier.TargetPosition = npc.Center - Main.ScreenSize.ToVector2() / 2;
                SmoothMoovingCameraModifier.Timer = 0;
            }
            for (int i = 0; i < 5; i++)
            {
                if (anim.GoreActive[i] && anim.GorePhisics[i])
                {
                    anim.GoreVelocity[i].Y += 0.3f;
                    anim.GorePosition[i] += anim.GoreVelocity[i];
                    anim.GoreRotation[i] += anim.GoreAngularVelocity[i];
                    anim.GoreAngularVelocity[i] *= 0.95f;
                    Dust.NewDust(anim.GorePosition[i] - Vector2.One * 8, 16, 16, DustID.Blood, Scale: 2);
                }
            }
            if (anim.Time == _2To3PhaseTransitionAnimation.Phase3TimeStart)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                phase = 3;
                npc.GetT().useVanillaDrawing = false;
                npc.GetT().useModDrawingInPreDraw = true;
                npc.GetT().drawCenter = new Vector2(npc.frame.Width / 2, npc.frame.Height - 98 + npc.height / 2);
                anim.GoreActive[0] = true;
                anim.GorePosition[0] = npc.Center;
                anim.GoreRotation[0] = npc.rotation;
                anim.GoreDrawSource[0] = drawCenter;
                lens.active = true;
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, Vector2.UnitY.RotatedBy(npc.rotation).X * 2, Vector2.UnitY.RotatedBy(npc.rotation).Y * 2);
                }
            }
            if (anim.Time <= _2To3PhaseTransitionAnimation.RotationStart)
            {

            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.RotationTime1End)
            {
                float oldRotation = npc.rotation;
                npc.rotation = anim.StartRotation + (anim.EndRotation1 - anim.StartRotation) * EasingInOut(_2To3PhaseTransitionAnimation.RotationTime1End - _2To3PhaseTransitionAnimation.RotationStart, anim.Time - _2To3PhaseTransitionAnimation.RotationStart, false, 0.99f);
                float rotationVelocity = npc.rotation - oldRotation;
                if(anim.Time > _2To3PhaseTransitionAnimation.Phase3TimeStart)
                {
                    anim.GorePosition[0] = npc.Center;
                    anim.GoreRotation[0] = npc.rotation;
                }
                if (anim.Time == (_2To3PhaseTransitionAnimation.RotationTime1End - _2To3PhaseTransitionAnimation.RotationStart) * 0.99f + _2To3PhaseTransitionAnimation.RotationStart)
                {
                    anim.GoreActive[0] = false;
                    anim.GoreActive[1] = true;
                    anim.GoreActive[2] = true;
                    anim.GorePosition[1] = npc.Center;
                    anim.GoreRotation[1] = npc.rotation;
                    anim.GoreDrawSource[1] = drawCenter;
                    anim.GoreDrawSource[2] = new Vector2(55, 139);
                    anim.GorePosition[2] = anim.GorePosition[1] + (anim.GoreDrawSource[2] - anim.GoreDrawSource[1]).RotatedBy(npc.rotation);
                    anim.GoreRotation[2] = npc.rotation;
                    anim.GoreAngularVelocity[2] = rotationVelocity;
                    anim.GoreVelocity[2] = anim.GorePosition[2] - (npc.Center + (anim.GoreDrawSource[2] - anim.GoreDrawSource[1]).RotatedBy(oldRotation));
                    anim.GorePhisics[2] = true;
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, anim.GoreVelocity[2].X, anim.GoreVelocity[2].Y, Scale: 2);
                    }
                }
                else if (anim.Time > (_2To3PhaseTransitionAnimation.RotationTime1End - _2To3PhaseTransitionAnimation.RotationStart) * 0.99f + _2To3PhaseTransitionAnimation.RotationStart)
                {
                    anim.GorePosition[1] = npc.Center + Vector2.UnitY.RotatedBy(npc.rotation) * 2;
                    anim.GoreRotation[1] = npc.rotation;
                    anim.GoreDrawSource[1] = anim.Gore1.Size() - npc.Hitbox.Size() / 2;
                }
            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.RotationTime2End)
            {
                if (anim.Time == _2To3PhaseTransitionAnimation.RotationTime1End)
                {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                    anim.GoreActive[1] = false;
                    anim.GoreActive[3] = true;
                    anim.GoreActive[4] = true;
                    anim.GoreDrawSource[3] = drawCenter;
                    anim.GoreDrawSource[4] = new Vector2(55, 93);
                }
                float oldRotation = npc.rotation;
                npc.rotation = anim.EndRotation1 + (anim.EndRotation2 - anim.EndRotation1) * EasingInOut(_2To3PhaseTransitionAnimation.RotationTime2End - _2To3PhaseTransitionAnimation.RotationTime1End, anim.Time - _2To3PhaseTransitionAnimation.RotationTime1End, false, 0.8f);
                anim.GorePosition[3] = npc.Center;
                anim.GoreRotation[3] = npc.rotation;
                if (anim.Time <= (_2To3PhaseTransitionAnimation.RotationTime2End - _2To3PhaseTransitionAnimation.RotationTime1End) * 0.8f + _2To3PhaseTransitionAnimation.RotationTime1End)
                {
                    Vector2 oldPos = anim.GorePosition[4];
                    float oldRot = anim.GoreRotation[4];
                    anim.GorePosition[4] = anim.GorePosition[3] + (anim.GoreDrawSource[4] - anim.GoreDrawSource[3]).RotatedBy(npc.rotation);
                    anim.GoreRotation[4] = npc.rotation - (npc.rotation - oldRotation) * 4;
                    if (anim.Time == (int)((_2To3PhaseTransitionAnimation.RotationTime2End - _2To3PhaseTransitionAnimation.RotationTime1End) * 0.8f + _2To3PhaseTransitionAnimation.RotationTime1End))
                    {
                        anim.GoreVelocity[4] = (anim.GorePosition[4] - oldPos) * 0.8f;
                        anim.GoreAngularVelocity[4] = anim.GoreRotation[4] - oldRot;
                        anim.GorePhisics[4] = true;
                        anim.GorePosition[4] += (new Vector2(41, 117) - new Vector2(55, 93)).RotatedBy(anim.GoreRotation[4]);
                        anim.GoreDrawSource[4] = new Vector2(41, 117);
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, anim.GoreVelocity[4].X, anim.GoreVelocity[4].Y, Scale: 2);
                        }
                    }
                }
            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.RotationTime3End)
            {
                if (anim.Time == _2To3PhaseTransitionAnimation.RotationTime2End)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                }
                float oldRotation = npc.rotation;
                npc.rotation = anim.EndRotation2 + (anim.EndRotation3 - anim.EndRotation2) * EasingInOut(_2To3PhaseTransitionAnimation.RotationTime3End - _2To3PhaseTransitionAnimation.RotationTime2End, anim.Time - _2To3PhaseTransitionAnimation.RotationTime2End, false, 0.8f);
                if (anim.Time <= (_2To3PhaseTransitionAnimation.RotationTime3End - _2To3PhaseTransitionAnimation.RotationTime2End) * 0.8f + _2To3PhaseTransitionAnimation.RotationTime2End)
                {
                    anim.GoreDrawSource[3] = new Vector2(55, 93);
                    Vector2 oldPos = anim.GorePosition[3];
                    float oldRot = anim.GoreRotation[3];
                    anim.GorePosition[3] = npc.Center + (anim.GoreDrawSource[3] - drawCenter).RotatedBy(npc.rotation);
                    anim.GoreRotation[3] = npc.rotation - (npc.rotation - oldRotation) * 4;
                    if (anim.Time == (int)((_2To3PhaseTransitionAnimation.RotationTime3End - _2To3PhaseTransitionAnimation.RotationTime2End) * 0.8f + _2To3PhaseTransitionAnimation.RotationTime2End))
                    {
                        anim.GoreVelocity[3] = (anim.GorePosition[3] - oldPos) * 0.8f;
                        anim.GoreAngularVelocity[3] = anim.GoreRotation[3] - oldRot;
                        anim.GorePhisics[3] = true;
                        anim.GorePosition[3] += (new Vector2(70, 117) - new Vector2(55, 93)).RotatedBy(anim.GoreRotation[4]);
                        anim.GoreDrawSource[3] = new Vector2(70, 117);
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, anim.GoreVelocity[3].X, anim.GoreVelocity[3].Y, Scale: 2);
                        }
                    }
                }
            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.CoughTimeStart)
            {
                npc.rotation = anim.EndRotation3;
            }
            else if (anim.Time <= _2To3PhaseTransitionAnimation.Cough4TimeEnd)
            {
                if (anim.Time == _2To3PhaseTransitionAnimation.CoughTimeStart)
                {
                    SoundStyle sound = new SoundStyle(SoundID.ForceRoarPitched.SoundPath);
                    sound.Pitch += 0.5f;
                    SoundEngine.PlaySound(sound, npc.Center);
                }
                if (anim.Time <= _2To3PhaseTransitionAnimation.Cough1TimeEnd && anim.Time >= _2To3PhaseTransitionAnimation.CoughTimeStart)
                {
                    npc.Center = anim.EoCPositionStart + (anim.EoCPositionEnd - anim.EoCPositionStart) * EasingInOut(_2To3PhaseTransitionAnimation.Cough1TimeEnd - _2To3PhaseTransitionAnimation.CoughTimeStart, anim.Time - _2To3PhaseTransitionAnimation.CoughTimeStart, true);
                    if (anim.Time == _2To3PhaseTransitionAnimation.Cough1TimeEnd)
                    {
                        SoundStyle sound = new SoundStyle(SoundID.ForceRoarPitched.SoundPath);
                        sound.Pitch += 0.5f;
                        SoundEngine.PlaySound(sound, npc.Center);
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, SpeedY: 6, Scale: 2);
                        }
                    }
                }
                else if (anim.Time <= _2To3PhaseTransitionAnimation.Cough2TimeEnd)
                {
                    npc.Center = anim.EoCPositionStart + (anim.EoCPositionEnd - anim.EoCPositionStart) * EasingInOut(_2To3PhaseTransitionAnimation.Cough2TimeEnd - _2To3PhaseTransitionAnimation.Cough1TimeEnd, anim.Time - _2To3PhaseTransitionAnimation.Cough1TimeEnd, true);
                    if (anim.Time == _2To3PhaseTransitionAnimation.Cough2TimeEnd)
                    {
                        SoundStyle sound = new SoundStyle(SoundID.ForceRoarPitched.SoundPath);
                        sound.Pitch += 0.5f;
                        SoundEngine.PlaySound(sound, npc.Center);
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, SpeedY: 6, Scale: 2);
                        }
                    }
                }
                else if (anim.Time <= _2To3PhaseTransitionAnimation.Cough3TimeEnd)
                {
                    npc.Center = anim.EoCPositionStart + (anim.EoCPositionEnd - anim.EoCPositionStart) * EasingInOut(_2To3PhaseTransitionAnimation.Cough3TimeEnd - _2To3PhaseTransitionAnimation.Cough2TimeEnd, anim.Time - _2To3PhaseTransitionAnimation.Cough2TimeEnd, true);
                    if (anim.Time == _2To3PhaseTransitionAnimation.Cough3TimeEnd)
                    {
                        SoundStyle sound = new SoundStyle(SoundID.ForceRoarPitched.SoundPath);
                        sound.Pitch += 0.5f;
                        SoundEngine.PlaySound(sound, npc.Center);
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, SpeedY: 6, Scale: 2);
                        }
                    }
                }
                else if (anim.Time <= _2To3PhaseTransitionAnimation.Cough4TimeEnd)
                {
                    npc.Center = anim.EoCPositionStart + (anim.EoCPositionEnd - anim.EoCPositionStart) * EasingInOut(_2To3PhaseTransitionAnimation.Cough4TimeEnd - _2To3PhaseTransitionAnimation.Cough3TimeEnd, anim.Time - _2To3PhaseTransitionAnimation.Cough3TimeEnd, true);
                    if (anim.Time == _2To3PhaseTransitionAnimation.Cough3TimeEnd + (int)((_2To3PhaseTransitionAnimation.Cough4TimeEnd - _2To3PhaseTransitionAnimation.Cough3TimeEnd) * 0.5))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, SpeedY: 6, Scale: 2);
                        }
                        lens.freeFall = true;
                        lens.velocity = (npc.position - npc.GetT().oldPositions[0]) * 0.8f;
                    }
                }
                if (!lens.freeFall)
                {
                    Vector2 acceleration = (npc.position - npc.GetT().oldPositions[0]) - (npc.GetT().oldPositions[0] - npc.GetT().oldPositions[1]);
                    float length = acceleration.Length();
                    if (length != 0)
                    {
                        anim.LensPosition -= (acceleration / length) * MathF.Max(0, length - 2);
                    }
                    lens.rotation = npc.rotation + MathF.PI / 2;
                    lens.Center = npc.Center + anim.LensPosition;
                }
            }
            else if (anim.Time <= _2To3PhaseTransitionAnimation.Rotation4TimeStart)
            {
                if (anim.Time == _2To3PhaseTransitionAnimation.Holder1SpawnTime)
                {
                    holders[0] = new LensHolder(npc, true);
                    holdersCount++;
                }
                if (anim.Time == _2To3PhaseTransitionAnimation.Holder2SpawnTime)
                {
                    holders[1] = new LensHolder(npc, false);
                    holdersCount++;
                }
            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.Rotation4TimeEnd)
            {
                npc.rotation = anim.EndRotation3 + (anim.EndRotation4 - anim.EndRotation3) * EasingInOut(_2To3PhaseTransitionAnimation.Rotation4TimeEnd - _2To3PhaseTransitionAnimation.Rotation4TimeStart, anim.Time - _2To3PhaseTransitionAnimation.Rotation4TimeStart);
            }
            else if (anim.Time < _2To3PhaseTransitionAnimation.ChargingTimeEnd)
            {
                if (anim.Time == _2To3PhaseTransitionAnimation.Rotation4TimeEnd)
                {
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                }
                int dust = Dust.NewDust(npc.Center + Vector2.UnitX * (250 - 1.5f * npc.width) - Vector2.UnitY * 1.5f * npc.height, npc.width * 3, npc.height * 3, ModContent.DustType<MagicParticles>());
                Main.dust[dust].customData = npc.whoAmI;
                if (anim.Time >= _2To3PhaseTransitionAnimation.LaserTimeStart)
                {
                    lens.Style = 1;
                    laser = true;
                    laserAngle = EasingOut(_2To3PhaseTransitionAnimation.ChargingTimeEnd - _2To3PhaseTransitionAnimation.LaserTimeStart, anim.Time - _2To3PhaseTransitionAnimation.LaserTimeStart) * MathF.PI / 4;
                    //Terrapain.screenColor = Color.White * MathF.Min(1, MathF.Max((180 - anim.Time + _2To3PhaseTransitionAnimation.LaserTimeStart) / 60f, 0));
                }
                npc.rotation = anim.EndRotation4;
                npc.Center = anim.EoCPositionStart + (anim.ChargingPositionEnd - anim.EoCPositionStart) * EasingIn(_2To3PhaseTransitionAnimation.ChargingTimeEnd - _2To3PhaseTransitionAnimation.Rotation4TimeEnd, anim.Time - _2To3PhaseTransitionAnimation.Rotation4TimeEnd);
                SmoothMoovingCameraModifier.TargetPosition = npc.Center - Main.ScreenSize.ToVector2() / 2;
            }
            else if (anim.Time == _2To3PhaseTransitionAnimation.ChargingTimeEnd)
            {
                canHit = true;
                laserAngle = MathF.PI / 4;
                Main.hideUI = anim.saveUI?? false;
                anim.Active = false;
                if (npc.life > 1)
                {
                    npc.immortal = false;
                    NextAttack3(npc);
                }
                else
                {
                    mainTimer = 800;
                    phase = 5;
                    attack = 0;
                }
            }
            anim.Time++;
        }
        float angularVelocity;
        float goalRotation;
        //use to stop attacks
        int mainTimer;
        // 0, 1 - using in atacks, 2 using in find frames, 3 using in post update portals
        int[] timers = new int[4];
        int[] dir = new int[2];
        int phase = 1;
        int attack;
        int attackCounter;
        public static int[] attacks1 = { 0, 1, 2, 0, 1, 3 };
        public static int[] attacks2 = { 0, 1, -1, 2, -1, 3, -1, 4, -1, 2, -1, 1, -1 };
        public static int[] attacks3 = { 1, -1, 0, -1, 2, -1 };
        public static int[] attacks4 = { 0, 1, 0, 2, 0, 3, 0, 4, 0, 5 };
        int animationSpeed = 12;
        int frame;
        bool dash;
        float maxVelocity = 20;
        float maxVelocityMultiplyer;
        Vector2 targetPosition;
        int defDamage = 12;
        List<int> portalDusts = new List<int>();
        bool canHit = false;
        bool canBeHit = true;
        public override void ModSetDefaults(NPC entity)
        {
            entity.GetT().canselDeathHitEffect = true;
            entity.life = (int)(entity.life * 1.35f);
        }
        public override int type => NPCID.EyeofCthulhu;
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (timers[2] == 0)
            {
                frame++;
                timers[2] = animationSpeed;
                if (frame > 2)
                {
                    frame = 0;
                }
            }
            npc.frame.Y = frameHeight * frame;
            if (phase > 2 || clone)
            {
                npc.frame.Y += frameHeight * 3;
            }
        }
        public bool clone;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            anim = new _2To3PhaseTransitionAnimation(npc);
            if (source.Context == "clone")
            {
                canHit = true;
                clone = true;
                npc.GetT().useVanillaDrawing = false;
                npc.GetT().useModDrawingInPreDraw = true;
                npc.GetT().drawCenter = new Vector2(npc.frame.Width / 2, npc.frame.Height - 98 + npc.height / 2);
                mainTimer = (int)npc.ai[2] + 25;
                npc.rotation = npc.ai[0] + npc.AngleTo(t.Target.Center) - MathF.PI / 2;
            }
        }
        public override void OnFirstTick(NPC npc)
        {
            dir[1] = 1;
            drawCenter = new Vector2(55, 38 + npc.height / 2);
        }
        public override void BossHeadSlot(NPC npc, ref int index)
        {
            index = 0;
            if (phase > 2)
            {
                index = 1;
            }
        }
        public override bool ModPreAI(NPC npc)
        {
            portal = false;
            t.damageMultiplier += 3;
            if (!clone)
            {
                npc.defense = Main.dayTime? 15 : 12;
                maxVelocityMultiplyer = 1;
                Lasers = new List<DrawLaser>();
                if (!anim.Active)
                {
                    npc.TargetClosest();
                    if (npc.target < 0 || npc.target > Main.player.Length || !t.Target.active || t.Target.dead)
                    {
                        npc.ai[0] = -201;
                        npc.active = false;
                    }
                    switch (phase)
                    {
                        case 1:
                            DoFirstPhase(npc);
                            break;
                        case 2:
                            DoSecondPhase(npc);
                            break;
                        case 3:
                            DoThirdPhase(npc);
                            break;
                        case 4:
                            DoFourthPhase(npc);
                            break;
                        case 5:
                            DoDesperation(npc);
                            break;
                    }


                    if (targetPosition != npc.Center && (phase == 2 || phase == 4 || (phase == 1 && attack == 0) || (phase == 3 && (attack == -1 || (attack == 1 && mainTimer > 800)))))
                    {
                        npc.velocity += npc.DirectionTo(targetPosition) * acceleration;
                        Vector2 vectorToTargetPosition = targetPosition - npc.Center;
                        float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, npc.velocity);
                        positiveRotation = NormalizeRotation(positiveRotation);
                        float negativeRotation = AngleBetweenVectors(npc.velocity, vectorToTargetPosition);
                        negativeRotation = NormalizeRotation(negativeRotation);
                        if (positiveRotation > negativeRotation)
                        {
                            npc.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                        }
                        else
                        {
                            npc.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                        }
                    }
                    if (npc.velocity.Length() > maxVelocity * maxVelocityMultiplyer)
                    {
                        if (dash)
                            npc.velocity -= npc.velocity.Normalized() * 0.8f;
                        else
                        {
                            npc.velocity.Normalize();
                            npc.velocity *= maxVelocity * maxVelocityMultiplyer;
                        }
                    }
                    AngularAcceleration(ref angularVelocity, 0.1f, 0.3f, goalRotation - MathF.PI / 2, ref npc.rotation);
                }

                if (timers[3] > 0)
                {
                    PostUpdatePortalOut(timers[3] - 1);
                    if (timers[3] == 11)
                        PostUpdatePortalIn();
                }
                if (anim.Active)
                {
                    UpdateAnimation(npc);
                }
                if (lens.active)
                {
                    lens.Update(npc);
                    for (int i = 0; i < holdersCount; i++)
                    {
                        holders[i].Update(ref lens);
                    }
                }
                if (laser)
                {
                    LightingSystem.AddLight(npc.Center.ToTileCoordinates(), Vector3.One * laserAngle * 10 / MathF.PI * 4);
                    focus = FindFocus(npc);
                    if (laserAngle > MathF.PI / 6)
                    LaserProjectileUpdate(npc);
                }
            }
            else
            {
                if (mainTimer == 25)
                {
                    npc.velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 80;
                    t.afterimage = true;
                }
                else if (mainTimer < 25)
                {
                    npc.velocity -= npc.velocity.Normalized() * 0.5f;
                }
                if (mainTimer == 0)
                {
                    npc.active = false;
                }
            }
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] > 0)
                    timers[i]--;
            }
            if (mainTimer > 0)
                mainTimer--;
            return false;
        }
        public float acceleration = 4;
        public List<int> servantsTypes;
        Vector2 EoCPosForPhase1Attack3ToFixTheShaking;
        Vector2 EoCVelForPhase1Attack3ToFixTheShaking;
        public void DoFirstPhase(NPC npc)
        {
            if (mainTimer <= timemax && attackCounter + 1 < attacks1.Length && attacks1[attackCounter + 1] == 2)
            {
                MakePortals1(npc, mainTimer);
            }
            if (mainTimer <= 20 && attackCounter + 1 < attacks1.Length && attacks1[attackCounter + 1] == 3)
            {
                MakePortals2(npc, mainTimer);
            }
            if (attack == 0)
            {
                targetPosition = t.Target.Center + Vector2.UnitY * 400;
                if (npc.Center.Distance(t.Target.Center + Vector2.UnitY * 400) < 80)
                {
                    canHit = true;
                    NextAttack1(npc);
                }
                goalRotation = AngleFromVector(npc.velocity);
            }
            if (attack == 1)
            {
                //t.Target.wingTime++;
                if (dir[0] == 0)
                {
                    dir[0] = npc.velocity.X.NonZeroSign() == 1 ? 1 : -1;
                }
                if (npc.Center.X > t.Target.Center.X + 400)
                {
                    dir[0] = -1;
                }
                if (npc.Center.X < t.Target.Center.X - 400)
                {
                    dir[0] = 1;
                }
                npc.velocity += npc.DirectionTo(t.Target.Center + Vector2.UnitY * 400 + Vector2.UnitX * 400 * dir[0]) * acceleration;
                goalRotation = AngleFromVector(npc.DirectionTo(SmartShoot(npc.Center, 20, t.Target.Center, t.Target.velocity, 60)));
                if (timers[0] == 0)
                {
                    Vector2 velocity = (SmartShoot(npc.Center, 20, t.Target.Center, t.Target.velocity, 60) - npc.Center).Normalized() * 5;
                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                    timers[0] = Main.dayTime? 18 : 21;
                }
                if (WorldDifficultySystem.suicide && timers[1] > 15)
                {
                    Vector2 vector2 = Vector2.Zero;
                    for (int i = 0; i < 10; i++)
                    {
                        vector2 += t.Target.Custom().oldVelocities[i];
                    }
                    vector2 /= 10;
                    npc.ai[2] = AngleFromVector(SmartShoot(t.Target.Center + Vector2.UnitX * 800, 20, t.Target.Center, vector2, 150) - (t.Target.Center + Vector2.UnitX * 800));
                    npc.ai[3] = AngleFromVector(SmartShoot(t.Target.Center + Vector2.UnitX * 800 * -1, 20, t.Target.Center, vector2, 150) - (t.Target.Center + Vector2.UnitX * 800 * -1));
                }
                if (timers[1] == 0)
                {
                    for (int i = 0; i < 21; i++)
                    {
                        Vector2 velocity = Vector2.UnitX * 5;
                        if (WorldDifficultySystem.suicide)
                        {
                            velocity = velocity.RotatedBy(npc.ai[(int)(2.5f + dir[1] * 0.5f)]);
                        }
                        else
                        {
                            velocity *= dir[1];
                        }
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), t.Target.Center + Vector2.UnitX * 800 * -dir[1] + Vector2.UnitY * 80 - Vector2.UnitY * 8 * i, velocity, ModContent.ProjectileType<DemonicEyeLazer>(), defDamage, 2, ai2: -1);
                        Main.projectile[proj].friendly = false;
                        Main.projectile[proj].hostile = true;
                        if (WorldDifficultySystem.suicide)
                        {
                            dir[1] *= -1;
                        }
                    }
                    if (WorldDifficultySystem.torture)
                    {
                        dir[1] *= -1;
                    }
                    timers[1] = 120;
                }
                if (timers[1] < mainTimer)
                {
                    int _dir = dir[1];
                    for (int i = 0; i < 21; i++)
                    {
                        Vector2 velocity = Vector2.UnitX * 5;
                        if (WorldDifficultySystem.suicide)
                        {
                            velocity = velocity.RotatedBy(npc.ai[(int)(2.5f + _dir * 0.5f)]);
                        }
                        else
                        {
                            velocity *= _dir;
                        }
                        DrawLaser laser = new DrawLaser();
                        laser.width = 6;
                        laser.start = t.Target.Center + Vector2.UnitX * 800 * -_dir + Vector2.UnitY * 80 - Vector2.UnitY * 8 * i;
                        laser.end = laser.start + velocity * 600;
                        laser.color = Color.Red;
                        laser.color.A = (byte)(208 * EasingIn(50, 100 - timers[1]));
                        laser.color.R = ClientConfig.Instance.UseShaders? laser.color.A : (byte)255;
                        if (WorldDifficultySystem.suicide)
                        {
                            _dir *= -1;
                        }
                        Lasers.Add(laser);
                    }
                }
                if (mainTimer == 0)
                {
                    NextAttack1(npc);
                }
            }
            if (attack == 2)
            {
                timemax = 30;
                if (timers[0] <= timemax && timers[0] < mainTimer)
                {
                    MakePortals1(npc, timers[0]);
                }
                if (timers[0] == 0)
                {
                    timers[0] = 70 - 10 * WorldDifficultySystem.TerrapainDifficulty;

                    float rotation = npc.ai[2];
                    npc.Center = t.Target.Center - UnitVectorFromRotation(rotation) * 575;
                    if (WorldDifficultySystem.suicide)
                    {
                        Vector2 vector = (SmartShoot(npc.Center, maxVelocity, t.Target.Custom().oldCenters[Main.getGoodWorld ? 4 : 6], t.Target.Custom().oldVelocities[Main.getGoodWorld ? 4 : 6], 60) - npc.Center).Normalized();
                        rotation = AngleFromVector(vector);
                    }
                    npc.rotation = rotation - MathF.PI / 2;
                    goalRotation = rotation;
                    npc.velocity = UnitVectorFromRotation(rotation) * (50 - WorldDifficultySystem.TerrapainDifficulty * 5);
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                    if (Main.dayTime)
                    {
                        int count = 10;
                        for(int i = 0; i < count; i++)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.UnitX.RotatedBy(npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i) * 5, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        }
                    }
                }
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, -npc.velocity.X * 0.5f, -npc.velocity.Y * 0.5f, Scale: 2);
                if (timers[0] == 1)
                {
                    canHit = true;
                }

                if (mainTimer == 0)
                {
                    maxVelocity = 20;
                    portalDusts = new List<int>();
                    npc.ai[0] = 0;
                    npc.ai[1] = 0;

                    timers[0] = 0;
                    timers[1] = 0;
                    NextAttack1(npc);
                }
                Lighting.AddLight(npc.Center, 2, 0, 0);
            }
            if (attack == 3)
            {
                npc.velocity = Vector2.Zero;
                //if (timers[0] == 0)
                //{
                //    dir[0] = random.Next(2) == 1 ? 1 : -1;
                //    dir[1] = random.Next(2);
                //    npc.Center = t.Target.Center + Vector2.UnitX * 800 * -dir[0];
                //    npc.acceleration = Vector2.UnitX * 50 * dir[0];
                //    if (dir[1] == 0)
                //    {
                //        int count = 8 + WorldDifficultySystem.TerrapainDifficulty * 2;
                //        for (int i = 0; i < count; i++)
                //        {
                //            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.UnitY * 700 - Vector2.UnitY * 1400f / (count - 1) * i, Vector2.UnitX * 5 * dir[0], ModContent.ProjectileType<DemonicEyeLazer>(), npc.defDamage, 2, ai2: -1);
                //            Main.projectile[proj].friendly = false;
                //            Main.projectile[proj].hostile = true;
                //        }
                //    }
                //    SoundEngine.PlaySound(SoundID.Roar, npc.position);
                //    npc.rotation = MathF.PI / 2 - MathF.PI / 2 * dir[0] - MathF.PI / 2;
                //    goalRotation = MathF.PI / 2 - MathF.PI / 2 * dir[0];
                //    timers[0] = 80;
                //    dash = true;
                //}
                //if (dir[1] == 1 && timers[1] == 0)
                //{
                //    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.UnitY * 5, ModContent.ProjectileType<DemonicEyeLazer>(), npc.defDamage, 2, ai2: 1);
                //    Main.projectile[proj].friendly = false;
                //    Main.projectile[proj].hostile = true;

                //    proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.UnitY * -5, ModContent.ProjectileType<DemonicEyeLazer>(), npc.defDamage, 2, ai2: 1);
                //    Main.projectile[proj].friendly = false;
                //    Main.projectile[proj].hostile = true;

                //    timers[1] = 7 - WorldDifficultySystem.TerrapainDifficulty;
                //}
                int TimeToPredict = 0;
                float predictRot = npc.rotation;
                Vector2 PredictPos = EoCPosForPhase1Attack3ToFixTheShaking;
                Vector2 PredictVelocity = EoCVelForPhase1Attack3ToFixTheShaking;
                while (TimeToPredict < 30)
                {
                    if (TimeToPredict > mainTimer)
                    {
                        break;
                    }
                    if (TimeToPredict >= timers[0] && timers[0] > 20)
                    {
                        break;
                    }
                    if (TimeToPredict == timers[0] && timers[0] <= 20)
                    {
                        PredictPos = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * 600;
                        PredictVelocity = UnitVectorFromRotation(npc.ai[2]) * 25;
                        PredictVelocity = PredictVelocity.RotatedBy(0.25f * MathF.PI);
                        predictRot = npc.ai[2];
                    }
                    predictRot += 0.125f * MathF.PI * (0.6f - WorldDifficultySystem.TerrapainDifficulty * 0.1f);
                    if ((TimeToPredict + timers[1]) % 2 == 0)
                    {
                        DrawLaser laser = new DrawLaser();
                        laser.start = PredictPos;
                        laser.end = PredictPos + Vector2.UnitY.RotatedBy(predictRot) * 1000;
                        laser.width = 8;
                        laser.color = Color.Purple;
                        if (ClientConfig.Instance.UseShaders)
                        {
                            laser.color *= (30 - TimeToPredict) / 75f;
                        }
                        else
                        {
                            laser.color.A = (byte)((30 - TimeToPredict) / 75f * 255);
                        }
                        Lasers.Add(laser);
                    }
                    PredictVelocity += PredictPos.DirectionTo(t.Target.Center) * PredictPos.Distance(t.Target.Center) / 500;
                    if (PredictVelocity.Length() > maxVelocity * maxVelocityMultiplyer)
                    {
                        PredictVelocity -= PredictVelocity.Normalized() * 0.8f;
                    }
                    PredictPos += PredictVelocity;
                    TimeToPredict++;
                }
                if (timers[0] <= 20 && timers[0] < mainTimer)
                {
                    MakePortals2(npc, timers[0]);
                }
                if (timers[0] == 0)
                {
                    timers[0] = 60;
                    EoCPosForPhase1Attack3ToFixTheShaking = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * 600;
                    EoCVelForPhase1Attack3ToFixTheShaking = UnitVectorFromRotation(npc.ai[2]) * 25;
                    EoCVelForPhase1Attack3ToFixTheShaking = EoCVelForPhase1Attack3ToFixTheShaking.RotatedBy(0.25f * MathF.PI);
                    npc.rotation = npc.ai[2];
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                }
                npc.rotation += 0.125f * MathF.PI * (0.6f - WorldDifficultySystem.TerrapainDifficulty * 0.1f);
                goalRotation = npc.rotation + MathF.PI * 0.5f;
                Vector2 velocity = UnitVectorFromRotation(goalRotation) * 5;
                if (timers[1] == 0)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), EoCPosForPhase1Attack3ToFixTheShaking, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                    if (Main.dayTime)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), EoCPosForPhase1Attack3ToFixTheShaking - Vector2.UnitY.RotatedBy(goalRotation) * 3, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), EoCPosForPhase1Attack3ToFixTheShaking + Vector2.UnitY.RotatedBy(goalRotation) * 3, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                    }
                    timers[1] = 2;
                }
                EoCVelForPhase1Attack3ToFixTheShaking += EoCPosForPhase1Attack3ToFixTheShaking.DirectionTo(t.Target.Center) * EoCPosForPhase1Attack3ToFixTheShaking.Distance(t.Target.Center) / 500;
                if (EoCVelForPhase1Attack3ToFixTheShaking.Length() > maxVelocity * maxVelocityMultiplyer)
                {
                    EoCVelForPhase1Attack3ToFixTheShaking -= EoCVelForPhase1Attack3ToFixTheShaking.Normalized() * 0.8f;
                }
                EoCPosForPhase1Attack3ToFixTheShaking += EoCVelForPhase1Attack3ToFixTheShaking;
                npc.Center = EoCPosForPhase1Attack3ToFixTheShaking;
                if (mainTimer == 0)
                {
                    npc.velocity = EoCVelForPhase1Attack3ToFixTheShaking;
                    npc.ai[0] = 0;
                    npc.ai[1] = 0;
                    dash = false;
                    NextAttack1(npc);
                }
                //Lighting.AddLight(npc.Center, 2, 0, 0);
            }
        }
        public void DoSecondPhase(NPC npc)
        {
            goalRotation = AngleFromVector(t.Target.Center - npc.Center);
            if (attack != 3 && attack != 4)
            {
                targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center) * 500;
                if (npc.Distance(targetPosition) < 175)
                {
                    maxVelocityMultiplyer = 1 - ((175 - npc.Distance(targetPosition)) / 175) * ((175 - npc.Distance(targetPosition)) / 175);
                }
            }
            if (attackCounter != -1)
            {
                switch (attack)
                {
                    case -1:
                        if (mainTimer == 0)
                        {
                            NextAttack2(npc);
                        }
                        break;
                    case 0:
                        if (timers[0] == 150)
                        {
                            servantsTypes = new List<int>([5, 5, ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.SheildedServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.SheildedServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(), ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>()]);
                            SoundEngine.PlaySound(SoundID.Roar);
                            npc.ai[0] = 0;
                        }
                        if (timers[0] > 0)
                        {
                            int index = random.Next(servantsTypes.Count);
                            goalRotation += EasingInOut(150, 150 - timers[0]) * 4 * MathF.PI;
                            if (timers[0] % 15 == 0)
                            {
                                if (servantsTypes[index] != ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>() || AllNPCByType(ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>()).Count < 2)
                                {
                                    int _npc = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index]);
                                    servantsTypes.RemoveAt(index);
                                    Main.npc[_npc].velocity = UnitVectorFromRotation(goalRotation) * 5;
                                }
                            }
                        }
                        if (mainTimer == 0)
                        {
                            NextAttack2(npc);
                        }
                        break;
                    case 1:
                        if (mainTimer > 480)
                        {
                            npc.ai[0] = 0;
                            goalRotation += EasingInOut(125, 125 - mainTimer + 600) * 4 * MathF.PI;
                            if (mainTimer % 5 == 0)
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("grid AI"), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>());
                                Main.npc[_npc].velocity = UnitVectorFromRotation(goalRotation) * 5;
                            }
                        }
                        else
                        {
                            t.Target.wingTime = 20;
                            GridLaserServants group = (GridLaserServants)Terrapain.group[Group.FindGroup("GridLaserServants")[0]];
                            if (group.members.Count < 24 && mainTimer % 10 == 0)
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("grid AI"), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>());
                                Main.npc[_npc].velocity = UnitVectorFromRotation(goalRotation) * 5;
                            }
                            if (mainTimer == 0)
                            {
                                NextAttack2(npc);
                                return;
                            }
                            if (mainTimer % 120 == 0)
                            {
                                int variant = WorldDifficultySystem.suicide? random.Next(2) : mainTimer == 480? 1 : 0;
                                group.target = t.Target;
                                switch (variant)
                                {
                                    case 0:
                                        {
                                            group.buildGrid = true;
                                            List<Vector2> points = new();
                                            List<float> rotations = new();
                                            for (int i = 1; i < 25; i++)
                                            {
                                                if (i % 3 == 0)
                                                {
                                                    points.Add(Vector2.UnitX.RotatedBy(i / 3 * MathF.PI / 4f - MathF.PI * (WorldDifficultySystem.suicide ? 0.05f : 0.08f)) * -450);
                                                    rotations.Add(i / 3 * MathF.PI / 4f + MathF.PI * 0.05f);
                                                }
                                                if (i % 3 == 1)
                                                {
                                                    points.Add(Vector2.UnitX.RotatedBy(i / 3 * MathF.PI / 4f) * -450);
                                                    rotations.Add(i / 3 * MathF.PI / 4f);
                                                }
                                                if (i % 3 == 2)
                                                {
                                                    points.Add(Vector2.UnitX.RotatedBy(i / 3 * MathF.PI / 4f + MathF.PI * (WorldDifficultySystem.suicide ? 0.05f : 0.08f)) * -450);
                                                    rotations.Add(i / 3 * MathF.PI / 4f - MathF.PI * 0.05f);
                                                }
                                            }
                                            group.points = points;
                                            group.rotatinos = rotations;
                                        }
                                        break;
                                    case 1:
                                        {
                                            group.buildGrid = true;
                                            List<Vector2> points = new();
                                            List<float> rotations = new();
                                            for (int i = 0; i < 18; i++)
                                            {
                                                points.Add(Vector2.UnitX.RotatedBy(i * MathF.PI / 9) * (WorldDifficultySystem.suicide ? -550 : -600));
                                                rotations.Add((i + 1) / 3 * MathF.PI / 3f);
                                                if (WorldDifficultySystem.suicide && i % 3 == 1)
                                                {
                                                    points.Add(Vector2.UnitX.RotatedBy((i + 0.5f) * MathF.PI / 9) * (WorldDifficultySystem.suicide ? -550 : -600));
                                                    rotations.Add((i / 3) * MathF.PI / 3 + MathF.PI / 6);
                                                }
                                            }
                                            group.points = points;
                                            group.rotatinos = rotations;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (mainTimer > 500)
                        {
                            npc.ai[0] = 0;
                            goalRotation += (EasingInOut(50, 50 - mainTimer + 500)) * 4 * MathF.PI;
                            if (mainTimer % 5 == 0)
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("ring AI"), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                                Main.npc[_npc].velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 10;
                            }
                        }
                        else
                        {
                            if (Group.FindGroup("RingAIServantsofCthulhu").Count == 0)
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("ring AI"), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                                Main.npc[_npc].velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 10;
                            }
                            RingAIServantsofCthulhu group = (RingAIServantsofCthulhu)Terrapain.group[Group.FindGroup("RingAIServantsofCthulhu")[0]];
                            if (group.Count < 7 + (WorldDifficultySystem.suicide? 1 : 0))
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("ring AI"), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                                Main.npc[_npc].velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 10;
                            }
                            if (mainTimer == 0)
                            {
                                NextAttack2(npc);
                                return;
                            }
                            if (mainTimer % 100 == 0)
                            {
                                group.target = npc.target;
                                group.timer = 100;
                                group.rotation = random.NextFloat(MathF.PI);
                            }
                        }
                        break;
                    case 3:
                        targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center).RotatedBy((EasingInOut(240, 240 - mainTimer) - EasingInOut(240, 240 - mainTimer - 1)) * MathF.PI * 4) * 500;
                        if (mainTimer % 3 == 0)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * 15, ModContent.ProjectileType<GhostServantofCthulhu>(), defDamage, 3);
                        }
                        if (mainTimer == 0)
                        {
                            NextAttack2(npc);
                        }
                        break;
                    case 4:
                        maxVelocityMultiplyer = 2.5f;
                        if (mainTimer > 400)
                        {
                            if (mainTimer == 750)
                            {
                                servantsTypes = new List<int>([
                                    5,
                                    5,
                                    ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                                ]);

                                SoundEngine.PlaySound(SoundID.Roar);
                                npc.ai[0] = 0;
                            }
                            if (mainTimer % 50 == 0)
                            {
                                int index = random.Next(servantsTypes.Count);
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center) * 20;
                                if (Main.getGoodWorld)
                                {
                                    _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                    Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(0.07f) * 20;
                                    _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                    Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(-0.07f) * 20;
                                }
                                servantsTypes.RemoveAt(index);
                                npc.ai[1] = random.NextFloat(MathF.PI * 2);
                            }
                        }
                        else
                        {
                            if (mainTimer == 400)
                            {
                                servantsTypes = new List<int>([
                                    5,
                                    5,
                                    ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                                    ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                                ]);
                            }
                            if (mainTimer == 0)
                            {
                                NextAttack2(npc);
                                break;
                            }
                            if (mainTimer > 100 && mainTimer % 50 == 0)
                            {
                                int index = random.Next(servantsTypes.Count);
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center) * 20;
                                if (WorldDifficultySystem.suicide || Main.getGoodWorld)
                                {
                                    _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                    Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(0.07f) * 20;
                                    _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], Target: npc.target);
                                    Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(-0.07f) * 20;
                                }
                                servantsTypes.RemoveAt(index);
                                npc.ai[1] = random.NextFloat(MathF.PI * 2);
                            }
                        }
                        targetPosition = t.Target.Center + Vector2.UnitX.RotatedBy(npc.ai[1]) * 500;
                        if (npc.Distance(targetPosition) < 75)
                        {
                            maxVelocityMultiplyer = (1 - (75 - npc.Distance(targetPosition)) / 75) * 2.5f;
                            if (mainTimer > 100)
                            {
                                DrawLaser laser = new DrawLaser();
                                laser.color = Color.Orange;
                                if (ClientConfig.Instance.UseShaders)
                                {
                                    laser.color *= mainTimer % 50 / 66f;
                                }
                                else
                                {
                                    laser.color.A = (byte)((50 - mainTimer % 50) / 66f);
                                }
                                laser.start = npc.Center;
                                laser.width = 15;
                                laser.end = npc.Center + npc.DirectionTo(t.Target.Center) * 350;
                                Lasers.Add(laser);
                                if (Main.getGoodWorld || (WorldDifficultySystem.suicide && mainTimer < 450))
                                {
                                    laser.end = npc.Center + npc.DirectionTo(t.Target.Center).RotatedBy(0.07f) * 350;
                                    Lasers.Add(laser);
                                    laser.end = npc.Center + npc.DirectionTo(t.Target.Center).RotatedBy(-0.07f) * 350;
                                    Lasers.Add(laser);
                                }
                            }
                        }
                        goalRotation = npc.DirectionTo(t.Target.Center).ToRotation();
                        break;
                }
            }
            else if (npc.Distance(t.Target.Center) < 520 && npc.Distance(t.Target.Center) > 480)
            {
                NextAttack2(npc);
            }
        }
        public void DoThirdPhase(NPC npc)
        {
            switch (attack)
            {
                case -1:
                    lens.Style = 0;
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center) * 250;
                    goalRotation = npc.Center.AngleTo(t.Target.Center);
                    if (npc.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultiplyer = 1 - (75 - npc.Distance(targetPosition)) / 75; 
                    }
                    if (laser && mainTimer > 15)
                    {
                        laserAngle -= MathF.PI / 4 / 15;
                        if (laserAngle <= 0)
                        {
                            laser = false;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack3(npc);
                    }
                    break;
                case 0:
                    if (timers[1] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                        timers[1] = 120;
                        //for (int i = 0; i < 6; i++)
                        //{
                        //    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX.RotatedBy(MathF.PI / 3 * i) * 25, ModContent.ProjectileType<GhostServantofCthulhu>(), defDamage, 3);
                        //}
                        //goalRotation = npc.DirectionTo(t.Target.Center).ToRotation();
                        //npc.velocity = npc.DirectionTo(t.Target.Center) * 40;
                        npc.velocity = goalRotation.ToRotationVector2() * 45;
                        t.afterimage = true;
                        t.afterimagesCount = 10;
                        dash = true;
                        dir[0] *= -1;
                        lens.Style = 1;
                        npc.ai[0] = 1;
                    }
                    if (timers[1] < 20)
                    {
                        npc.velocity = npc.DirectionFrom(t.Target.Center) * (1 - ((float)timers[0] / (20 - WorldDifficultySystem.TerrapainDifficulty * 2))) * 25;
                        goalRotation = npc.DirectionTo(t.Target.Custom().oldCenters[10]).ToRotation();
                    }
                    if (npc.velocity.Length() < 25)
                    {
                        dash = false;
                        t.afterimage = false;
                    }
                    if (timers[0] == 0 && timers[1] < 80 && npc.ai[0] == 1)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.UnitY.RotatedBy(npc.rotation) * 5, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        timers[0] = 5;
                    }
                    if (timers[1] < 80 && timers[1] > 25)
                    {
                        lens.Style = 2;
                        npc.velocity *= 0.95f;
                        float oldRotation = goalRotation;
                        goalRotation += (1 + 2 - timers[1] / 40f) * MathF.PI / 80 * dir[0];
                        float rotationToPlayer = npc.DirectionTo(t.Target.Center).ToRotation();
                        float oldRotationToPlayer = npc.DirectionTo(t.Target.Custom().oldPositions[0]).ToRotation();
                        if ((IsAngleBetweenAngles(oldRotation, rotationToPlayer, goalRotation) || IsAngleBetweenAngles(rotationToPlayer, oldRotation, oldRotationToPlayer)))
                        {
                            timers[1] = 25;
                        }
                        if (mainTimer == 1)
                        {
                            mainTimer++;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        lens.Style = 1;
                        dash = false;
                        t.afterimage = false;
                        NextAttack3(npc);
                        mainTimer = 120;
                    }
                    break;
                case 1:
                    npc.defense = 36;
                    if (mainTimer > 800)
                    {
                        lens.Style = 1;
                        targetPosition = t.Target.Center;
                        if (npc.Center.Distance(t.Target.Center) < 500)
                        {
                            goalRotation = npc.rotation + MathF.PI / 2;
                            mainTimer = 800;
                        }
                        else
                        {
                            goalRotation = AngleFromVector(npc.DirectionTo(t.Target.Center));
                        }
                    }
                    else
                    {
                        foreach (var player in Main.ActivePlayers)
                        {
                            player.wingTime = 20;
                        }
                        if (laserAngle < MathF.PI / 4)
                        {
                            laser = true;
                            laserAngle += MathF.PI / 4 / 15;
                        }
                        if (mainTimer < 600)
                        {
                            npc.immortal = true;
                        }
                        lens.Style = 2;
                        float Progress = 1 - mainTimer / 800f;
                        if (npc.velocity.Length() < 1)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.Normalized() * (npc.velocity.Length() - 1);
                        }
                        goalRotation += (1 + Progress * (Main.dayTime ? 2.1f : 1.8f)) * MathF.PI * 0.01f * dir[0];
                        if (timers[0] == 0)
                        {
                            int count = 3 + WorldDifficultySystem.TerrapainDifficulty;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis("Fly straight"), npc.Center, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (12 + (Main.getGoodWorld ? 4 : 0)), ModContent.ProjectileType<GhostServantofCthulhu>(), defDamage, 3);
                            }
                            timers[0] = 40 - (int)(Progress * 26);
                        }
                        if (npc.Distance(t.Target.Center) > 1500)
                        {
                            npc.ai[0] = -200;
                            if (npc.ai[0] == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Roar, npc.position);
                            }
                        }
                        else
                        {
                            npc.ai[0] = 0;
                        }
                        if (mainTimer == 0)
                        {
                            npc.immortal = false;
                            NextAttack3(npc);
                        }
                    }
                    break;
                case 2:
                    lens.Style = 0;
                    if (timers[0] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                        timers[0] = 80 - WorldDifficultySystem.TerrapainDifficulty * 5;
                        int count = 10 + WorldDifficultySystem.TerrapainDifficulty * 2;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, (Vector2.UnitX.RotatedBy(MathF.PI / count * 2 * i) + npc.DirectionTo(t.Target.Center) * 0.1f).ToUnit() * random.Next(21, 26), ModContent.ProjectileType<BloodSpike>(), defDamage, 4);
                        }
                        for (int i = 0; i < 200; i++)
                        {
                            Vector2 velocity = Vector2.UnitX.RotatedBy(random.NextFloat(MathF.PI * 2)) * 30 * random.NextFloat(0.5f, 1);
                            Dust.NewDust(npc.Center, 0, 0, DustID.Blood, velocity.X, velocity.Y, Scale : 2.5f);
                        }
                        dash = true;
                        t.afterimage = true;
                        npc.velocity = npc.DirectionTo(t.Target.Custom().oldCenters[10]) * 50;
                        goalRotation = npc.DirectionTo(t.Target.Custom().oldCenters[10]).ToRotation();
                    }
                    if (timers[0] < 25 - WorldDifficultySystem.TerrapainDifficulty * 2)
                    {
                        npc.velocity = npc.DirectionFrom(t.Target.Center) * (1 - ((float)timers[0] / (20 - WorldDifficultySystem.TerrapainDifficulty * 2))) * 25;
                        goalRotation = npc.DirectionTo(t.Target.Custom().oldCenters[10]).ToRotation();
                    }
                    if (laser && mainTimer > 15)
                    {
                        laserAngle -= MathF.PI / 4 / 15;
                        if(laserAngle <= 0)
                        {
                            laser = false;
                        }
                    }
                    if (timers[0] == 1 && mainTimer == 0)
                    {
                        dash = false;
                        t.afterimage = false;
                        NextAttack3(npc);
                    }
                    break;
            }
        }
        public void DoFourthPhase(NPC npc)
        {
            switch (attack)
            {
                case -1:
                    NextAttack4(npc);
                    //    if (npc.ai[2] == 0)
                    //    {
                    //        npc.ai[2] = t.Target.Center.X;
                    //        npc.ai[3] = t.Target.Center.Y;
                    //    }
                    //    if (npc.ai[0] == 0)
                    //    {
                    //        targetPosition = npc.DirectionFrom(new Vector2(npc.ai[2], npc.ai[3])) * 1000 + new Vector2(npc.ai[2], npc.ai[3]);
                    //        if (npc.Distance(targetPosition) < 25)
                    //        {
                    //            npc.ai[0] = 1;
                    //            npc.ai[1] = MathF.PI * 2;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        float rotation = NormalizeRotation(npc.position.DirectionTo(new Vector2(npc.ai[2], npc.ai[3])).ToRotation() - npc.oldPosition.DirectionTo(new Vector2(npc.ai[2], npc.ai[3])).ToRotation(), false);
                    //        targetPosition = npc.DirectionFrom(new Vector2(npc.ai[2], npc.ai[3])).RotatedBy(0.05f) * 1000 + new Vector2(npc.ai[2], npc.ai[3]);
                    //        npc.ai[1] -= rotation;
                    //        if (npc.ai[1] <= 0)
                    //        {
                    //            NextAttack4(npc);
                    //        }
                    //    }
                    //    goalRotation = npc.DirectionTo(targetPosition).ToRotation();
                    //    Point CenterTile = new Vector2(npc.ai[2], npc.ai[3]).ToTileCoordinates();
                    //    for (int x = npc.Center.ToTileCoordinates().X - 3; x <= npc.Center.ToTileCoordinates().X + 3; x++)
                    //    {
                    //        for (int y = npc.Center.ToTileCoordinates().Y - 3; y <= npc.Center.ToTileCoordinates().Y + 3; y++)
                    //        {
                    //            if (MathF.Sqrt((x - CenterTile.X) * (x - CenterTile.X) + (y - CenterTile.Y) * (y - CenterTile.Y)) > 59 && MathF.Sqrt((x - npc.Center.ToTileCoordinates().X) * (x - npc.Center.ToTileCoordinates().X) + (y - npc.Center.ToTileCoordinates().Y) * (y - npc.Center.ToTileCoordinates().Y)) <= 3)
                    //            {
                    //                if (Main.tile[x, y].HasTile)
                    //                {
                    //                    if (Main.tile[x, y].TileType != ModContent.TileType<HardenedCrimtaneBrick>() && Main.tile[x, y].TileType != ModContent.TileType<HardenedDemoniteBrick>())
                    //                    {
                    //                        Tile tile = new Tile();
                    //                        tile.CopyFrom(Main.tile[x, y]);
                    //                        EyeofCthulhuSystem.replasedTiles.Add(tile);
                    //                        EyeofCthulhuSystem.tileCoordinates.Add(new Point(x, y));
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    EyeofCthulhuSystem.tilesToClear.Add(new Point(x, y));
                    //                }
                    //                if (!Main.tile[x, y].HasTile || (Main.tile[x, y].TileType != ModContent.TileType<HardenedCrimtaneBrick>() && Main.tile[x, y].TileType != ModContent.TileType<HardenedDemoniteBrick>()))
                    //                {
                    //                    Main.tile[x, y].Clear(TileDataType.All);
                    //                    if (Main.ActiveWorldFileData.HasCrimson)
                    //                    {
                    //                        Main.tile[x, y].ResetToType((ushort)ModContent.TileType<HardenedCrimtaneBrick>());
                    //                    }
                    //                    else
                    //                    {
                    //                        Main.tile[x, y].ResetToType((ushort)ModContent.TileType<HardenedDemoniteBrick>());
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    break;
                case 0:
                    lens.Style = 0;
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center) * 250;
                    goalRotation = npc.Center.AngleTo(t.Target.Center);
                    if (npc.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultiplyer = 1 - (75 - npc.Distance(targetPosition)) / 75;
                    }
                    if (laser && mainTimer > 15)
                    {
                        laserAngle -= MathF.PI / 4 / 15;
                        if (laserAngle <= 0)
                        {
                            laser = false;
                        }
                    }
                    int next = attackCounter + 1;
                    if (next >= attacks4.Length)
                    {
                        next = 0;
                    }
                    switch (attacks4[next])
                    {
                        case 1:
                        case 3:
                        case 4:
                        case 5:
                            break;
                        default:
                            if (mainTimer < 15)
                            {
                                laser = true;
                                laserAngle += MathF.PI / 4 / 15;
                            }
                            break;
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack4(npc);
                    }
                    break;
                case 1:
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center).RotatedBy(1f * dir[0]) * (450 - 200 * ((300 - mainTimer) / 450f));
                    goalRotation = AngleFromVector(npc.DirectionTo(SmartShoot(npc.Center, 20, t.Target.Center, t.Target.velocity, 60)));
                    if (timers[0] == 0)
                    {
                        Vector2 velocity = (SmartShoot(npc.Center, 20, t.Target.Center, t.Target.velocity, 60) - npc.Center).Normalized() * 5;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        timers[0] = 25 - WorldDifficultySystem.TerrapainDifficulty * 5;
                        if (Main.dayTime)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + npc.rotation.ToRotationVector2() * 4, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + npc.rotation.ToRotationVector2() * -4, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack4(npc);
                    }
                    break;
                case 2:
                    npc.defense = 36;
                    if (mainTimer > 800)
                    {
                        lens.Style = 1;
                        targetPosition = t.Target.Center;
                        goalRotation = AngleFromVector(npc.DirectionTo(t.Target.Center));
                        if (npc.Center.Distance(t.Target.Center) < 500)
                        {
                            mainTimer = 801;
                        }
                    }
                    else if (mainTimer == 800)
                    {
                        servantsTypes = new List<int>([
                            5,
                            5,
                            ModContent.NPCType<Servants.EyeofCthulhu.SheildedServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.SheildedServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>(),
                        ]);
                    }
                    else
                    {
                        foreach (var player in Main.ActivePlayers)
                        {
                            player.wingTime = 20;
                        }
                        if (mainTimer < 600)
                        {
                            npc.immortal = true;
                        }
                        lens.Style = 2;
                        targetPosition = npc.Center;
                        float Progress = 1 - mainTimer / 800f;
                        if (npc.velocity.Length() < 1)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.Normalized() * (npc.velocity.Length() - 1);
                        }
                        goalRotation += (1 + Progress * (Main.dayTime ? 2.2f : 1.9f)) * MathF.PI * 0.009f * dir[0];
                        if (timers[0] == 0)
                        {
                            int count = 2 + WorldDifficultySystem.TerrapainDifficulty;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis("Fly straight"), npc.Center, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (10 + (Main.getGoodWorld ? 5 : 0)), ModContent.ProjectileType<GhostServantofCthulhu>(), defDamage, 3);
                            }
                            timers[0] = 40 - (int)(Progress * 25);
                        }
                        if (servantsTypes.Count != 0 && timers[1] == 0)
                        {
                            int index = 0;
                            if (servantsTypes[index] != ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>() || AllNPCByType(ModContent.NPCType<Servants.EyeofCthulhu.HealerServantofCthulhu>()).Count < 3)
                            {
                                int _npc = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index]);
                                servantsTypes.RemoveAt(index);
                                Main.npc[_npc].velocity = UnitVectorFromRotation(goalRotation) * 5;
                            }
                            timers[1] = 80 - (int)(Progress * 40);
                        }
                        if (npc.Distance(t.Target.Center) > 1500)
                        {
                            npc.ai[0] = -200;
                            if (npc.ai[0] == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Roar, npc.position);
                            }
                        }
                        else
                        {
                            npc.ai[0] = 0;
                        }
                        if (mainTimer == 0)
                        {
                            npc.immortal = false;
                            NextAttack4(npc);
                            npc.ai[0] = -201;
                        }
                    }
                    break;
                case 3:
                    npc.defense = 32;
                    lens.Style = 0;
                    targetPosition = t.Target.Center + npc.DirectionFrom(t.Target.Center) * 250;
                    goalRotation = npc.Center.AngleTo(t.Target.Center);
                    if (npc.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultiplyer = 1 - (75 - npc.Distance(targetPosition)) / 75;
                    }
                    if (timers[1] == 30)
                    {
                        int count = 3 + WorldDifficultySystem.TerrapainDifficulty;
                        float randomRotation = random.NextFloat(MathF.PI * 2);
                        for (int i = 0; i < count; i++)
                        {
                            float rotation = MathF.PI * 2 * i / count + randomRotation;
                            Projectile.NewProjectile(npc.GetSource_FromThis("ring AI"), t.Target.Center + Vector2.UnitX.RotatedBy(rotation) * 175, t.Target.velocity, ModContent.ProjectileType<GhostServantofCthulhu>(), defDamage, 2, ai0: rotation, ai2: npc.target);
                        }
                    }
                    if (timers[1] > 15 - WorldDifficultySystem.TerrapainDifficulty * 5)
                    {
                        Vector2 vector2 = Vector2.Zero;
                        for (int i = 0; i < 10; i++)
                        {
                            vector2 += t.Target.Custom().oldVelocities[i];
                        }
                        vector2 /= 10;
                        npc.ai[2] = vector2.X;
                        npc.ai[3] = vector2.Y;
                    }
                    if (timers[1] == 0)
                    {
                        int dir = 1;
                        Vector2 velocity1 = (SmartShoot(t.Target.Center + Vector2.UnitX * 800 * 1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitX * 800 * 1)).Normalized() * 5;
                        Vector2 velocity2 = (SmartShoot(t.Target.Center + Vector2.UnitX * 800 * -1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitX * 800 * -1)).Normalized() * 5;
                        for (int i = 0; i < 21; i++)
                        {
                            Vector2 velocity;
                            if (dir == 1)
                            {
                                velocity = velocity1;
                            }
                            else
                            {
                                velocity = velocity2;
                            }
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), t.Target.Center + Vector2.UnitX * 800 * dir + Vector2.UnitY * 80 - Vector2.UnitY * 8 * i, velocity, ModContent.ProjectileType<DemonicEyeLazer>(), defDamage, 2, ai2: -1);
                            Main.projectile[proj].friendly = false;
                            Main.projectile[proj].hostile = true;
                            dir *= -1;
                        }
                        velocity1 = (SmartShoot(t.Target.Center + Vector2.UnitY * 800 * 1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitY * 800 * 1)).Normalized() * 5;
                        velocity2 = (SmartShoot(t.Target.Center + Vector2.UnitY * 800 * -1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitY * 800 * -1)).Normalized() * 5;
                        for (int i = 0; i < 21; i++)
                        {
                            Vector2 velocity;
                            if (dir == 1)
                            {
                                velocity = velocity1;
                            }
                            else
                            {
                                velocity = velocity2;
                            }
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), t.Target.Center + Vector2.UnitY * 800 * dir + Vector2.UnitX * 80 - Vector2.UnitX * 8 * i, velocity, ModContent.ProjectileType<DemonicEyeLazer>(), defDamage, 2, ai2: -1);
                            Main.projectile[proj].friendly = false;
                            Main.projectile[proj].hostile = true;
                            dir *= -1;
                        }
                        timers[1] = 140;
                    }
                    if (timers[1] < mainTimer)
                    {
                        int dir = 1;
                        Vector2 velocity1 = (SmartShoot(t.Target.Center + Vector2.UnitX * 800 * 1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitX * 800 * 1)).Normalized() * 5;
                        Vector2 velocity2 = (SmartShoot(t.Target.Center + Vector2.UnitX * 800 * -1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitX * 800 * -1)).Normalized() * 5;
                        for (int i = 0; i < 21; i++)
                        {
                            Vector2 velocity;
                            if (dir == 1)
                            {
                                velocity = velocity1;
                            }
                            else
                            {
                                velocity = velocity2;
                            }
                            DrawLaser laser = new DrawLaser();
                            laser.width = 6;
                            laser.start = t.Target.Center + Vector2.UnitX * 800 * dir + Vector2.UnitY * 80 - Vector2.UnitY * 8 * i;
                            laser.end = laser.start + velocity * 600;
                            laser.color = Color.Red;
                            laser.color.A = (byte)(208 * EasingIn(50, 100 - timers[1]));
                            laser.color.R = ClientConfig.Instance.UseShaders ? laser.color.A : (byte)255;
                            dir *= -1;
                            Lasers.Add(laser);
                        }
                        velocity1 = (SmartShoot(t.Target.Center + Vector2.UnitY * 800 * 1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitY * 800 * 1)).Normalized() * 5;
                        velocity2 = (SmartShoot(t.Target.Center + Vector2.UnitY * 800 * -1, 20, t.Target.Center, new Vector2(npc.ai[2], npc.ai[3]), 150) - (t.Target.Center + Vector2.UnitY * 800 * -1)).Normalized() * 5;
                        for (int i = 0; i < 21; i++)
                        {
                            Vector2 velocity;
                            if (dir == 1)
                            {
                                velocity = velocity1;
                            }
                            else
                            {
                                velocity = velocity2;
                            }
                            DrawLaser laser = new DrawLaser();
                            laser.width = 6;
                            laser.start = t.Target.Center + Vector2.UnitY * 800 * dir + Vector2.UnitX * 80 - Vector2.UnitX * 8 * i;
                            laser.end = laser.start + velocity * 600;
                            laser.color = Color.Red;
                            laser.color.A = (byte)(208 * EasingIn(50, 100 - timers[1]));
                            laser.color.R = ClientConfig.Instance.UseShaders ? laser.color.A : (byte)255;
                            dir *= -1;
                            Lasers.Add(laser);
                        }
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack4(npc);
                    }
                    break;
                case 4:
                    float progress = MathF.Min((550 - mainTimer) / 400f, 1);
                    if (timers[0] == 0)
                    {
                        if (npc.alpha == 0)
                        {
                            int _npc = NPC.NewNPC(npc.GetSource_FromThis("clone"), (int)npc.Center.X, (int)npc.Center.Y, npc.type, ai0: 0, ai1: npc.whoAmI, ai2: 30, Target: npc.target);
                            Main.npc[_npc].life = npc.life;
                            Main.npc[_npc].position = npc.position;
                            npc.alpha = 255;
                            npc.immortal = true;
                            canHit = false;
                            canBeHit = false;
                        }
                        else
                        {
                            Vector2 position = t.Target.Center + Vector2.UnitX.RotatedBy(random.NextFloat(MathF.PI * 2)) * 1000;
                            int _npc = NPC.NewNPC(npc.GetSource_FromThis("clone"), (int)position.X, (int)position.Y, npc.type, ai0: random.NextFloat(MathF.PI * 0.3f) - random.NextFloat(MathF.PI * 0.3f), ai1: npc.whoAmI, ai2: 80 - (int)((50 + WorldDifficultySystem.TerrapainDifficulty * 5) * progress), Target: npc.target);
                            Main.npc[_npc].life = npc.life;
                        }
                        timers[0] = 70 - (int)((50 + WorldDifficultySystem.TerrapainDifficulty * 4f) * progress);
                    }
                    npc.position = t.Target.position + Vector2.UnitY * 650;
                    targetPosition = npc.position;
                    if (mainTimer == 0)
                    {
                        npc.alpha = 0;
                        npc.immortal = false;
                        canHit = true;
                        canBeHit = true;
                        NextAttack4(npc);
                    }
                    break;
                case 5:
                    if (mainTimer == 449)
                    {
                        npc.ai[0] = 0;
                        servantsTypes = new List<int>([
                            5,
                            5,
                            ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.LaserServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                            ModContent.NPCType<Servants.EyeofCthulhu.FireShooterServantofCthulhu>(),
                        ]);
                    }
                    if (mainTimer == 0)
                    {
                        NextAttack2(npc);
                        canHit = true;
                        break;
                    }
                    if (mainTimer % 50 == 0)
                    {
                        int index = random.Next(servantsTypes.Count);
                        int _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], ai0: servantsTypes[index] == 5? (Main.dayTime? 40 : 35) : 0, Target: npc.target);
                        Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center) * (Main.dayTime? 25 : 20);
                        if (WorldDifficultySystem.suicide || mainTimer <= 250 || Main.getGoodWorld)
                        {
                            _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], ai0: servantsTypes[index] == 5? (Main.dayTime? 40 : 35) : 0, Target: npc.target);
                            Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(0.08f) * (Main.dayTime? 25 : 20);
                            _npc = NPC.NewNPC(npc.GetSource_FromThis("fly straight"), (int)npc.Center.X, (int)npc.Center.Y, servantsTypes[index], ai0: servantsTypes[index] == 5? (Main.dayTime? 40 : 35) : 0, Target: npc.target);
                            Main.npc[_npc].velocity = npc.DirectionTo(t.Target.Center).RotatedBy(-0.08f) * (Main.dayTime? 25 : 20);
                        }
                        servantsTypes.RemoveAt(index);
                        npc.ai[1] = random.NextFloat(MathF.PI * 2);
                    }
                    targetPosition = t.Target.Center + Vector2.UnitX.RotatedBy(npc.ai[1]) * 500;
                    if (npc.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultiplyer = (1 - (75 - npc.Distance(targetPosition)) / 75) * 2.5f;
                        if (mainTimer >= 50)
                        {
                            DrawLaser laser = new DrawLaser();
                            laser.color = Color.Orange;
                            if (ClientConfig.Instance.UseShaders)
                            {
                                laser.color *= mainTimer % 50 / 70f;
                            }
                            else
                            {
                                laser.color.A = (byte)((50 - mainTimer % 50) / 70f);
                            }
                            laser.start = npc.Center;
                            laser.width = 15;
                            laser.end = npc.Center + npc.DirectionTo(t.Target.Center) * 30;
                            Lasers.Add(laser);
                            if (Main.getGoodWorld || (WorldDifficultySystem.suicide || mainTimer < 300))
                            {
                                laser.end = npc.Center + npc.DirectionTo(t.Target.Center).RotatedBy(0.08f) * 350;
                                Lasers.Add(laser);
                                laser.end = npc.Center + npc.DirectionTo(t.Target.Center).RotatedBy(-0.08f) * 350;
                                Lasers.Add(laser);
                            }
                        }
                    }
                    goalRotation = npc.DirectionTo(t.Target.Center).ToRotation();
                    break;
            }
        }
        public void DoDesperation(NPC npc)
        {
            float auraRadius = 1000;
            foreach (var player in Main.ActivePlayers)
            {
                player.wingTime = 20;
            }
            float attack0RotSpeedTorture = 2.7f;
            float attack0RotSpeedSuicide = 2.8f;
            float attack1RotSpeedTorture = 1f;
            float attack1RotSpeedSuicide = 1.2f;
            float attack2RotSpeedTorture = -2.3f;
            float attack2RotSpeedSuicide = -2.5f;
            float attack2RotSpeedTortureLegendary = -2.1f;
            float attack2RotSpeedSuicideLegendary = -2.2f;
            float attack3RotSpeedSuicide = 2.4f;
            float attack3RotSpeedTortureLegendary = 2.4f;
            float attack3RotSpeedSuicideLegendary = 2.5f;
            switch (attack)
            {
                case 0:
                    npc.life = (int)(npc.lifeMax * mainTimer / 540f) + 1;
                    npc.ai[0] = -201;
                    if (mainTimer > 540)
                    {
                        lens.Style = 1;
                        targetPosition = t.Target.Center;
                        goalRotation = npc.rotation + MathF.PI / 2;
                        if (laserAngle < MathF.PI / 4 || !laser)
                        {
                            if (!laser)
                            {
                                laserAngle = 0;
                                laser = true;
                            }
                            laserAngle += MathF.PI / 60;
                        }
                        else
                        {
                            laserAngle = MathF.PI / 4;
                        }
                        if (laserAngle == MathF.PI / 4)
                        {
                            mainTimer = 540;
                        }
                    }
                    else
                    {
                        lens.Style = 2;
                        float progress = 1 - mainTimer / 540f;
                        auraRadius = 1000 - 500 * progress;
                        if (npc.velocity.Length() < 1)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.Normalized() * (npc.velocity.Length() - 1);
                        }
                        goalRotation += (1 + progress * ((WorldDifficultySystem.torture? attack0RotSpeedTorture : attack0RotSpeedSuicide) - 1)) * MathF.PI * 0.01f;

                        //{
                        //    int count = 5 - WorldDifficultySystem.TerrapainDifficulty;
                        //    for (int i = 0; i < count; i++)
                        //    {
                        //        DrawLaser laser = new DrawLaser();
                        //        laser.color = Color.Red;
                        //        if (ClientConfig.Instance.UseShaders)
                        //        {
                        //            laser.color *= ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f);
                        //        }
                        //        else
                        //        {
                        //            laser.color.A = (byte)(255 * ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f));
                        //        }
                        //        laser.start = npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50;
                        //        laser.end = laser.start + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (auraRadius - 50);
                        //        laser.width = 8;
                        //        Lasers.Add(laser);
                        //    }
                        //}

                        if (timers[0] == 0)
                        {
                            int count = 5 - WorldDifficultySystem.TerrapainDifficulty;
                            if (WorldDifficultySystem.suicide)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis("boomerang"), npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 15, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (30 + (Main.getGoodWorld ? 5 : 0)), ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
                                }
                                timers[0] = 40 - (int)(progress * 20);
                            }
                            else
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis("Fly straight"), npc.Center, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (10 + (Main.getGoodWorld ? 5 : 0)), ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
                                }
                                timers[0] = 40 - (int)(progress * 25);
                            }
                        }
                        if (mainTimer == 0)
                        {
                            laserAngle = 0;
                            laser = false;
                            attack = 1;
                            mainTimer = 350;
                        }
                    }
                    break;
                case 1:
                    npc.life = (int)(npc.lifeMax * mainTimer / 350f) + 1;
                    auraRadius = 500 - MathF.Min(350 - mainTimer, 50) * (WorldDifficultySystem.suicide? 1.5f : 1);
                    goalRotation += MathHelper.Lerp(WorldDifficultySystem.torture ? attack0RotSpeedTorture : attack0RotSpeedSuicide, WorldDifficultySystem.torture? attack1RotSpeedTorture : attack1RotSpeedSuicide, MathF.Min(350 - mainTimer, 50) / 50) * MathF.PI * 0.01f;
                    if (mainTimer > 250)
                    {
                        int count = Main.getGoodWorld? 5 : 4;
                        for (int i = 0; i < count; i++)
                        {
                            DrawLaser laser = new DrawLaser();
                            laser.color = Color.Red;
                            if (ClientConfig.Instance.UseShaders)
                            {
                                laser.color *= ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f);
                            }
                            else
                            {
                                laser.color.A = (byte)(255 * ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f));
                            }
                            laser.start = npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50;
                            laser.end = laser.start + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (auraRadius - 50);
                            laser.width = 8;
                            Lasers.Add(laser);
                        }
                    }
                    if (mainTimer % 2 == 0 && mainTimer < 250)
                    {
                        int count = Main.getGoodWorld ? 5 : 4;
                        for (int i = 0; i < count; i++)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 10, ProjectileID.FlamesTrap, 14, 1);
                            Main.projectile[proj].friendly = false;
                        }
                    }
                    if (mainTimer == 0)
                    {
                        attack = 2;
                        mainTimer = 450;
                    }
                    break;
                case 2:
                    {
                        if (Main.getGoodWorld)
                        {
                            int count = 4;
                            for (int i = 0; i < count; i++)
                            {
                                DrawLaser laser = new DrawLaser();
                                laser.color = Color.Red;
                                if (ClientConfig.Instance.UseShaders)
                                {
                                    laser.color *= (mainTimer % 50 / 50f) * 0.8f;
                                }
                                else
                                {
                                    laser.color.A = (byte)(255 * (mainTimer % 50 / 50f) * 0.8f);
                                }
                                laser.start = npc.Center + (-npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50;
                                laser.end = laser.start + (-npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (auraRadius - 50);
                                laser.width = 8;
                                Lasers.Add(laser);
                            }
                            if (mainTimer % 50 < 20)
                            {
                                if (mainTimer % 2 == 0 && mainTimer < 250)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + (-npc.rotation + MathF.PI / 4 + MathF.PI / 2 * i).ToRotationVector2() * 50, (npc.rotation + MathF.PI / 4 + MathF.PI / 2 * i).ToRotationVector2() * 10, ProjectileID.FlamesTrap, 14, 1);
                                        Main.projectile[proj].friendly = false;
                                    }
                                }
                            }
                        }
                        npc.life = (int)(npc.lifeMax * mainTimer / 450f) + 1;
                        auraRadius = 500 - 100 * (WorldDifficultySystem.suicide ? 1.5f : 1);
                        float Progress = 1 - mainTimer / 450f;
                        if (Main.getGoodWorld)
                        {
                            goalRotation += MathHelper.Lerp(WorldDifficultySystem.torture? attack1RotSpeedTorture : attack1RotSpeedSuicide, WorldDifficultySystem.torture ? attack2RotSpeedTortureLegendary : attack2RotSpeedSuicideLegendary, Progress) * MathF.PI * 0.01f;
                        }
                        else
                        {
                            goalRotation += MathHelper.Lerp(WorldDifficultySystem.torture ? attack1RotSpeedTorture : attack1RotSpeedSuicide, WorldDifficultySystem.torture ? attack2RotSpeedTorture : attack2RotSpeedSuicide, Progress) * MathF.PI * 0.01f;
                        }
                        if (mainTimer % 3 == 0)
                        {
                            Vector2 velocity = Vector2.UnitY.RotatedBy(npc.rotation) * 5;
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ModContent.ProjectileType<UVLaser>(), defDamage, 2);
                        }
                        if (mainTimer < 50 && WorldDifficultySystem.suicide || Main.getGoodWorld)
                        {
                            int count = Main.getGoodWorld? 4 : 3;
                            for (int i = 0; i < count; i++)
                            {
                                DrawLaser laser = new DrawLaser();
                                laser.color = Color.Red;
                                if (ClientConfig.Instance.UseShaders)
                                {
                                    laser.color *= ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f);
                                }
                                else
                                {
                                    laser.color.A = (byte)(255 * ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f));
                                }
                                laser.start = npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50;
                                laser.end = laser.start + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (auraRadius - 50);
                                laser.width = 8;
                                Lasers.Add(laser);
                            }
                        }
                        if (mainTimer == 0)
                        {
                            if (WorldDifficultySystem.torture)
                            {
                                npc.GetT().canselDeathHitEffect = false;
                                npc.immortal = false;
                                npc.StrikeInstantKill();
                            }
                            else
                            {
                                attack = 3;
                                mainTimer = 450;
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        npc.life = (int)(npc.lifeMax * mainTimer / 450f) + 1;
                        auraRadius = 350;
                        float Progress = 1 - mainTimer / 450f;
                        if (Main.getGoodWorld)
                        {
                            goalRotation += MathHelper.Lerp(WorldDifficultySystem.torture ? attack2RotSpeedTortureLegendary : attack2RotSpeedSuicideLegendary, WorldDifficultySystem.torture? attack3RotSpeedTortureLegendary : attack3RotSpeedSuicideLegendary, Progress) * MathF.PI * 0.01f;
                        }
                        else
                        {
                            goalRotation += MathHelper.Lerp(attack2RotSpeedSuicide, attack3RotSpeedSuicide, Progress) * MathF.PI * 0.01f;
                        }


                        {
                            int count = Main.getGoodWorld? 4 : 3;
                            for (int i = 0; i < count; i++)
                            {
                                DrawLaser laser = new DrawLaser();
                                laser.color = Color.Red;
                                if (ClientConfig.Instance.UseShaders)
                                {
                                    laser.color *= ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f);
                                }
                                else
                                {
                                    laser.color.A = (byte)(255 * ((1 - (MathF.Abs(mainTimer % 10 - 5) / 5f) * (MathF.Abs(mainTimer % 10 - 5) / 5f)) * 0.5f + 0.3f));
                                }
                                laser.start = npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 50;
                                laser.end = laser.start + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (auraRadius - 50);
                                laser.width = 8;
                                Lasers.Add(laser);
                            }
                        }


                        if (timers[0] == 0)
                        {
                            int count = 3;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis("boomerang"), npc.Center + (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * 20, (npc.rotation + MathF.PI / count + MathF.PI / count * 2 * i).ToRotationVector2() * (30 + (Main.getGoodWorld ? 5 : 0)), ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
                            }
                            timers[0] = 40 - (int)(Progress * 20);
                        }
                        if (mainTimer == 0)
                        {
                            if (Main.dayTime)
                            {
                                attack++;
                            }
                            else
                            {
                                npc.GetT().canselDeathHitEffect = false;
                                npc.immortal = false;
                                npc.StrikeInstantKill();
                            }
                        }
                    }
                    break;
                case 4:
                    auraRadius = MathHelper.Lerp(350, 650, (666 - mainTimer) / 50);
                    if (mainTimer % 60 == 0)
                    {
                        //Projectile.NewProjectile(npc.GetSource_FromThis("flyStraight"), npc.Center, npc.rotation)
                    }
                    if (mainTimer == 0)
                    {
                        npc.GetT().canselDeathHitEffect = false;
                        npc.immortal = false;
                        npc.StrikeInstantKill();
                    }
                    break;
            }
            if (timers[1] == 0) 
            {
                auraRadius = MathF.Max(auraRadius, npc.Distance(t.Target.Center) / 1.207106f);
                float rot = npc.DirectionTo(t.Target.Center).ToRotation();
                int count = (int)(auraRadius / 15);
                Vector2 pos = Vector2.One.RotatedBy(rot) * auraRadius;
                Vector2 pos1 = Vector2.UnitX.RotatedBy(rot) * auraRadius;
                for (int i = 0; i < count; i++)
                {
                    Vector2 pos2 = pos1.RotatedBy((float)i / count * MathF.PI * 2) + npc.Center;
                    Projectile.NewProjectile(npc.GetSource_FromThis("aura"), pos.RotatedBy((float)i / count * MathF.PI * 2) + npc.Center, -Vector2.UnitY.RotatedBy(rot + (float)i / count * MathF.PI * 2) * 10, ModContent.ProjectileType<GhostServantofCthulhu>(), 25, 10, ai0: pos2.X, ai1: pos2.Y, ai2: auraRadius);
                }
                timers[1] = 18;
            }
        }
        int LaserProjectile;
        void LaserProjectileUpdate(NPC npc)
        {
            if (MathF.Abs((lens.Center - npc.Center).RotatedBy(-npc.rotation - MathF.PI / 2).ToRotation()) < laserAngle && IsAngleBetweenAngles((lens.Center - npc.Center).ToRotation() + 0.2f, (focus - npc.Center).ToRotation(), (lens.Center - npc.Center).ToRotation() - 0.2f))
            {
                Projectile proj = Main.projectile[LaserProjectile];
                if (!proj.active || proj.type != ModContent.ProjectileType<EoCLaser>())
                {
                    LaserProjectile = Projectile.NewProjectile(npc.GetSource_FromThis(), focus, Vector2.Zero, ModContent.ProjectileType<EoCLaser>(), 20, 0);
                    proj = Main.projectile[LaserProjectile];
                }
                proj.ai[0] = 30 / npc.Distance(lens.Center) * lens.Center.Distance(focus);
                proj.ai[1] = (focus - lens.Center).ToRotation();
                proj.ai[2] = (focus - lens.Top).ToRotation() - proj.ai[1];
                proj.timeLeft = 2;
                proj.Hitbox = GetRectangle(focus + Vector2.UnitX.RotatedBy(proj.ai[1] + proj.ai[2]) * proj.ai[0], focus + Vector2.UnitX.RotatedBy(proj.ai[1] - proj.ai[2]) * proj.ai[0], focus - Vector2.UnitX.RotatedBy(proj.ai[1] + proj.ai[2]) * proj.ai[0], focus - Vector2.UnitX.RotatedBy(proj.ai[1] - proj.ai[2]) * proj.ai[0]);
                proj.Center = focus;
            }
        }
        void NextAttack1(NPC npc)
        {
            if (CheckPhase(npc))
            {
                return;
            }
            attackCounter++;
            if (attackCounter >= attacks1.Length)
            { 
                attackCounter = 0; 
            }
            attack = attacks1[attackCounter];
            switch (attack)
            {
                case 0:
                    canHit = false;
                    timers[0] = 0;
                    timers[1] = 0;
                    break;
                case 1:
                    dir[0] = 0;
                    dir[1] = 1;
                    timers[0] = timers[0] = 20 - WorldDifficultySystem.TerrapainDifficulty * 2;
                    timers[1] = 120;
                    mainTimer = 250;
                    break;
                case 2:
                    maxVelocity = 30;
                    canHit = false;
                    npc.ai[0] = 3;
                    npc.ai[1] = 4;
                    mainTimer = 359 + (WorldDifficultySystem.suicide? 40 : 0);
                    timers[0] = 0;
                    dir[1] = 1;
                    break;
                case 3:
                    npc.ai[0] = 3;
                    npc.ai[1] = 4;
                    mainTimer = 359;
                    timers[0] = 0;
                    timers[1] = 0;
                    dir[0] = 0;
                    dir[1] = 0;
                    EoCPosForPhase1Attack3ToFixTheShaking = npc.Center;
                    EoCVelForPhase1Attack3ToFixTheShaking = npc.velocity;
                    break;
            }
        }
        void NextAttack2(NPC npc)
        {
            if (CheckPhase(npc))
            {
                return;
            }
            attackCounter++;
            if (attackCounter >= attacks2.Length)
            {
                attackCounter = 0;
            }
            attack = attacks2[attackCounter];
            npc.ai[0] = -201;
            switch (attack)
            {
                case -1:
                    mainTimer = 45;
                    break;
                case 0:
                    timers[0] = 151;
                    mainTimer = 400;
                    break;
                case 1:
                    timers[0] = 0;
                    mainTimer = 606;
                    break;
                case 2:
                    timers[0] = 0;
                    mainTimer = 546 + (WorldDifficultySystem.suicide? 5 : 0);
                    break;
                case 3:
                    mainTimer = 241;
                    break;
                case 4:
                    mainTimer = 751;
                    break;
            }
        }
        void NextAttack3(NPC npc)
        {
            if (CheckPhase(npc))
            {
                return;
            }
            attackCounter++;
            if (attackCounter >= attacks3.Length)
            {
                attackCounter = 0;
            }
            attack = attacks3[attackCounter];
            timers[0] = 0;
            timers[1] = 0;
            switch (attack)
            {
                case -1:
                    mainTimer = 65;
                    break;
                case 0:
                    timers[1] = 20;
                    mainTimer = 161;
                    dir[0] = -1;
                    npc.ai[0] = 0;
                    break;
                case 1:
                    if (WorldDifficultySystem.suicide)
                    {
                        dir[0] = random.NextBool() ? 1 : -1;
                    }
                    else
                    {
                        dir[0] = 1;
                    }
                    mainTimer = 1000;
                    npc.ai[0] = 0;
                    break;
                case 2:
                    mainTimer = 200;
                    break;
            }
        }
        void NextAttack4(NPC npc)
        {
            if (CheckPhase(npc))
            {
                return;
            }
            attackCounter++;
            if (attackCounter >= attacks4.Length)
            {
                attackCounter = 0;
            }
            attack = attacks4[attackCounter];
            timers[0] = 0;
            timers[1] = 0;
            switch (attack)
            {
                case 0:
                    mainTimer = 65;
                    break;
                case 1:
                    dir[0] = random.NextBool()? 1 : -1;
                    timers[0] = Main.dayTime? 30 : 45;
                    mainTimer = 300;
                    break;
                case 2:
                    dir[0] = random.NextBool()? 1 : -1;
                    mainTimer = 1000;
                    break;
                case 3:
                    timers[1] = 140;
                    mainTimer = 400;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    break;
                case 4:
                    mainTimer = 550;
                    break;
                case 5:
                    mainTimer = 450;
                    canHit = false;
                    break;
            }
        }
        bool CheckPhase(NPC npc)
        {
            if (npc.life / (float)npc.lifeMax < 0.75f && !(npc.life / (float)npc.lifeMax < 0.55f) && phase == 1)
            {
                canHit = false;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.immortal = true;
                phase = 2;
                attackCounter = -1;
                Tip("EyeofCthulhuTip");
                return true;
            }
            if (npc.life / (float)npc.lifeMax < 0.5f && npc.life / (float)npc.lifeMax > 0.35f && phase <= 2)
            {
                canHit = false;
                npc.velocity = Vector2.Zero;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.immortal = true;
                phase = 2;
                anim = new _2To3PhaseTransitionAnimation(npc);
                anim.Active = true;
                attackCounter = -1;
                return true;
            }
            if (npc.life / (float)npc.lifeMax < 0.35f && npc.life != 1 && phase <= 3)
            {
                canHit = false;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                if (phase < 3)
                {
                    npc.immortal = true;
                    phase = 2;
                    anim = new _2To3PhaseTransitionAnimation(npc);
                    anim.Active = true;
                }
                else
                { 
                    npc.immortal = false;
                    phase = 4;
                }
                attackCounter = -1;
                attack = -1;
                return true;
            }
            if (npc.life == 1)
            {
                phase = 5;
                attack = 0;
                mainTimer = 800;
                return true;
            }
            return false;
        }
        //List<Group> servants = new List<Group>();
        void ServamtsCooperaingAI(NPC npc)
        {

        }
        Vector2 MaxedVelocity(NPC npc)
        {
            if (npc.velocity.Length() > maxVelocity)
            {
                return npc.velocity.Normalized() * maxVelocity;
            }
            return npc.velocity;
        }
        Vector2 scale = new Vector2(450, 80);
        Vector2 portalOutPosition;
        Vector2 portalInPosition;
        Color portalThingColor = Color.Yellow;
        float portalThingAlpha;
        Color portalColor = Color.White;
        float portalAlpha;
        float portalOutRotation;
        float portalInRotation;
        bool portal;
        int portalDustCount = 20;
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (npc.immortal && projectile.type == ModContent.ProjectileType<ServantofCthulhuSoul>() && phase != 5 && !anim.Active)
            {
                npc.life -= hit.Damage;
                if (npc.life < 0)
                {
                    npc.checkDead();
                }
            }
        }
        Vector2 FindFocus(NPC npc)
        {
            Vector2 fromEyeToLensTop = lens.Top - npc.Center;
            Vector2 fromEyeToLensCenter = lens.Center - npc.Center;
            Vector2 lensFocus = Vector2.Zero;
            if (npc.Distance(lens.focus1) > npc.Distance(lens.focus2))
            {
                lensFocus = lens.focus1;
            }
            else
            {
                lensFocus = lens.focus2;
            }
            Vector2 helpPoint = AlmostGarantedRayColision(lensFocus, lensFocus + Vector2.UnitY.RotatedBy(lens.rotation), lens.Center, lens.Center + fromEyeToLensTop).Value;
            Vector2 newRayDirection = helpPoint - lens.Top;
            return AlmostGarantedRayColision(lens.Top, lens.Top + newRayDirection, npc.Center, npc.Center + fromEyeToLensCenter).Value;
        }
        Vector2 focus;
        int timemax = 30;
        void MakePortals1(NPC npc, int time)
        {
            if (time == timemax)
            {
                portalAlpha = 1;
                portalDusts = new List<int>();
                npc.ai[2] = random.NextFloat(-MathF.PI, MathF.PI);
                portalOutRotation = npc.ai[2];
                portalOutPosition = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * (600 - 25 * WorldDifficultySystem.TerrapainDifficulty);
                if (WorldDifficultySystem.suicide)
                {
                    Vector2 vector = (SmartShoot(portalOutPosition, maxVelocity, t.Target.Custom().oldCenters[Main.getGoodWorld ? 4 : 6], t.Target.Custom().oldVelocities[Main.getGoodWorld ? 4 : 6], 60) - portalOutPosition).Normalized();
                    portalOutRotation = AngleFromVector(vector);
                }
                MakeNewPortal(portalOutPosition, portalOutRotation + MathF.PI / 2, portalDustCount, ref portalDusts);
                portalInPosition = npc.Center + npc.velocity * time;
                portalInRotation = AngleFromVector(npc.velocity);
                MakeNewPortal(portalInPosition, portalInRotation + MathF.PI / 2, portalDustCount, ref portalDusts);
            }
            else
            {
                portal = true;
                scale.X = 100 + EasingOut(timemax, timemax - time) * 350;
                portalThingAlpha = (timemax - time) / 30f;

                portalOutRotation = npc.ai[2];
                portalOutPosition = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * (600 - 25 * WorldDifficultySystem.TerrapainDifficulty);
                if (WorldDifficultySystem.suicide)
                {
                    Vector2 vector = (SmartShoot(portalOutPosition, maxVelocity, t.Target.Custom().oldCenters[Main.getGoodWorld? 4 : 6], t.Target.Custom().oldVelocities[Main.getGoodWorld ? 4 : 6], 60) - portalOutPosition).Normalized();
                    portalOutRotation = AngleFromVector(vector);
                }
                if (random.Next(2) == 0)
                {
                    Vector2 pos = new Vector2(random.NextFloat(-scale.X * 0.1f, scale.X * 0.9f), random.NextFloat(-scale.Y * 0.5f, scale.Y * 0.5f));
                    pos = pos.RotatedBy(portalOutRotation);
                    pos += portalOutPosition;
                    int dust = Dust.NewDust(pos, 0, 0, DustID.Torch, Scale: 2.5f);
                    Main.dust[dust].velocity = UnitVectorFromRotation(portalOutRotation).RotatedByRandom(0.15) * random.NextFloat(6, 10);
                }
                UpdatePortal(portalOutPosition, portalOutRotation + MathF.PI / 2, 0, portalDustCount - 1, ref portalDusts);
                portalInPosition = npc.Center + npc.velocity * time;
                portalInRotation = AngleFromVector(npc.velocity);
                UpdatePortal(portalInPosition, portalInRotation + MathF.PI / 2, portalDustCount, portalDustCount * 2 - 1, ref portalDusts);
            }
            if (time == 0)
            {
                timers[3] = 11;
            }
        }
        void MakePortals2(NPC npc, int time)
        {
            if (time == 20)
            {
                portalAlpha = 1;
                portalDusts = new List<int>();
                npc.ai[2] = random.NextFloat(-MathF.PI, MathF.PI);
                portalOutPosition = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * 600;
                portalOutRotation = npc.ai[2] + 0.25f * MathF.PI;
                MakeNewPortal(portalOutPosition, portalOutRotation + MathF.PI / 2, portalDustCount, ref portalDusts);
                portalInPosition = npc.Center + npc.velocity * time;
                portalInRotation = AngleFromVector(npc.velocity);
                MakeNewPortal(portalInPosition, portalInRotation + MathF.PI / 2, portalDustCount, ref portalDusts);
            }
            else
            {
                portal = true;
                scale.X = 100 + EasingOut(20, 20 - time) * 350;
                portalThingAlpha = (20 - time) / 20f;
                portalOutRotation = npc.ai[2] + 0.25f * MathF.PI;
                portalOutPosition = t.Target.Center - UnitVectorFromRotation(npc.ai[2]) * 600;
                if (random.Next(2) == 0)
                {
                    Vector2 pos = new Vector2(random.NextFloat(-scale.X * 0.1f, scale.X * 0.9f), random.NextFloat(-scale.Y * 0.5f, scale.Y * 0.5f));
                    pos = pos.RotatedBy(portalOutRotation);
                    pos += portalOutPosition;
                    int dust = Dust.NewDust(pos, 0, 0, DustID.Torch, Scale: 2.5f);
                    Main.dust[dust].velocity = UnitVectorFromRotation(portalOutRotation).RotatedByRandom(0.15) * random.NextFloat(6, 10);
                }
                UpdatePortal(portalOutPosition, portalOutRotation + MathF.PI / 2, 0, portalDustCount - 1, ref portalDusts);
                portalInPosition = npc.Center + npc.velocity * time;
                portalInRotation = AngleFromVector(npc.velocity);
                UpdatePortal(portalInPosition, portalInRotation + MathF.PI / 2, portalDustCount, portalDustCount * 2 - 1, ref portalDusts);
            }
            if (time == 0)
            {
                timers[3] = 11;
            }
        }
        void PostUpdatePortalOut(int time)
        {
            if (time == 10)
            {
                for (int i = 0; i < portalDustCount; i++)
                {
                    Dust dust = Main.dust[portalDusts[i]];
                    if (dust.active && dust.type == ModContent.DustType<MagicParticles>())
                    {
                        dust.noGravity = false;
                        dust.velocity = (dust.position - portalOutPosition) / 2;
                    }
                }
            }
            portalThingAlpha = time / 10f;
            portalAlpha = time / 10f;
            portal = true;
        }
        void PostUpdatePortalIn()
        {
            for (int i = portalDustCount; i < portalDustCount * 2; i++)
            {
                Dust dust = Main.dust[portalDusts[i]];
                if (dust.active && dust.type == ModContent.DustType<MagicParticles>())
                {
                    dust.noGravity = false;
                }

            }
        }
        public void MakeNewPortal(Vector2 position, float rotation, int count, ref List<int> PortalDusts)
        {
            Lighting.AddLight(position, 1, 1, 1);
            for (int j = 0; j < count; j++)
            {
                Vector2 _position = new Vector2();
                _position.X = random.NextFloat(-1, 1);
                _position.X = MathF.Asin(_position.X);
                _position.Y = random.NextFloat(-MathF.Cos(_position.X), MathF.Cos(_position.X));
                _position *= 20;
                _position = _position.RotatedBy(rotation);
                _position += position;
                int dust = Dust.NewDust(_position, 0, 0, ModContent.DustType<MagicParticles>(), Scale: 3);
                Main.dust[dust].velocity = Vector2.Zero;
                Main.dust[dust].noGravity = true;
                PortalDusts.Add(dust);
            }
        }
        public void UpdatePortal(Vector2 position, float rotation, int start, int end, ref List<int> PortalDusts)
        {
            Lighting.AddLight(position, 1, 1, 1);
            for (int j = start; j < end + 1; j++)
            {
                Dust dust = Main.dust[PortalDusts[j]];
                if (!dust.active || dust.type != ModContent.DustType<MagicParticles>())
                {
                    int _dust = Dust.NewDust(position, 0, 0, ModContent.DustType<MagicParticles>(), Scale: 3);
                    Main.dust[_dust].noGravity = true;
                    Main.dust[_dust].velocity = Vector2.Zero;
                    PortalDusts[j] = _dust;
                }
                Vector2 _position = new Vector2();
                _position.X = random.NextFloat(-1, 1);
                _position.X = MathF.Asin(_position.X);
                _position.Y = random.NextFloat(-MathF.Cos(_position.X), MathF.Cos(_position.X));
                _position *= 60;
                _position = _position.RotatedBy(rotation);
                _position += position;
                Main.dust[PortalDusts[j]].position = _position;
            }
        }
        public override bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
            if (portal)
            {
                Texture2D value = ExtraTextureRegistry.Portal.Value;
                spriteBatch.Draw(value, portalOutPosition - Main.screenPosition, null, portalColor * portalAlpha, portalOutRotation, value.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(value, portalInPosition - Main.screenPosition, null, portalColor * portalAlpha, portalInRotation, value.Size() / 2, 1, SpriteEffects.None, 0);
            }
            //lens.Center = Main.LocalPlayer.Center;
            //if (Main.LocalPlayer.controlUp)
            //{
            //    lens.warp -= 0.01f;
            //    if (lens.warp < 0.1f)
            //    {
            //        lens.warp = 0.1f;
            //    }
            //}
            //if (Main.LocalPlayer.controlDown)
            //{
            //    lens.warp += 0.01f;
            //}
            //if (Main.mouseRight)
            //{
            //    lens.rotation += 0.0314f;
            //}
            //if (Main.mouseLeft)
            //{
            //    lens.rotation -= 0.0314f;
            //}
            if (lens.active)
            {
                lens.Draw(spriteBatch);
                
                for (int i = 0; i < holdersCount; i++)
                {
                    holders[i].Draw(spriteBatch);
                }
            }
            if (laser && ClientConfig.Instance.UseShaders)
            {
                ManagedShader EoCShader = ShaderManager.GetShader("Terrapain.EoCShader");
                EoCShader.TrySetParameter("value", laserAngle * 4 / MathF.PI);
                EoCShader.TrySetParameter("Center", npc.GetT().drawCenter);
                EoCShader.TrySetParameter("Size", TextureAssets.Npc[npc.type].Size());
                EoCShader.TrySetParameter("FrameStart", new Vector2(npc.frame.X, npc.frame.Y));
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, EoCShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            }
            if (clone)
            {
                Vector2 pos = npc.Center - screenPos;
                Vector2 dir = (npc.rotation - MathF.PI / 2).ToRotationVector2();
                float multiply = 0;
                float value = Math.Abs(NormalizeRotation(npc.rotation - MathF.PI / 2, false));
                if (Math.Abs(dir.Y) > MathF.Abs(dir.X))
                {
                    multiply = (Main.ScreenSize.Y / 2 - pos.Y) / dir.Y; 
                }
                else
                {
                    multiply = (Main.ScreenSize.X / 2 - pos.X) / dir.X;
                }
                pos += dir * multiply;
                if (ClientConfig.Instance.UseShaders)
                {
                    ManagedShader shader = ShaderManager.GetShader("Terrapain.EyeofCthulhuCloneDash");
                    shader.TrySetParameter("scale", new Vector2(3000, 100));
                    shader.TrySetParameter("EyeOfCthulhu", (npc.Center - pos - screenPos).RotatedBy(-npc.rotation - MathF.PI / 2).X);
                    shader.TrySetParameter("player", (Main.LocalPlayer.Center - pos - screenPos).RotatedBy(-npc.rotation - MathF.PI / 2));
                    shader.TrySetParameter("shineColor", new Vector4(0.8f, 0.8f, 0.8f, 0.8f));
                    shader.TrySetParameter("borderColor", new Vector4(0, 0, 0.8f, 0.8f));
                    shader.TrySetParameter("mainColor", Color.LightBlue.ToVector4());
                    shader.TrySetParameter("time", -mainTimer);
                    shader.TrySetParameter("alpha", 0.33f * MathF.Min(1, (mainTimer - 15) / 15f) * MathF.Max(3 - (25 - mainTimer) * (25 - mainTimer) / 49f, 1));
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    spriteBatch.GraphicsDevice.Textures[1] = ExtraTextureRegistry.EyeofCthulhuCloneDahs1.Value;
                    spriteBatch.GraphicsDevice.Textures[2] = ExtraTextureRegistry.EyeofCthulhuCloneDahs2.Value;
                }
                else
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, pos, null, Color.LightBlue * 0.33f * MathF.Min(1, (mainTimer - 15) / 15f) * MathF.Max(3 - (25 - mainTimer) * (25 - mainTimer) / 49f, 1), npc.rotation + MathF.PI / 2, ExtraTextureRegistry.WhitePixel.Value.Size() * 0.5f, new Vector2(3000, 100), SpriteEffects.None, 1);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return true;
        }
        public override void ModPostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
            if (laser)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            for (int i = 0; i < 5; i++)
            {
                if (anim.GoreActive[i])
                {
                    Color color = Lighting.GetColor(anim.GorePosition[i].ToTileCoordinates());
                    spriteBatch.Draw(anim.Gores[i], anim.GorePosition[i] - Main.screenPosition, null, color, anim.GoreRotation[i], anim.GoreDrawSource[i], 1, SpriteEffects.None, 0);
                }
            }
            if (Lasers.Count > 0)
            {
                Texture2D texture1;
                if (ClientConfig.Instance.UseShaders)
                {
                    ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
                    Shade.TrySetParameter("lenght", Lasers[0].start.Distance(Lasers[0].end));
                    Shade.TrySetParameter("width", Lasers[0].width);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    texture1 = null;
                }
                else
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    texture1 = ExtraTextureRegistry.CubedGradient10Mirrored.Value;
                }
                foreach (var laser in Lasers)
                {
                    spriteBatch.DrawLine(laser.start, laser.end, laser.color, laser.width, texture1);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (portal)
            {
                ManagedShader Shade = ShaderManager.GetShader("Terrapain.PortalShader");
                Texture2D value = ExtraTextureRegistry.WhitePixel.Value;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(value, portalOutPosition - Main.screenPosition, null, portalThingColor * portalThingAlpha, portalOutRotation, new Vector2(0.1f, 0.5f), scale, SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public bool laser;
        public float laserAngle;
        public override void PostDrawNPCs(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (laser)
            {
                lens.RealFocus = focus;
                if (ClientConfig.Instance.UseShaders)
                {
                    spriteBatch.End();
                    ManagedShader effect = ShaderManager.GetShader("Terrapain.EyeofCthulhuLensShader");
                    effect.TrySetParameter("screenPos", Main.screenPosition - npc.Center);
                    effect.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
                    effect.TrySetParameter("lensTop", (lens.Top - npc.Center).RotatedBy(-NormalizeRotation(npc.rotation + MathF.PI * 0.5f, true)));
                    effect.TrySetParameter("lensBottom", (lens.Bottom - npc.Center).RotatedBy(-NormalizeRotation(npc.rotation + MathF.PI * 0.5f, true)));
                    effect.TrySetParameter("lightningAngle", -NormalizeRotation(npc.rotation + MathF.PI * 0.5f, true));
                    effect.TrySetParameter("lightningAngle2", laserAngle);
                    //Vector2 fromEyeToLensTop = lens.Top - npc.Center;
                    //Vector2 fromEyeToLensCenter = lens.Center - npc.Center;
                    //Vector2 lensFocus = Vector2.Zero;
                    //if (npc.Distance(lens.focus1) > npc.Distance(lens.focus2))
                    //{
                    //    lensFocus = lens.focus1;
                    //}
                    //else
                    //{
                    //    lensFocus = lens.focus2;
                    //}
                    //Vector2 helpPoint = AlmostGarantedRayColision(lensFocus, lensFocus + Vector2.UnitY.RotatedBy(lens.rotation), lens.Center, lens.Center + fromEyeToLensTop).Value;
                    //Vector2 newRayDirection = helpPoint - lens.Top;
                    effect.TrySetParameter("focus", (focus - npc.Center).RotatedBy(-NormalizeRotation(npc.rotation + MathF.PI * 0.5f, true)));
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, effect.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                    Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                    spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, rekt, null, Color.Black, 0f, ExtraTextureRegistry.WhitePixel.Value.Size() * 0.5f, 0, 1f);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
                else
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Texture2D texture = ExtraTextureRegistry.Lighting.Value;
                    Vector2 scale = new(2, 2 * MathF.Tan(laserAngle));
                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, npc.rotation + MathF.PI / 2, texture.Size() * Vector2.UnitY * 0.5f, scale, SpriteEffects.None, 1);
                    if (MathF.Abs((lens.Center - npc.Center).RotatedBy(-npc.rotation - MathF.PI / 2).ToRotation()) < laserAngle && IsAngleBetweenAngles((lens.Center - npc.Center).ToRotation() + 0.2f, (focus - npc.Center).ToRotation(), (lens.Center - npc.Center).ToRotation() - 0.2f))
                    {
                        float rotation1 = focus.DirectionTo(lens.Top).ToRotation();
                        float rotation2 = focus.DirectionTo(lens.Bottom).ToRotation();
                        float rotation3 = rotation1 - rotation2;
                        float rotation4 = NormalizeRotation(rotation3, false);
                        float rotation5 = MathF.Abs(rotation4);
                        float rotation = rotation5 / 2;
                        //float rotation = MathF.Abs(NormalizeRotation(focus.DirectionTo(lens.Top).ToRotation() - focus.DirectionTo(lens.Bottom).ToRotation(), false)) / 2;
                        scale = new(focus.Distance(lens.Center) / texture.Size().X, focus.Distance(lens.Center) / texture.Size().X * MathF.Tan(rotation));
                        rotation = (focus.DirectionTo(lens.Top).ToRotation() + focus.DirectionTo(lens.Bottom).ToRotation()) / 2;
                        spriteBatch.Draw(texture, focus - Main.screenPosition, null, Color.White, rotation, texture.Size() * Vector2.UnitY * 0.5f, scale, SpriteEffects.None, 1);
                        spriteBatch.Draw(texture, focus - Main.screenPosition, null, Color.White, rotation - MathF.PI, texture.Size() * Vector2.UnitY * 0.5f, scale, SpriteEffects.None, 1);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }
            //if (ClientConfig.Instance.drawHitbox)
            //{
            //    if (MathF.Abs((lens.Center - npc.Center).RotatedBy(-npc.rotation - MathF.PI / 2).ToRotation()) < laserAngle && IsAngleBetweenAngles((lens.Center - npc.Center).ToRotation() + 0.2f, (focus - npc.Center).ToRotation(), (lens.Center - npc.Center).ToRotation() - 0.2f))
            //    {
            //        Projectile proj = Main.projectile[LaserProjectile];
            //        if (!(!proj.active || proj.type != ModContent.ProjectileType<EoCLaser>()))
            //        {
            //            Texture2D texture = ExtraTextureRegistry.Triangle.Value;
            //            Vector2 scale = new Vector2(MathF.Abs(MathF.Sin(proj.ai[2])) * proj.ai[0] / 100, MathF.Abs(MathF.Cos(proj.ai[2])) * proj.ai[0] / 100);
            //            Main.spriteBatch.Draw(texture, proj.Center - Main.screenPosition, null, ClientConfig.Instance.hitboxColor, proj.ai[1] - MathF.PI / 2, Vector2.UnitX * 50, scale, SpriteEffects.None, 0);
            //            Main.spriteBatch.Draw(texture, proj.Center - Main.screenPosition, null, ClientConfig.Instance.hitboxColor, proj.ai[1] + MathF.PI / 2, Vector2.UnitX * 50, scale, SpriteEffects.None, 0);
            //        }
            //    }
            //}
            //spriteBatch.DrawLine(npc.Center, lens.Top, Color.Red, 4);
            //spriteBatch.DrawLine(npc.Center, npc.Center + fromEyeToLensCenter * 20, Color.White, 4);
            //spriteBatch.DrawLine(lens.Top, lens.Top + newRayDirection * 20, Color.Blue, 4);
            //spriteBatch.DrawLine(lensFocus - Vector2.UnitY.RotatedBy(lens.rotation) * 800, lensFocus + Vector2.UnitY.RotatedBy(lens.rotation) * 800, Color.Brown, 4);
            //spriteBatch.DrawLine(lens.Center, lens.Center + fromEyeToLensTop * 20, Color.Green, 4);
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (clone)
            {
                hit.HideCombatText = true;
                NPC _npc = Main.npc[(int)npc.ai[1]];
                if (_npc.immortal)
                {
                    _npc.immortal = false;
                    _npc.StrikeNPC(hit);
                    _npc.immortal = true;
                }
                else
                {
                    _npc.StrikeNPC(hit);
                }
            }
        }
        public override bool CheckDead(NPC npc)
        {
            if (clone)
            {
                npc.active = false;
                return false;
            }
            if (phase != 5)
            {
                npc.immortal = true;
                npc.life = 1;
                if (phase < 3)
                {
                    phase = 2;
                    anim = new _2To3PhaseTransitionAnimation(npc);
                    anim.Active = true;
                }
                return false;
            }
            return true;
        }
        public override bool? CanBeHitByItem(NPC npc, Terraria.Player player, Item item)
        {
            return canBeHit? null : false;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return canBeHit;
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return canBeHit? null : false;
        }
        public override bool CanHitPlayer(NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            if (!canHit)
            {
                return false;
            }
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override void OnKill(NPC npc)
        {
            BossDownedSystem.BossDowned(1);
        }
    }
    //public class EyeofCthulhuSystem : ModSystem
    //{
    //    public static List<Tile> replasedTiles = [];
    //    public static List<Point> tileCoordinates = [];
    //    public static List<Point> tilesToClear = [];
    //    public override void PostUpdateWorld()
    //    {
    //        if (replasedTiles.Count != 0 && !NPC.AnyNPCs(NPCID.EyeofCthulhu))
    //        {
    //            for (int i = 0; i < replasedTiles.Count; i++)
    //            {
    //                Main.tile[tileCoordinates[i]].CopyFrom(replasedTiles[i]);
    //            }
    //            replasedTiles = [];
    //            tileCoordinates = [];
    //            for (int i = 0; i < tilesToClear.Count; i++)
    //            {
    //                Main.tile[tilesToClear[i]].Clear(TileDataType.Tile);
    //            }
    //        }
    //    }
    //}
}