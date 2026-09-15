using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;

namespace Terrapain.Common.Global.Trails
{
    public class SwordTrail : ProjectileTrail
    {
        public List<Vector2> catchTop = [];
        public List<Vector2> catchBottom = [];
        public Vector2 Top;
        public Vector2 Bottom;
        public override void Update(Projectile proj)
        {
            catchTop.Add(proj.Center + (Top * proj.scale).RotatedBy(proj.rotation));
            catchBottom.Add(proj.Center + (Bottom * proj.scale).RotatedBy(proj.rotation));
            if (length >= 0)
            {
                while (catchTop.Count > length)
                {
                    catchTop.RemoveAt(0);
                    catchBottom.RemoveAt(0);
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Projectile proj)
        {
            if (catchTop.Count > 1)
            {
                Graphics.RenderSwordTrail(catchTop, catchBottom, TopColor, BottomColor, null);
            }
        }

        private Color BottomColor(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
        {
            return Color.Transparent;
        }

        private Color TopColor(float trailLengthInterpolant, float length, float totatlLength, Vector2 Position)
        {
            return new Color(startColor.ToVector4() * trailLengthInterpolant + endColor.ToVector4() * (1 - trailLengthInterpolant));
        }
    }
}