using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu
{
    public class ServantofCthulhuSoul : ModProjectile
    {
        public static int defDamage = 70;
        NPC target
        {
            get => Main.npc[(int)Projectile.ai[0]];
        }
        List<Vector2> Points = new List<Vector2>();
        List<Vector2> Velocities = new List<Vector2>();
        float maxVelocity = 18;
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }
        Terrapain.LightningDrawInfo lightning;
        UnifiedRandom rand = new UnifiedRandom();
        public override void AI()
        {
            Vector2 vectorToTargetPosition;
            if (Projectile.ai[0] > 0 && Projectile.ai[0] < 200 && target.active)
            {
                vectorToTargetPosition = Projectile.DirectionTo(target.Center);
            }
            else
            {
                vectorToTargetPosition = -Vector2.UnitY;
            }
            Projectile.velocity += vectorToTargetPosition;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
            }
            else
            {
                Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
            }
            if (Projectile.velocity.Length() > maxVelocity)
            {
                Projectile.velocity = Projectile.velocity.Normalized() * maxVelocity;
            }
            if (Points.Count == 0 || rand.Next(4) == 0)
            {
                Points.Add(Projectile.Center);
                Velocities.Add(-Projectile.velocity.RotatedByRandom(0.4) * 0.3f);
            }
            List<Terrapain.LightningPartInfo> parts = new List<Terrapain.LightningPartInfo>();
            Terrapain.LightningPartInfo part = new Terrapain.LightningPartInfo();
            part.start = Projectile.Center;
            part.startWidth = 10;
            for (int i = Points.Count - 1; i >= 0; i--)
            {
                Points[i] += Velocities[i];
                if (Projectile.Distance(Points[i]) > 350)
                {
                    Points.RemoveAt(i);
                    Velocities.RemoveAt(i);
                }
                else
                {
                    part.end = Points[i];
                    part.endWidth = (350 - Projectile.Distance(Points[i])) / 35;
                    parts.Add(part);
                    part.start = part.end;
                    part.startWidth = part.endWidth;
                }
            }
            if (Points.Count == 0)
            {
                Points.Add(Projectile.Center);
                Velocities.Add(-Projectile.velocity.RotatedByRandom(0.4) * 0.3f);
                Points[0] += Velocities[0];
                part.end = Points[0];
                part.endWidth = (350 - Projectile.Distance(Points[0])) / 35;
                parts.Add(part);
                part.start = part.end;
                part.startWidth = part.endWidth;
            }
            lightning = new Terrapain.LightningDrawInfo()
            {
                color = Color.Aquamarine,
                lightningPartInfos = parts,
                start = Projectile.Center,
                end = Points[Points.Count - 1],
                width = 10,
            };
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == -1 || !target.active || target.whoAmI != this.target.whoAmI)
            {
                return false;
            }
            return null;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightning.lightningPartInfos != null)
            {
                Main.spriteBatch.DrawLightning(lightning);
            }
            return false;
        }
    }
}
