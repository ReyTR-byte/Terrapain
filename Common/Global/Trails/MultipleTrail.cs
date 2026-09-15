using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;

namespace Terrapain.Common.Global.Trails
{
    public class MultipleTrail : ProjectileTrail
    {
        public List<ProjectileTrail> trails;
        public override void Update(Projectile proj)
        {
            foreach (var trail in trails)
            {
                trail.Update(proj);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Projectile proj)
        {
            foreach (var trail in trails)
            {
                trail.Draw(spriteBatch, proj);
            }
        }
    }
}