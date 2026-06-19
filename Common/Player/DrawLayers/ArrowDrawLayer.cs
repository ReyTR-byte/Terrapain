using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Common.Global;
using Terrapain.Common.Global.UseStyles;
using Terrapain.Content.Projectiles.Ammo.Bouquet;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Terrapain.Common.Player.DrawLayers
{
    public class ArrowDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.HeldItem.useStyle == TGlobalItem.BowOverride && drawInfo.drawPlayer.ItemAnimationActive;
        }
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HeldItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Item item = drawInfo.drawPlayer.HeldItem;

            Main.instance.LoadProjectile(item.GetGlobalItem<BowsOverride>().projectile);
            Asset<Texture2D> t = TextureAssets.Projectile[item.GetGlobalItem<BowsOverride>().projectile];
            Rectangle frame = new Rectangle(0, 0, t.Width(), t.Height() / Main.projFrames[item.GetGlobalItem<BowsOverride>().projectile]);
 
            int dir = drawInfo.drawPlayer.direction;

            float rotationAdd = 0;
            
                if (dir == 1)
                {
                    rotationAdd = MathF.PI / 2;
                }
                else
                {
                    rotationAdd = -MathF.PI / 2;
                }

            Vector2 position = drawInfo.drawPlayer.MountedCenter + TGlobalItem.GetHandOffset(drawInfo.drawPlayer) - Vector2.UnitY.RotatedBy(drawInfo.drawPlayer.itemRotation + rotationAdd) * 30 - Main.screenPosition;
            position += Vector2.UnitY.RotatedBy(drawInfo.drawPlayer.itemRotation + rotationAdd) * MathF.Min((float)item.GetGlobalItem<BowsOverride>().bowTime / drawInfo.drawPlayer.itemAnimationMax, 5) * 3;

            if (item.GetGlobalItem<BowsOverride>().projectile == ModContent.ProjectileType<BouquetArrow>())
            {
                rotationAdd -= MathF.PI / 2;
            }
            int p = Projectile.NewProjectile(drawInfo.drawPlayer.GetSource_FromThis(), position, Vector2.Zero, item.GetGlobalItem<BowsOverride>().projectile, 0, 0, drawInfo.drawPlayer.whoAmI);
            float scale = Main.projectile[p].scale;
            Main.projectile[p].active = false;

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = frame.Size() / 2;

            
            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                frame,
                Lighting.GetColor(drawInfo.drawPlayer.itemLocation.ToTileCoordinates()),
                drawInfo.drawPlayer.itemRotation + rotationAdd,
                origin,
                scale,
                effects,
                0
                )
            );
        }
    }
}
