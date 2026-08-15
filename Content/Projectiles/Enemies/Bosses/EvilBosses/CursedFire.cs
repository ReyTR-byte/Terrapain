using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Content.Projectiles.Abstract;
using Terrapain.Content.TUtilities.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class CursedFire : Laser
    {
        public override Vector2 direction { get => Projectile.ai[0].ToRotationVector2(); set => Projectile.ai[0] = value.ToRotation(); }
        public override float lenght { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public List<Vector2> offsets = [];

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.hostile = true;
            Projectile.timeLeft = 6;
            Projectile.tileCollide = false;
        }
        static UnifiedRandom random = new UnifiedRandom();
        public override void AI()
        {
            RebuildLaserHitBox();
            if (random.NextBool(50))
            {
                Dust.NewDust(Projectile.Center + direction * lenght * random.NextFloat(), 0, 0, DustID.Smoke, Scale: 2.5f);
            }
            if (offsets.Count == 0 || offsets[0].Y > 10)
            {
                offsets.Insert(0, Vector2.Zero);
            }
            for (int i = 0; i < offsets.Count; i++)
            {
                offsets[i] += new Vector2(random.NextFloat(-2, 2), random.Next(3, 6));
            }
            while (offsets.Count > 0 && offsets.Last().Y > lenght)
            {
                offsets.RemoveAt(offsets.Count - 1);
            }
            for (int i = 1; i < offsets.Count; i++)
            {
                if(offsets[i].Y - 8 < offsets[i - 1].Y)
                {
                    offsets.RemoveAt(i);
                    i--;
                }
            }
            if (offsets.Count > 2)
            {
                Vector2 Target = (Vector2.Zero + offsets[1]) / 2;
                offsets[0] = offsets[0] * 0.6f + Target * 0.4f;
                for (int i = 1; i < offsets.Count - 1; i++)
                {
                    Target = (offsets[i - 1] + offsets[i + 1]) / 2;
                    offsets[i] = offsets[i] * 0.6f + Target * 0.4f;
                }
                Target = (new Vector2(0, lenght) + offsets[offsets.Count - 2]) / 2;
                offsets[offsets.Count - 1] = offsets[offsets.Count - 1] * 0.6f + Target * 0.4f;
            }
            if (offsets.Count == 2)
            {
                Vector2 Target = (Vector2.Zero + offsets[1]) / 2;
                offsets[0] = offsets[0] * 0.6f + Target * 0.4f;
                Target = (new Vector2(0, lenght) + offsets[0]) / 2;
                offsets[1] = offsets[1] * 0.6f + Target * 0.4f;
            }
            if (offsets.Count == 1)
            {
                Vector2 Target = new Vector2(0, lenght / 2);
                offsets[0] = offsets[0] * 0.6f + Target * 0.4f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
                if (offsets.Count == 2)
                {

                }
                if (offsets.Count > 2)
                {

                }
                float WidthFunction(float progress, float length, float totalLength, Vector2 position)
                {
                    return 20 * (1 - progress);
                }
                Color ColorFunction(float progress, float length, float totalLength, Vector2 position)
                {
                    return new Color(Color.Yellow.ToVector4() * (1 - progress) + Color.Red.ToVector4() * progress);
                }

                List<Vector2> points = [Projectile.Center];
                foreach (var point in offsets)
                {
                    points.Add(point.RotatedBy(Projectile.ai[0] - MathF.PI / 2) + Projectile.Center);
                }
                points.Add(Projectile.Center + direction * lenght);
                points = Graphics.SmoothTrail(points, 6);
                TrailSettings ts = new TrailSettings(WidthFunction, ColorFunction);
                Graphics.RenderTrail(points, ts);
                ManagedShader startShader = ShaderManager.GetShader("Terrapain.TrailStart");
                var blackTile = ExtraTextureRegistry.BlackPixel;
                float rotation = points[1].DirectionTo(points[0]).ToRotation();
                //foreach (var point in points)
                //{
                //    spriteBatch.Draw(blackTile.Value, point - Main.screenPosition, null, startColor, 0, new Vector2(0.5f, 0.5f), 2, SpriteEffects.None, 0);
                //}
                Vector2 pos = points[0];
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, startShader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(blackTile.Value, pos - Main.screenPosition, null, Color.Yellow, rotation, new Vector2(0, 0.5f), new Vector2(20, 40), SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
