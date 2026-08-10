using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Friendly
{
    public class FriendlyCrownGem : CrownGem
    {
        public override string Texture => "Terrapain/Content/Projectiles/Enemies/Bosses/KingSlime/CrownGem";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.GetT().trail.smooth = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            MaxVelocity = Projectile.velocity.Length() * 1.5f;
        }
        float MaxVelocity;
        public override void AI()
        {
            Projectile.ai[2]--;
            if (Projectile.ai[2] < 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Functions.CommonTerrapainFlyingMovement(Projectile, Projectile.Center + new Vector2(Projectile.ai[0], Projectile.ai[1]), 0.1f, MaxVelocity, 0.5f, 0);
            }
        }
    }
}
