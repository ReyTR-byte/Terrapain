using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content;
using Terrapain.Common.Config;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using static Terrapain.Content.Functions;

namespace Terrapain.Common.Global.TGlobalNPCs
{
	public class BrainOfCthulhu : NPCBehaviour
	{
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return entity.type == NPCID.BrainofCthulhu;
        }
		bool limbo;
        public override void ModSetDefaults(NPC entity)
        {
			var t = entity.GetT();
			t.drawOffcet = new Vector2(0, 18);
            t.useVanillaDrawing = false;
            t.useModDrawingInPreDraw = true;
			t.canBeHooked = false;
        }
		public override void HitEffect(NPC npc, NPC.HitInfo hit)
		{
			if (npc.life - hit.Damage < 0 && !limbo)
			{
				npc.life = 1;
				npc.immortal = true;
				limbo = true;
			}
		}
		bool drawClons;
		bool Intoduction = true;
		bool Mix;
		bool Break;
		bool End;
		int choose;
		bool wrong;
		int animation;
		int ItntroductionLenth = 80;
		int MixLenth => ClientConfig.Instance.LimboSpeed;
		int BreakLenth = 6;
		int LimboLength = 500; 
		int timer;
		int index;
		int MixStyle;
		int[][] MixStyles =
		{
			[1, 0, 3, 2, 5, 4, 7, 6],
			[3, 2, 1, 0, 7, 6, 5, 4],
			[7, 6, 5, 4, 3, 2, 1, 0],
			[4, 5, 6, 7, 0, 1, 2, 3],
			[5, 4, 7, 6, 1, 0, 3, 2],
			[2, 3, 0, 1, 6, 7, 4, 5],
			[6, 7, 4, 5, 2, 3, 0, 1],
			[1, 2, 3, 4, 5, 6, 7, 0],
			[7, 0, 1, 2, 3, 4, 5, 6],
			[1, 2, 3, 0, 5, 6, 7, 4],
			[4, 5, 6, 7, 1, 2, 3, 0],
			[5, 6, 7, 4, 0, 1, 2, 3],
			[7, 4, 5, 6, 0, 1, 2, 3],
			[4, 5, 6, 7, 3, 0, 1, 2]
		};

		Vector2 StartPos;
		Vector2 EndPos;
		Vector2[] Points = 
		{ 
			new Vector2(250, 375), 
			new Vector2(250, 125), 
			new Vector2(250, -125), 
			new Vector2(250, -375), 
			new Vector2(-250, 375), 
			new Vector2(-250, 125), 
			new Vector2(-250, -125), 
			new Vector2(-250, -375) 
		};

		float[] Radians = 
		{ 
			0.375f * MathF.PI, 
			0.125f * MathF.PI, 
			-0.125f * MathF.PI, 
			-0.375f * MathF.PI, 
			0.625f * MathF.PI, 
			0.875f * MathF.PI, 
			-0.875f * MathF.PI, 
			-0.625f * MathF.PI 
		};

		UnifiedRandom random = new UnifiedRandom();
		
