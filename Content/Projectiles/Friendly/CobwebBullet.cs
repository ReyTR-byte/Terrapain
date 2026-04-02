using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terrapain.Common.Global;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terrapain.Content.Buffs;


namespace Terrapain.Content.Projectiles.Friendly
{
    public class CobwebBullet: ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimage = true;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().afterimagesCount = 4;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.087f * 2, 0.0878f * 2, 0.0749f * 2);
            if (Projectile.velocity.Y < 5)
                Projectile.velocity.Y += 0.03f;
            float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
            if (Projectile.velocity.Y < 0)
                angel = 2 * Convert.ToSingle(Math.PI) - angel;
            Projectile.rotation = angel;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 30);
            UnifiedRandom ur = new UnifiedRandom();
            if (ur.NextFloat() <= 0.05f)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 2, ModContent.ProjectileType<ScorspiderWeb>(), Projectile.damage, 0, Projectile.owner);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
            }
        }
        public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}