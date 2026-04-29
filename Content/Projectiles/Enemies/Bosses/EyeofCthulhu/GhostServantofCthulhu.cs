using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Projectiles.Enemies.Bosses.EyeofCthulhu
{
    public class GhostServantofCthulhu : ModProjectile
    {
        int attack
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        int timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        int AIStyle;
        float maxVelocity = 19;
        float angularVelocity = 0;
        float maxAngularVelocity = 0.3f;
        float angularAcceleration = 0.03f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.GetT().fullLight = true;
            Projectile.GetT().useVanillaDrawing = false;
            Projectile.GetT().useModDrawingInPostDraw = true;
            Projectile.GetT().drawCenter = new Vector2(30, 10);
        }
        int GoodWorldDeathSours = -1;
        bool flyStraight;
        NPC Parent = null;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity != Vector2.Zero ? Projectile.velocity.ToRotation() : 0;
            if (source is EntitySource_Death && Main.getGoodWorld)
                GoodWorldDeathSours = Convert.ToInt32(source.Context);
            else if (source is EntitySource_Parent && source.Context == "Fly straight")
            {
                flyStraight = true;
                Parent = (NPC)((EntitySource_Parent)source).Entity;
            }
            else if (source.Context == "ring AI")
            {
                AIStyle = 1;
                timer = 25;
                Projectile.alpha = 255;
            }
            else if (source.Context == "boomerang")
            {
                AIStyle = 2;
                timer = 100;
                Parent = (NPC)((EntitySource_Parent)source).Entity;
                Projectile.ai[2] = Projectile.velocity.Length() / 50;
            }
            else if (source.Context == "aura")
            {
                AIStyle = 3;
            }
        }
        public override void AI()
        {
            if (AIStyle == 0)
            {
                Projectile.ai[2]--;
                if (Projectile.ai[2] <= 0)
                {
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                    Projectile.ai[2] = 8;
                }
                if (timer > 0)
                {
                    timer--;
                }
                if (Parent != null && Parent.active && Parent.ai[0] == -200)
                {
                    flyStraight = false;
                }
                if (Projectile.timeLeft < 580 && !flyStraight)
                {
                    if (attack == 0)
                    {
                        Player Target = Main.player[Player.FindClosest(Projectile.position, 20, 20)];
                        AngularAcceleration(ref angularVelocity, angularAcceleration, maxAngularVelocity, AngleFromVector(Target.Center - Projectile.Center), ref Projectile.rotation);
                        float maxVelocityMultyplier = 1;
                        Vector2 targetPosition;
                        targetPosition = Target.Center + Target.velocity * 20;
                        Projectile.velocity += Projectile.DirectionTo(targetPosition) * 1.2f;
                        Vector2 vectorToTargetPosition = targetPosition - Projectile.Center;
                        float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
                        positiveRotation = NormalizeRotation(positiveRotation);
                        float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
                        negativeRotation = NormalizeRotation(negativeRotation);
                        if (positiveRotation > negativeRotation)
                        {
                            Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                        }
                        else
                        {
                            Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                        }
                        if (Projectile.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                        {
                            Projectile.velocity = Projectile.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                        }
                        if (timer <= 0 && Target.Distance(Projectile.Center) < 350)
                        {
                            attack = 1;
                        }
                    }
                    else if (attack == 1)
                    {
                        timer = 15;
                        Projectile.velocity = UnitVectorFromRotation(Projectile.rotation) * 35;
                        attack = 2;
                    }
                    else if (attack == 2)
                    {
                        Projectile.velocity *= (Projectile.velocity.Length() - 0.3f) / Projectile.velocity.Length();
                        Projectile.alpha += 13;
                        if (timer == 0)
                        {
                            attack = 0;
                            timer = 20;
                            if (Main.getGoodWorld)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ServantofCthulhuSoul>(), 50, 0, ai0: GoodWorldDeathSours);
                            }
                            Projectile.active = false;
                        }
                    }
                }
            }
            else if (AIStyle == 1)
            {
                if (timer > 0)
                {
                    if (Projectile.alpha > 60)
                    {
                        Projectile.alpha -= 15;
                    }
                    float maxVelocityMultyplier = 1;
                    Vector2 targetPosition;
                    if (timer > 10 || Main.getGoodWorld)
                    {
                        targetPosition = Main.player[(int)Projectile.ai[2]].Center + Vector2.UnitX.RotatedBy(Projectile.ai[0]) * (175 + 2 * (25 - timer)); 
                    }
                    else
                    {
                        targetPosition = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.ai[0]) * 2;
                    }
                    Projectile.rotation = Projectile.DirectionTo(Main.player[(int)Projectile.ai[2]].Center).ToRotation();
                    if (targetPosition != Projectile.Center)
                    {
                        Projectile.velocity += Projectile.DirectionTo(targetPosition) * 1.2f;
                        Vector2 vectorToTargetPosition = targetPosition - Projectile.Center;
                        float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, Projectile.velocity);
                        positiveRotation = NormalizeRotation(positiveRotation);
                        float negativeRotation = AngleBetweenVectors(Projectile.velocity, vectorToTargetPosition);
                        negativeRotation = NormalizeRotation(negativeRotation);
                        if (positiveRotation > negativeRotation)
                        {
                            Projectile.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
                        }
                        else
                        {
                            Projectile.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
                        }
                    }
                    if (Projectile.Distance(targetPosition) < 75)
                    {
                        maxVelocityMultyplier = 1 - (75 - Projectile.Distance(targetPosition)) / 75;
                    }
                    if (Projectile.velocity.Length() > maxVelocity * maxVelocityMultyplier)
                    {
                        Projectile.velocity = Projectile.velocity.ToUnit() * maxVelocity * maxVelocityMultyplier;
                    }
                }
                else if (timer == -5)
                {
                    Projectile.velocity = UnitVectorFromRotation(Projectile.rotation) * 35;
                }
                else if (timer < -5)
                {
                    Projectile.velocity *= (Projectile.velocity.Length() - 0.3f) / Projectile.velocity.Length();
                    Projectile.alpha += 13;
                    if (timer == -20)
                    {
                        attack = 0;
                        timer = 20;
                        if (Main.getGoodWorld)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<ServantofCthulhuSoul>(), ServantofCthulhuSoul.defDamage, 0, ai0: GoodWorldDeathSours);
                        }
                        Projectile.active = false;
                    }
                }
                timer--;
            }
            else if (AIStyle == 2)
            {
                Projectile.velocity += Projectile.DirectionTo(Parent.Center) * Projectile.ai[2];
                timer--;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (timer == 0)
                {
                    Projectile.active = false;
                }
            }
            else if (AIStyle == 3)
            {
                Projectile.alpha = (byte)(195 * (Projectile.Distance(new Vector2(Projectile.ai[0], Projectile.ai[1])) / Projectile.ai[2]));
                if (Projectile.Distance(new Vector2(Projectile.ai[0], Projectile.ai[1])) > Projectile.ai[2] * 1.1f)
                {
                    Projectile.active = false;
                }
            }
        }
    }
}
