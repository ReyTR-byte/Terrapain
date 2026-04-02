using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.UI.Assets.AbilitiesIcons
{
    public abstract class AbilityIcon
    {
        public virtual string Texture => this.GetPath();
        public virtual Vector2 DrawCenter => Vector2.Zero;
        public int frameCount;
        public int animationSpeed;
        // 0 = no animation, 1 = animate always, 2 = animate on use
        public int animationType;
        public int animationTimer;
        public int frame;
        public virtual Rectangle FindFrame(Texture2D texture, int reload)
        {
            if(animationType == 0)
            {
                return texture.Bounds;
            }
            Rectangle Frame = texture.Bounds;
            Frame.Height /= frameCount;
            if (animationType == 1)
            {
                if (animationTimer == 0)
                {
                    frame++;
                    animationTimer = animationSpeed;
                    if (frame >= frameCount)
                        frame = 0;
                }
            }
            else if (animationType == 2)
            {
                frame = reload / animationType;
                if (frame > frameCount - 1)
                {
                    frame = frameCount - 1;
                }
            }
            Frame.Y = Frame.Height * frame;
            return Frame;
        }
        public virtual void Draw(SpriteBatch spriteBach, Vector2 position, Vector2 scale, int reload)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            spriteBach.Draw(texture.Value, position, FindFrame(texture.Value, reload), Color.White, 0, DrawCenter, scale, SpriteEffects.None, 0);
            if (animationTimer > 0)
            { 
                animationTimer--; 
            }
            else if (animationTimer < 0)
            {
                animationTimer = 0;
            }
        }
    }
}
