using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class KingSlimeKatana : ModProjectile
    {
        public override string Texture => "Terrapain/Content/Items/Weapons/MeleeWeapons/FireSword";
        public override void SetDefaults()
        {
            Projectile.width = 239;
            Projectile.height = 239;

            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }
        float angularVelocity;
        float angularAcceleration = 0.045f;
        float maxAngularVelocity = 0.6f;
        float DrawRotation
        {
            get {
                if (Projectile.spriteDirection == 1)
                    return Projectile.rotation + 0.99227211237719f;
                else
                    return Projectile.rotation + 2.14932054121260f;
            }
        }
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
        }
        float Length
        {
            get => Projectile.scale * 117.046999107f;
            set => Projectile.scale = value / 117.046999107f;
        }
        int Attack
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        NPC King
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }
        Player Target
        {
            get => King.GetT().Target;
        }
        int Progress
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public override void AI()
        {
            if (King.active && King.type == ModContent.NPCType<NinjaKingSlime>())
            {

                float targetRotation = Projectile.rotation;
                Vector2 targetPosition = Projectile.Center;
                switch (Attack)
                {
                    case 0:
                        {
                            if (Progress < 30)
                            {
                                King.velocity.X *= 0.985f;
                                targetRotation = King.DirectionTo(Target.Center).ToRotation();
                            }
                            else if (Progress < 45)
                            {
                                King.velocity = dir * 18;
                            }

                            float progress1 = EasingIn(30, Progress);
                            float progress2 = EasingInOut(10, Progress - 30);
                            targetPosition = King.Center + dir * 30 - dir * 80 * progress1 + dir * 120 * progress2;
                            Projectile.spriteDirection = dir.X.NonZeroSign();
                            if (Progress > 55)
                            {
                                Progress = 0;
                                if (King.Distance(Target.Center) < 200)
                                {
                                    Attack = 1;
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            float progress1 = EasingOut(20, Progress);
                            float oldProgress2 = EasingInOut(20, Progress - 20 - 1);
                            float progress2 = EasingInOut(20, Progress - 20);
                            float progress3 = MathHelper.Clamp((Progress - 40) / 20f, 0, 1);
                            if (Progress < 20)
                            {
                                targetRotation = King.DirectionTo(Target.Center).ToRotation() + (King.DirectionTo(Target.Center).X > 0 ? MathF.PI / 2 : -MathF.PI / 2);

                                float positiveRotation = AngleBetweenVectors(King.DirectionTo(Target.Center), targetRotation.ToRotationVector2());
                                positiveRotation = NormalizeRotation(positiveRotation);
                                float negativeRotation = AngleBetweenVectors(targetRotation.ToRotationVector2(), King.DirectionTo(Target.Center));
                                negativeRotation = NormalizeRotation(negativeRotation);
                                if (positiveRotation > negativeRotation)
                                {
                                    Projectile.spriteDirection = -1;
                                }
                                else
                                {
                                    Projectile.spriteDirection = 1;
                                }
                            }
                            else
                            {
                                targetRotation += (progress2 - oldProgress2) * MathF.PI * Projectile.spriteDirection;
                                Projectile.rotation = targetRotation;
                            }
                            if (Progress == 20)
                            {
                                King.velocity = King.DirectionTo(Target.Center) * 15;
                            }
                            Projectile.scale = 1 + 2 * progress1 - 2 * progress3;
                            targetPosition = King.Center + targetRotation.ToRotationVector2() * 30;
                            if (Progress > 60)
                            {
                                Progress = 0;
                                if (King.Distance(Target.Center) > 200)
                                {
                                    Attack = 0;
                                }
                            }
                        }
                        break;
                }
                AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, targetRotation, ref Projectile.rotation);
                if (targetPosition != Projectile.Center)
                {
                    Projectile.Center += MathF.Min(25, Projectile.Distance(targetPosition)) * Projectile.DirectionTo(targetPosition);
                }
                Vector2 Center = Projectile.Center;
                Projectile.width = (int)(dir.X * Length * 2);
                Projectile.height = (int)(dir.Y * Length * 2);
                Projectile.Center = Center;
                Progress++;
            }
            else
            {
                Projectile.active = false;
            }
        }
        public override bool? CanCutTiles()
        {
            RayCutTile(Projectile.Center, Projectile.Center + dir * Length, Main.player[Projectile.owner]);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            return Collision(Projectile.Center, dir, Length, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false);
                
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 origin = new Vector2(8, 98);
            if (Projectile.spriteDirection == -1)
            {
                origin.X = TextureAssets.Projectile[Type].Width() - 8;
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, DrawRotation, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
