using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class DiamondStaffLaser : ModProjectile
    {
        public override string Texture => base.Texture;//"Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        int tail
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int head
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        bool start
        {
            get => Projectile.ai[2] == 1;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 100;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            //Projectile.GetT().useVanillaDrawing = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            direction = Projectile.velocity.Normalized();
            Projectile.velocity = Vector2.Zero;
        }
        Vector2 direction;
        public override void AI()
        {
            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 500)
            {
                Projectile.active = false;
                return;
            }
            Lighting.AddLight(Projectile.Center, TorchID.White);
            if (start)
            {
                if (head == -1)
                {
                    head = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 4, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                    int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 8, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, head);
                    Main.projectile[head].ai[1] = proj2;
                    int proj3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 12, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, proj2, -1);
                    Main.projectile[proj2].ai[1] = proj3;
                }
                else
                {
                    Projectile Head = Main.projectile[head];
                    int i = 1;
                    Projectile.Center = Main.player[Projectile.owner].MountedCenter + TGlobalItem.GetHandOffset(Main.player[Projectile.owner]);
                    direction = Projectile.DirectionTo(Main.MouseWorld);
                    Projectile.Center += direction * 20;
                    while (Head.active && Head.type == Type && Head.owner == Projectile.owner)
                    {
                        Head.Center = Projectile.Center + direction * 4 * i;
                        ((DiamondStaffLaser)Head.ModProjectile).direction = direction;
                        i++;
                        if (Head.ai[1] < 0)
                        {
                            break;
                        }
                        Head = Main.projectile[(int)Head.ai[1]];
                    }
                }
            }
            else
            {
                Projectile Tail = Main.projectile[tail];
                if (Tail.active && Tail.type == Type && Tail.owner == Projectile.owner || start)
                {
                    if (head == -1 && Main.player[Projectile.owner].ownedProjectileCounts[Type] < 25)
                    {
                        head = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 4, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                        int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 8, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, head);
                        Main.projectile[head].ai[1] = proj2;
                        int proj3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + direction * 12, direction, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, proj2, -1);
                        Main.projectile[proj2].ai[1] = proj3;
                    }
                }
                else
                {
                    Projectile.active = false;
                }
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Functions.HitTiles(Projectile))
            {
                OnTileCollide(Vector2.Zero);
            }
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (head != -1)
            {
                Projectile Head = Main.projectile[head];
                if (Head.active && Head.type == Type && Head.owner == Projectile.owner)
                {
                    Head.active = false;
                }
                head = -1;
            }
            Collision.HitTiles(Projectile.position, direction * 5, Projectile.width, Projectile.height);
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
    }
}
