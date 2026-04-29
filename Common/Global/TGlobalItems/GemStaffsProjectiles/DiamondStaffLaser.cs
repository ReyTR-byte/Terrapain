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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Luminance.Core.Graphics;
using Steamworks;
using Terrapain.Assets.Extratextures;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.DataAnnotations;

namespace Terrapain.Common.Global.TGlobalItems.GemStaffsProjectiles
{
    public class DiamondStaffLaser : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        float lenght
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int cooldown
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        Vector2 dir
        {
            get => Projectile.rotation.ToRotationVector2();
            set => Projectile.rotation = value.ToRotation();
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 2;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            //Projectile.GetT().useVanillaDrawing = false;
        }
        public override bool? CanCutTiles()
        {
            Functions.RayCutTile(Projectile.Center, Projectile.Center + dir * lenght, Main.player[Projectile.owner]);
            return false;
        }
        public override void CutTiles()
        {
        }
        public override void OnSpawn(IEntitySource source)
        {
            dir = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        public override void AI()
        {
            float totalLength = 0;
            while (totalLength < lenght)
            {
                Lighting.AddLight(Projectile.Center + dir * totalLength, TorchID.White);
                totalLength += 20;
            }
            Terraria.Player own = Main.player[Projectile.owner];
            if (!own.controlUseItem)
            {
                Projectile.active = false;
                return;
            }
            else if (own.HeldItem.active && own.manaCost * own.HeldItem.mana > own.statMana)
            {
                if (own.itemAnimation == 1)
                {
                    Projectile.active = false;
                    return;
                }
            }
            else
            {
                Projectile.timeLeft = 2;
                Projectile.Center = Main.player[Projectile.owner].MountedCenter + TGlobalItem.GetHandOffset(Main.player[Projectile.owner]);
                Projectile.rotation = own.itemRotation + (own.direction == -1? MathF.PI: 0) - 0.25f * MathF.PI * own.direction;
                Projectile.Center += dir * 20;
                cooldown -= 1;
                lenght += 35;
                lenght = Math.Min(lenght, 800);
                Vector2 Center = Projectile.Center;
                Projectile.width = (int)(Math.Abs(dir.X) * lenght * 2);
                Projectile.height = (int)(Math.Abs(dir.Y) * lenght * 2);
                Projectile.Center = Center;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 Pos = Vector2.Zero;
            if (Functions.Collision(Projectile.Center, dir, lenght, targetHitbox.Location.ToVector2(), targetHitbox.Width, targetHitbox.Height, ref Pos, false))
            {
                lenght = Pos.Distance(Projectile.Center);
                return cooldown <= 0;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            cooldown = 10;
            Projectile.penetrate++;
        }
        public override void OnHitPlayer(Terraria.Player target, Terraria.Player.HurtInfo info)
        {
            cooldown = 10;
            Projectile.penetrate++;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Vector2 collide = Functions.RayColisionInTheWorld(Projectile.Center, Projectile.Center + dir * lenght);
            if (collide != Vector2.Zero)
            {
                lenght = Projectile.Distance(collide);

            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.DrawLine(Projectile.Center, Projectile.Center + dir * lenght, Color.LightBlue, 20);
            return false;
        }
        public float WidthFunction(float value) => 12f;
        public Color ColorFunction(float value) => Color.LightBlue;
    }
}
