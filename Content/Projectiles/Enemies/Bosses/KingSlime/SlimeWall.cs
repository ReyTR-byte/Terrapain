using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Luminance.Common.Utilities;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.Utilities;
using Terrapain.Common.Player;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class SlimeWall : ModProjectile
    {
        public int width
        {
            get => Tiles * 40;
            set => Tiles = value / 40;
        }
        public int Tiles
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public Vector2 dir
        {
            get => Projectile.velocity.Normalized();
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        UnifiedRandom random = new UnifiedRandom();
        public override void AI()
        {
            Vector2 rotatedDir = dir.RotatedBy(MathF.PI / 2);
            Vector2 start = Projectile.Center - rotatedDir * (Tiles - 1) * 20;
            for (int i = 0; i < Tiles; i++)
            {
                if (random.NextBool(12))
                {
                    Vector2 pos1 = start + rotatedDir * 40 * i - rotatedDir * 20;
                    Vector2 pos2 = start + rotatedDir * 40 * i + rotatedDir * 20;
                    Vector2 pos = pos1 + (pos2 - pos1) * random.NextFloat();
                    Dust.NewDust(pos, 0, 0, DustID.t_Slime, newColor: Color.LightBlue);
                }
            }
            foreach (var player in Main.ActivePlayers)
            {
                if (!player.dead)
                {
                    Vector2 Pos = Vector2.Zero;
                    if (Functions.Collision(Projectile.Center - dir.RotatedBy(0.5f * MathF.PI) * width / 2, dir.RotatedBy(0.5f * MathF.PI), width, player.position, player.width, player.height, ref Pos, false))
                    {
                        if (dir.Y < -0.25f)
                        {
                            if (player.equippedWings != null)
                            {
                                player.wingTime = ArmorIDs.Wing.Sets.Stats[player.equippedWings.wingSlot].FlyTime;
                            }
                            player.GetModPlayer<PlayerMovement>().DownMovementTimer = 20;
                        }
                        if (dir.Y > 0.25f)
                        {
                            player.GetModPlayer<PlayerMovement>().UpMovementTimer = 20;
                        }
                        if (dir.X < -0.25f)
                        {
                            player.GetModPlayer<PlayerMovement>().RightMovementTimer = 20;
                        }
                        if (dir.X > 0.25f)
                        {
                            player.GetModPlayer<PlayerMovement>().LeftMovementTimer = 20;
                        }
                        Vector2 p1 = player.Custom().oldPositions[1] - Projectile.oldPosition - Projectile.Size / 2;
                        Vector2 p2 = p1 + new Vector2(player.width, 0);
                        Vector2 p3 = p1 + new Vector2(0, player.height);
                        Vector2 p4 = p1 + new Vector2(player.width, player.height);
                        p1.RotateBy(-Projectile.rotation);
                        p2.RotateBy(-Projectile.rotation);
                        p3.RotateBy(-Projectile.rotation);
                        p4.RotateBy(-Projectile.rotation);
                        if (p1.X > 0 && p2.X > 0 && p3.X > 0 && p4.X > 0)
                        {
                            player.velocity.RotateBy(-Projectile.rotation);
                            player.velocity.X = Math.Abs(player.velocity.X * 1.05f);
                            float speed = player.velocity.X;
                            player.velocity.RotateBy(Projectile.rotation);
                            player.velocity += Projectile.velocity.Normalized() * MathHelper.Clamp(20 - speed, 0, Projectile.velocity.Length() * 2);
                            player.position += player.velocity;
                            if ((dir.X > 0.5f || dir.X < -0.5f) && player.itemAnimation == 0)
                            {
                                player.ChangeDir(dir.X.NonZeroSign());
                            }
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 rotatedDir = dir.RotatedBy(MathF.PI / 2);
            Vector2 start = Projectile.Center - rotatedDir * (Tiles - 1) * 20;
            for (int i = 0; i < Tiles; i++)
            {
                int f = i == 0? 0 : i == Tiles - 1? 2 : 1;
                Rectangle frame = new Rectangle(0, 40 * f, 40, 40);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, start + rotatedDir * 40 * i - Main.screenPosition, frame, Lighting.GetColor((start + rotatedDir * 40 * i).ToTileCoordinates()), Projectile.rotation, new Vector2(36, 20), 1, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
