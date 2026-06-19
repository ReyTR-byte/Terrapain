using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Ammo.Bouquet
{
    public class BouquetArrow : ModProjectile
    {
        public static List<int> petals = new List<int>();
        public override void SetStaticDefaults()
        {
            petals.Add(ModContent.ProjectileType<DaybloomPetal>());
            petals.Add(ModContent.ProjectileType<MoonglowPetal>());
            petals.Add(ModContent.ProjectileType<BlinkrootPetal>());
            petals.Add(ModContent.ProjectileType<DeathweedPetal>());
            petals.Add(ModContent.ProjectileType<WaterleafPetal>());
            petals.Add(ModContent.ProjectileType<FireblossomPetal>());
            petals.Add(ModContent.ProjectileType<ShiverthornPetal>());
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
        }
        UnifiedRandom random = new();
        public override void AI()
        {
            Projectile.velocity.Y += 0.09f;
            float angel = (float)Math.Acos(Projectile.velocity.X / Projectile.velocity.Length());
            if (Projectile.velocity.Y < 0)
                angel = 2 * (float)Math.PI - angel;
            Projectile.rotation = angel;
            if (Projectile.timeLeft != 600 && Projectile.timeLeft % 10 == 0 && Projectile.ai[0] < petals.Count)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.1), petals[random.Next(petals.Count)], Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
