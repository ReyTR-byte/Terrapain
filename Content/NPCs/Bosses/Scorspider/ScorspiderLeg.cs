using Luminance.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Dusts;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
    public class ScorspiderLeg
    {
        public bool Grounded;
        public Vector2 Ground;
        public float MaxLength;
        public Vector2 velocity;
        public float angularVelocity1;
        public float angularVelocity2;
        public float length;
        public bool notNormalPosition;
        public int direction;
        /// <summary>
        /// The 0-1 interpolant of how far this leg is in its forward step animation.
        /// </summary>
        public float StepAnimationInterpolant;
        /// <summary>
        /// An action to perform when the leg completes its animation.
        /// </summary>
        public Action<ScorspiderLeg, NPC> OnCompleteAnimation;

        /// <summary>
        /// The standard offset for this leg from its owner when not moving.
        /// </summary>
        public Vector2 DefaultOffset;

        /// <summary>
        /// Where the leg started at at the beginning of its step animation.
        /// </summary>
        public Vector2 EndEffectorPositionAtStartOfStep;

        /// <summary>
        /// Where the leg should end up at the end of its step animation.
        /// </summary>
        public Vector2 StepDestination;

        /// <summary>
        /// The effective offset for this leg from its owner when not moving. Unlike <see cref="DefaultOffset"/>, this is subject to safety conditions such as tile-collision checks.
        /// </summary>
        public Vector2 MovingDefaultStepPosition;

        /// <summary>
        /// The speed at which the current animation interpolates at.
        /// </summary>
        public float InterpolationSpeed;

        /// <summary>
        /// The kinematic chain that governs the orientation of this leg.
        /// </summary>
        public KinematicChain Leg;

        /// <summary>
        /// The general size factor of this spider's legs.
        /// </summary>
        public readonly float LegSizeFactor;

        public readonly int Index;

        /// <summary>
        /// Which animation mode the animation should use.
        /// Options: Linear (0), Accel (1), Decel (2), Accel and decel (3)
        /// </summary>
        public int AnimationMode;

        /// <summary>
        /// Whether the step sound should be played when completing the animation.
        /// </summary>
        public bool StepSound;

        public const int Linear = 0;
        public const int Accel = 1;
        public const int Decel = 2;
        public const int AccelDecel = 3;

        public ScorspiderLeg(Vector2 defaultOffset, float legSizeFactor, float legLength1, float legLength2, int index, float maxLength)
        {
            LegSizeFactor = legSizeFactor;
            DefaultOffset = defaultOffset;
            StepAnimationInterpolant = -1f;
            Leg = new();
            Leg.Add(new(LegSizeFactor * legLength1));
            Leg.Add(new(LegSizeFactor * legLength2));
            length = legLength1 + legLength2;
            direction = DefaultOffset.X.NonZeroSign();
            Index = index;
            AnimationMode = Accel;
            MaxLength = maxLength;
        }
        public Vector2 LegCenter(ScorspiderBody owner) => owner.NPC.Center + owner.LegBraces[Index].RotatedBy(owner.NPC.rotation);
        public Vector2 DefaultPosition(ScorspiderBody owner) => LegCenter(owner) + DefaultOffset + new Vector2(owner.NPC.velocity.X * 3.5f, 0);
        public void Update(NPC owner)
        {
            direction = DefaultOffset.X.NonZeroSign();
            bool shouldStep = false;
            ScorspiderBody scorspider = (ScorspiderBody)owner.ModNPC;
            Vector2 oldPosition = Leg.EndEffectorPosition;
            Leg.StartingPoint = LegCenter(scorspider);
            Leg.UpdateEndEffector();
            float stepRange = MathF.Abs(owner.velocity.X) > 0.5f? 80 : 5;
            if (Leg.StartingPoint.Distance(Ground) > length - 10)
            {
                Grounded = false;
            }
            if (StepDestination == Vector2.Zero && MathF.Abs((DefaultPosition(scorspider) - Leg.EndEffectorPosition).RotatedBy(-owner.rotation).X) > stepRange)
            {
                shouldStep = true;
            }
            if (shouldStep)
            {
                var NeighbourLegs = scorspider.Legs.Where(l => (l.Index % 4 == Index % 4 || l.Index == Index - 1 || l.Index == Index + 1) && l.direction == direction).ToArray();
                int notGroundedLegs = NeighbourLegs.Count(l => l.StepDestination != Vector2.Zero);
                if (notGroundedLegs == 0 && Grounded)
                {
                    Grounded = false;
                    StepDestination = Vector2.One;
                }
            }
            bool findGround = false;
            if (!Grounded && StepDestination == Vector2.Zero && !notNormalPosition)
            {
                Ground = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation), owner, out findGround);
            }
            float rot = Functions.NormalizeRotation(Leg[1].Rotation - Leg[0].Rotation, false);
            if ((rot > 0 && direction == -1) || (rot < 0 && direction == 1) || notNormalPosition)
            {
                findGround = false;
                Grounded = false;
                StepDestination = Vector2.Zero;
                notNormalPosition = true;
            }
            if (findGround)
            {
                Vector2 futureLegPoint = Leg.EndEffectorPosition;
                Functions.CommonTerrapainFlyingMovement(ref futureLegPoint, ref velocity, Ground, 0.4f + MathF.Abs(owner.velocity.X) / 6, 100, 4f, 50);

                if (Ground.Distance(futureLegPoint) < velocity.Length() * 1.5f)
                {
                    Leg.Update(StepDestination);
                    Grounded = true;
                    velocity = Vector2.Zero;
                }
                else
                {
                    Leg.Update(Leg.EndEffectorPosition + velocity);
                }
            }
            else if (Grounded)
            {
                Point tile = (Ground + Vector2.UnitY).ToTileCoordinates();
                if (!Main.tile[tile].HasTile || !Main.tileSolid[Main.tile[tile].TileType] || (NPCLoader.CanFallThroughPlatforms(owner) ?? false && Main.tileSolidTop[Main.tile[tile].TileType]))
                {
                    Grounded = false;
                }
                else
                {
                    Leg.Update(Ground);
                }
            }
            else if (StepDestination != Vector2.Zero)
            {
                StepDestination = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation), owner, out findGround);
                if (!findGround)
                {
                    StepDestination = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation + 0.5f * direction), owner, out findGround);
                    if (!findGround)
                    {
                        StepDestination = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation + 0.5f * -direction), owner, out findGround);
                        if (!findGround)
                        {
                            StepDestination = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation + 1f * direction), owner, out findGround);
                            if (!findGround)
                            {
                                StepDestination = ScorspiderBody.FindGround(DefaultPosition(scorspider), Vector2.UnitY.RotatedBy(owner.rotation + 1f * -direction), owner, out findGround);
                            }
                        }
                    }
                }

                Vector2 futureLegPoint = Leg.EndEffectorPosition;
                float value = direction == owner.velocity.X.NonZeroSign() ? 1 : 2.5f;
                //Functions.CommonTerrapainFlyingMovement(ref futureLegPoint, ref velocity, StepDestination, MathF.PI * 2, 10 + MathF.Abs(owner.velocity.X) * 1.3f * value, (0.39f + MathF.Abs(owner.velocity.X) / 7) * value, 40);
                velocity = Leg.EndEffectorPosition.DirectionTo(StepDestination) * MathF.Min((velocity.Length() + (0.5f + MathF.Abs(owner.velocity.X) / 7) * value), 10 + MathF.Abs(owner.velocity.X) * 1.3f * value);
                futureLegPoint += velocity;
                float maxHeight = MathF.Abs(StepDestination.X - Leg.EndEffectorPosition.X) * 4;
                float heightDifference = MathF.Abs(StepDestination.Y + maxHeight - futureLegPoint.Y);
                if (futureLegPoint.Y > StepDestination.Y - maxHeight && owner.velocity.X.NonZeroSign() == (StepDestination.X - futureLegPoint.X).NonZeroSign())
                {
                    velocity.RotateBy(MathF.PI / 7 * (futureLegPoint.X - StepDestination.X).NonZeroSign());
                }
                int StepDirection = (StepDestination.X - Leg.EndEffectorPosition.X).NonZeroSign();
                if (futureLegPoint.Y > StepDestination.Y)
                {
                    velocity.Y = MathF.Min(velocity.Y, -1);
                }
                if (futureLegPoint.X > StepDestination.X && owner.velocity.X > 0)
                {
                    velocity.X = MathF.Min(velocity.X, -1);
                }
                if (futureLegPoint.X < StepDestination.X && owner.velocity.X < 0)
                {
                    velocity.X = MathF.Max(velocity.X, 1);
                }
                if (StepDestination.Distance(futureLegPoint) < velocity.Length() * 1.3f)
                {
                    Leg.Update(StepDestination);
                    Grounded = true;
                    Ground = StepDestination;
                    StepDestination = Vector2.Zero;
                    velocity = Vector2.Zero;
                }
                else
                {
                    Leg.Update(Leg.EndEffectorPosition + velocity);
                }
                //if (DefaultOffset.X.NonZeroSign() == 1)
                //{
                //    if (Functions.NormalizeRotation(Leg[0].Rotation) > -0.05f)
                //    {
                //        Leg[0].Rotation = -0.05f;
                //    }
                //}
                //else
                //{
                //    if (Functions.NormalizeRotation(Leg[0].Rotation) > -MathF.PI + 0.05f)
                //    {
                //        Leg[0].Rotation = -MathF.PI + 0.05f;
                //    }
                //}
            }
            else
            {
                float targetRotation = new Vector2(40 * DefaultOffset.X.NonZeroSign() - owner.velocity.X * 0.5f, owner.velocity.Y * -1 - 40).ToRotation();
                if (Functions.AngularAcceleration(ref angularVelocity1, 0.03f, 0.3f, targetRotation, ref Leg[0].Rotation))
                {
                    notNormalPosition = false;
                }
                if (rot > 0 && direction == -1)
                {
                    if (rot < MathF.PI / 2)
                    {
                        angularVelocity2 = MathF.Max(angularVelocity2 - 0.03f, -0.3f);
                    }
                    else
                    {
                        angularVelocity2 = MathF.Min(angularVelocity2 + 0.03f, 0.3f);
                    }
                    Leg[1].Rotation += angularVelocity2;
                }
                else if (rot < 0 && direction == 1)
                {
                    if (rot > -MathF.PI / 2)
                    {
                        angularVelocity2 = MathF.Min(angularVelocity2 + 0.03f, 0.3f);
                    }
                    else
                    {
                        angularVelocity2 = MathF.Max(angularVelocity2 - 0.03f, -0.3f);
                    }
                    Leg[1].Rotation += angularVelocity2;
                }
                else
                {
                    float fall = MathHelper.Clamp(-owner.velocity.Y / 5 + 1, -1, 1);
                    targetRotation = new Vector2(owner.velocity.X * fall * -1 - DefaultOffset.X.NonZeroSign() * 6, 35 - owner.velocity.Y).ToRotation();
                    Functions.AngularAcceleration(ref angularVelocity2, 0.03f, 0.3f, targetRotation, ref Leg[1].Rotation);
                }
                Leg.UpdateEndEffector();
            }    
        }

        public void UpdateMovementAnimation(Vector2 gravityDirection, NPC owner)
        {
            // Increment the animation interpolant.
            float interpolationSpeed = InterpolationSpeed;
            float velSq = owner.velocity.LengthSquared();
            float velocityNorm = 90;
            if (velSq > velocityNorm)
                interpolationSpeed *= velSq / velocityNorm;
            StepAnimationInterpolant += interpolationSpeed;


            // Calculate the current movement destination based on the animation's completion.
            // This gradually goes from the starting position and ends up at the step destination, making a slight upward arc while doing so.
            float x = Utilities.Saturate(StepAnimationInterpolant);

            // Move differently based on the animation type.
            switch (AnimationMode)
            {
                case Accel:
                    x *= x;
                    break;
                case Decel:
                    x = 2 * x - x * x;
                    break;
                case AccelDecel:
                    x = 3 * x * x - 2 * x * x * x;
                    break;
            }

            Vector2 movementDestination = Vector2.Lerp(EndEffectorPositionAtStartOfStep, StepDestination, x);
            movementDestination -= gravityDirection * Utilities.Convert01To010(StepAnimationInterpolant) * 18f;

            // Move the leg.
            Leg.Update(movementDestination);

            // Stop the animation once it has completed.
            if (StepAnimationInterpolant >= 1f)
            {
                Grounded = true;
                StepAnimationInterpolant = 0f;
                if (StepSound)
                    SoundEngine.PlaySound(SoundID.Dig with { Pitch = -0.5f }, StepDestination);
                if (OnCompleteAnimation != null)
                {
                    OnCompleteAnimation.Invoke(this, owner);
                    OnCompleteAnimation = null;
                }
            }
        }

        public void KeepLegInPlace(Vector2 gravityDirection, Vector2 destination)
        {
            // Stay at the step destination.
            // This will, barring the above exception, be where the leg stopped at the last time a step was performed.
            Leg.Update(destination);
        }

        public void StartStepAnimation(NPC owner, Vector2 gravityDirection, Vector2 forwardDirection, float interpolationSpeed = 0.05f)
        {
            interpolationSpeed *= 4f;
            // Calculate the position to step towards.
            float ownerDirection = Vector2.Dot(owner.velocity, forwardDirection).NonZeroSign();
            float offsetDirection = DefaultOffset.X.NonZeroSign();
            Vector2 aimAheadOffset = new Vector2(Math.Abs(forwardDirection.X), Math.Abs(forwardDirection.Y)) * owner.velocity.ClampLength(0f, 3.67f) * 12f;
            if (ownerDirection != offsetDirection)
                aimAheadOffset *= 2.2f;
            else
                aimAheadOffset /= LegSizeFactor;
            aimAheadOffset.X += Main.rand.NextFloatDirection() * 20f;
            if (aimAheadOffset.HasNaNs())
                aimAheadOffset = Vector2.Zero;

            // Start the animation.
            StepAnimationInterpolant = 0.02f;
            EndEffectorPositionAtStartOfStep = Leg.EndEffectorPosition;
            InterpolationSpeed = interpolationSpeed;
            var hive = (ScorspiderBody)owner.ModNPC;
            //if (hive.PhaseTwo)
            //    InterpolationSpeed *= 1.5f;
            AnimationMode = AccelDecel;
            StepSound = true;
        }

        public void StartCustomAnimation(NPC owner, Vector2 endPosition, float interpolationSpeed = 0.05f, int animationMode = AccelDecel, bool stepSound = false)
        {
            // Start the animation.
            StepAnimationInterpolant = 0.02f;
            EndEffectorPositionAtStartOfStep = Leg.EndEffectorPosition;
            StepDestination = endPosition;
            InterpolationSpeed = interpolationSpeed;
            AnimationMode = animationMode;
            StepSound = stepSound;
        }

        public Vector2 GetEndPoint() => Leg.EndEffectorPosition;
        public void SetAnimationEndAction(Action<ScorspiderLeg, NPC> action) => OnCompleteAnimation = action;
    }
}
