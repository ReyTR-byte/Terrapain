using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class SapphireStaff : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }
        public int[] SapphireSharps = [-1,-1,-1,-1,-1,-1];
        public int SapphireSharpsCount;
        public int ToReplace;
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.player[Projectile.owner].ownedProjectileCounts[Type] > 0)
            {
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.type == Type && proj.whoAmI != Projectile.whoAmI && proj.owner == Projectile.owner)
                    {
                        proj.timeLeft = 100;
                        if (((SapphireStaff)proj.ModProjectile).SapphireSharps[((SapphireStaff)proj.ModProjectile).ToReplace] > -1)
                        {
                            Projectile proj2 = Main.projectile[((SapphireStaff)proj.ModProjectile).SapphireSharps[((SapphireStaff)proj.ModProjectile).ToReplace]];
                            if (proj2.active && proj2.type == ModContent.ProjectileType<SapphireSharp>() && proj2.owner == Projectile.owner)
                            {
                                proj2.ai[2] = -1;
                            }
                        }
                        ((SapphireStaff)proj.ModProjectile).SapphireSharps[((SapphireStaff)proj.ModProjectile).ToReplace] = (int)Projectile.ai[0];
                    }
                }
                Projectile.active = false;
            }
            else
            {
                SapphireSharps[0] = (int)Projectile.ai[0];
            }
        }
        public override void AI()
        {
            var own = Main.player[Projectile.owner];
            if (!own.controlUseItem || (own.HeldItem.active && own.manaCost * own.HeldItem.mana > own.statMana))
            {
                Projectile.active = false;
                return;
            }

            Projectile.Center = Main.MouseWorld;
            List<int> activs = new List<int>();
            for (int i = 0; i < SapphireSharps.Length; i++)
            {
                if (SapphireSharps[i] > -1)
                {
                    Projectile proj = Main.projectile[SapphireSharps[i]];
                    if (proj.active && proj.type == ModContent.ProjectileType<SapphireSharp>() && proj.owner == Projectile.owner)
                    {
                        activs.Add(i);

                        //proj.ai[0] = Projectile.Center.X + (Vector2.UnitX.RotatedBy(Projectile.rotation + SapphireSharpsCount))
                    }
                    else
                    {
                        ToReplace = i;
                    }
                }
                else
                {
                    ToReplace = i;
                }
            }
            SapphireSharpsCount = activs.Count;
            if (SapphireSharps.Length == SapphireSharpsCount)
            {
                int oldest = 99999999;
                for (int i = 0; i < SapphireSharps.Length; i++)
                {
                    Projectile proj = Main.projectile[SapphireSharps[i]];
                    if (oldest > proj.timeLeft)
                    {
                        oldest = proj.timeLeft;
                        ToReplace = i;
                    }
                }
            }
            for (int i = 0; i < activs.Count; i++)
            {
                Projectile proj = Main.projectile[SapphireSharps[activs[i]]];
                Vector2 Pos = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + MathF.PI / SapphireSharpsCount * i * 2) * 60;
                proj.ai[0] = Pos.X;
                proj.ai[1] = Pos.Y;
                proj.ai[2] = 0;
            }
            Projectile.rotation += 0.08f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
