using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Common.Player;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories
{
    public class ScorspiderHeartAccesory : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 47));
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.height = 30;
            Item.width = 30;
            Item.value = Item.buyPrice(0, 6, 35, 0);
            Item.damage = 8;
            Item.knockBack = 2; 
        }
        int timer;
        bool oldColision;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            bool colision = Functions.CheckGround(player);
            if (timer == 0 && colision && !oldColision && player.GetModPlayer<TerrapainPlayer>().oldVelocities[1].Y > 12)
            {
                int proj;
                for (int i = 0; i < 15; i++)
                {
                    float rotation = (float)Math.PI * 0.2f + 0.6f * (i / 14f) * (float)Math.PI;
                    Vector2 velocity = Functions.UnitVectorFromRotation(rotation) * 12;
                    velocity.Y *= -1;
                    proj = Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, velocity, ModContent.ProjectileType<ScorspiderShellShard>(), Item.damage, Item.knockBack, player.whoAmI);
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].friendly = true;
                }
                for (int i = 0; i < 21; i++)
                {
                    Vector2 position = player.Center;
                    position.Y -= 200;
                    position.X += (i - 10) * 34;
                    proj = Projectile.NewProjectile(Item.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<ScorspiderSpike>(), Item.damage, Item.knockBack, player.whoAmI, 0, 0, 3);
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].friendly = true;
                }
                proj = Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center, new Vector2(2, 0), ModContent.ProjectileType<ScorspiderRocket>(), Item.damage, Item.knockBack, player.whoAmI);
                Main.projectile[proj].hostile = false;
                Main.projectile[proj].friendly = true;
                proj = Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center, new Vector2(-2, 0), ModContent.ProjectileType<ScorspiderRocket>(), Item.damage, Item.knockBack, player.whoAmI);
                Main.projectile[proj].hostile = false;
                Main.projectile[proj].friendly = true;
            }
            if (colision)
            {
                timer = 30;
            }
            else 
            {
                if (timer > 0)
                {
                    timer--;
                }
            }
            oldColision = colision;
            if (player.controlDown && player.mount.Type == MountID.None)
            {
                player.maxFallSpeed += 4;
                player.gravity += 0.2f * player.gravity.NonZeroSign();
            }
        }
    }
}