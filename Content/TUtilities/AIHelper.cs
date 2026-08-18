using Terraria;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.TUtilities
{
    public static class AIHelper
    {
        public static void CommonTerrapainFlyingMovement(Entity entity, Vector2 targetPosition, float rotatingSpeed, float MaxSpeed, float acceleration, float BreakingZone, bool instantBreak = true)
		{
			if (entity.Center == targetPosition)
			{
				if (BreakingZone > 0)
				{
					if (instantBreak)
						entity.velocity = Vector2.Zero;
                }
				return;
			}
            float maxVelocityMultyplier = 1;
            entity.velocity += entity.DirectionTo(targetPosition) * acceleration;
            if (entity.Distance(targetPosition) < BreakingZone)
            {
                maxVelocityMultyplier = 1 - (BreakingZone - entity.Distance(targetPosition)) / BreakingZone;
            }
            Vector2 vectorToTargetPosition = targetPosition - entity.Center;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, entity.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(entity.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                entity.velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                entity.velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
            if (entity.velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                if (instantBreak)
                    entity.velocity = entity.velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
                else if (entity.velocity.Length() > 0)
                    entity.velocity = entity.velocity.Normalized() * MathF.Max(entity.velocity.Length() - acceleration * 2, MaxSpeed * maxVelocityMultyplier);
            }
        }
        public static void CommonTerrapainFlyingMovement(Vector2 position, ref Vector2 velocity, Vector2 targetPosition, float rotatingSpeed, float MaxSpeed, float acceleration, float BreakingZone)
        {
            float maxVelocityMultyplier = 1;
            if (targetPosition != position)
            {
                velocity += position.DirectionTo(targetPosition) * acceleration;
            }
            if (position.Distance(targetPosition) < BreakingZone)
            {
                maxVelocityMultyplier = 1 - (BreakingZone - position.Distance(targetPosition)) / BreakingZone;
            }
            Vector2 vectorToTargetPosition = targetPosition - position;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
            if (velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                velocity = velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
            }
        }
        public static void OnlyRotationalMovement(Entity entity, Vector2 targetPosition, float rotatingSpeed)
        {
            if (targetPosition == entity.Center)
            {
                return;
            }
            Vector2 vectorToTargetPosition = targetPosition - entity.Center;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, entity.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(entity.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                entity.velocity.RotateBy(MathF.Max(-negativeRotation, -rotatingSpeed));
            }
            else
            {
                entity.velocity.RotateBy(MathF.Min(positiveRotation, rotatingSpeed));
            }
        }
        public static bool AngularAcceleration(ref float angularVelocity, float acceleration, float maxAngularVelocity, float goalRotation, ref float rotation, bool Break = true)
		{
			bool rotateToTarget = false;
            goalRotation = NormalizeRotation(goalRotation, true);
            rotation = NormalizeRotation(rotation, true);

            if (rotation != goalRotation)
            {
                if (goalRotation < (float)Math.PI)
                {
                    if (rotation > goalRotation && rotation < goalRotation + Math.PI)
                    {
                        if (angularVelocity > -maxAngularVelocity)
                            angularVelocity -= acceleration;
                    }
                    else
                    {
                        if (angularVelocity < maxAngularVelocity)
                            angularVelocity += acceleration;
                    }
                }
                else
                {
                    if (rotation < goalRotation && rotation > goalRotation - Math.PI)
                    {
                        if (angularVelocity < maxAngularVelocity)
                            angularVelocity += acceleration;
                    }
                    else
                    {
                        if (angularVelocity > -maxAngularVelocity)
                            angularVelocity -= acceleration;
                    }
                }
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
					{
						angularVelocity = 0;
					}
                }
                goalRotation += 2 * (float)Math.PI;
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
					{
						angularVelocity = 0;
					}
                }
                goalRotation -= 4 * (float)Math.PI;
                if ((rotation + angularVelocity > goalRotation && rotation < goalRotation) || (rotation + angularVelocity < goalRotation && rotation > goalRotation))
                {
                    rotation = goalRotation;
                    rotateToTarget = true;
					if (Break)
                    { 
						angularVelocity = 0; 
					}
                }
                else
                {
                    rotation += angularVelocity;
                }
            }
			return rotateToTarget;
        }
    }
}