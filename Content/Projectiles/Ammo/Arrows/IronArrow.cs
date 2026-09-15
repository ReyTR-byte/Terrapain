using Terrapain.Common.Global;
using Terrapain.Content.TUtilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Ammo.Arrows
{
    public class IronArrow: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Projectile.velocity.Length() * 2.5f;
        }
        public override void AI()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy)
                {
                    float distance = npc.Distance(Projectile.Center);
                    if (distance < 100)
                    {
                        distance = (100 - distance) / 100;
                        distance *= distance;
                        AIHelper.CommonTerrapainFlyingMovement(Projectile, npc.Center, 0.25f * distance, Projectile.ai[0], 1.2f * distance, 0, false);
                        if (Projectile.timeLeft % 2 == 0 && TGlobalNPC.random.NextBool(3))
                        {
                            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Iron);
                        }
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}