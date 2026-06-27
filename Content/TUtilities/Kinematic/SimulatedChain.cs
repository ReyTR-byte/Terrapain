using Luminance.Common.Easings;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Terrapain.Content.TUtilities.Kinematic
{
    public class SimulatedChain
    {
        public SimulatedChain(int fragments, float lengthPerFragment, Vector2 start, float rotation, float mass)
        {
            Fragments = new SimulatedJoint[fragments];
            for (int i = 0; i < fragments; i++)
            {
                Fragments[i] = new SimulatedJoint(lengthPerFragment, mass, start + Vector2.UnitX.RotatedBy(rotation) * (lengthPerFragment / 2 + lengthPerFragment * i), rotation);
            }
        }
        public int Count => Fragments.Length;
        public SimulatedJoint[] Fragments;
        public void Update()
        {
            for (int i = 0; i < Fragments.Length; i++)
            {
                if (!Fragments[i].fixedAt.HasValue)
                    Fragments[i].velocity += Vector2.UnitY * 0.3f;
            }
            float averageDistanceMiss = -1;
            int itteration = 0;
            while ((averageDistanceMiss == -1 || averageDistanceMiss > 0.1f) && itteration < 10)
            {
                for (int i = 0; i < Fragments.Length; i++)
                {
                    if (i != Fragments.Length - 1)
                    {
                        if (Fragments[i].fixedAt != null)
                        {
                            Vector2 pos1 = Fragments[i].fixedAt.Value;
                            Vector2 pos2 = Fragments[i + 1].futurePosition;
                            Vector2 dir = pos1.DirectionTo(pos2);
                            Vector2 pos3 = pos1 + dir * Fragments[i].length;
                            Fragments[i + 1].velocity += pos3 - Fragments[i + 1].futurePosition;
                        }
                        else if (Fragments[i + 1].fixedAt != null)
                        {
                            Fragments[i].velocity += Fragments[i + 1].fixedAt.Value + Fragments[i + 1].fixedAt.Value.DirectionTo(Fragments[i].futurePosition) * Fragments[i].length - Fragments[i].futurePosition;
                        }
                        else
                        {
                            float mass = Fragments[i].mass + Fragments[i + 1].mass;
                            float Force = Fragments[i].futurePosition.Distance(Fragments[i + 1].futurePosition) - Fragments[i].length;
                            Force *= mass;
                            Force /= 4;
                            Vector2 forceDirection = Fragments[i].futurePosition.DirectionTo(Fragments[i + 1].futurePosition);
                            Fragments[i].ApplyForce(forceDirection * Force);
                            Fragments[i + 1].ApplyForce(-forceDirection * Force);
                        }
                    }
                }
                itteration++;
                for (int i = 0; i < Fragments.Length - 1; i++)
                {
                    averageDistanceMiss += MathF.Abs(Fragments[i].futurePosition.Distance(Fragments[i + 1].futurePosition) - Fragments[i].length);
                }
                averageDistanceMiss /= Fragments.Length - 1;
            }
            for (int i = 0; i < Fragments.Length; i++)
            {
                Fragments[i].Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture2D, Rectangle? frame, Color color, bool lightColor, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerDepth)
        {
            for (int i = 0; i < Fragments.Length - 1; i++)
            {
                Vector2 position = (Fragments[i].position + Fragments[i + 1].position) / 2;
                Color drawColor = color;
                if (lightColor)
                {
                    drawColor = Lighting.GetColor(position.ToTileCoordinates(), drawColor);
                }
                float rotation = Fragments[i].position.DirectionTo(Fragments[i + 1].position).ToRotation();
                spriteBatch.Draw(texture2D, position - Main.screenPosition, frame, drawColor, rotation, origin, scale, effect, layerDepth);
            }
        }
        public void DrawAsLines(SpriteBatch sprite, float Width, Color color)
        {
            for (int i = 0; i < Fragments.Length - 1; i++)
            {
                Vector2 start = Fragments[i].position;
                Vector2 end = Fragments[i + 1].position;
                sprite.DrawLine(start, end, color, Width);
            }
        }
    }
}
