using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.CameraModifiers;
using Terrapain.Common.Config;
using Terrapain.Common.Global.TGlobalNPCs;
using Terrapain.Common.System;
using Terrapain.Content.Projectiles.Enemies.Bosses.EvilBosses;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

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
            entity.knockBackResist = 0;
            segmentsLifes = [];
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
        public int[] attacks1 = [0, 1, 0, 2, 0, 3];
        public int[] attacks2 = [0, 1, 0, 2, 0, 3];
        public int attackCounter;
        public int timer;
        public int mainTimerMax;
        public int MainTimer;
        public float progress;
        public static List<int> segmentsLifes;

        int ichorSpike => ModContent.ProjectileType<IchorSpike>();
        int ichoreSpikeDamage = 25;
        float ichoreSpikeKnockback = 10;

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
        int frame;
        int animTimer;
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (animTimer <= 0)
            {
                frame = (frame + 1) % 4;
                animTimer = 8;
            }
            animTimer--;
            npc.frame.Y = frame * frameHeight;
            if (openTimer > 0)
            {
                openTimer--;
                npc.frame.Y += 4 * frameHeight;
            }
        }
        
		public override bool ModPreAI(NPC npc)
		{
            if (EaterOfWorldsRealeseAnimation)
            {
                if (timer == 0)
                {
                    SoundEngine.PlaySound(npc.HitSound, npc.Center);
                    npc.velocity = random.NextVector2Unit(-0.6f, 0.6f) * 20;
                    npc.velocity *= random.NextDir();
                    for (int i = 0; i < 25; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X * 0.75f, npc.velocity.Y * 0.75f, Scale: 2);
                    }
                    timer = 60 - (int)(progress * 20);
                }
                CommonTerrapainFlyingMovement(npc, new Vector2(npc.ai[0], npc.ai[1]), 0.15f, 5, 0.375f, 70, false);
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, Scale: 2);
                if (MainTimer == 0)
                {
                    EaterOfWorldsRealeseAnimation = false;
                    SetMainTimer(400);
                }
            }
            else
            {
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
                    case 2:
                        DoSecondPhase(npc);
                        break;
                    case 3:
                        break;
                    case 4:
                        DoLimbo(npc);
                        break;
                }
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
                    CheckPhase(npc);
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
                        int _timer = 80;
                        if (WorldDifficultySystem.suicide)
                        {
                            float velocity = 25;
                            int count = 2;
                            float distance = 20;
                            Vector2 direction = random.NextVector2Unit() * distance;
                            int oldnpc = -1;
                            for (int i = 0; i < count; i++)
                            {
                                string context = i == count - 1? "MakeGroup" : null;
                                oldnpc = NewNPC(npc.GetSource_FromThis(context), npc.Center + direction, npc.DirectionTo(t.Target.Center) * velocity, NPCID.Creeper, 0, oldnpc, 0, 60);
                                direction.RotateBy(MathF.PI / count);
                            }
                        }
                        else
                        {
                            float velocity = 25;
                            NewNPC(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * velocity, NPCID.Creeper, 0, 0, 0, 60);
                        }
                        timer = _timer - (int)(progress * _timer / 2);
                        openTimer = 12;
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                    CheckPhase(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 2:
                    int time = 180;
                    if (timer == 0)
                    {
                        Vector2 dir = npc.DirectionTo(t.Target.Center);
                        npc.velocity = dir * 35;
                        npc.ai[0] *= -1;
                        int count = 5;
                        float speed = 18;
                        int _time = 20;
                        float startAngle = MathF.PI / 2;
                        if (count % 2 == 0)
                        {
                            startAngle += MathF.PI / count;
                        }
                        float angleBetween = MathF.PI * 2 / count;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, (startAngle + angleBetween * i).ToRotationVector2() * speed, ichorSpike, ichoreSpikeDamage, ichoreSpikeKnockback, -1, _time, dir.X, dir.Y);
                        }
                        timer = time;
                    }
                    if (timer > time - 60)
                    {
                        npc.velocity = npc.velocity.Normalized() * (npc.velocity.Length() - 0.2f);
                    }
                    else
                    {
                        CheckPhase(npc);
                        float p = 1 - MathF.Min(timer / 60f, 1);
                        TargetPosition = t.Target.Center + npc.Center.DirectionFrom(t.Target.Center).RotatedBy(0.15f * npc.ai[0] * (1 - p)) * (350 + p * 65);
                        Movement(npc, TargetPosition);
                    }
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
                case 3:
                    if (timer == 0)
                    {
                        time = 60;
                        Vector2 dir = npc.DirectionTo(t.Target.Center);
                        int count = 5;
                        float speed = 18;
                        int _time = 15;
                        float startAngle = MathF.PI / 2;
                        if (count % 2 == 0)
                        {
                            startAngle += MathF.PI / count;
                        }
                        float angleBetween = MathF.PI * 2 / count;
                        for (int i = 0; i < count; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, (startAngle + angleBetween * i).ToRotationVector2() * speed, ichorSpike, ichoreSpikeDamage, ichoreSpikeKnockback, -1, _time, dir.X, dir.Y);
                        }
                        timer = time;
                    }
                    float p1 = MathF.Min(mainTimerMax - MainTimer, 40) / 40;
                    TargetPosition = t.Target.Center + npc.Center.DirectionFrom(t.Target.Center).RotatedBy(0.15f * npc.ai[0] * (1 + progress)) * (350 + p1 * 350);
                    Movement(npc, TargetPosition);
                    CheckPhase(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack1(npc);
                    }
                    break;
            }
        }
        public void DoSecondPhase(NPC npc)
        {
            switch (attack)
            {
                case -1:
                    Movement(npc, new Vector2(npc.ai[0], npc.ai[1]));
                    if (MainTimer == mainTimerMax - 1)
                    {
                        int eow = NewNPC(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * 20, NPCID.EaterofWorldsHead, 0, 80, -1);
                        EaterofWorldsHead.Restart(eow);
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                    openTimer = 12;
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    break;
                case 0:
                    Movement(npc);
                    CheckPhase(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    break;
                case 1:
                    if (timer == 0 && segmentsLifes.Count > 0)
                    {
                        openTimer = 12;
                        int num = 3;
                        if (segmentsLifes.Count < 6)
                        {
                            num = segmentsLifes.Count;
                        }
                        int eow = NewNPC(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * 20, NPCID.EaterofWorldsHead, 0, num, -1);
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                        timer = 60;
                    }

                    Movement(npc);
                    CheckPhase(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    break;
                case 2:
                    Movement(npc, new Vector2(npc.ai[0], npc.ai[1]));
                    if (MainTimer == mainTimerMax - 1)
                    {
                        int eow = NewNPC(npc.GetSource_FromThis(), npc.Center, npc.DirectionTo(t.Target.Center) * 20, NPCID.EaterofWorldsHead, 0, segmentsLifes.Count, -1);
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    }
                    openTimer = 12;
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    break;
                case 3:
                    Movement(npc);
                    CheckPhase(npc);
                    if (MainTimer == 0)
                    {
                        NextAttack2(npc);
                    }
                    break;
            }
        }
        public bool EaterOfWorldsRealeseAnimation;
        public bool CheckPhase(NPC npc)
        {
            if (phase == 1 && npc.life < npc.lifeMax * 0.6f)
            {
                attack = -1;
                phase = 2;
                EaterOfWorldsRealeseAnimation = true;
                attackCounter = 0;
                SetMainTimer(600);
                npc.ai[0] = npc.Center.X;
                npc.ai[1] = npc.Center.Y;
                Main.instance.CameraModifiers.Add(new SmoothMoovingCameraModifier() { AimTime = 30, OriginalCameraPosition = Main.screenPosition, TotalTime = MainTimer - 120, StartZoom = Main.GameZoomTarget, TargetZoom = ClientConfig.Instance.CutsceneCameraZoom, hideUI = ClientConfig.Instance.CutsceneHideUI });
                SmoothMoovingCameraModifier.TargetPosition = npc.Center - Main.ScreenSize.ToVector2() / 2;
                SmoothMoovingCameraModifier.Timer = 0;
                return true;
            }
            return false;
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
                case 2:
                    SetMainTimer(600);
                    if (WorldDifficultySystem.suicide)
                    {
                        npc.ai[0] = random.NextDir() * 1.2f;
                    }
                    else
                    {
                        npc.ai[0] = -1;
                    }
                    break;
                case 3:
                    SetMainTimer(480);
                    if (WorldDifficultySystem.suicide)
                    {
                        npc.ai[0] = random.NextDir() * 1.2f;
                    }
                    else
                    {
                        npc.ai[0] = -1;
                    }
                    break;
            }
        }
        public void NextAttack2(NPC npc)
        {
            Start:
            attackCounter++;
            {
                if (attackCounter >= attacks2.Length)
                {
                    attackCounter = 0;
                }
            }
            attack = attacks2[attackCounter];
            switch (attack)
            {
                case 0:
                    SetMainTimer(200);
                    break;
                case 1:
                    if (segmentsLifes.Count < 2)
                    {
                        goto Start; 
                    }
                    SetMainTimer(800);
                    break;
                case 2:
                    npc.ai[0] = npc.Center.X;
                    npc.ai[1] = npc.Center.Y;
                    if (segmentsLifes.Count < 2)
                    {
                        goto Start;
                    }
                    SetMainTimer(200);
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
            return (limbo || EaterOfWorldsRealeseAnimation)? false : null;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return !(limbo || EaterOfWorldsRealeseAnimation);
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return (limbo || EaterOfWorldsRealeseAnimation)? false : null;
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
