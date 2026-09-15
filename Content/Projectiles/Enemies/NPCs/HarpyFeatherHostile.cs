using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Common.Global.Trails;
using Terrapain.Content.TUtilities;
using Terraria.Audio;
using ReLogic.Reflection;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class HarpyFeatherHostile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.HarpyFeather}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 250;
            Projectile.GetT().trail = new ProjectileTrail()
            {
                startWidth = 24,
                endWidth = 0,
                startColor = Color.Blue * 0.4f,
                endColor = Color.Purple * 0.2f,
                length = 10
            };
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source.Context == null)
            {
                AIStyle = -1;
            }
            else
            {
                AIStyle = Convert.ToInt32(source.Context);
            }
            if (AIStyle == 0)
            {
                Projectile.ai[2] = -200;
                MaxSpeed = Projectile.velocity.Length() * 2f;
            }
            else if (AIStyle == 1)
            {
                MaxSpeed = Projectile.velocity.Length() * 1.3f;
            }
            else if (AIStyle == 3)
            {
                MaxSpeed = Projectile.velocity.Length();
            }
        }
        float MaxSpeed;
        int AIStyle;
        public override void AI()
        {
            if (AIStyle == 0)
            {
                if (Projectile.ai[0] > 0)
                {
                    Player Target = Main.player[(int)Projectile.ai[1]];
                    Vector2 dir = Projectile.DirectionTo(Target.Center);
                    AIHelper.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + dir, 0, Math.Max(Projectile.velocity.Length() - 0.1f, 2.5f), 0.2f, 0);
                    Projectile.ai[0]--;
                }
                else
                {
                    if (Projectile.ai[2] == -200)
                    {
                        Player Target = Main.player[(int)Projectile.ai[1]];
                        Vector2 dir = Projectile.DirectionTo(Target.Center);
                        Projectile.ai[1] = dir.X;
                        Projectile.ai[2] = dir.Y;
                    }
                    AIHelper.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + new Vector2(Projectile.ai[1], Projectile.ai[2]), 0.1f, MaxSpeed, 0.2f, 0);
                }
            }
            else if (AIStyle == 1)
            {
                Vector2 dir = Projectile.DirectionTo(new Vector2(Projectile.ai[1], Projectile.ai[2]));
                if (Projectile.ai[0] > 0)
                {
                    AIHelper.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + dir, 0, Math.Max(Projectile.velocity.Length() - 0.1f, 2.5f), 0.2f, 0);
                    Projectile.ai[0]--;
                }
                else
                {
                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[1] = dir.X;
                        Projectile.ai[2] = dir.Y;
                        Projectile.ai[0] = -1;
                    }
                    AIHelper.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + new Vector2(Projectile.ai[1], Projectile.ai[2]), 0.1f, MaxSpeed, 0.8f, 0);
                }
            }
            else if (AIStyle == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Player target = Main.player[(int)Projectile.ai[0]];
                if (target.active && !target.dead)
                {
                    AIHelper.OnlyRotationalMovement(Projectile, target.Center, 0.0175f);
                }
            }
            else if (AIStyle == 3)
            {
                Player player = Main.player[(int)Projectile.ai[1]];
                Projectile.ai[0]--;
                if (Projectile.ai[0] > 60)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.DirectionFrom(player.Center) * 500 + player.Center;
                    Projectile.rotation = Projectile.DirectionTo(player.Center + player.velocity * 20).ToRotation() + MathF.PI / 2;
                }
                else if (Projectile.ai[0] == 60)
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.DirectionFrom(player.Center) * 500 + player.Center;
                    Vector2 pos = player.Center + player.velocity * 20;
                    Vector2 dir = Projectile.DirectionTo(pos);
                    Projectile.rotation = dir.ToRotation() + MathF.PI / 2;
                    Projectile.ai[1] = dir.X;
                    Projectile.ai[2] = dir.Y;
                }
                else if (Projectile.ai[0] > 0)
                {
                    float speed = Functions.EasingIn(60, 60 - Projectile.ai[0]) * 2;
                    Projectile.position.X -= Projectile.ai[1] * speed;
                    Projectile.position.Y -= Projectile.ai[2] * speed;
                }
                else if (Projectile.ai[0] == 0)
                {
                    Projectile.velocity.X = Projectile.ai[1] * MaxSpeed;
                    Projectile.velocity.Y = Projectile.ai[2] * MaxSpeed;
                }
            }
            if (AIStyle != 3)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

			if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
			{
				Projectile.velocity.X = -oldVelocity.X * 0.95f;
			}
			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
			{
				Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
			}
            if (Projectile.ai[0] <= 0 || AIStyle == 2)
            {
                AIStyle = -1;
            }
            return false;
        }
    }
}