using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terrapain.Content.Buffs;
using Terrapain.Content.Projectiles.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class AmethystSharp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.GetT().afterimage = true;
            Projectile.GetT().afterimagesCount = 5;
            Projectile.GetT().drawTrail = true;
            Projectile.GetT().trailColor = Color.Purple * 0.5f;
            Projectile.GetT().trailLength = 10;
            Projectile.GetT().trailWidth = 18;
        }
        float angularVelocity;
        static UnifiedRandom ur = new UnifiedRandom();
        public override void OnSpawn(IEntitySource source)
        {
            angularVelocity = ur.NextFloat() * 0.3f - 0.15f;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.02f, 0.55f);
            if (Projectile.timeLeft % 5 == 0 && ur.NextBool(3))
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
            }
        }
    }
}
