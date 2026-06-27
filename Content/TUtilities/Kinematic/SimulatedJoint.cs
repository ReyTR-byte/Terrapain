using Luminance.Common.Easings;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Terrapain.Content.TUtilities.Kinematic
{
    public struct SimulatedJoint
    {
        public SimulatedJoint(float length, float mass, Vector2 position, float rotation = 0)
        {
            this.length = length;
            this.mass = mass;
            this.position = position;
        }
        public float length;
        public float mass;
        public Vector2 velocity;
        public Vector2 position;
        public Vector2 futurePosition => fixedAt?? position + velocity;
        public Vector2? fixedAt;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Force"></param>
        /// <param name="ForcePoint">-1 on the end, 0 on center, 1 on the start</param>
        public void ApplyForce(Vector2 Force)
        {
            velocity += Force / mass;
        }
        public void Update()
        {
            if (fixedAt.HasValue)
            {
                position = fixedAt.Value;
                velocity = Vector2.Zero;
            }
            else
            {
                position += velocity;
            }
        }
    }
}
