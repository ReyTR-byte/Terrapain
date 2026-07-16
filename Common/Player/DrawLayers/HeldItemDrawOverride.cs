using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace Terrapain.Common.Player.DrawLayers
{
    public class HeldItemDrawOverride : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return TGlobalItem.UseDrawOverride.Contains(drawInfo.drawPlayer.HeldItem.useStyle) && drawInfo.drawPlayer.ItemAnimationActive && !drawInfo.drawPlayer.HeldItem.noUseGraphic;
        }
        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.HeldItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            int num = drawInfo.drawPlayer.HeldItem.type;

            Main.instance.LoadItem(num);
            Asset<Texture2D> t = TextureAssets.Item[num];
            Rectangle frame = ((Main.itemAnimations[num] == null) ? t.Frame() : Main.itemAnimations[num].GetFrame(t.Value));

            Vector2 position = drawInfo.drawPlayer.itemLocation - Main.screenPosition;

            int dir = drawInfo.drawPlayer.direction;
            int swingDir = drawInfo.drawPlayer.HeldItem.GetT().drawDir;

            SpriteEffects effects = SpriteEffects.None;
            float rotationAdd = 0;
            Vector2 origin = drawInfo.drawPlayer.HeldItem.GetT().DrawOrigin?? new Vector2(0, frame.Height);
            if (swingDir == 0)
            {
                if (dir == -1)
                {
                    rotationAdd = MathF.PI;
                    origin.Y = frame.Height - origin.Y;
                    effects = SpriteEffects.FlipVertically;
                }
            }
            else
            {
                if (dir == -1 && swingDir == 1 || swingDir == -1 && dir == 1)
                {
                    effects = SpriteEffects.FlipHorizontally;
                    origin.X = frame.Width - origin.X;
                }
                if (swingDir == -1)
                {
                    rotationAdd += 2 * (drawInfo.drawPlayer.HeldItem.GetT().spriteRotation.HasValue? MathF.PI / 2 - drawInfo.drawPlayer.HeldItem.GetT().spriteRotation.Value : MathF.PI / 4) * dir;
                }
            }


            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                frame,
                Lighting.GetColor(drawInfo.drawPlayer.itemLocation.ToTileCoordinates()),
                drawInfo.drawPlayer.itemRotation + rotationAdd,
                origin,
                drawInfo.drawPlayer.GetAdjustedItemScale(drawInfo.heldItem),
                effects,
                0
                )
            );
        }
    }
}
