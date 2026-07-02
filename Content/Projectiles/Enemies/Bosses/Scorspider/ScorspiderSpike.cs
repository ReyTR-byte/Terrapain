using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider
{
    public class ScorspiderSpike : ModProjectile
    {
        private float i
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float count
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private int attackStyle
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPostDraw = true;
        }
        NPC Sting;
        Vector2 positionAboutPlayer;
        Vector2 startVlocity;
        float startRotation;
        public override void OnSpawn(IEntitySource source)
        {
            if (attackStyle == 5)
            {
                Sting = (NPC)((EntitySource_Parent)source).Entity;
            }

            Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            positionAboutPlayer = Projectile.position - player.position;

            startVlocity = Projectile.velocity;
            if (Projectile.velocity != Vector2.Zero)
            {
                float angel = MathF.Acos(Projectile.velocity.X / Projectile.velocity.Length());
                if (Projectile.velocity.Y < 0)
                    angel = 2 * Convert.ToSingle(Math.PI) - angel;
                startRotation = angel;
            }
        }
        int timer;
        bool gravity = true;
        int[] dusts = new int[100];
        Vector2 memoryVelocity;
        public override void AI()
        {
            if (Projectile.friendly)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];

            timer++;

            switch (attackStyle)
            {
                case 0:
                    if (WorldDifficultySystem.suicide && Projectile.hostile)
                    {
                        if (Projectile.velocity.X > 0)
                        {
                            if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
                            {
                                Vector2 ProjPos = Projectile.position;
                                ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                            }
                        }
                        else
                        {
                            if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
                            {
                                Vector2 ProjPos = Projectile.position;
                                ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                            }
                        }
                    }
                    break;
                case 1:
                    if (timer == 80)
                    {
                        gravity = false;
                        if (i % 2 == 0)
                        {
                            Projectile.velocity = Functions.UnitVectorFromRotation(0.3f * (float)Math.PI) * 12;
                        }
                        else
                        {
                            Projectile.velocity = Functions.UnitVectorFromRotation(0.7f * (float)Math.PI) * 12;
                        }
                    }
                    break;
                case 2:
                    if (timer == 80)
                    {
                        gravity = false;
                        Projectile.velocity = Projectile.DirectionTo(player.Center + new Vector2(0, 60)) * 35;
                    }
                    break;
                case 3:
                    if (timer == 1)
                    {
                        Projectile.alpha = 255;
                        gravity = false;
                        Projectile.rotation = 0.5f * (float)Math.PI;
                    }
                    else if (Projectile.alpha > 0)
                    {
                        if (Projectile.alpha - 8 > 0)
                            Projectile.alpha -= 8;
                        else
                        {
                            Projectile.alpha = 0;
                            Projectile.velocity = new Vector2(0, 15);
                            gravity = true;
                        }
                    }
                    break;
                case 4:
                    if (timer < 60)
                    {
                        if (!WorldDifficultySystem.suicide)
                            Projectile.hostile = false;
                    }
                    else
                    {
                        Projectile.hostile = true;
                    }
                    if (WorldDifficultySystem.suicide)
                    {
                        if (Projectile.velocity.X > 0)
                        {
                            if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
                            {
                                Vector2 ProjPos = Projectile.position;
                                ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                            }
                        }
                        else
                        {
                            if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
                            {
                                Vector2 ProjPos = Projectile.position;
                                ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                            }
                        }
                    }
                    break;
                case 5:
                    if (timer < 90)
                    {
                        if (WorldDifficultySystem.suicide)
                        {
                            if (Projectile.velocity.X > 0)
                            {
                                if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
                                {
                                    Vector2 ProjPos = Projectile.position;
                                    ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                                }
                            }
                            else
                            {
                                if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
                                {
                                    Vector2 ProjPos = Projectile.position;
                                    ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
                                }
                            }
                        }
                    }
                    else
                    {
                        gravity = false;
                        Projectile.velocity = Projectile.DirectionTo(Sting.Center) * 20;
                        if (Projectile.Distance(Sting.Center) < 25)
                        {
                            Projectile.active = false;
                        }
                    }
                    break;
                case 6:
                    gravity = false;
                    if (timer == 1)
                    {
                        memoryVelocity = Projectile.velocity;
                        Projectile.velocity = Vector2.Zero;
                    }
                    if (timer == 20)
                    {
                        Projectile.velocity = memoryVelocity;
                    }
                    break;
                case 7:
                    if (timer < 60)
                    {
                        Projectile.position = player.position + positionAboutPlayer;
                        Projectile.rotation = startRotation;
                        Projectile.velocity = Vector2.Zero;
                        gravity = false;
                    }
                    if (timer == 60)
                    {
                        Projectile.velocity = startVlocity;
                        gravity = true;
                    }
                    break;
            }

            if (gravity)
                Projectile.velocity.Y += 0.15f;
            /*if (count != 0 && timer == 80 && WorldDifficultySystem.suicide)
			{
				Vector2 target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)].position;
				target.X += -(float)(count - 1) / 2 * 1.5f * timer + (attackStyle == 1 ? i : count - 1 - i) * 1.5f * timer;
				Projectile.velocity = Projectile.DirectionTo(target) * 15;
			}*/
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
        }
        public override bool CanHitPlayer(Player target)
        {
            if (attackStyle == 3 && Projectile.alpha > 0)
            {
                return false;
            }
            return base.CanHitPlayer(target);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if ((attackStyle == 1 && timer >= 80) || attackStyle == 3)
            {
                Vector2 center = Projectile.Center;
                float Rotation = Projectile.rotation - MathHelper.PiOver2;
                Color drawColor = new Color(0.7f, 0.1f, 0.12f, 0.6f * (255f - Projectile.alpha) / 255);

                Main.EntitySpriteDraw(ExtraTextureRegistry.WhitePixel.Value, center - Main.screenPosition, ExtraTextureRegistry.BlackPixel.Value.Bounds, drawColor, Rotation, new Vector2(0.5f, 0), new Vector2(8, Main.ScreenSize.ToVector2().Length()), SpriteEffects.None);
            }
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
    }
}
