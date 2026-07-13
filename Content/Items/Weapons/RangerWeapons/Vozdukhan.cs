using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Content.NPCs;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace Terrapain.Content.Items.Weapons.RangerWeapons
{
    public class Vozdukhan : ModItem
    {
        public bool active => life > 0;
        int lifeMax = 5;
        int life = 5;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(99999999, 2));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 18;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = TGlobalItem.ShootOverride;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item36;
            
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 10;
            Item.knockBack = 6f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<VozdukhanProjectile>();
            Item.shootSpeed = 15f;
            Item.value = Item.buyPrice(gold: 1);

            Item.noUseGraphic = true;
        }
        public override void UpdateInventory(Player player)
        {
            if (active)
            {
                Item.UseSound = SoundID.Item36;
            }
            else
            {
                Item.UseSound = null;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (life <= 0)
                {
                    return false;
                }
                life--;
                return true;   
            }
            life++;
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return life < lifeMax;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            int width = texture.Width;
            int height = texture.Height / 2;
            int Frame = active? 0 : 1;
            frame =  new(0, height * Frame, width, height);
            spriteBatch.Draw(texture, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0);
            if (Item.color != Color.Transparent)
            {
                spriteBatch.Draw(texture, position, frame, itemColor, 0, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            int width = texture.Width;
            int height = texture.Height / 2;
            int Frame = active ? 0 : 1;
            Rectangle frame = new(0, height * Frame, width, height);

            Vector2 vector = frame.Size() / 2f;
            Vector2 vector2 = new Vector2((float)(Item.width / 2) - vector.X, Item.height - frame.Height);
            Vector2 vector3 = Item.position - Main.screenPosition + vector + vector2;

            spriteBatch.Draw(texture, vector3, frame, alphaColor, rotation, vector, scale, SpriteEffects.None, 0);
            alphaColor.A = 0;
            if (Item.shimmered)
            {
                spriteBatch.Draw(texture, vector3, frame, alphaColor, rotation, vector, scale, SpriteEffects.None, 0);
            }
            if (Item.color != Color.Transparent)
            {
                spriteBatch.Draw(texture, vector3, frame, Item.GetColor(lightColor), rotation, vector, scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(0, 6);
        }
    }
}
