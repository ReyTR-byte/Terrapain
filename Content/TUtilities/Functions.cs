using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terrapain.Terrapain;

namespace Terrapain.Content
{
	public static class Functions
	{
        public static Vector2 ToUnit(this Vector2 v)
        {
			if (v == Vector2.Zero)
			{
				v = Vector2.Zero;
			}
			else
			{
				v /= v.Length();
			}
            return v;
        }
        public static Vector2 ToUnit(this Vector2 v, Vector2 ifVectorIsZero)
		{
            if (v == Vector2.Zero)
            {
                v = ifVectorIsZero;
            }
			else
			{
				v /= v.Length();
			}
			return v;
		}
		public static void ToInt(ref this Vector2 targetVector)
		{
			targetVector = new((int)targetVector.X, (int)targetVector.Y);
		}
        public static Vector2 GetInt(this Vector2 targetVector)
        {
            return new((int)targetVector.X, (int)targetVector.Y);
        }
        public static bool IsAngleBetweenAngles(float angle1, float targetAngle, float angle2)
		{
			targetAngle -= angle1;
			targetAngle = NormalizeRotation(targetAngle, false);
			angle2 -= angle1;
            angle2 = NormalizeRotation(angle2, false);
			return angle2.NonZeroSign() == targetAngle.NonZeroSign() && Math.Abs(angle2) >= Math.Abs(targetAngle);
        }
		public static void TryHit(Player player, Item item, NPC target, NPC.HitModifiers hitModifiers, int damage, float knockBack, bool useCanHit, bool useItemHitList)
		{
			vanillaHit = false;
			bool value1 = !target.friendly;
			bool value2 = PlayerLoader.CanHitNPC(player, target);
			bool value4 = NPCLoader.CanBeHitByItem(target, player, item).HasValue? NPCLoader.CanBeHitByItem(target, player, item).Value : value1 && value2;
			bool value3 = PlayerLoader.CanHitNPCWithItem(player, item, target).HasValue? PlayerLoader.CanHitNPCWithItem(player, item, target).Value : value1 && value2 && value4;
			bool value5 = useCanHit? player.CanHit(target) : true;
			bool value6 = useItemHitList ? !item.GetT().hitList[target.whoAmI] : true;
			if (((value1 && value2 && value4) || (value3)) && value5 && value6)
			{
				NPC.HitModifiers modifiers = hitModifiers;
				UnifiedRandom random = new UnifiedRandom();
				bool crit = (item == null ? 0 : item.crit) + player.GetTotalCritChance(modifiers.DamageType) > random.NextFloat(0, 100);
				if (target.knockBackResist == 0)
				{
					modifiers.DisableKnockback();
				}
				NPCLoader.ModifyIncomingHit(target, ref modifiers);
				modifiers.Defense.Base += target.defense;
				NPC.HitInfo hitInfo = modifiers.ToHitInfo((int)player.GetTotalDamage(modifiers.DamageType).ApplyTo(damage), crit, knockBack * target.knockBackResist, true, player.luck);
				player.addDPS(hitInfo.Damage);
				PlayerLoader.OnHitAnything(player, target.Center.X, target.Center.Y, target);
				target.StrikeNPC(hitInfo);
				//NPCLoader.HitEffect(npc, hitInfo);
				if (item != null)
				{
					ItemLoader.OnHitNPC(item, player, target, hitInfo, hitInfo.Damage);
					//DashAccesory.ModItem.OnHitNPC(Player, npc, hitInfo, hitInfo.Damage); 
				}
				PlayerLoader.OnHitNPC(player, target, hitInfo, hitInfo.Damage);
			}
			vanillaHit = true;
        }
		public static List<NPC> NPCColisionList(Rectangle hitbox)
		{
			List<NPC> list = new List<NPC>();
			foreach (var npc in Main.npc)
			{
				if (npc.active && RectangleColision(npc.Hitbox, hitbox))
				{
					list.Add(npc);
				}
			}
			return list;
		}
		public static Rectangle GetRectangle(Vector2 V1, Vector2 V2)
		{
			int num1 = V1.X > V2.X? (int)V2.X : (int)V1.X;
            int num2 = V1.Y > V2.Y? (int)V2.Y : (int)V1.Y;
            int num3 = V1.X > V2.X? (int)(V1.X - V2.X) : (int)(V2.X - V1.X);
            int num4 = V1.Y > V2.Y? (int)(V1.Y - V2.Y) : (int)(V2.Y - V1.Y);
            return new Rectangle(num1, num2, num3, num4);
		}
        public static Rectangle GetRectangle(Vector2 V1, Vector2 V2, Vector2 V3, Vector2 V4)
        {
            int num1 = V1.X < V2.X ? (int)V1.X : (int)V2.X;
			num1 = num1 < V3.X ? num1 : (int)V3.X;
            num1 = num1 < V4.X ? num1 : (int)V4.X;
            int num2 = V1.Y < V2.Y ? (int)V1.Y : (int)V2.Y;
            num2 = num2 < V3.Y ? num2 : (int)V3.Y;
            num2 = num2 < V4.Y ? num2 : (int)V4.Y;
            int num3 = V1.X > V2.X ? (int)V1.X : (int)V2.X;
            num3 = num3 > V3.X ? num3 : (int)V3.X;
            num3 = num3 > V4.X ? num3 : (int)V4.X;
            int num4 = (int)Math.Max(Math.Max(Math.Max(V1.Y, V2.Y), V3.Y), V4.Y);
            return new Rectangle(num1, num2, num3 - num1, num4 - num2);
        }
        public static float GetLengthInHitBox(Rectangle hitbox, float rotation)
		{
			if (rotation > MathF.PI / 2 || rotation < 0)
			{
				return 0;
			}
			if (rotation == MathF.PI / 2)
			{
				return hitbox.Height;
			}
			if (rotation == 0)
			{
				return hitbox.Width;
			}
			float h1 = hitbox.Width * MathF.Tan(rotation);
			float h2 = hitbox.Height * MathF.Tan(MathF.PI - rotation);
			float l1 = MathF.Sqrt(hitbox.Width * hitbox.Width + h1 * h1);
			float l2 = MathF.Sqrt(hitbox.Height * hitbox.Height + h2 * h2);
			return MathF.Min(l1, l2);
        }
		public static Vector2 Normalized(this Vector2 vector)
		{
			Vector2 result = vector;
			result.Normalize();
			return result;
		}
        public static TGlobalNPC GetT(this NPC npc)
        {
            return npc.GetGlobalNPC<TGlobalNPC>();
        }
        public static TGlobalProjectile GetT(this Projectile projectile)
        {
            return projectile.GetGlobalProjectile<TGlobalProjectile>();
        }
        public static TGlobalItem GetT(this Item item)
        {
            return item.GetGlobalItem<TGlobalItem>();
        }
        public static TerrapainPlayer Custom(this Player player)
		{
			return player.GetModPlayer<TerrapainPlayer>();
		}
		public static void SetItemRotation(this Player player, float angle)
		{
			player.itemRotation = player.ToItemRotation(angle);
		}
		public static float ToItemRotation(this Player player, float rotation)
		{
			return MathHelper.WrapAngle(player.direction == 1 ? rotation : (float)Math.PI - rotation) * player.direction;
        }
        public static Vector2 SmartShoot(Vector2 sourcePosition, float projectileSpeed, Vector2 targetPosition, Vector2 targetVelocity, int time)
        {
            float closest = 0;
            Vector2 targetFuturePosition = Vector2.Zero;
            for (int i = 0; i < time; i++)
            {
                float distance = Math.Abs((targetPosition + targetVelocity * i - sourcePosition).Length() - projectileSpeed * i);
                if (distance < closest || i == 0)
                {
                    closest = distance;
                    targetFuturePosition = targetPosition + targetVelocity * i;
                }
            }
            return targetFuturePosition;
        }
		public static LightningDrawInfo NewLightning(Vector2 start, Vector2 end, float width, float widthSpread = 0.05f, float partLength = 0, float lengthSpread = 0.2f, float spread = 0.5f, bool fixedStart = false, Color? color = null, UnifiedRandom random = null)
		{
			if (random == null)
			{
				random = new UnifiedRandom();
			}
			if (!color.HasValue)
			{
				color = Color.White;
			}
			LightningDrawInfo lightning = new LightningDrawInfo() { color = color.Value, end = end, start = start, width = width};
			if (partLength == 0)
			{
				partLength = width * 5;
			}
			float lightningLength = (start - end).Length();
			partLength /= lightningLength;
			lightning.lightningPartInfos = new List<LightningPartInfo>();
			//float sqrt = MathF.Sqrt(2);
			while (((lightning.lightningPartInfos.Count == 0 || lightning.lightningPartInfos[lightning.lightningPartInfos.Count - 1].end != Vector2.UnitX)) && lightning.lightningPartInfos.Count < 20)
			{
                LightningPartInfo l = new LightningPartInfo();
				if (lightning.lightningPartInfos.Count == 0)
				{
					l.start = Vector2.Zero;
					if (!fixedStart)
					{
						l.start.Y = random.NextFloat(-spread, spread);
					}
					l.startWidth = random.NextFloat(1 - widthSpread, 1 + widthSpread) * width;
				}
				else
				{
					l.start = lightning.lightningPartInfos[lightning.lightningPartInfos.Count - 1].end;
					l.startWidth = lightning.lightningPartInfos[lightning.lightningPartInfos.Count - 1].endWidth;

                }
				float rotation = random.NextFloat(-0.55f * MathF.PI, 0.55f * MathF.PI);
				float length = random.NextFloat(partLength * (1 - lengthSpread), partLength * (1 + lengthSpread));
				l.end = l.start + UnitVectorFromRotation(rotation) * length;
				if (MathF.Abs(l.end.Y) > spread)
				{
					l.end.Y = spread * l.end.Y.NonZeroSign();
				}
				l.endWidth = (1 - l.end.X) * random.NextFloat(1 - widthSpread, 1 + widthSpread) * width;
				if (l.end.X >= 1)
				{
					l.endWidth = 0;
					l.end = Vector2.UnitX;
				}
				lightning.lightningPartInfos.Add(l);
			}
			for (int i = 0; i < lightning.lightningPartInfos.Count; i++)
			{
				var part = lightning.lightningPartInfos[i];
				part.start *= lightning.dist.Length();
				part.start.RotateBy(lightning.dist.ToRotation());
				part.start += start;
				part.end *= lightning.dist.Length();
                part.end.RotateBy(lightning.dist.ToRotation());
                part.end += start;
				lightning.lightningPartInfos[i] = part;
            }
			return lightning;
		}
		public static void DrawLightning(this SpriteBatch sprite, LightningDrawInfo lightning)
		{
			if (!ClientConfig.Instance.UseShaders)
			{
				for (int i = 0; i < lightning.lightningPartInfos.Count; i++)
				{
					var l = lightning.lightningPartInfos[i];
					Vector2 realStart = l.start;
					Vector2 realEnd = l.end;
					float length = (realStart - realEnd).Length();
					float realRotation = AngleFromVector(realEnd - realStart);
					float biggestWidth = Math.Max(l.startWidth, l.endWidth);
					Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
					Vector2 scale = new Vector2(length, biggestWidth);
					sprite.Draw(texture, realStart - Main.screenPosition, null, lightning.color, realRotation, new Vector2(0, 0.5f), scale, SpriteEffects.None, 0);
				}
			}
			else
			{
                Vector2 TopLeft = Vector2.Zero;
                Vector2 BottomLeft = Vector2.Zero;
				Vector2 TopRight = Vector2.Zero;
				Vector2 BottomRight = Vector2.Zero;

                ManagedShader shader = ShaderManager.GetShader("Terrapain.LightningShader");

                Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
                for (int i = 0; i < lightning.lightningPartInfos.Count; i++)
                {
                    var l = lightning.lightningPartInfos[i];
                    float length = (l.start - l.end).Length();
                    float rotation = AngleFromVector(l.end - l.start);
                    float biggestWidth = Math.Max(l.startWidth, l.endWidth);
					if (i == 0)
					{
                        ManagedShader HalfCircle = ShaderManager.GetShader("Terrapain.HalfCircle");
                        sprite.End();
                        sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, HalfCircle.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                        sprite.Draw(texture, l.start - Main.screenPosition, null, lightning.color, rotation, new Vector2(0.5f, 0.5f), l.startWidth, SpriteEffects.None, 0);
                        TopLeft = l.start - (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                        BottomLeft = l.start + (Vector2.UnitY * l.startWidth / 2).RotatedBy(rotation);
                    }
					else
					{
						TopLeft = TopRight;
						BottomLeft = BottomRight;
					}
					if (i + 1 < lightning.lightningPartInfos.Count)
					{
						Vector2 TopRightCandidate = l.end - (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
						var l1 = lightning.lightningPartInfos[i + 1];
						float rotation1 = AngleFromVector(l1.end - l1.start);
						Vector2 start2 = l1.start - (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1);
						Vector2 end2 = l1.end - (Vector2.UnitY * l1.endWidth / 2).RotatedBy(rotation1);

						TopRight = AlmostGarantedRayColision(TopLeft, TopRightCandidate, start2, end2)?? TopRightCandidate;

                        Vector2 BottomRightCandidate = l.end + (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
						start2 = l1.start + (Vector2.UnitY * l1.startWidth / 2).RotatedBy(rotation1);
                        end2 = l1.end + (Vector2.UnitY * l1.endWidth / 2).RotatedBy(rotation1);

						BottomRight = AlmostGarantedRayColision(BottomLeft, BottomRightCandidate, start2, end2)?? BottomRightCandidate;
                    }
					else
					{
                        TopRight = l.end + (Vector2.UnitY * l.endWidth / 2).RotatedBy(rotation);
                        BottomRight = l.end + (Vector2.UnitY * -l.endWidth / 2).RotatedBy(rotation);
						if (l.endWidth > 0)
						{
                            ManagedShader HalfCircle = ShaderManager.GetShader("Terrapain.HalfCircle");
                            sprite.End();
                            sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, HalfCircle.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
                            sprite.Draw(texture, l.start - Main.screenPosition, null, lightning.color, rotation, new Vector2(0.5f, 0.5f), (l.end - l.start).ToRotation(), SpriteEffects.None, 0);
                        }
                    }

					Vector2 realStart = (l.startWidth > l.endWidth? l.start : l.end);
					Vector2 realEnd = (l.startWidth <= l.endWidth ? l.start : l.end);
					float realRotation = (realEnd - realStart).ToRotation();
					Vector2[] points = new Vector2[6];
					points[0] = realStart;
					points[1] = realEnd;
                    points[2] = (l.startWidth > l.endWidth ? TopLeft : BottomRight);
                    points[3] = (l.startWidth > l.endWidth ? BottomLeft : TopRight);
                    points[4] = (l.startWidth <= l.endWidth ? TopLeft : BottomRight);
                    points[5] = (l.startWidth <= l.endWidth ? BottomLeft : TopRight);
					for (int j = 0; j < 6; j++)
					{
						points[j] -= realStart;
						points[j].RotateBy(-realRotation);
					}
					float halfedWidth = 0;
					for (int j = 2; j < 6; j++)
					{
						halfedWidth = MathF.Max(halfedWidth, MathF.Abs(points[j].Y));
					}
					float minX = 0;
					float maxX = 0;
                    for (int j = 0; j < 6; j++)
                    {
						points[j].Y += halfedWidth;
						points[j].Y /= halfedWidth * 2;
						minX = MathF.Min(minX, points[j].X);
                        maxX = MathF.Max(maxX, points[j].X);
                    }
					float _length = maxX - minX;
                    for (int j = 0; j < 6; j++)
                    {
                        points[j].X -= minX;
						points[j].X /= _length;
                    }
					shader.TrySetParameter("left", points[0]);
					shader.TrySetParameter("leftTop", points[2]);
                    shader.TrySetParameter("leftBottom", points[3]);
                    shader.TrySetParameter("right", points[1]);
                    shader.TrySetParameter("rightTop", points[5]);
                    shader.TrySetParameter("rightBottom", points[4]);
                    shader.TrySetParameter("widthLeft", (l.startWidth > l.endWidth ? l.startWidth : l.endWidth) / halfedWidth / 2);
                    shader.TrySetParameter("widthRight", (l.startWidth <= l.endWidth? l.startWidth : l.endWidth) / halfedWidth / 2);
					sprite.End();
                    sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
					Vector2 scale = new Vector2(_length, halfedWidth * 2);
                    sprite.Draw(texture, realStart - Main.screenPosition, null, lightning.color, realRotation, points[0], scale, SpriteEffects.None, 0);
					//sprite.DrawLine(l.start, TopLeft, Color.Red, 8);
					//sprite.DrawLine(l.start, BottomLeft, Color.Blue, 8);
					//sprite.DrawLine(BottomLeft, BottomRight, Color.Green, 8);
					//sprite.DrawLine(TopLeft, TopRight, Color.Black, 8);
					//sprite.DrawLine(l.end, TopRight, Color.White, 8);
					//sprite.DrawLine(l.end, BottomRight, Color.Gray, 8);
				}
				sprite.End();
			   	sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
		}
		public static Vector2 ToTextureCoords(Vector2 point, Vector2 LeftTop, Vector2 RightBottom, float rotation)
		{
			point -= LeftTop;
			RightBottom -= LeftTop;
			LeftTop = Vector2.Zero;
			point.RotateBy(-rotation);
			RightBottom.RotateBy(-rotation);
			point /= RightBottom;
			return point;
		}
		public static Vector2? RayColision(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
		{
			Vector2 vec1 = end1 - start1;
			float rotation = AngleFromVector(vec1);
			vec1.RotateBy(-rotation);
			end2 -= start1;
			start2 -= start1;
			end2.RotateBy(-rotation);
			start2.RotateBy(-rotation);
			if ((end2.Y < 0 && start2.Y < 0) || (end2.Y > 0 && start2.Y > 0))
			{
				return null;
			}
			float height = start2.Y - end2.Y;
			if (height == 0)
			{
				return null;
			}
			float kef = start2.Y / height;
			Vector2 vec2 = end2 - start2;
			Vector2 point = start2 + vec2 * kef;
			if (point.X > vec1.X || point.X < 0)
			{
				return null;
			}
			point.RotateBy(rotation);
			point += start1;
			return point;
		}
        public static Vector2? AlmostGarantedRayColision(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            Vector2 vec1 = end1 - start1;
            float rotation = AngleFromVector(vec1);
            vec1.RotateBy(-rotation);
            end2 -= start1;
            start2 -= start1;
            end2.RotateBy(-rotation);
            start2.RotateBy(-rotation);
            float height = start2.Y - end2.Y;
			if (height == 0)
			{
				return null;
			}
            float kef = start2.Y / height;
            Vector2 vec2 = end2 - start2;
            Vector2 point = start2 + vec2 * kef;
            point.RotateBy(rotation);
            point += start1;
            return point;
        }
        public static void RotateBy(this ref Vector2 vect, float radians)
		{
			vect = vect.RotatedBy(radians);
		}
		public static void CommonTerrapainFlyingMovement(Entity entity, Vector2 targetPosition, float rotatingSpeed, float MaxSpeed, float acceleration, float BreakingZone)
		{
            float maxVelocityMultyplier = 1;
            if (targetPosition != entity.Center)
            {
                entity.velocity += entity.DirectionTo(targetPosition) * acceleration;
            }
            if (entity.Distance(targetPosition) < MaxSpeed)
            {
                maxVelocityMultyplier = 1 - (MaxSpeed - entity.Distance(targetPosition)) / MaxSpeed;
            }
            Vector2 vectorToTargetPosition = targetPosition - entity.Center;
            float positiveRotation = AngleBetweenVectors(vectorToTargetPosition, entity.velocity);
            positiveRotation = NormalizeRotation(positiveRotation);
            float negativeRotation = AngleBetweenVectors(entity.velocity, vectorToTargetPosition);
            negativeRotation = NormalizeRotation(negativeRotation);
            if (positiveRotation > negativeRotation)
            {
                entity.velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
            }
            else
            {
                entity.velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
            }
            if (entity.velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                entity.velocity = entity.velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
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
                velocity.RotateBy(MathF.Max(-negativeRotation, -0.2f));
            }
            else
            {
                velocity.RotateBy(MathF.Min(positiveRotation, 0.2f));
            }
            if (velocity.Length() > MaxSpeed * maxVelocityMultyplier)
            {
                velocity = velocity.ToUnit() * MaxSpeed * maxVelocityMultyplier;
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
        public static Vector2 RandSpread(Vector2 velocity, int HierDeg, int LowerDeg)
		{
			float ProjectileSped = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2)));
			Vector2 dir = velocity / ProjectileSped;
			double angel = Math.Acos(dir.X);
			if (dir.Y >= 0)
				angel = Math.PI * 2 - angel;
			UnifiedRandom rand = new UnifiedRandom();
			angel += Convert.ToDouble(rand.Next(LowerDeg, HierDeg)) * ((2 * Math.PI) / 180);
			dir = new Vector2(Convert.ToSingle(Math.Cos(angel)) * -1, Convert.ToSingle(Math.Sin(angel)));
			velocity = dir * ProjectileSped * -1;
			return velocity;
		}
		public static Vector2 Rotate(Vector2 velocity, float deg)
		{
			float ProjectileSped = Convert.ToSingle(Math.Sqrt(Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2)));
			Vector2 dir = velocity / ProjectileSped;
			double angel = Math.Acos(dir.X);
			if (dir.Y >= 0)
				angel = Math.PI * 2 - angel;
			angel += deg * (2 * Math.PI / 180);
			dir = new Vector2(Convert.ToSingle(Math.Cos(angel)) * -1, Convert.ToSingle(Math.Sin(angel)));
			velocity = dir * ProjectileSped * -1;
			return velocity;
		}
		public static float getAngel(Vector2 velocity)
		{
			velocity.Normalize();
			double angel = Math.Acos(velocity.X);
			if (velocity.Y >= 0)
				angel += Math.PI;
			return Convert.ToSingle(angel);
		}
		public static bool Collision(Vector2 Pos1, Vector2 DirectedTo, float rad, Vector2 Pos2, int width, int height)
		{
			//Chatic("call");
			if (Pos1.X > Pos2.X && Pos1.X < Pos2.X + width && Pos1.Y > Pos2.Y && Pos1.Y < Pos2.Y + height)
			{
				//Chatic("inside");
				return true;
			}

			rad = Vector2.Distance(Pos1, DirectedTo) + rad;
			Vector2 crossPoint;
			Vector2 dir = DirectedTo - Pos1;
			dir.Normalize();

			/*if (dir.Y != 0)
			{
				crossPoint.Y = Pos2.Y;
				crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X < Pos2.X || crossPoint.X > Pos2.X + width)
				{
					crossPoint.Y = Pos2.Y + height;
					crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
					if (crossPoint.X < Pos2.X || crossPoint.X > Pos2.X + width)
					{
						return false;
					}
					else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
					{
						if (dir.X != 0)
						{
							crossPoint.X = Pos2.X;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
							{
								crossPoint.X = Pos2.X + width;
								crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
								if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
								{
									return false;
								}
								else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
								{
									return false;
								}
							}
						}
					}
				}
				else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
				{
					if (dir.X != 0)
					{
						crossPoint.X = Pos2.X;
						crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
						if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
						{
							crossPoint.X = Pos2.X + width;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
							{
								return false;
							}
							else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
							{
								return false;
							}
						}
					}
				}
			}
			else
			{
				if (DirectedTo.Y < Pos2.Y || DirectedTo.Y > Pos2.Y + height)
				{
					return false;
				}
				if (Math.Abs(Pos2.X - DirectedTo.X) > rad && Math.Abs(Pos2.X + width - DirectedTo.X) > rad)
				{
					return false;
				}
				crossPoint.Y = DirectedTo.Y;
				crossPoint.X = dir.X > 0 ? Pos2.X : Pos2.X + width;
			}*/

			Vector2 closestCrossPoint = Vector2.Zero;
			if (dir.Y != 0)
			{
				crossPoint.Y = Pos2.Y;
				crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X > Pos2.X && crossPoint.X < Pos2.X + width && (crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign())
				{
					//Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
					closestCrossPoint = crossPoint;
				}
				crossPoint.Y = Pos2.Y + height;
				crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X > Pos2.X && crossPoint.X < Pos2.X + width && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && (crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign())
				{
					//Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
					closestCrossPoint = crossPoint;
				}
			}
			if (dir.X != 0)
			{
				crossPoint.X = Pos2.X;
				crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
				if (crossPoint.Y > Pos2.Y & crossPoint.Y < Pos2.Y + height && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && (crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign() && (crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign())
				{
					//Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
					closestCrossPoint = crossPoint;
				}
				crossPoint.X = Pos2.X + width;
				crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
				if (crossPoint.Y > Pos2.Y & crossPoint.Y < Pos2.Y + height && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && (crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign())
				{
					//Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
					closestCrossPoint = crossPoint;
				}
			}

			if (closestCrossPoint != Vector2.Zero && Vector2.Distance(closestCrossPoint, Pos1) <= rad)
			{
				crossPoint = closestCrossPoint;
			}
			else
			{
				return false;
			}
			Pos1 /= 16;
			crossPoint /= 16;
			int W = Math.Abs((int)Pos1.X - (int)crossPoint.X);
			int H = Math.Abs((int)Pos1.Y - (int)crossPoint.Y);
			int LeastX = Pos1.X < crossPoint.X ? (int)Pos1.X : (int)crossPoint.X;
			int LeastY = Pos1.Y < crossPoint.Y ? (int)Pos1.Y : (int)crossPoint.Y;

			for (int w = 0; w <= W; w++)
			{
				for (int h = 0; h <= H; h++)
				{
					if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
					{
						if (dir.Y != 0)
						{
							crossPoint.Y = LeastY + h;
							crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
							{
								return false;
							}
							crossPoint.Y = LeastY + h + 1;
							crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
							{
								return false;
							}
							crossPoint.X = LeastX + w;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
							{
								return false;
							}
							crossPoint.X = LeastX + w + 1;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
							{
								return false;
							}
						}
						else if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
						{
							return false;
						}
					}
				}
			}
			//Functions.Chatic(true);
			return true;
		}
        public static bool Collision(Vector2 Pos1, Vector2 dir, float rad, Vector2 Pos2, int width, int height, ref Vector2 HitPoint, bool tileCollide = true)
        {
			if (dir.X < 0.00001 && dir.X > -0.00001)
			{
				dir.X = 0;
			}
            if (dir.Y < 0.00001 && dir.Y > -0.00001)
            {
                dir.Y = 0;
            }
            //Chatic("call");
            if (Pos1.X > Pos2.X && Pos1.X < Pos2.X + width && Pos1.Y > Pos2.Y && Pos1.Y < Pos2.Y + height)
            {
				HitPoint = Pos1;
                return true;
            }

            Vector2 crossPoint;

            /*if (dir.Y != 0)
			{
				crossPoint.Y = Pos2.Y;
				crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X < Pos2.X || crossPoint.X > Pos2.X + width)
				{
					crossPoint.Y = Pos2.Y + height;
					crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
					if (crossPoint.X < Pos2.X || crossPoint.X > Pos2.X + width)
					{
						return false;
					}
					else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
					{
						if (dir.X != 0)
						{
							crossPoint.X = Pos2.X;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
							{
								crossPoint.X = Pos2.X + width;
								crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
								if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
								{
									return false;
								}
								else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
								{
									return false;
								}
							}
						}
					}
				}
				else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
				{
					if (dir.X != 0)
					{
						crossPoint.X = Pos2.X;
						crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
						if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
						{
							crossPoint.X = Pos2.X + width;
							crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
							if (crossPoint.Y < Pos2.Y || crossPoint.Y > Pos2.Y + height)
							{
								return false;
							}
							else if (Vector2.Distance(DirectedTo, crossPoint) > rad)
							{
								return false;
							}
						}
					}
				}
			}
			else
			{
				if (DirectedTo.Y < Pos2.Y || DirectedTo.Y > Pos2.Y + height)
				{
					return false;
				}
				if (Math.Abs(Pos2.X - DirectedTo.X) > rad && Math.Abs(Pos2.X + width - DirectedTo.X) > rad)
				{
					return false;
				}
				crossPoint.Y = DirectedTo.Y;
				crossPoint.X = dir.X > 0 ? Pos2.X : Pos2.X + width;
			}*/

            Vector2 closestCrossPoint = Vector2.Zero;
            if (dir.Y != 0)
            {
                crossPoint.Y = Pos2.Y;
                crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
                if (crossPoint.X > Pos2.X && crossPoint.X < Pos2.X + width && ((crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign()))
                {
                    //Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
                    closestCrossPoint = crossPoint;
                }
                crossPoint.Y = Pos2.Y + height;
                crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
                if (crossPoint.X > Pos2.X && crossPoint.X < Pos2.X + width && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && ((crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign()))
                {
                    //Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
                    closestCrossPoint = crossPoint;
                }
            }
            if (dir.X != 0)
            {
                crossPoint.X = Pos2.X;
                crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
                if (crossPoint.Y > Pos2.Y & crossPoint.Y < Pos2.Y + height && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && ((crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign()))
                {
                    //Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
                    closestCrossPoint = crossPoint;
                }
                crossPoint.X = Pos2.X + width;
                crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
                if (crossPoint.Y > Pos2.Y & crossPoint.Y < Pos2.Y + height && (Vector2.Distance(closestCrossPoint, Pos1) > Vector2.Distance(crossPoint, Pos1) || closestCrossPoint == Vector2.Zero) && ((crossPoint.X - Pos1.X).NonZeroSign() == dir.X.NonZeroSign() && (crossPoint.Y - Pos1.Y).NonZeroSign() == dir.Y.NonZeroSign()))
                {
                    //Dust.NewDust(crossPoint, 0, 0, DustID.Torch);
                    closestCrossPoint = crossPoint;
                }
            }

            if (closestCrossPoint != Vector2.Zero && Vector2.Distance(closestCrossPoint, Pos1) <= rad)
            {
                crossPoint = closestCrossPoint;
            }
            else
            {
                return false;
            }
			HitPoint = crossPoint;
			float dist = Vector2.Distance(closestCrossPoint, Pos1);

            if (tileCollide)
			{
				Pos1 /= 16;
				crossPoint /= 16;
				int W = Math.Abs((int)Pos1.X - (int)crossPoint.X);
				int H = Math.Abs((int)Pos1.Y - (int)crossPoint.Y);
				int LeastX = Pos1.X < crossPoint.X ? (int)Pos1.X : (int)crossPoint.X;
				int LeastY = Pos1.Y < crossPoint.Y ? (int)Pos1.Y : (int)crossPoint.Y;

				for (int w = 0; w <= W; w++)
				{
					for (int h = 0; h <= H; h++)
					{
						if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
						{
							if (dir.Y != 0)
							{
								crossPoint.Y = LeastY + h;
								crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
								if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
								{
									return false;
								}
								crossPoint.Y = LeastY + h + 1;
								crossPoint.X = Pos1.X - dir.X * ((Pos1.Y - crossPoint.Y) / dir.Y);
								if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
								{
									return false;
								}
								crossPoint.X = LeastX + w;
								crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
								if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
								{
									return false;
								}
								crossPoint.X = LeastX + w + 1;
								crossPoint.Y = Pos1.Y - dir.Y * ((Pos1.X - crossPoint.X) / dir.X);
								if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
								{
									return false;
								}
							}
							else if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
							{
								return false;
							}
						}
					}
				}
			}
            //Functions.Chatic(true);
            return true;
        }
        public static bool CanHit(Vector2 Pos, Vector2 target, int width, int height)
		{
			if (Pos.X >= target.X && Pos.X <= target.X + width && Pos.Y >= target.Y && Pos.Y <= target.Y + height)
			{
                Pos /= 16;
                if (Main.tile[(int)Pos.X, (int)Pos.Y].HasTile && Main.tileSolid[Main.tile[(int)Pos.X, (int)Pos.Y].TileType] && !Main.tileSolidTop[Main.tile[(int)Pos.X, (int)Pos.Y].TileType])
                {
                    return false;
                }
				return true;
            }
			Vector2 crossPoint;
			Vector2 dir = target + new Vector2((float)width / 2, (float)height / 2) - Pos;
			dir.Normalize();

			Vector2 closestCrossPoint = Vector2.Zero;
			if (dir.Y != 0)
			{
				crossPoint.Y = target.Y;
				crossPoint.X = Pos.X - dir.X * ((Pos.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X > target.X && crossPoint.X < target.X + width)
				{
					closestCrossPoint = crossPoint;
				}
				crossPoint.Y = target.Y + height;
				crossPoint.X = Pos.X - dir.X * ((Pos.Y - crossPoint.Y) / dir.Y);
				if (crossPoint.X > target.X && crossPoint.X < target.X + width && (Vector2.Distance(closestCrossPoint, Pos) > Vector2.Distance(crossPoint, Pos) || closestCrossPoint == Vector2.Zero))
				{
					closestCrossPoint = crossPoint;
				}
			}
			if (dir.X != 0)
			{
				crossPoint.X = target.X;
				crossPoint.Y = Pos.Y - dir.Y * ((Pos.X - crossPoint.X) / dir.X);
				if (crossPoint.Y > target.Y & crossPoint.Y < target.Y + height && (Vector2.Distance(closestCrossPoint, Pos) > Vector2.Distance(crossPoint, Pos) || closestCrossPoint == Vector2.Zero))
				{
					closestCrossPoint = crossPoint;
				}
				crossPoint.X = target.X + width;
				crossPoint.Y = Pos.Y - dir.Y * ((Pos.X - crossPoint.X) / dir.X);
				if (crossPoint.Y > target.Y & crossPoint.Y < target.Y + height && (Vector2.Distance(closestCrossPoint, Pos) > Vector2.Distance(crossPoint, Pos) || closestCrossPoint == Vector2.Zero))
				{
					closestCrossPoint = crossPoint;
				}
			}

			if (closestCrossPoint != Vector2.Zero)
			{
				crossPoint = closestCrossPoint;
			}
			else
			{
				return false;
			}
			for (int i = 0; i < crossPoint.Length(); i += 2)
			{
				Vector2 direcrion = crossPoint - Pos;
				direcrion.Normalize();
				//Dust.NewDust(Pos + direcrion * i, 0, 0, DustID.ShimmerSpark);
			}
			Pos /= 16;
			crossPoint /= 16;
			int W = Math.Abs((int)Pos.X - (int)crossPoint.X);
			int H = Math.Abs((int)Pos.Y - (int)crossPoint.Y);
			int LeastX = Pos.X < crossPoint.X ? (int)Pos.X : (int)crossPoint.X;
			int LeastY = Pos.Y < crossPoint.Y ? (int)Pos.Y : (int)crossPoint.Y;

			for (int w = 0; w <= W; w++)
			{
				for (int h = 0; h <= H; h++)
				{
					if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
					{
						if (dir.Y != 0)
						{
							crossPoint.Y = LeastY + h;
							crossPoint.X = Pos.X - dir.X * ((Pos.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
							{
								return false;
							}
							crossPoint.Y = LeastY + h + 1;
							crossPoint.X = Pos.X - dir.X * ((Pos.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
							{
								return false;
							}
							crossPoint.X = LeastX + w;
							crossPoint.Y = Pos.Y - dir.Y * ((Pos.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
							{
								return false;
							}
							crossPoint.X = LeastX + w + 1;
							crossPoint.Y = Pos.Y - dir.Y * ((Pos.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
							{
								return false;
							}
						}
						else if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public static bool CircleColision(Entity source, float rad, Entity target)
		{
			return CircleColision(source.Center, rad, target.position, target.width, target.height);
		}
		public static bool CircleColision(Vector2 source, float rad, Entity target)
		{
			return CircleColision(source, rad, target.position, target.width, target.height);
		}
		public static bool CircleColision(Vector2 Pos1, float rad, Vector2 Pos2, int width, int height)
		{
			Vector2 collidePoint;
			if (Pos1.Distance(Pos2) <= rad)
			{
				return true;
			}
			else if (Pos1.Distance(Pos2 + new Vector2(width, 0)) <= rad)
			{
				return true;
			}
			else if (Pos1.Distance(Pos2 + new Vector2(width, height)) <= rad)
			{
				return true;
			}
			else if (Pos1.Distance(Pos2 + new Vector2(0, height)) <= rad)
			{
				return true;
			}
			else if (Math.Abs(Pos1.X - Pos2.X) <= rad)
			{
				collidePoint.X = Pos2.X;
				collidePoint.Y = Pos1.Y + (float)Math.Sin(Math.Acos((Pos1.X - Pos2.X) / rad)) * rad;
				if (collidePoint.Y > Pos2.Y && collidePoint.Y < Pos2.Y + height)
				{
					return true;
				}
			}
			else if (Math.Abs(Pos1.X - Pos2.X - width) <= rad)
			{
				collidePoint.X = Pos2.X + width;
				collidePoint.Y = Pos1.Y + (float)Math.Sin(Math.Acos((Pos1.X - Pos2.X + width) / rad)) * rad;
				if (collidePoint.Y > Pos2.Y && collidePoint.Y < Pos2.Y + height)
				{
					return true;
				}
			}
			else if (Math.Abs(Pos1.Y - Pos2.Y) <= rad)
			{
				collidePoint.Y = Pos2.Y;
				collidePoint.X = Pos1.X + (float)Math.Sin(Math.Acos((Pos1.Y - Pos2.Y) / rad)) * rad;
				if (collidePoint.X > Pos2.X && collidePoint.X < Pos2.X + width)
				{
					return true;
				}
			}
			else if (Math.Abs(Pos1.Y - Pos2.Y - height) <= rad)
			{
				collidePoint.Y = Pos2.Y + height;
				collidePoint.X = Pos1.X + (float)Math.Sin(Math.Acos((Pos1.Y - Pos2.Y - height) / rad)) * rad;
				if (collidePoint.X > Pos2.X && collidePoint.X < Pos2.X + width)
				{
					return true;
				}
			}
			return false;
		}
		public static Vector2 RayColisionInTheWorld(Vector2 start, Vector2 end, bool checkPltforms = false)
		{
			if (start.Y > end.Y)
			{
				checkPltforms = false;
			}
			Vector2 crossPoint = Vector2.Zero;
			start /= 16;
			end /= 16;
			Vector2 dir = (end - start).Normalized();
            int W = (int)end.X - (int)start.X;
            int H = (int)end.Y - (int)start.Y;
            int LeastX = (int)start.X;
            int LeastY = (int)start.Y;
			if (Main.tile[start.ToTileCoordinates()].HasTile && Main.tileSolid[Main.tile[start.ToTileCoordinates()].TileType])
			{
				return start;
			}

            for (int w = 0; Math.Abs(w) <= Math.Abs(W); w += W == 0? 1 : Math.Sign(W))
            {
                for (int h = 0; Math.Abs(h) <= Math.Abs(H); h += H == 0 ? 1 : Math.Sign(H))
                {
                    if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && (!Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType] || checkPltforms))
                    {
						bool flag = Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType];
                        if (dir.Y != 0)
						{
                            Vector2 result = Vector2.Zero;
							if (Main.tile[LeastX + w, LeastY + h].IsHalfBlock)
							{
                                float distance = float.MaxValue;
                                crossPoint.Y = LeastY + h + 0.5f;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
                                {
                                    result = crossPoint;
                                    distance = start.Distance(result);
                                }
                                crossPoint.Y = LeastY + h + 1;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1 && crossPoint.Distance(start) < distance && !flag)
                                {
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h + 0.5f && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance && !flag)
                                {
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w + 1;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h + 0.5f && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance && !flag)
                                {
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                if (result != Vector2.Zero)
                                {
                                    return result * 16;
                                }
                            }
							else if (Main.tile[LeastX + w, LeastY + h].BottomSlope && Main.tile[LeastX + w, LeastY + h].RightSlope && !flag)
							{
                                float distance = float.MaxValue;
								bool touchSlope = false;
								bool touchSlope1 = false;
                                bool touchSlope2 = false;
                                crossPoint.Y = LeastY + h;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
                                {
									touchSlope = true;
									touchSlope1 = true;
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
                                {
									touchSlope = !touchSlope;
                                    touchSlope2 = true;
                                    if (crossPoint.Distance(start) < distance)
									{
										result = crossPoint;
										distance = crossPoint.Distance(start);
									}
                                }
								if (touchSlope && (dir.X < 0 && touchSlope2 || dir.Y < 0 && touchSlope1))
								{
									Vector2 v = dir;
									v.X = MathF.Abs(v.X);
									v.Y = MathF.Abs(v.Y);
									v /= v.X + v.Y;
									if (touchSlope1)
									{
										float val = 1 - (result.X - LeastX - w);
										float y = val * v.Y;
										float x = 1 - y;
										result.X = LeastX + w + x;
										result.Y = LeastY + h + y;
                                    }
									else
									{
										float val = 1 - (result.Y - LeastY - h);
										float x = val * v.X;
										float y = 1 - x;
										result.X = LeastX + w + x;
										result.Y = LeastY + h + y;
                                    }
                                }
                                if (result != Vector2.Zero)
                                {
                                    return result * 16;
                                }
                            }
							else if (Main.tile[LeastX + w, LeastY + h].BottomSlope && Main.tile[LeastX + w, LeastY + h].LeftSlope && !flag)
							{
                                float distance = float.MaxValue;
                                bool touchSlope = false;
                                bool touchSlope1 = false;
                                bool touchSlope2 = false;
                                crossPoint.Y = LeastY + h;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
                                {
                                    touchSlope = true;
                                    touchSlope1 = true;
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w + 1;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
                                {
                                    touchSlope = !touchSlope;
                                    touchSlope2 = true;
                                    if (crossPoint.Distance(start) < distance)
                                    {
                                        result = crossPoint;
                                        distance = crossPoint.Distance(start);
                                    }
                                }
                                if (touchSlope && (dir.X > 0 && touchSlope2 || dir.Y < 0 && touchSlope1))
                                {
                                    Vector2 v = dir;
                                    v.X = MathF.Abs(v.X);
                                    v.Y = MathF.Abs(v.Y);
                                    v /= v.X + v.Y;
                                    if (touchSlope1)
                                    {
                                        float val = result.X - LeastX - w;
                                        float y = val * v.Y;
                                        float x = 1 - y;
                                        result.X = LeastX + w + 1 - x;
                                        result.Y = LeastY + h + y;
                                    }
                                    else
                                    {
                                        float val = 1 - (result.Y - LeastY - h);
                                        float x = val * v.X;
                                        float y = 1 - x;
                                        result.X = LeastX + w + 1 - x;
                                        result.Y = LeastY + h + y;
                                    }
                                }
                                if (result != Vector2.Zero)
                                {
                                    return result * 16;
                                }
                            }
							else if (Main.tile[LeastX + w, LeastY + h].TopSlope && Main.tile[LeastX + w, LeastY + h].LeftSlope)
							{
                                float distance = float.MaxValue;
                                bool touchSlope = false;
                                bool touchSlope1 = false;
                                bool touchSlope2 = false;
                                crossPoint.Y = LeastY + h + 1;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
                                {
                                    touchSlope = true;
                                    touchSlope1 = true;
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w + 1;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
                                {
                                    touchSlope = !touchSlope;
                                    touchSlope2 = true;
                                    if (crossPoint.Distance(start) < distance)
                                    {
                                        result = crossPoint;
                                        distance = crossPoint.Distance(start);
                                    }
                                }
                                if (touchSlope && (dir.X > 0 && touchSlope2 || dir.Y > 0 && touchSlope1))
                                {
                                    Vector2 v = dir;
                                    v.X = MathF.Abs(v.X);
                                    v.Y = MathF.Abs(v.Y);
                                    v /= v.X + v.Y;
                                    if (touchSlope1)
                                    {
                                        float val = result.X - LeastX - w;
                                        float y = val * v.Y;
                                        float x = 1 - y;
                                        result.X = LeastX + w + 1 - x;
                                        result.Y = LeastY + h + 1 - y;
                                    }
                                    else
                                    {
                                        float val = result.Y - LeastY - h;
                                        float x = val * v.X;
                                        float y = 1 - x;
                                        result.X = LeastX + w + 1 - x;
                                        result.Y = LeastY + h + 1 - y;
                                    }
                                }
                                if (result != Vector2.Zero && (touchSlope || !flag))
                                {
                                    return result * 16;
                                }
                            }
							else if (Main.tile[LeastX + w, LeastY + h].TopSlope && Main.tile[LeastX + w, LeastY + h].RightSlope)
							{
                                float distance = float.MaxValue;
                                bool touchSlope = false;
                                bool touchSlope1 = false;
                                bool touchSlope2 = false;
                                crossPoint.Y = LeastY + h + 1;
                                crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
                                if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
                                {
                                    touchSlope = true;
                                    touchSlope1 = true;
                                    result = crossPoint;
                                    distance = crossPoint.Distance(start);
                                }
                                crossPoint.X = LeastX + w;
                                crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
                                if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1)
                                {
                                    touchSlope = !touchSlope;
                                    touchSlope2 = true;
                                    if (crossPoint.Distance(start) < distance)
                                    {
                                        result = crossPoint;
                                        distance = crossPoint.Distance(start);
                                    }
                                }
                                if (touchSlope && (dir.X < 0 && touchSlope2 || dir.Y > 0 && touchSlope1))
                                {
                                    Vector2 v = dir;
                                    v.X = MathF.Abs(v.X);
                                    v.Y = MathF.Abs(v.Y);
                                    v /= v.X + v.Y;
                                    if (touchSlope1)
                                    {
                                        float val = 1 - (result.X - LeastX - w);
                                        float y = val * v.Y;
                                        float x = 1 - y;
                                        result.X = LeastX + w + x;
                                        result.Y = LeastY + h + 1 - y;
                                    }
                                    else
                                    {
                                        float val = result.Y - LeastY - h;
                                        float x = val * v.X;
                                        float y = 1 - x;
                                        result.X = LeastX + w + x;
                                        result.Y = LeastY + h + 1 - y;
                                    }
                                }
                                if (result != Vector2.Zero && (touchSlope || !flag))
                                {
                                    return result * 16;
                                }
                            }
							else
							{
								float distance = float.MaxValue;
								crossPoint.Y = LeastY + h;
								crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
								if (crossPoint.X >= LeastX + w && crossPoint.X <= LeastX + w + 1)
								{
									result = crossPoint;
									distance = start.Distance(result);
								}
								crossPoint.Y = LeastY + h + 1;
								crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
								if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1 && crossPoint.Distance(start) < distance && !flag)
								{
									result = crossPoint;
									distance = crossPoint.Distance(start);
								}
								crossPoint.X = LeastX + w;
								crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
								if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance && !flag)
								{
									result = crossPoint;
									distance = crossPoint.Distance(start);
								}
								crossPoint.X = LeastX + w + 1;
								crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
								if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance && !flag)
								{
									result = crossPoint;
									distance = crossPoint.Distance(start);
								}
								if (result != Vector2.Zero)
								{
									return result * 16;
								}
							}
                        }
                        else if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileSolid[Main.tile[LeastX + w, LeastY + h].TileType] && !Main.tileSolidTop[Main.tile[LeastX + w, LeastY + h].TileType])
                        {
                            return new Vector2(LeastX + w + (dir.X > 0? 0 : 1), start.Y) * 16;
                        }
                    }
                }
            }
            return Vector2.Zero;
		}
		public static void RayCutTile(Vector2 start, Vector2 end, Player player)
		{
			bool[] tileCutIgnorance = player.GetTileCutIgnorance(false, false);
            Vector2 crossPoint = Vector2.Zero;
            start /= 16;
            end /= 16;
            Vector2 dir = (end - start).Normalized();
            int W = (int)end.X - (int)start.X;
            int H = (int)end.Y - (int)start.Y;
            int LeastX = (int)start.X;
            int LeastY = (int)start.Y;

            for (int w = 0; Math.Abs(w) <= Math.Abs(W); w += W == 0 ? 1 : Math.Sign(W))
            {
                for (int h = 0; Math.Abs(h) <= Math.Abs(H); h += H == 0 ? 1 : Math.Sign(H))
                {
                    if (Main.tile[LeastX + w, LeastY + h].HasTile && Main.tileCut[Main.tile[LeastX + w, LeastY + h].TileType] && !tileCutIgnorance[Main.tile[LeastX + w, LeastY + h].TileType] && WorldGen.CanCutTile(LeastX + w, LeastY + h, Terraria.Enums.TileCuttingContext.AttackProjectile))
                    {
						if (dir.Y != 0)
						{
							Vector2 result = Vector2.Zero;
							float distance = float.MaxValue;
							crossPoint.Y = LeastY + h;
							crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1)
							{
								result = crossPoint;
								distance = start.Distance(result);
							}
							crossPoint.Y = LeastY + h + 1;
							crossPoint.X = start.X - dir.X * ((start.Y - crossPoint.Y) / dir.Y);
							if (crossPoint.X > LeastX + w && crossPoint.X < LeastX + w + 1 && crossPoint.Distance(start) < distance)
							{
								result = crossPoint;
								distance = crossPoint.Distance(start);
							}
							crossPoint.X = LeastX + w;
							crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance)
							{
								result = crossPoint;
								distance = crossPoint.Distance(start);
							}
							crossPoint.X = LeastX + w + 1;
							crossPoint.Y = start.Y - dir.Y * ((start.X - crossPoint.X) / dir.X);
							if (crossPoint.Y > LeastY + h && crossPoint.Y < LeastY + h + 1 && crossPoint.Distance(start) < distance)
							{
								result = crossPoint;
								distance = crossPoint.Distance(start);
							}
							if (result != Vector2.Zero)
							{
								WorldGen.KillTile(LeastX + w, LeastY + h);
                                if (Main.netMode != 0)
                                    NetMessage.SendData(17, -1, -1, null, 0, LeastX + w, LeastY + h);
                            }
						}
						else
						{
                            WorldGen.KillTile(LeastX + w, LeastY + h);
                            if (Main.netMode != 0)
                                NetMessage.SendData(17, -1, -1, null, 0, LeastX + w, LeastY + h);
                        }
                    }
                }
            }
        }
		public static float DistanceBetweenHitboxes(Entity e1, Entity e2)
		{
			return DistanceBetweenHitboxes(e1.position, e1.width, e1.height, e2.position, e2.width, e2.height);
		}
		public static float DistanceBetweenHitboxes(Entity e1, Vector2 hitboxPosition, int width, int height)
		{
			return DistanceBetweenHitboxes(e1.position, e1.width, e1.height, hitboxPosition, width, height);
		}
		public static float DistanceBetweenHitboxes(Vector2 hitbox1Position, int width1, int height1, Vector2 hitbox2Position, int width2, int height2)
		{
			if (RectangleColision(hitbox1Position, width1, height1, hitbox2Position, width2, height2))
			{
				return 0;
			}
			Vector2 pos1 = hitbox1Position;
			int w1 = width1;
			int h1 = height1;
			Vector2 pos2 = hitbox2Position;
			int w2 = width2;
			int h2 = height2;
			float distance = 0;
			if (pos1.Y > pos2.Y + h2)
			{
				distance = pos1.Y - (pos2.Y + h2);
			}
			if (pos2.Y > pos1.Y + h1)
			{
				distance = pos2.Y - (pos1.Y + h1);
			}
			if (pos1.X > pos2.X + w2)
			{
				distance = pos1.X - (pos2.X + w2);
			}
			if (pos2.X > pos1.X + w1)
			{
				distance = pos2.X - (pos1.X + w1);
			}
			if (pos1.Y > pos2.Y + h2 && pos1.X > pos2.X + w2)
			{ 
				distance = new Vector2(pos2.X + w2, pos2.Y + h2).Distance(pos1);
			}
			if (pos2.Y > pos1.Y + h1 && pos2.X > pos1.X + w1)
			{
				distance = new Vector2(pos1.X + w1, pos1.Y + h1).Distance(pos2);
			}
			if (pos2.Y > pos1.Y + h1 && pos1.X > pos2.X + w2)
			{
				distance = new Vector2(pos1.X, pos1.Y + h1).Distance(new Vector2(pos2.X + w2, pos2.Y));
			}
			if (pos1.Y > pos2.Y + h2 && pos2.X > pos1.X + w1)
			{
				distance = new Vector2(pos2.X, pos2.Y + h2).Distance(new Vector2(pos1.X + w1, pos1.Y));
			}
			return distance;
		}
		public static float DistanceToClosestNPC(Entity player, bool friendly = false)
		{
			float closest = -1;
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && !npc.friendly && (closest == -1 || DistanceBetweenHitboxes(player, npc) < closest))
				{
					closest = DistanceBetweenHitboxes(player, npc);
				}
			}
			return closest;
		}
        public static float DistanceToClosestNPC(Vector2 position, bool friendly = false)
        {
            float closest = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && (closest == -1 || DistanceBetweenHitboxes(npc, position, 0, 0) < closest))
                {
                    closest = DistanceBetweenHitboxes(npc, position, 0, 0);
                }
            }
            return closest;
        }
        public static NPC FindClosestNPC(Vector2 position, bool? friendly = false, int type = -1)
        {
			NPC ret = null;
			float closest = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (type == -1 || npc.type == type) && (!friendly.HasValue || npc.friendly == friendly.Value) && (closest == -1 || DistanceBetweenHitboxes(npc, position, 0, 0) < closest))
                {
                    closest = DistanceBetweenHitboxes(npc, position, 0, 0);
					ret = npc;
                }
            }
            return ret;
        }
		public static NPC FindClosestNPCinCollection(Vector2 position, List<NPC> collection)
		{
            NPC ret = null;
            float closest = -1;
			foreach (NPC npc in collection)
            {
                if (npc.active  && (closest == -1 || DistanceBetweenHitboxes(npc, position, 0, 0) < closest))
                {
                    closest = DistanceBetweenHitboxes(npc, position, 0, 0);
                    ret = npc;
                }
            }
            return ret;
        }
		public static List<NPC> AllNPCByType(int type)
		{
			List<NPC> NPCs = new List<NPC>();
			foreach(var npc in Main.npc)
			{
				if (npc.active && npc.type == type)
				{
					NPCs.Add(npc);
				}
			}
			return NPCs;
		}
		public static bool TriangleCollision(Entity target, Vector2 point1, Vector2 point2, Vector2 point3)
		{
			return TriangleCollision(target.Hitbox, point1, point2, point3);
		}
        public static bool TriangleCollision(Rectangle hitbox, Vector2 point1, Vector2 point2, Vector2 point3)
        {
			if (hitbox.X < point1.X && hitbox.X + hitbox.Width > point1.X && hitbox.Y < point1.Y && hitbox.Y + hitbox.Height > point1.Y)
			{
				return true;
			}
            if (hitbox.X < point2.X && hitbox.X + hitbox.Width > point2.X && hitbox.Y < point2.Y && hitbox.Y + hitbox.Height > point2.Y)
            {
                return true;
            }
            if (hitbox.X < point3.X && hitbox.X + hitbox.Width > point3.X && hitbox.Y < point3.Y && hitbox.Y + hitbox.Height > point3.Y)
            {
                return true;
            }
			Vector2[] points = [
				hitbox.Location.ToVector2(),
				hitbox.TopRight(),
				hitbox.BottomLeft(),
				hitbox.BottomRight(),
			];
			foreach (var p in points)
			{
				float value1 = (point1.X - point2.X) * (p.Y - point2.Y) - (point1.Y - point2.Y) * (p.X - point2.X);
                float value2 = (point2.X - point3.X) * (p.Y - point3.Y) - (point2.Y - point3.Y) * (p.X - point3.X);
                float value3 = (point2.X - point1.X) * (p.Y - point1.Y) - (point3.Y - point1.Y) * (p.X - point1.X);
				if ((value1 >= 0 && value2 >= 0 && value3 >= 0) || (value1 <= 0 && value2 <= 0 && value3 <= 0))
				{
					return true;
				}
            }
			if (RayColision(points[0], points[1], point1, point2).HasValue)
			{
				return true;
			}
            if (RayColision(points[1], points[3], point1, point2).HasValue)
            {
                return true;
            }
            if (RayColision(points[2], points[3], point1, point2).HasValue)
            {
                return true;
            }
            if (RayColision(points[0], points[1], point2, point3).HasValue)
            {
                return true;
            }
            if (RayColision(points[1], points[3], point2, point3).HasValue)
            {
                return true;
            }
            if (RayColision(points[2], points[3], point2, point3).HasValue)
            {
                return true;
            }
            if (RayColision(points[0], points[1], point1, point3).HasValue)
            {
                return true;
            }
            if (RayColision(points[1], points[3], point1, point3).HasValue)
            {
                return true;
            }
            if (RayColision(points[2], points[3], point1, point3).HasValue)
            {
                return true;
            }
			return false;
        }
        public static bool RectangleColision(Rectangle rect1, Rectangle rect2)
		{
			return RectangleColision(rect1.Location.ToVector2(), rect1.Width, rect1.Height, rect2.Location.ToVector2(), rect2.Width, rect2.Height);
		}
		public static bool RectangleColision(Entity ent1, Entity ent2)
		{
			return RectangleColision(ent1.position, ent1.width, ent1.height, ent2.position, ent2.width, ent2.height);
		}
		public static bool RectangleColision(Entity ent, Vector2 hitboxPosition, int width, int height)
		{
			return RectangleColision(ent.position, ent.width, ent.height, hitboxPosition, width, height);
		}
		public static bool RectangleColision(Vector2 pos1, int w1, int h1, Vector2 pos2, int w2, int h2)
		{
			if (pos1.X + w1 < pos2.X || pos2.X + w2 < pos1.X)
			{
				return false;
			}
			if (pos1.Y + h1 < pos2.Y || pos2.Y + h2 < pos1.Y)
			{
				return false;
			}
			return true;
		}
		public static bool HitTiles(Entity e, bool hitPlatorms = false)
		{
			return HitTiles(e.position, e.width, e.height, hitPlatorms);
		}
		public static bool HitTiles(Vector2 Pos, int width, int height, bool hitPlatorms = false)
		{
			Pos /= 16;
			Vector2 Pos2 = new Vector2(Pos.X + (float)width / 16, Pos.Y + (float)height / 16);
			if (Pos.X < 0)
				Pos.X = 0;
			if (Pos.Y < 0)
				Pos.Y = 0;
			if (Pos2.X > Main.maxTilesX)
				Pos2.X = Main.maxTilesX;
			if (Pos2.Y > Main.maxTilesY)
				Pos2.Y = Main.maxTilesY;

			for (int w = 0; w <= (int)Pos2.X - (int)Pos.X; w++)
			{
				for (int h = 0; h <= (int)Pos2.Y - (int)Pos.Y; h++)
				{
					if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].HasTile && Main.tileSolid[Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TileType] && (!Main.tileSolidTop[Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TileType] || hitPlatorms))
					{
						if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].IsHalfBlock)
						{
							if (Pos2.Y > (int)Pos.Y + h + 0.5f)
							{
								return true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TopSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TopSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].LeftSlope)
						{
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].BottomSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].BottomSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								return true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								return true;
							}
						}
						else
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static bool CheckGround(Entity e, bool collidePlatforms = true)
		{
			return CheckGround(e.BottomLeft, e.width, collidePlatforms);
		}
		public static bool CheckGround(Vector2 BottomLeft, float width, bool collidePlatforms = true)
		{
			BottomLeft /= 16;
			width /= 16;
			Vector2 BottomRight = BottomLeft + Vector2.UnitX * width;
			for (int i = 0; i < BottomLeft.X + width - (int)BottomLeft.X; i++)
			{
				int y = (int)BottomLeft.Y;
				int x = (int)BottomLeft.X + i;
				Tile tile = Main.tile[x, y];
				if (tile.HasTile && Main.tileSolid[tile.TileType] && (!Main.tileSolidTop[tile.TileType] || collidePlatforms))
				{
					if (tile.IsHalfBlock)
					{
						if (BottomLeft.Y - (int)BottomLeft.Y >= 0.5f)
						{
							return true;
						}
					}
					else if (tile.TopSlope && tile.LeftSlope)
					{
						if ((int)BottomRight.X - BottomRight.X + 1 <= BottomLeft.Y - (int)BottomLeft.Y)
						{
							return true;
						}
					}
					else if (tile.TopSlope && tile.RightSlope)
					{
						if (BottomLeft.X - (int)BottomLeft.X <= BottomLeft.Y - (int)BottomLeft.Y)
						{
							return true;
						}
					}
					else
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool StairsColision(Entity e, out List<Vector2> tiles)
		{
			return StairsColision(e.position, e.width, e.height, out tiles);
		}
		public static bool StairsColision(Vector2 Pos, int width, int height, out List<Vector2> tiles)
		{
			Pos /= 16;
			Vector2 Pos2 = new Vector2(Pos.X + (float)width / 16, Pos.Y + (float)height / 16);
			if (Pos.X < 0)
				Pos.X = 0;
			if (Pos.Y < 0)
				Pos.Y = 0;
			if (Pos2.X > Main.maxTilesX)
				Pos2.X = Main.maxTilesX;
			if (Pos2.Y > Main.maxTilesY)
				Pos2.Y = Main.maxTilesY;
			tiles = new List<Vector2>();
			bool collide = false;
			for (int w = 0; w <= (int)Pos2.X - (int)Pos.X; w++)
			{
				for (int h = 0; h <= (int)Pos2.Y - (int)Pos.Y; h++)
				{
					if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].HasTile && Main.tileSolidTop[Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TileType])
					{
						if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].IsHalfBlock)
						{
							if (Pos2.Y > (int)Pos.Y + h + 0.5f)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TopSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].TopSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].LeftSlope)
						{
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].BottomSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
						}
						else if (Main.tile[(int)Pos.X + w, (int)Pos.Y + h].BottomSlope && Main.tile[(int)Pos.X + w, (int)Pos.Y + h].RightSlope)
						{
							if ((int)Pos.X + w > Pos.X && (int)Pos.X + w < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h > Pos.Y && (int)Pos.Y + h < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
							if ((int)Pos.X + w + 1 > Pos.X && (int)Pos.X + w + 1 < Pos2.X && (int)Pos.Y + h + 1 > Pos.Y && (int)Pos.Y + h + 1 < Pos2.Y)
							{
								tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
								collide = true;
							}
						}
						else
						{
							tiles.Add(new Vector2((int)Pos.X + w, (int)Pos.Y + h));
							collide = true;
						}
					}
				}
			}
			return collide;
		}
		/// <summary>
		/// finds hypotinuse and cathet length
		/// </summary>
		/// <param name="VectorFromPoint1ToPoint3WithUnknownLenght">hypotiuse</param>
		/// <param name="KnownVectorFromPoint1ToPoint2">cathet</param>
		/// <returns></returns>
		public static float[] RightTriangle(Vector2 VectorFromPoint1ToPoint3WithUnknownLenght, Vector2 KnownVectorFromPoint1ToPoint2)
		{
			Vector2 V1 = VectorFromPoint1ToPoint3WithUnknownLenght;
			Vector2 V2 = KnownVectorFromPoint1ToPoint2;
			Vector2 V3 = new Vector2(V1.Y, V1.X);
			float[] sides = new float[3];
			sides[0] = V2.Length();
			float angle = AngleBetweenVectors(V1, V2);
			sides[1] = (float)Math.Cos(angle) * V2.Length();
			angle = AngleBetweenVectors(V3, V2);
			sides[2] = (float)Math.Cos(angle) * V2.Length();
			return sides;
		}
		/// <summary>
		/// A and B is Vertices on hipotinuse
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="a">length from B to C</param>
		/// <returns>returns unknown point on right angle</returns>
        public static Vector2 RightTriangle(Vector2 A, Vector2 B, float a)
        {
			Vector2 start = A;
			float rotation = (B - A).ToRotation();
			A -= start;
			B -= start;
			B.RotateBy(-rotation);
			float c = B.X;
			float b = MathF.Sqrt(c * c - a * a);
			float angle = MathF.Tan(a / b);
			Vector2 C = A + Vector2.UnitX.RotatedBy(angle) * b;
			C.RotateBy(rotation);
			C += start;
			return C;
        }

        public static float AngleFromVector(Vector2 V)
		{
			V.Normalize();
			float angle = (float)Math.Acos(V.X);
			if (V.Y < 0)
				angle *= -1;
			return angle;
		}
		public static float AngleBetweenVectors(Vector2 V1, Vector2 V2)
		{
			return AngleFromVector(V1) - AngleFromVector(V2);
		}
		public static Vector2 UnitVectorFromRotation(float rotation)
		{
			return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
		}
		public static float NormalizeRotation(float rotation, bool to2pi = true)
		{
			int i = rotation < 0? -1 : 0;
				//Chatic("start");
				//Chatic(rotation);
    //            Chatic(rotation / ((float)Math.PI));
    //            Chatic(rotation / ((float)Math.PI * 2));
    //            Chatic(rotation / ((float)Math.PI * 2) + i);
    //            Chatic((int)(rotation / ((float)Math.PI * 2) + i));
    //            Chatic((int)(rotation / ((float)Math.PI * 2) + i) * (float)Math.PI);
    //            Chatic((int)(rotation / ((float)Math.PI * 2) + i) * (float)Math.PI * 2);
            rotation -= (int)(rotation / (MathF.PI * 2) + i) * MathF.PI * 2;
				//Chatic(rotation);
			if (!to2pi)
			{
                if (rotation > Math.PI)
				{
					rotation -= MathF.PI * 2;
                    //Chatic(rotation, true);
                }
                //Chatic(rotation, "End");
            }
			return rotation;
		}
        public static float NormalizeRotation(ref float rotation, bool to2pi = true)
        {
            int i = rotation < 0 ? -1 : 0;
            if (to2pi)
            {
                //Chatic("start");
                //Chatic(rotation);
                //            Chatic(rotation / ((float)Math.PI));
                //            Chatic(rotation / ((float)Math.PI * 2));
                //            Chatic(rotation / ((float)Math.PI * 2) + i);
                //            Chatic((int)(rotation / ((float)Math.PI * 2) + i));
                //            Chatic((int)(rotation / ((float)Math.PI * 2) + i) * (float)Math.PI);
                //            Chatic((int)(rotation / ((float)Math.PI * 2) + i) * (float)Math.PI * 2);
                rotation -= (int)(rotation / ((float)Math.PI * 2) + i) * (float)Math.PI * 2;
                //Chatic(rotation);
            }
            else
            {
                //Chatic(rotation);
                int sign = rotation > 0 ? 1 : -1;
                //Chatic(sign);
                rotation *= sign;
                //Chatic(rotation);
                rotation -= (int)(rotation / ((float)Math.PI * 2f)) * (float)Math.PI * 2f;
                //Chatic(rotation);
                if (rotation > Math.PI)
                {
                    rotation = (float)Math.PI - rotation;
                    //Chatic(rotation, true);
                }
                rotation *= sign;
                //Chatic(rotation, "End");
            }
            return rotation;
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width, Texture2D texture = null)
        {
            if (!(start == end))
            {
                start -= Main.screenPosition;
                end -= Main.screenPosition;
                float rotation = (end - start).ToRotation();
                Texture2D value = texture?? ExtraTextureRegistry.WhitePixel.Value;
                Vector2 scale = new Vector2(width / value.Size().X, Vector2.Distance(start, end) / value.Size().Y);
                Vector2 origin = new Vector2((float)value.Width * 0.5f, 0);
                spriteBatch.Draw(value, start, null, color, rotation - MathF.PI / 2, origin, scale, SpriteEffects.None, 0f);
            }
        }
        public static float EasingInOut(float timeMax, float time, bool from_0_to_1_to_0 = false, float breakingPoint = 0.5f)
        {
			float num = from_0_to_1_to_0? 0 : 1;
            float num1 = time / timeMax;

            if (num1 <= breakingPoint)
            {
				num = EasingIn(breakingPoint, num1) * breakingPoint;
            }
            else if (num1 < 1)
            {
				num = breakingPoint + EasingOut(1 - breakingPoint, num1 - breakingPoint) * (1 - breakingPoint);
            }
            if (from_0_to_1_to_0)
            {
                if (num1 < breakingPoint)
                {
                    num = EasingIn(breakingPoint, num1);
                }
                else if (num1 < 1)
                {
                    num = 1 - EasingIn(1 - breakingPoint, num1 - breakingPoint);
                }
                else
                {
                    num = 0;
                }
            }
            if (num1 < 0)
            {
                num = 0;
            }
            return num;
        }
        public static float EasingIn(float timeMax, float time)
        {
            float num = 1;
			if (timeMax != 0)
			{
				float num1 = time / timeMax;
				if (num1 < 0)
				{
					num = 0;
				}
				else if (num1 < 1)
				{
					num = num1 * num1;
				}
			}
            return num;
        }
        public static float EasingOut(float timeMax, float time)
        {
            float num = 1;
            float num1 = time / timeMax;
            if (num1 < 0)
            {
                num = 0;
            }
            else if (num1 < 1)
            {
                num = (1 - num1) * num1 * 2 + num1 * num1;
            }
            return num;
        }
        public static void Chatic(params object[] obj)
		{
			string Text = "";
			foreach (object obj2 in obj)
			{
				if (Text != "")
				{
					Text += ", ";
				}
				Text += obj2.ToString();
			}
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(Text), Color.AliceBlue);
		}
        public static void Tip(string key, params object[] obj)
        {
			if (!ClientConfig.Instance.Tips)
			{
				return;
			}
            string Text = "";
            foreach (object obj2 in obj)
            {
                if (Text != "")
                {
                    Text += ", ";
                }
                Text += obj2.ToString();
            }
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(NetworkText.FromKey("Mods.Terrapain.Tips.Tip").ToString() + " " + NetworkText.FromKey("Mods.Terrapain.Tips." + key, obj)), Color.AliceBlue);
        }
        public static bool CheckActiveAccessories(Player player)
		{
			for (int i = 0; i < 7; i++)
			{
				Item item = player.armor[i + 3];
				if (item.active && ((item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem != null && (!item.GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityUnarmedOnly || player.GetModPlayer<TerrapainPlayer>().unarmed)) || item.GetGlobalItem<TGlobalItem>().dashAccessory))
                    return true;
            }
			return false;
		}
        public static List<Item> GetActiveAccessories(Player player)
        {
			List<Item> items = new List<Item>();
            for (int i = 0; i < 7; i++)
            {

				if (player.armor[i + 3].active && player.armor[i + 3].GetGlobalItem<TGlobalItem>().activeAccessory && (!player.armor[i + 3].GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.AbilityUnarmedOnly || player.GetModPlayer<TerrapainPlayer>().unarmed))
				{
					items.Add(player.armor[i + 3]);
				}
            }
            return items;
        }
        public static void AuraHoldPlayer(float radius, Vector2 Source)
		{
			foreach(Player p in Main.player)
			{
				if (p.active && !p.dead)
				{
					if ((p.position - Source).Length() > radius || (p.TopRight - Source).Length() > radius || (p.BottomRight - Source).Length() > radius || (p.BottomLeft - Source).Length() > radius)
					{
						if (AngleFromVector(p.Center - Source) >= 0 && AngleFromVector(p.Center - Source) < 0.5 * (float)Math.PI)
						{
							p.BottomRight = (p.BottomRight - Source) / (p.BottomRight - Source).Length() * radius + Source;
							p.velocity = p.velocity.RotatedBy(AngleFromVector(p.BottomRight - Source));
							if (p.velocity.X < 0)
							{
								p.velocity.X = 0;
							}
							p.velocity = p.velocity.RotatedBy(-AngleFromVector(p.BottomRight - Source));
						}
						if (AngleFromVector(p.Center - Source) >= 0.5 * (float)Math.PI && AngleFromVector(p.Center - Source) < (float)Math.PI)
						{
							p.BottomLeft = (p.BottomLeft - Source) / (p.BottomLeft - Source).Length() * radius + Source;
							p.velocity = p.velocity.RotatedBy(AngleFromVector(p.BottomLeft - Source));
							if (p.velocity.X < 0)
							{
								p.velocity.X = 0;
							}
							p.velocity = p.velocity.RotatedBy(-AngleFromVector(p.BottomLeft - Source));
						}
						if (AngleFromVector(p.Center - Source) < 0 && AngleFromVector(p.Center - Source) >= -0.5f * (float)Math.PI)
						{
							p.TopRight = (p.TopRight - Source) / (p.TopRight - Source).Length() * radius + Source;
							p.velocity = p.velocity.RotatedBy(AngleFromVector(p.TopRight - Source));
							if (p.velocity.X < 0)
							{
								p.velocity.X = 0;
							}
							p.velocity = p.velocity.RotatedBy(-AngleFromVector(p.TopRight - Source));
						}
						if (AngleFromVector(p.Center - Source) < -0.5f * (float)Math.PI && AngleFromVector(p.Center - Source) >= -(float)Math.PI)
						{
							p.position = (p.position - Source) / (p.position - Source).Length() * radius + Source;
							p.velocity = p.velocity.RotatedBy(AngleFromVector(p.position - Source));
							if (p.velocity.X < 0)
							{
								p.velocity.X = 0;
							}
							p.velocity = p.velocity.RotatedBy(-AngleFromVector(p.position - Source));
						}
					}
				}
			}
		}
		public static bool CheckBoss()
		{
			bool boss = false;
			foreach (NPC n in Main.npc)
			{
				if (n.active && n.boss)
				{
					boss = true;
				}
			}
			return boss;
		}
	}
}