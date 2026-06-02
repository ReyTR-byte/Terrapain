using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime
{
    public class SmokeBomb : ModProjectile
    {
        int Damage
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int TrueTimeLeft
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }
        static UnifiedRandom random = new();
        public override void OnSpawn(IEntitySource source)
        {
            random = new();
            Projectile.timeLeft = TrueTimeLeft;
            Projectile.rotation = random.NextFloat(MathF.PI * 2);
            Projectile.ai[2] = random.NextFloat(-0.2f, 0.2f);
        }
        public override void AI()
        {
            Projectile.velocity += Vector2.UnitY * 0.3f;
            Projectile.rotation += Projectile.ai[2];
            if(Projectile.timeLeft == 1)
            {
                Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + oldVelocity, oldVelocity, 10, 10);
            Projectile.Kill();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SmokeBombSmoke>(), Damage, Projectile.knockBack, Projectile.owner);

        }
    }
}
