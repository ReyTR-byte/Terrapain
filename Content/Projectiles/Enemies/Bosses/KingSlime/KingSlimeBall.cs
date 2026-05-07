using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class KingSlimeBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (onFire)
            {
                target.AddBuff(BuffID.OnFire, 150);
            }

            Vector2 CollidePosition = Vector2.Zero;
            if (Projectile.oldVelocity.Y > 0)
                CollidePosition.X = Math.Abs(target.position.Y - Projectile.Center.Y) * Projectile.oldVelocity.X / Projectile.oldVelocity.Length() + Projectile.Center.X;
            else
            {
                if (Projectile.oldVelocity.Y < 0)
                    CollidePosition.X = Math.Abs(target.position.Y + target.height - Projectile.Center.Y) * Projectile.oldVelocity.X / Projectile.oldVelocity.Length() + Projectile.Center.X;
                else
                {
                    Projectile.velocity.X *= -0.95f;
                    return;
                }
            }
            if (CollidePosition.X > target.position.X && CollidePosition.X < target.position.X + target.width)
            {
                Projectile.velocity.Y *= -0.95f;
            }
            else
            {
                Projectile.velocity.X *= -0.95f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if(Projectile.penetrate == 0)
            {
                Projectile.Kill();
            }
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.95f;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
            }

            return false;
        }
        bool onFire
        {
            get => Projectile.ai[0] == 1;
            set => Projectile.ai[0] = value? 1 : 0;
        }
        public override void AI()
        {
            Projectile.alpha = 255 - (int)(MathF.Min(Projectile.timeLeft / 50f, 1) * 255);
            Projectile.velocity.Y += 0.3f;
            if (Projectile.wet)
            {
                Projectile.velocity *= 0.99f;
                onFire = false;
                Projectile.velocity.Y -= 0.15f;
                Projectile.position += Projectile.velocity / 2;
            }
            if (Projectile.lavaWet)
            {
                Projectile.velocity *= 0.97f;
                onFire = true;
                Projectile.velocity.Y -= 0.2f;
                Projectile.position += Projectile.velocity / 2;
            }
            if (Projectile.honeyWet)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity.Y -= 0.17f;
                Projectile.position += Projectile.velocity / 2;
            }
            if (onFire)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }
            else
            {
                foreach (var player in Main.player)
                {
                    if (player.active && !player.dead && player.HasBuff(BuffID.Inferno) && player.Distance(Projectile.Center) < 150)
                    {
                        onFire = true;
                    }
                }
                if ((int)Projectile.Center.X >= 0 && (int)Projectile.Center.Y >= 0 && ((int)Projectile.Center.X / 16) < Main.maxTilesX && ((int)Projectile.Center.Y / 16) < Main.maxTilesY && ((Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileType == TileID.Campfire && Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileFrameY < 36) || Main.tile[(int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)].TileType == TileID.Torches))
                {
                    onFire = true;
                }
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.timeLeft > 25? null : false;
        }
        public override void OnKill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
