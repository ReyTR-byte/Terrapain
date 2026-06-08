using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content.Projectiles.Enemies.Bosses.KingSlime;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.KingSlime
{
    [AutoloadBossHead]
    public class KingSlimeCrown : ModNPC
    {
        public override string BossHeadTexture => "Terrapain/Content/NPCs/Bosses/VanillaBosses/KingSlime/KingSlimeCrown_Head_Boss";
        public override string Texture => "Terrapain/Assets/ExtraTextures/ShaderTextures/BlackPixel";
        public override void SetDefaults()
        {
            NPC.width = 65;
            NPC.height = 69;
            NPC.lifeMax = 900;
            NPC.damage = 25;
            NPC.defense = 15;
            NPC.boss = true;

            NPC.alpha = 30;

            NPC.knockBackResist = 0f;

            NPC.npcSlots = 10f;

            NPC.noTileCollide = false;
            NPC.noGravity = false;

            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;

            NPC.MaxFallSpeedMultiplier = MultipliableFloat.One * 500;

            AnimationType = NPCID.KingSlime;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }
        Vector2 CrownAtKingSlime
        {
            get
            {
                Vector2 Center = CrownedKingSlime.Top;
                switch (CrownedKingSlime.frame.Y)
                {
                    case 0:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 15 * CrownedKingSlime.scale;
                        break;
                    case 120:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 25 * CrownedKingSlime.scale;
                        break;
                    case 240:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 15 * CrownedKingSlime.scale;
                        break;
                    case 360:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 5 * CrownedKingSlime.scale;
                        break;
                    case 480:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 15 * CrownedKingSlime.scale;
                        break;
                    case 600:
                        Center = CrownedKingSlime.Top - Vector2.UnitY * 17 * CrownedKingSlime.scale;
                        break;
                }
                return Center;
            }
        }


        public int crownedKingSlime;
        public NPC CrownedKingSlime => Main.npc[crownedKingSlime];
        CrownedKingSlime CKS => (CrownedKingSlime)CrownedKingSlime.ModNPC;
        bool CKSactive => CrownedKingSlime != null && CrownedKingSlime.active && CrownedKingSlime.type == ModContent.NPCType<CrownedKingSlime>();

        public int ninjaKingSlime;
        public NPC NinjaKingSlime => Main.npc[ninjaKingSlime];
        NinjaKingSlime NKS => (NinjaKingSlime)NinjaKingSlime.ModNPC;
        bool NKSactive => NinjaKingSlime != null && NinjaKingSlime.active && NinjaKingSlime.type == ModContent.NPCType<NinjaKingSlime>();

        public int kingSlime;
        public NPC KingSlime => Main.npc[kingSlime];
        KingSlime KS => KingSlime.GetGlobalNPC<KingSlime>();
        bool KSactive => KingSlime != null && KingSlime.active && KingSlime.type == NPCID.KingSlime;

        public static int CrownGem => ModContent.ProjectileType<CrownGem>();
        public int CrownGemDamage = 12;
        public int CrownGemKnockBack = 4;

        int Laser => ModContent.ProjectileType<KingSlimeCrownLaser2>();
        public int LaserDamage = 15;
        public float LaserKnockBack = 2;

        int mainTimer;
        int timer;
        int attackCounter = -1;
        int[] attacks = [1, 2, 1, 3];
        public override void AI()
        {
            if (!KSactive)
            {
                NPC.active = false;
            }
            NPC.TargetClosest();
            if (CKSactive)
            {
                if (NPC.ai[0] == -1)
                {
                    Vector2 targetPosition = CrownAtKingSlime;
                    CommonTerrapainFlyingMovement(NPC, targetPosition, 0.02f, 20, 1.2f, 75);
                    float targetRotation = NPC.velocity.ToRotation() - MathF.PI / 2;
                    float distance = NPC.Distance(targetPosition);
                    if (distance < 75)
                    {
                        targetRotation *= distance / 75;
                    }
                    if (distance < 5)
                    {
                        NPC.ai[0] = -2;
                    }
                    AngularAcceleration(ref NPC.ai[3], 0.03f, 0.3f, targetRotation, ref NPC.rotation);
                }
                else if (NPC.ai[0] == -2)
                {
                    NPC.Center = CrownAtKingSlime;
                    NPC.velocity = Vector2.Zero;
                    NPC.rotation = 0;
                    NPC.ai[3] = 0;
                }    
            }
            else
            {
                NPC.ai[0] = Math.Max(1, NPC.ai[0]);
            }
            if (mainTimer > 0)
            {
                mainTimer--;
            }
            if (timer > 0)
            {
                timer--;
            }
            DoFirstPhase();

        }
        Player Target => Main.player[NPC.target];
        UnifiedRandom rand => TGlobalNPC.random;
        int time0 = 10;
        int time1 = 20;
        int time2 = 60;
        int time3 => WorldDifficultySystem.suicide? 120 : 145;
        int time4 = 60;
        void DoFirstPhase()
        {
            switch (NPC.ai[0])
            {
                case 0:
                    int count = WorldDifficultySystem.suicide? 4 : 3;
                    int time = 15;
                    if (NPC.ai[1] == 0)
                    {
                        NPC.ai[2] = rand.NextFloat(MathF.PI * 2);
                        NPC.ai[1] = count;
                        timer = time * 2;
                    }
                    Vector2 targetPosition = Target.Center + Vector2.UnitX.RotatedBy(NPC.ai[2]) * 400;
                    CommonTerrapainFlyingMovement(NPC, targetPosition, 0.2f, 50, 2, 75);
                    AngularAcceleration(ref NPC.ai[3], 0.03f, 0.3f, NPC.DirectionTo(Target.Center).ToRotation(), ref NPC.rotation);
                    if (NPC.Distance(targetPosition) < 50 && timer == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy((NPC.ai[1] - (count + 1) / 2f) * MathF.PI / 8) * 20, CrownGem, CrownGemDamage, CrownGemKnockBack);
                        NPC.ai[1]--;
                        timer = time;
                    }
                    if (!CKSactive)
                    {
                        NextAttack();
                    }
                    break;
                case 1:
                    targetPosition = Target.Center + NPC.DirectionFrom(Target.Center) * 400;
                    CommonTerrapainFlyingMovement(NPC, targetPosition, 0.2f, 50, 2, 75);
                    AngularAcceleration(ref NPC.ai[3], 0.03f, 0.3f, NPC.DirectionTo(Target.Center).ToRotation(), ref NPC.rotation);
                    if (mainTimer == 0 && (!NKSactive || NKS.CurentAttack == 0))
                    {
                        NextAttack();
                    }
                    break;
                case 2:
                    time = 60;
                    count = 5;
                    float targetRotation = 0;
                    if (timer == 0)
                    {
                        if (NPC.ai[1] % 2 == 0)
                        {
                        NPC.velocity = NPC.DirectionTo(Target.Center) * (WorldDifficultySystem.suicide? 23 : 20);
                        float rotatePerIteration = WorldDifficultySystem.suicide? MathF.PI / 8 : MathF.PI / 6;
                        float startRotation = NPC.DirectionTo(Target.Center).ToRotation() - rotatePerIteration * (count - 1) / 2f;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(startRotation + i * rotatePerIteration) * 20, CrownGem, CrownGemDamage, CrownGemKnockBack);
                        }
                        timer = time;
                        targetRotation = NPC.velocity.ToRotation() + MathF.PI / 2;
                        }
                        else
                        {
                            NPC.velocity = NPC.DirectionTo(Target.Center) * 20;
                            float rotatePerIteration = WorldDifficultySystem.suicide ? MathF.PI / 20 : MathF.PI / 24;
                            float startRotation = NPC.DirectionTo(Target.Center).ToRotation() - rotatePerIteration * (count - 1) / 2f;
                            for (int i = 0; i < count; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(startRotation + i * rotatePerIteration) * 18, CrownGem, CrownGemDamage, CrownGemKnockBack);
                            }
                            timer = time;
                            targetRotation = NPC.velocity.ToRotation() + MathF.PI / 2;
                        }
                        NPC.ai[1]++;
                    }
                    else if (timer >= 40 )
                    {
                        targetRotation = NPC.velocity.ToRotation() + MathF.PI / 2;
                    }
                    else if (timer < 40 && timer >= 20)
                    {
                        NPC.velocity *= 0.975f;
                        targetRotation = NPC.velocity.ToRotation() + MathF.PI / 2;
                    }
                    else if (timer < 20)
                    {
                        targetPosition = Target.Center + NPC.DirectionFrom(Target.Center) * 400;
                        CommonTerrapainFlyingMovement(NPC, targetPosition, 0.2f, 20, 2, 75);
                        targetRotation = NPC.DirectionTo(Target.Center).ToRotation();
                    }
                    AngularAcceleration(ref NPC.ai[3], 0.03f, 0.3f, targetRotation, ref NPC.rotation);
                    if (mainTimer == 0)
                    {
                        NextAttack();
                    }
                    break;
                case 3:
                    targetRotation = NPC.DirectionTo(Target.Center).ToRotation() + MathF.PI / 2;
                    if (timer == 0 && NPC.ai[1] == 0)
                    {
                        NPC.velocity = NPC.DirectionTo(Target.Center) * 20;
                        timer = time1;
                        NPC.ai[1] = 1;
                    }
                    else if (NPC.ai[1] == 1)
                    {
                        NPC.velocity *= 0.975f;
                        targetRotation = NPC.velocity.ToRotation() + MathF.PI / 2;
                        if (timer == 0)
                        {
                            NPC.ai[1] = 2;
                            timer = time2;
                        }
                    }
                    else if (NPC.ai[1] == 2)
                    {
                        if (NPC.velocity != Vector2.Zero)
                        {
                            NPC.velocity = NPC.velocity.Normalized() * MathF.Max(NPC.velocity.Length() - 2.5f, 0);
                        }
                        if (NPC.ai[2] == 0)
                        {
                            int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, Vector2.Zero, Laser, LaserDamage, LaserKnockBack, -1, 1, NPC.whoAmI, 60);
                            Main.projectile[p].timeLeft = time2 + time3 + time4;
                            p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, Vector2.Zero, Laser, LaserDamage, LaserKnockBack, -1, -1, NPC.whoAmI, 60);
                            Main.projectile[p].timeLeft = time2 + time3 + time4;
                            NPC.ai[2] = rand.NextFloat(-0.1f, 0.1f);
                            NPC.ai[2] += 0.1f * NPC.ai[2].NonZeroSign();
                        }
                        float progress = (time2 - timer) / (float)time2;
                        NPC.rotation += progress * NPC.ai[2];
                        targetRotation = NPC.rotation;
                        if (timer == 0)
                        {
                            NPC.ai[1] = 3;
                            timer = time3;
                        }
                    }
                    else if (NPC.ai[1] == 3)
                    {
                        float progress = timer / (float)time3;
                        NPC.rotation += progress * NPC.ai[2];
                        targetRotation = NPC.rotation;
                        if (timer == 0)
                        {
                            NPC.ai[1] = 4;
                            timer = time4;
                        }
                    }
                    else if (NPC.ai[1] == 4)
                    {
                        targetRotation = NPC.rotation;
                        int rate = 10;
                        if (timer % rate == 0 && timer > 30)
                        {
                            float speed = WorldDifficultySystem.suicide? 17.5f : 14;
                            float progress = (time4 - timer) * 2f / time4;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.rotation + MathF.PI / 2 + progress * MathF.PI / 2) * speed, CrownGem, CrownGemDamage, CrownGemKnockBack);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.rotation + MathF.PI / 2 - progress * MathF.PI / 2) * speed, CrownGem, CrownGemDamage, CrownGemKnockBack);
                        }
                        if (timer == 0)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            timer = 10;
                        }
                    }
                    AngularAcceleration(ref NPC.ai[3], 0.03f, 0.3f, targetRotation, ref NPC.rotation);
                    if (mainTimer == 0)
                    {
                        NextAttack();
                    }
                    break;
            }
        }
        void NextAttack()
        {
            attackCounter++;
            if (attackCounter >= attacks.Length)
            {
                attackCounter = 0;
            }
            NPC.ai[0] = attacks[attackCounter];
            switch (NPC.ai[0])
            {
                case 1:
                    mainTimer = 100;
                    break;
                case 2:
                    mainTimer = 450;
                    NPC.ai[1] = 0;
                    break;
                case 3:
                    mainTimer = (time0 + time1 + time2 + time3 + time4) * 3;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    break;
            }
        }
        public override void OnKill()
        {
            if (KSactive)
            {
                KS.kingSlimeCrownKilled = true;
            }
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, GoreID.KingSlimeCrown);
        }
        public override void BossHeadSlot(ref int index)
        {
            if (NPC.ai[0] == -2)
            {
                index = -1;
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            if (MathF.Abs(NormalizeRotation(NPC.rotation, false)) > MathF.PI / 2)
            {
                if (NormalizeRotation(NPC.rotation, false) > 0)
                {
                    rotation = MathF.PI - NPC.rotation;
                }
                else
                {
                    rotation = -MathF.PI + NPC.rotation;
                }
            }
            else
            {
                rotation = NPC.rotation;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.ai[0] > 0;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return NPC.ai[0] == -2? false : null;
        }
        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return NPC.ai[0] == -2? false : null;
        }
        public override bool CanBeHitByNPC(NPC attacker)
        {
            return NPC.ai[0] != -2;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return NPC.ai[0] == -2 ? false : null;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Extra[ExtrasID.KingSlimeCrown].Value;
            if (NPC.ai[0] == -2)
            {
                NPC.Center = CrownAtKingSlime;
            }
            if (!((NPC.ai[0] == 2 && timer > 20) || NPC.ai[0] == 3) && MathF.Abs(NormalizeRotation(NPC.rotation, false)) > MathF.PI / 2)
            {
                NPC.spriteDirection = -1;
            }
            else
            {
                NPC.spriteDirection = 1;
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, null, drawColor, NPC.rotation, new Vector2(41, NPC.spriteDirection == 1? 41 : 15), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            return false;
        }
    }
}
