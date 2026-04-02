using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies
{
    public class UVLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.penetrate = -1; 
            Projectile.timeLeft = 600;
            Projectile.alpha = 60; 
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.GetT().fullLight = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().useModDrawingInPostDraw = true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.8f, 0, 0.8f);
            Projectile.rotation = Functions.AngleFromVector(Projectile.velocity);
        }
    }
}
