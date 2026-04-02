using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria.DataStructures;

namespace Terrapain.Content.Projectiles.Minions
{
    public class GranithStuffProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

        }
        public override void SetDefaults()
        {
            Projectile.sentry = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.friendly = true;
        }
        int timer;
        public override void OnSpawn(IEntitySource source)
        {
            Main.player[Projectile.owner].UpdateMaxTurrets();
        }
        public override void AI()
        {
            NPC Target = null;
            foreach (var target in Main.npc)
            {
                if (target.active && !target.friendly && target.type != NPCID.CultistArcherBlue && target.type != NPCID.CultistArcherWhite && target.type != NPCID.CultistDevote && Functions.DistanceBetweenHitboxes(target, Projectile.Center, 0, 0) < 550 && (Target == null || Functions.DistanceBetweenHitboxes(target, Projectile.Center, 0, 0) < Functions.DistanceBetweenHitboxes(Target, Projectile.Center, 0, 0)))
                {
                    Target = target;
                }
            }
            if (Target != null && timer <= 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(Target.Center), ModContent.ProjectileType<ScorspiderRocket>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                //Functions.Chatic(Target.Center);
                //Functions.Chatic(Functions.DistanceBetweenHitboxes(Target, Projectile.Center, 0, 0));
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;

                Projectile.scale = 1.15f;
                timer = 25;
            }
            timer--;
            if (Projectile.scale > 1)
            {
                Projectile.scale -= 0.01f;
            }
        }
    }
}
