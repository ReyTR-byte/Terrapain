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
using Terraria.ModLoader;

namespace Terrapain.Common.Player.DrawLayers
{
    public class ItemDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return TGlobalItem.UseModDrawStyles.Contains(drawInfo.drawPlayer.HeldItem.useStyle) && drawInfo.drawPlayer.ItemAnimationActive;
        }
        public override Position GetDefaultPosition()
        {
            return new Between(Terraria.DataStructures.PlayerDrawLayers.SolarShield, Terraria.DataStructures.PlayerDrawLayers.ArmOverItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Asset<Texture2D> t = TextureAssets.Item[drawInfo.drawPlayer.HeldItem.type];

            //FargoSoulsPlayer player = drawInfo.drawPlayer.FargoSouls();
            Vector2 position = drawInfo.drawPlayer.itemLocation - Main.screenPosition;

            int dir = drawInfo.drawPlayer.direction;
            int swingDir = drawInfo.drawPlayer.HeldItem.GetT().drawDir;

            SpriteEffects effects = SpriteEffects.None;
            float rotationAdd = 0;
            Vector2 origin = drawInfo.drawPlayer.HeldItem.GetT().DrawOrigin.HasValue? drawInfo.drawPlayer.HeldItem.GetT().DrawOrigin.Value : new Vector2(2, t.Height() - 2);
            if (dir == -1 && swingDir == 1 || swingDir == -1 && dir == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
                origin.X = t.Width() - origin.X;
            }
            if (swingDir == -1)
            {
                rotationAdd += 2 * (drawInfo.drawPlayer.HeldItem.GetT().spriteRotation.HasValue? MathF.PI / 2 - drawInfo.drawPlayer.HeldItem.GetT().spriteRotation.Value : MathF.PI / 4) * dir;

            }


            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                null,
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
