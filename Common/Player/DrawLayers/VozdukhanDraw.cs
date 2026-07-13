using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content;
using Terrapain.Content.Items.Weapons.RangerWeapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Terrapain.Common.Player.DrawLayers
{
    public class VozdukhanDraw : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.heldItem.type == ModContent.ItemType<Vozdukhan>() && drawInfo.drawPlayer.ItemAnimationActive;
        }
        public override Position GetDefaultPosition()
        {
            return new Between(PlayerDrawLayers.SolarShield, PlayerDrawLayers.ArmOverItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Asset<Texture2D> t = TextureAssets.Item[drawInfo.drawPlayer.HeldItem.type];
            Vozdukhan item = (Vozdukhan)drawInfo.heldItem.ModItem;

            int width = t.Width();
            int height = t.Height() / 2;
            int Frame = item.active? 0 : 1;
            Rectangle frame = new(0, Frame * height, width, height);
            Vector2 position = drawInfo.drawPlayer.itemLocation - Main.screenPosition;

            int dir = drawInfo.drawPlayer.direction;

            SpriteEffects effects = SpriteEffects.None;
            float rotationAdd = 0;
            Vector2 origin = new Vector2(2, t.Height() / 2 - 2);
            if (dir == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
                origin.X = t.Width() - origin.X;
            }


            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                frame,
                Lighting.GetColor(drawInfo.drawPlayer.itemLocation.ToTileCoordinates()),
                drawInfo.drawPlayer.itemRotation,
                origin,
                drawInfo.drawPlayer.GetAdjustedItemScale(drawInfo.heldItem),
                effects,
                0


                )
            );
        }
    }
}
