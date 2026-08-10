using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class CrownGem : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.GetT().useModDrawingInPreDraw = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().trail = new()
            {
                startColor = Color.Red * 0.5f,
                endColor = Color.Red * 0.5f,
                length = 10,
                startWidth = 17,
                smooth = false
            };
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Kill();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
            }
            SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
        }
    }
}
