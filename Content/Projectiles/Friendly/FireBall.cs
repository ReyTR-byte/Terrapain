using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class FireBall : ModProjectile
    {
        int state
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        bool Shoot
        {
            get => Projectile.ai[1] == 1;
            set => Projectile.ai[1] = value ? 1 : 0;
        }
        bool rotateToMouse
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        bool oldShoot;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 15;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        float speed;
        int basicDamage;
        public override void OnSpawn(IEntitySource source)
        {
            basicDamage = Projectile.damage;
            speed = Projectile.velocity.Length();
            float angle = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
			if (Projectile.velocity.Y < 0)
				angle = 2 * Convert.ToSingle(Math.PI) - angle;
            Projectile.rotation = angle;
        }
        UnifiedRandom rand = new UnifiedRandom();
        int timer;
        float angularVelocity;
        int oldState;
        public override void AI()
        {
            float angle = rand.NextFloat() * (float)Math.PI * 2;
            Vector2 posInCircle = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * rand.NextFloat() * (10 + state * 2.5f);
            Dust.NewDust(Projectile.Center + posInCircle, 0, 0, DustID.Torch, posInCircle.X / 25, posInCircle.X / 25, Scale: 2);
                    
            Projectile.damage = basicDamage + basicDamage * state;
            Player owner = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, 2f * (state + 1) / 5f, 1.9f * (state + 1) / 5f, 1.8f * (state + 1) / 5f);
            if (!Shoot)
            {
                Projectile.position = owner.Center + new Vector2((float)Math.Cos(owner.itemRotation - 0.25 * (float)Math.PI * owner.direction) * owner.direction, (float)Math.Sin(owner.itemRotation - 0.25 * (float)Math.PI * owner.direction) * owner.direction) * 50 - new Vector2(Projectile.width / 2, Projectile.height / 2);
                Projectile.timeLeft = 300;
                Projectile.velocity = Vector2.Zero;
            }
            else if (rotateToMouse || (Shoot && !oldShoot))
            {
                float goalAngle = Projectile.AngleTo(Main.MouseWorld);
                goalAngle = goalAngle % (2f * (float)Math.PI);
                if (goalAngle < 0)
                {
                    goalAngle += (float)Math.PI * 2;
                }
                Projectile.rotation = Projectile.rotation % (2f * (float)Math.PI);
                if (Projectile.rotation < 0)
                {
                    Projectile.rotation += (float)Math.PI * 2;
                }

                if (goalAngle < (float)Math.PI)
                {
                    if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.1f;
                    }
                    else
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.1f;
                    }
                }
                else
                {
                    if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
                    {
                        if (angularVelocity < 0.3f)
                            angularVelocity += 0.1f;
                    }
                    else
                    {
                        if (angularVelocity > -0.3f)
                            angularVelocity -= 0.1f;
                    }
                }
                if ((Projectile.rotation + angularVelocity >= goalAngle && Projectile.rotation <= goalAngle) || (Projectile.rotation + angularVelocity <= goalAngle && Projectile.rotation >= goalAngle))
                {
                    Projectile.rotation = goalAngle;
                    angularVelocity = 0;
                }
                else
                {
                    Projectile.rotation += angularVelocity;
                }
                Projectile.velocity = new Vector2((float)Math.Cos(Projectile.rotation), (float)Math.Sin(Projectile.rotation)) * speed;
            }
            timer--;
            if (timer <= 0 || oldState != state)
            {
                Projectile.frame = state * 3 + rand.Next(3);
                timer = 6;
            }
            oldState = state;
            oldShoot = Shoot;
            if (Functions.HitTiles(Projectile.Center - new Vector2((float)(20 + 5 * state) / 2, (float)(20 + 5 * state) / 2), 20 + 5 * state, 20 + 5 * state))
            {
                Projectile.Kill();
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Functions.CircleColision(Projectile.Center, 10 + state * 2.5f, target.position, target.width, target.height))
            {
                return null;
            }
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 60);
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Collision.HitTiles(Projectile.Center - new Vector2((float)(20 + 5 * state) / 2, (float)(20 + 5 * state) / 2), Projectile.velocity, 20 + 5 * state, 20 + 5 * state);
            for (int i = 0; i < 5 * (state + 1); ++i)
            {
                float angle = rand.NextFloat() * (float)Math.PI * 2;
                Vector2 posInCircle = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * rand.NextFloat() * (10 + state * 2.5f);
                Dust.NewDust(Projectile.Center + posInCircle, 0, 0, DustID.Torch, posInCircle.X / 25, posInCircle.X / 25, Scale: 2);
            }
        }
    }
}