using ILGPU.IR.Values;
using Terrapain.Common.Global;
using Terrapain.Content.TUtilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses
{
    public class FakeBrain : ModProjectile
    {
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/WhitePixel";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 110;
            Projectile.scale = 1.05f;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            var t = Projectile.GetT();
			t.drawCenter = new Vector2(100, 66);
            t.useVanillaDrawing = false;
            t.useModDrawingInPreDraw = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            var t = Projectile.GetT();
            t.useVanillaDrawing = true;
            t.useModDrawingInPreDraw = false;
            NPC brain = Main.npc[(int)Projectile.ai[0]];
            if (Main.npc[(int)Projectile.ai[0]].active && Main.npc[(int)Projectile.ai[0]].type == NPCID.BrainofCthulhu)
            {
                Projectile.frame = brain.frame.Y / brain.frame.Height;
            }
            else
            {
                if (Projectile.ai[2] <= 0)
                {
                    Projectile.frame = (Projectile.frame + 1) % 4;
                    Projectile.ai[2] = 10;
                }
                else
                {
                    Projectile.ai[2]--;
                }
            }
            if (Projectile.ai[1] == 1)
            {
                Projectile.velocity = Projectile.velocity.Normalized() * MathF.Max((Projectile.velocity.Length() - 0.1f), 0);
                if (Projectile.velocity == Vector2.Zero && Projectile.ai[2] != -1)
                {
                    Projectile.ai[1] = 2;
                }
            }
            else if (Projectile.ai[1] == 2)
            {
                Player player = Main.player[(int)Projectile.ai[2]];
                if (Projectile.velocity.Length() < 16)
                {
                    Projectile.velocity = Projectile.DirectionTo(player.Center) * (Projectile.velocity.Length() + 1);
                }
            }
            Rotating(Projectile);
            Projectile.alpha = Math.Max(0, Projectile.alpha - 8);
        }
        float angularVelocity;
        public void Rotating(Projectile npc)
        {
            float targetRotation = 0;
            if (npc.velocity != Vector2.Zero)
            {
                Vector2 dir = npc.velocity;
                dir.Y = MathF.Abs(dir.Y);
                targetRotation = dir.ToRotation() + MathF.PI / 2;
                float k = MathF.Min(npc.velocity.Length() / 100, 0.8f);
                targetRotation = NormalizeRotation(targetRotation, false);
                targetRotation *= k;
            }
            AIHelper.AngularAcceleration(ref angularVelocity, 0.005f, 0.05f, targetRotation, ref npc.rotation);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //if (!Main.npc[(int)Projectile.ai[0]].active || Main.npc[(int)Projectile.ai[0]].type != NPCID.BrainofCthulhu)
                Main.instance.LoadNPC(NPCID.BrainofCthulhu);
            
            Projectile.GetT().TDrawProjectile(Projectile, TextureAssets.Npc[NPCID.BrainofCthulhu].Value, lightColor);
            return false;
        }
    }
}