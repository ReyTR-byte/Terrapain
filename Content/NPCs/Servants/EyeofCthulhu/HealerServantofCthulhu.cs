using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.DrawTasks;
using Terrapain.Common.Global;
using Terrapain.Content.Auras;
using Terrapain.Content.Groups;
using Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Servants.EyeofCthulhu
{
    public class HealerServantofCthulhu : ModNPC
    {
        int owner;
        NPC Owner { get => Main.npc[owner]; }
        HealingAura aura;
        int animationSpeed = 12;
        int animationtimer;
        int frame;
        float maxVelocity = 10;
        float maxVelocityMultyplier = 1;
        float angularVelocity;
        float maxAngularVelocity = 0.08f;
        float angularAcceleration = 0.01f;
        float rotation;

        public override void FindFrame(int frameHeight)
        {
            if (animationtimer == 0)
            {
                frame++;
                if (frame == 2)
                {
                    frame = 0;
                }
                NPC.frame = new Rectangle(0, frame * 22, 30, 22);
                animationtimer = animationSpeed;
            }
            animationtimer--;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 1;
            NPC.defense = 0;
            NPC.lifeMax = 30;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.GetT().drawCenter = new Vector2(20, 12);
            NPC.GetT().useModDrawingInPreDraw = true;
            NPC.GetT().useVanillaDrawing = false;
            NPC.GetT().textureDirection = 1;
            NPC.spriteDirection = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            owner = ((EntitySource_Parent)source).Entity.whoAmI;
            NPC.TargetClosest(false);
            rotation = NPC.AngleTo(NPC.GetT().Target.Center);

            List<int> whiteList = new List<int>();
            whiteList.Add(5);
            whiteList.Add(ModContent.NPCType<FireShooterServantofCthulhu>());
            whiteList.Add(ModContent.NPCType<LaserServantofCthulhu>());
            whiteList.Add(ModContent.NPCType<HealerServantofCthulhu>());
            whiteList.Add(ModContent.NPCType<SheildedServantofCthulhu>());
            aura = new HealingAura(false, whiteList, 20, 300);
            aura.Center = NPC.Center;
            if (Group.FindGroup("HealerServantofCthulhu").Count == 0)
            {
                Group.NewGroup(new Groups.HealerServantofCthulhu());
            }
        }
        public override void AI()
        {
            maxVelocityMultyplier = 1;
            aura.Center = NPC.Center;
            aura.Update();
            float targetRotation;
            if (NPC.ai[1] != -1 && Main.npc[(int)NPC.ai[1]].active)
            {
                NPC patient = Main.npc[(int)NPC.ai[1]];
                NPC.TargetClosest(false);
                Vector2 targetPosition = patient.Center + (patient.Center - NPC.GetT().Target.Center).ToUnit(new Vector2(0, NPC.spriteDirection)) * 100;
                if (targetPosition.Distance(NPC.GetT().Target.Center) < 550)
                {
                    targetPosition += targetPosition.DirectionFrom(NPC.GetT().Target.Center) * MathF.Min(50, 550 - targetPosition.Distance(NPC.GetT().Target.Center));
                }
                else
                {
                    targetPosition -= targetPosition.DirectionFrom(NPC.GetT().Target.Center) * MathF.Min(50, targetPosition.Distance(NPC.GetT().Target.Center) - 550);
                }
                NPC.velocity += NPC.DirectionTo(targetPosition) * 1.2f;
                targetRotation = AngleFromVector(patient.Center - NPC.Center);
                if (NPC.Distance(targetPosition) < 75)
                {
                    maxVelocityMultyplier = 1 - (75 - NPC.Distance(targetPosition)) / 75;
                }
            }
            else
            {
                NPC eoc = FindClosestNPC(NPC.Center, null, NPCID.EyeofCthulhu);
                if (eoc != null)
                {
                    NPC.velocity += NPC.DirectionTo(eoc.Center) * 1.2f;
                }
                else
                {
                    NPC.velocity += Vector2.UnitY * -1.2f;
                }
                targetRotation = AngleFromVector(NPC.GetT().Target.Center - NPC.Center);
            }
            if (NPC.velocity.Length() > maxVelocity * maxVelocityMultyplier)
            {
                NPC.velocity = NPC.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
            }
            AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, targetRotation, ref rotation, true);
            NPC.rotation = rotation;
            NPC.spriteDirection = NPC.rotation.ToRotationVector2().X.NonZeroSign();
            if (NPC.spriteDirection == -1)
            {
                NPC.rotation = (NPC.rotation.ToRotationVector2() * -1).ToRotation();
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TGlobalNPC.PostDrawNPCsDrawTasks.Add(new AuraDrawTask(aura));
        }
        public override void OnKill()
        {
            if (!Main.getGoodWorld)
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.ProjectileType<ServantofCthulhuSoul>(), ServantofCthulhuSoul.defDamage, 0, ai0: owner);
            }
            else
            {
                Projectile.NewProjectile(NPC.GetSource_Death(owner.ToString()), NPC.Center, NPC.velocity, ModContent.ProjectileType<GhostServantofCthulhu>(), 10, 3);
            }
        }
    }
}