		public override bool ModPreAI(NPC npc)
		{
			var t = npc.GetT(); 
			if (limbo)
			{
				t.fulllight = true;
				if (Intoduction)
				{
					if (animation == 1)
					{
						npc.velocity = Vector2.Zero;
					}
					if (animation == 4)
					{
						npc.TargetClosest();
						StartPos = npc.Center - t.Target.Center;
						float closest = -1;
						for (int i = 0; i < 8; i++)
						{
							Vector2 pos = Points[i];
							float distance = (pos - StartPos).Length();
							if(closest == -1 || distance < closest)
							{
								closest = distance;
								EndPos = pos;
								index = i;
							}
						}
					}
					if (animation >= 4 && animation <= 60)
					{
						npc.Center = StartPos + (EndPos - StartPos) * EasingInOut(57, animation - 3) + t.Target.Center;
					}
					if (animation == 60)
					{
						StartPos = EndPos;
						drawClons = true;
					}
					if (animation > 60)
					{
                        npc.friendly = true;
                        npc.width = 0;
                        npc.height = 0;
                        npc.position = t.Target.Center;
                        t.useModDrawingInPreDraw = false;
                    }
					if (animation == ItntroductionLenth)
					{
						animation = 0;
                        Intoduction = false;
						Mix = true;
                    }
					animation++;
				}
				else
				{
					npc.friendly = true;
					npc.width = 0;
					npc.height = 0;
					npc.position = t.Target.Center;
					t.useModDrawingInPreDraw = false;
				}
				if (Mix)
				{
					if (animation == 1)
					{
						MixStyle = random.Next(MixStyles.Length);
						EndPos = Points[MixStyles[MixStyle][index]];
					}
					//npc.Center = StartPos + (EndPos - StartPos) * EasingInOut(MixLenth, animation) + Target.Center;
					if (animation == MixLenth)
					{
						StartPos = EndPos;
						animation = 0;
						Mix = false;
						Break = timer < LimboLength;
						End = !Break;
						index = MixStyles[MixStyle][index];
					}
					animation++;
					timer++;
				}
				if (Break)
				{
					npc.Center = t.Target.Center;
					if (animation == BreakLenth)
					{
						animation = 0;
						Break = false;
						Mix = true;
					}
					animation++;
                    timer++;
                }
				if (End)
				{
					if (animation == 1)
					{
						Functions.Tip("BrainOfCthulhuTip");
					}
                    npc.Center = t.Target.Center;
                    animation++;
					if (animation > 59)
					{
						float rotation = Functions.NormalizeRotation(Radians[index] + (animation - 59) * 2f / 350f * MathF.PI, false);
						rotation *= -1;
						if (choose > 0)
						{
							choose++;
						}
						if (t.Target.controlJump && choose == 0)
						{
							choose = 1;
							if (rotation >= (0.5f - 0.125f) * MathF.PI && rotation <= (0.5f + 0.125f) * MathF.PI)
							{
								//Functions.Chatic("Right");
								wrong = false;
							}
							else 
							{
								wrong = true;
							}
							//Functions.Chatic(wrong);
						}
						if (choose >= 1000)
						{
							if (wrong)
							{
								PlayerDeathReason reason = new PlayerDeathReason();
								reason.SourceNPCIndex = npc.whoAmI;
								t.Target.KillMe(reason, 989898121212222, 1);
								npc.active = false;
							}
							else
							{
								npc.immortal = false;
								npc.StrikeInstantKill();
							}
						}
					}
				}
				return false;
			}
			return true;
		}
        public override bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
			if (drawClons)
			{
				for (int i = 0; i < 8; i++)
				{
					Vector2 position = Vector2.Zero;
					if (Mix)
					{
						Vector2 startPos = Points[i];
						Vector2 endPos = Points[MixStyles[MixStyle][i]];
                        position = startPos + (endPos - startPos) * EasingInOut(MixLenth, animation) + t.Target.Center;
					}
					else if(!End)
					{
						position = Points[i] + t.Target.Center;
                    }
					else
					{
						if (animation < 60)
						{
                            Vector2 startPos = Points[i];
                            Vector2 endPos = Functions.UnitVectorFromRotation(Radians[i]) * 500;
                            position = startPos + (endPos - startPos) * EasingInOut(59, animation) + t.Target.Center;
							//Functions.Chatic(animation);
                        }
						else if(animation >= 60 )
						{
                            Vector2 endPos = Functions.UnitVectorFromRotation(Radians[i] + (animation - 59 + ((float)choose / 20f) * ((float)choose / 20f)) * 2f / 350f * MathF.PI) * (500 - choose / 2f);
                            position = endPos + t.Target.Center;
                        }
					}
					Vector2 DrawCenter = t.drawCenter;
                    if (npc.spriteDirection == -1)
                    {
                        DrawCenter.X = texture.Width - DrawCenter.X;
                    }
                    Color color = Color.White;
					if (Intoduction && animation > 60 && animation < 80 && i == index)
					{
						Vector4 _Color = Vector4.One * 255;
						_Color -= Color.Purple.ToVector4() * 255f * EasingInOut(19, animation - 60, true);
						color.R = (byte)_Color.X;
                        color.G = (byte)_Color.Y;
                        color.B = (byte)_Color.Z;
                    }
					if (End && animation > 15 && choose == 0)
					{
                        Vector4 _Color = Vector4.One * 255;
						float rotation = Functions.NormalizeRotation(Radians[i] + (animation - 59) * 2f / 350f * MathF.PI, false);
						rotation *= -1;
                        _Color -= Color.Purple.ToVector4() * 255f * EasingInOut(0.2f * MathF.PI, rotation - 0.4f * MathF.PI, true);
                        color.R = (byte)_Color.X;
                        color.G = (byte)_Color.Y;
                        color.B = (byte)_Color.Z;
						//if (_Color != Vector4.One * 255)
						//{
						//	Functions.Chatic(Functions.NormalizeRotation(Radians[i] + (animation - 15) * 2f / 350f * MathF.PI, false), rotation);
						//}
                    }
					color = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16), color);
                    color.A = (byte)(255 - npc.alpha);
                    Vector2 textureCenter = npc.frame.Size() * 0.5f;
                    Main.EntitySpriteDraw(texture, position - Main.screenPosition + t.drawOffcet, npc.frame, color, npc.rotation, textureCenter, 1, npc.spriteDirection * t.textureDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                }
			}
			return false;
        }
	}
}
