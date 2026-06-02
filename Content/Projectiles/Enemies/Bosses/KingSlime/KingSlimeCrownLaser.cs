using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class KingSlimeCrownLaser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        float lenght
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        NPC KingSlime
        {
            get
            {
                if ((int)Projectile.ai[1] < 0 || (int)Projectile.ai[1] >= Main.maxNPCs)
                {
                    return null;
                }

                return Main.npc[(int)Projectile.ai[1]];
            }
        }
        Projectile MainLaser
        {
            get { 
                if ((int)Projectile.ai[1] < 0 || (int)Projectile.ai[1] >= Main.maxProjectiles)
                {
                    return null;
                }

                return Main.projectile[(int)Projectile.ai[1]];
            } 
        }
        float AngularAcceleration
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        float rotationAboutMainLaser
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        Vector2 directionFromMainLaser
        {
            get => Projectile.ai[2].ToRotationVector2();
        }
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
            set => Projectile.rotation = value.ToRotation();
        }
        bool main;
        float angularVelocity;
        float maxRotationSpeed;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source.Context == "Main")
            {
                dir = Projectile.velocity;
                maxRotationSpeed = Projectile.velocity.Length() / 60;
                Projectile.velocity = Vector2.Zero;
                main = true;
                Projectile.NewProjectile(Projectile.GetSource_FromThis("Suport"), Projectile.Center, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, -1, AngularAcceleration.NonZeroSign(), Projectile.whoAmI, dir.RotatedBy(MathF.PI / 2).ToRotation());
                Projectile.NewProjectile(Projectile.GetSource_FromThis("Suport"), Projectile.Center, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, -1, AngularAcceleration.NonZeroSign(), Projectile.whoAmI, dir.RotatedBy(-MathF.PI / 2).ToRotation());
            }
            else
            {
                Projectile.spriteDirection = (int)Projectile.ai[0];
                lenght = 0;
            }
        }
        public override bool? CanCutTiles()
        {
            Functions.RayCutTile(Projectile.Center, Projectile.Center + dir * lenght, Main.player[Projectile.owner]);
            return false;
        }
        public override void AI()
        {
            if (main)
            {
                Projectile.alpha = Math.Max(Projectile.alpha - 5, 0);
                if (Projectile.timeLeft < 50)
                {
                    Projectile.alpha = 255 - (int)(MathF.Min(Projectile.timeLeft / 50f, 1) * 255);
                }
                if (KingSlime != null && KingSlime.active && KingSlime.type == NPCID.KingSlime && !KingSlime.immortal)
                {
                    Vector2 Center = KingSlime.Top;
                    switch (KingSlime.frame.Y)
                    {
                        case 0:
                            Center = KingSlime.Top - Vector2.UnitY * 15;
                            break;
                        case 120:
                            Center = KingSlime.Top - Vector2.UnitY * 25;
                            break;
                        case 240:
                            Center = KingSlime.Top - Vector2.UnitY * 15;
                            break;
                        case 360:
                            Center = KingSlime.Top - Vector2.UnitY * 5;
                            break;
                        case 480:
                            Center = KingSlime.Top - Vector2.UnitY * 15;
                            break;
                        case 600:
                            Center = KingSlime.Top - Vector2.UnitY * 17;
                            break;
                    }

                    Projectile.rotation += angularVelocity;
                    angularVelocity += AngularAcceleration;
                    angularVelocity = MathF.Min(MathF.Abs(angularVelocity), maxRotationSpeed) * AngularAcceleration.NonZeroSign();
                    lenght += 18;
                    lenght = Math.Min(lenght, 2000);
                    Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                    Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                    Projectile.Center = Center;
                }
                else
                {
                    Projectile.active = false;
                }
            }
            else
            {
                if (MainLaser != null && MainLaser.active && MainLaser.type == Type)
                {
                    Projectile.timeLeft = MainLaser.timeLeft;
                    Vector2 targetPosition = MainLaser.Center + directionFromMainLaser * 50;
                    rotationAboutMainLaser -= 0.1f * Projectile.spriteDirection;
                    float maxVelocity = 25;
                    float maxVelocityMultyplier = 1;
                    if (targetPosition != Projectile.Center)
                    {
                        Projectile.velocity = Projectile.DirectionTo(targetPosition) * Projectile.velocity.Length();
                        Projectile.velocity += Projectile.DirectionTo(targetPosition) * 1.2f;
                    }
                    if (Projectile.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultyplier = 1 - (75 - Projectile.Distance(targetPosition)) / 75;
                    }
                    if (Projectile.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                    {
                        Projectile.velocity = Projectile.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                    }
                    Projectile.alpha = Math.Max(Projectile.alpha - 5, 0);
                    lenght += 18;
                    if (MainLaser.timeLeft < 50)
                    {
                        Projectile.alpha = MainLaser.alpha;
                    }
                    dir = Projectile.DirectionTo(MainLaser.Center + MainLaser.rotation.ToRotationVector2() * 750);
                    Vector2 Center = Projectile.Center;
                    lenght = Math.Min(lenght, 2000);
                    Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                    Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                    Projectile.Center = Center;
                }
                else
                {
                    Projectile.active = false;
                }
            }
            float totalLength = 0;
            while (totalLength < lenght)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, TorchID.Red);
                totalLength += 20;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            if (Functions.Collision(Projectile.Center, dir, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override void OnHitPlayer(Terraria.Player target, Terraria.Player.HurtInfo info)
        {
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Vector2 collide = Functions.RayColisionInTheWorld(Projectile.Center, Projectile.Center + dir * lenght);
            if (collide != Vector2.Zero)
            {
                lenght = Projectile.Distance(collide);

            }
            return false;
        }
        UnifiedRandom random = new UnifiedRandom();
        int shaderTime;
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = 1 - (Projectile.alpha / 255f);
            if (!main)
            {
                Texture2D Ruby = ModContent.Request<Texture2D>("Terrapain/Content/Projectiles/Enemies/Bosses/KingSlime/CrownGem").Value;
                Main.spriteBatch.Draw(Ruby, Projectile.Center - Main.screenPosition, null, lightColor * opacity, 0, Ruby.Size() / 2, 1, SpriteEffects.None, 0);
            }
            float width = random.NextFloat(25, 28);
            ManagedShader Shade = ShaderManager.GetShader("Terrapain.LaserShader");
            Shade.TrySetParameter("lenght", lenght + width);
            Shade.TrySetParameter("width", width);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.Red * 0.8f * opacity, width);
            width *= 3;
            ManagedShader shader = ShaderManager.GetShader("Terrapain.DiamondLaserGlowShader");
            shader.TrySetParameter("color", (Color.Pink * 0.85f * opacity).ToVector4());
            shader.TrySetParameter("width", width);
            shader.TrySetParameter("height", lenght + width);
            shader.TrySetParameter("rastyajenie", 500 / width);
            shader.TrySetParameter("time", shaderTime);
            shader.TrySetParameter("speed", 0.96f);
            Texture2D texture = ExtraTextureRegistry.Glow2.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.DrawLine(Projectile.Center - dir * width / 2, Projectile.Center + dir * (lenght + width / 2), Color.Pink, width, texture);
            shaderTime++;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public float WidthFunction(float value) => 12f;
        public Color ColorFunction(float value) => Color.LightBlue;
    }
}