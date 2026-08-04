using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Config;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using static Terrapain.Content.Functions;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using ILGPU.Runtime.Cuda;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
	public class BrainOfCthulhu : NPCBehaviour
	{
        public override int type => NPCID.BrainofCthulhu;
        public override void ModSetDefaults(NPC entity)
        {
            entity.dontTakeDamage = false;
			var t = entity.GetT();
			t.drawCenter = new Vector2(100, 66);
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
				phase = 4;
			}
		}
        public int phase = 1;
        public int attack;
        public int[] attacks1 = [0, 1, 0, 2];
        public int attackCounter;
        public int timer;
        public int mainTimerMax;
        public int MainTimer;
        public float progress;

		bool limbo => phase == 4;
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

        public float maxSpeed;
        public float acceleration;
        public int openTimer;
        public override void FindFrame(NPC npc, int frameHeight)
        {
            int frame = (npc.frame.Y / frameHeight) % 4;
            npc.frame.Y = frame * frameHeight;
            if (openTimer > 0)
            {
                openTimer--;
                npc.frame.Y += 4 * frameHeight;
            }
        }
        
		public override bool ModPreAI(NPC npc)
		{
            if (npc.immune[0] > 0)
            {

            }

            npc.TargetClosest();
            npc.immortal = false;
            maxSpeed = 10;
            acceleration = 0.1f;
			var t = npc.GetT();
			switch (phase)
            {
                case 1:
                    DoFirstPhase(npc);
                    break;
                case 4:
                    DoLimbo(npc);
                    break;
            }
            if (MainTimer > 0)
            {
                MainTimer--;
            }
            progress = 1 - (float)MainTimer / mainTimerMax;
            if (timer > 0 && ! limbo)
            {
                timer--;
            }
            Rotating(npc);
            if (float.IsNaN(npc.position.X))
            {
                npc.active = false;
            }
			return false;
		}
        float angularVelocity;
        public void Rotating(NPC npc)
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
            AngularAcceleration(ref angularVelocity, 0.005f, 0.05f, targetRotation, ref npc.rotation);
        }
        public void Movement(NPC npc)
        {
            Vector2 TargetPosition = t.Target.Center + npc.Center.DirectionFrom(t.Target.Center) * 350;
            Movement(npc, TargetPosition);
        }
        public void Movement(NPC npc, Vector2 TargetPosition)
        {
            CommonTerrapainFlyingMovement(npc, TargetPosition, MathF.PI * 0.15f, maxSpeed, acceleration, 75);
        }
        public void DoFirstPhase(NPC npc)
        {
            switch (attack)
            {
                case 0:
                    Movement(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 1:
                    Vector2 TargetPosition = t.Target.Center + npc.Center.DirectionFrom(t.Target.Center).RotatedBy(0.15f * npc.ai[0]) * 350;
                    Movement(npc, TargetPosition);
                    if (timer == 0)
                    {
                        int _timer = 60;
                        if (WorldDifficultySystem.suicide)
                        {
                            float velocity = 18;
                            int count = 2;
                            float distance = 20;
                            Vector2 direction = random.NextVector2Unit() * distance;
                            int oldnpc = -1;
                            for (int i = 0; i < count; i++)
                            {
                                string context = i == count - 1? "MakeGroup" : null;
                                oldnpc = NewNPC(npc.GetSource_FromThis(context), npc.Center + direction, npc.DirectionTo(t.Target.Center) * velocity, NPCID.Creeper, 0, oldnpc);
                                direction.RotateBy(MathF.PI / count);
                            }
                        }
                        else
                        {
                            float velocity = 17;
                            NewNPC(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * velocity, NPCID.Creeper);
                        }
                        timer = _timer - (int)(progress * _timer / 2);
                        openTimer = 12;
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 2:
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        public void SetMainTimer(int time)
        {
            MainTimer = time;
            mainTimerMax = time;
        }
        public void NextAttack1(NPC npc)
        {
            attackCounter++;
            {
                if (attackCounter >= attacks1.Length)
                {
                    attackCounter = 0;
                }
            }
            attack = attacks1[attackCounter];
            switch (attack)
            {
                case 0:
                    SetMainTimer(200);
                    break;
                case 1:
                    SetMainTimer(400);
                    if (WorldDifficultySystem.suicide)
                    {
                        npc.ai[0] = random.NextDir() * 1.2f;
                    }
                    else
                    {
                        npc.ai[0] = 1;
                    }
                    break;
            }
        }
		public void DoLimbo(NPC npc)
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
                        if (closest == -1 || distance < closest)
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
                    Tip("BrainOfCthulhuTip");
                }
                npc.Center = t.Target.Center;
                animation++;
                if (animation > 59)
                {
                    float rotation = NormalizeRotation(Radians[index] + (animation - 59) * 2f / 350f * MathF.PI, false);
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
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return !limbo && npc.immune[player.whoAmI] == 0;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return !limbo;
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return !limbo && (projectile.owner == -1 || npc.immune[projectile.owner] == 0);
        }
        public override void DrawBehind(NPC npc, int index)
        {
            if (limbo)
            {
                Main.instance.DrawCacheNPCsOverPlayers.Add(index);
            }
            else
            {
                Main.instance.DrawCacheNPCProjectiles.Add(index);
            }
        }
        public override bool ModPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
        {
            npc.hide = true;
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
                            Vector2 endPos = UnitVectorFromRotation(Radians[i]) * 500;
                            position = startPos + (endPos - startPos) * EasingInOut(59, animation) + t.Target.Center;
							//Functions.Chatic(animation);
                        }
						else if(animation >= 60 )
						{
                            Vector2 endPos = UnitVectorFromRotation(Radians[i] + (animation - 59 + ((float)choose / 20f) * ((float)choose / 20f)) * 2f / 350f * MathF.PI) * (500 - choose / 2f);
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
