using Humanizer;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Runtime.Serialization;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.Dusts;
using Terrapain.Content.Items.Bags;
using Terrapain.Content.Items.DropRulls;
using Terrapain.Content.Items.Placeable.Trophies;
using Terrapain.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Terrapain.Content.NPCs.Bosses.Scorspider
{
	[AutoloadBossHead]
	public class ScorspiderBody : ModNPC
	{
		public static int prisonWidth;
		public static int prisonHeight;
		public static Vector2 PrisonPosition;
		public static bool prisoned;
		UnifiedRandom rand = new UnifiedRandom();
		public bool tailUp;
		public bool sit;
		private int head;
		private int sting;
		private int[] leggs = new int[8];
        private List<int> tails = new List<int>();
        int damage = 20;

		private int state
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private int subState
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}
		private float timer
		{
			get => NPC.ai[2];
			set => NPC.ai[2] = value;
		}
		private int timer2
		{
			get => (int)NPC.ai[3];
			set => NPC.ai[3] = value;
		}

		private bool secondPhase => state == 1;
		private bool thirdPhase => state == 2;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
			{
				PortraitScale = 0.6f,
				PortraitPositionYOverride = 0f
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
		}
		public override void SetDefaults()
		{
			NPC.width = 140;
			NPC.height = 80;

			NPC.damage = damage;
			NPC.defense = 20;

			NPC.lifeMax = 6500;

			NPC.knockBackResist = 0f;

			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.value = 750000;

			NPC.noTileCollide = false;
			NPC.noGravity = false;

			NPC.aiStyle = -1;
			NPC.stairFall = true;
			NPC.HitSound = SoundID.NPCHit4;
		}
		public override void OnSpawn(IEntitySource source)
		{
			ShaderSystem.ScorspiderTimer = 20;
			ShaderSystem.drawScorspiderBorders = false;
			ShaderSystem.ScorspiderAuraTimer = 20;
			ShaderSystem.drawScorspiderAura = false;
			head = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI);
			sting = NPC.NewNPC(source, (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, head);
			Main.npc[head].ai[1] = sting;

			for (int leg = 0; leg < leggs.Length; leg++)
			{
				leggs[leg] = Projectile.NewProjectile(source, NPC.position, Vector2.Zero, ModContent.ProjectileType<ScorspiderLeg>(), 0, 1, -1, leg % 2);
			}

			timer = 300;
			timer2 = 60;

			if (Main.GameMode != 3)
			{
				NPC.lifeMax = 6500 * (Main.GameMode + 3) / 3;
				NPC.life = 6500 * (Main.GameMode + 3) / 3;
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Ingredients.ScorspiderShellShard>(), 1, 15, 20));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Ingredients.ScorspiderCobweb>(), 1, 15, 20));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Tools.ScorspiderHook>(), 3));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Accessories.ScorspiderHeartAccesory>(), 3));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Weapons.MeleeWeapons.Sharper>(), 3));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Weapons.MagicWeapons.GranithBook>(), 3));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ScorspiderBag>()));
			
			LeadingConditionRule suicide = new(new SuicideDropRule());
			suicide.OnSuccess(new DropOneByOne(ModContent.ItemType<ScorspiderTrophy>(), Terrapain.SuicideTrophyDropParameters));
			npcLoot.Add(suicide);

			LeadingConditionRule notSuicide = new(new NotSuicideDropRule());
			notSuicide.OnSuccess(new DropOneByOne(ModContent.ItemType<ScorspiderTrophy>(), Terrapain.NormalTrophyDropParameters));
			npcLoot.Add(notSuicide);

            LeadingConditionRule masterOrTorture = new(new MasterOrTortureDropRule());
			masterOrTorture.OnSuccess(new DropOneByOne(ModContent.ItemType<Items.Placeable.Relics.ScorspiderRelic>(), Terrapain.SuicideTrophyDropParameters));
			npcLoot.Add(masterOrTorture);
        }

		Vector2 headPosition = new Vector2(90, -10);
		Vector2 stingPosition = new Vector2(-160, -40);

		Vector2 headIdlePosition = new Vector2(70, 16);
		Vector2 stingIdlePosition = new Vector2(-160, -40);
		Vector2 stingAttackPosition = new Vector2(-20, -120);

		float idleLeggsHeight = -64f;
		float distBetwLeggs = 30f;
		float firstLeg = 4f;
		Vector2 TailToBody = new Vector2(0, 50f);
		Vector2[] leggsPositions = new Vector2[8];

		int movementState = 0;
		int movementTimer = 0;

		int[] attackLine = [2, 3, 4, 2, 3, 2, 4, 3];
		int[] attackLine2 = [1, 3, 4, 3, 1];
		int[] attackLine3 = [6, 7, 8, 9];
		int attackCount = 0;

		float angularVelocity;
		int oldDirection;
		float speed => (6f + (float)(3 + WorldDifficultySystem.TerrapainDifficulty) / 3 + (float)state / 2) / (subState == 0 ? 1 : (WorldDifficultySystem.suicide ? 1.5f : 1.7f));
		int jumpForce = 15;
		Vector2 offset = Vector2.Zero;
		int timeLeft = 150;
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}

		public override bool? CanFallThroughPlatforms()
		{
			return true;
		}
	
		public override void AI()
		{
			//Functions.Chatic(1);
			if ((!Main.npc[head].active || Main.npc[head].type != ModContent.NPCType<ScorspiderHead>()) && (Main.npc[sting].active && Main.npc[sting].type == ModContent.NPCType<ScorspiderHead>()))
			{
				head = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI, sting);
				Main.npc[sting].ai[1] = head;
			}
			if ((Main.npc[head].active && Main.npc[head].type == ModContent.NPCType<ScorspiderHead>()) && (!Main.npc[sting].active || Main.npc[sting].type != ModContent.NPCType<ScorspiderSting>()))
			{
				sting = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, head);
				Main.npc[head].ai[1] = sting;
			}
			if ((!Main.npc[head].active || Main.npc[head].type != ModContent.NPCType<ScorspiderHead>()) && (!Main.npc[sting].active || Main.npc[sting].type != ModContent.NPCType<ScorspiderSting>()))
			{
				head = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderHead>(), NPC.whoAmI);
				sting = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<ScorspiderSting>(), NPC.whoAmI, head);
				Main.npc[head].ai[1] = sting;
			}

            //Functions.Chatic(2);
            oldDirection = NPC.direction;

			NPC Head = Main.npc[head];
			NPC Sting = Main.npc[sting];

			Head.lifeMax = NPC.lifeMax;
			Head.life = NPC.life;
			Head.spriteDirection = - NPC.direction;

			Sting.lifeMax = NPC.lifeMax;
			Sting.life = NPC.life;
			if (!(subState == 1 || subState == 3 || subState == 9))
				Sting.rotation = NPC.direction == 1 ? 0 : (float)Math.PI;

			Head.ai[0] = NPC.whoAmI;
			Sting.ai[0] = NPC.whoAmI;

            //Functions.Chatic(3);

            Projectile[] Leggs = new Projectile[8];
			for (int i = 0; i < Leggs.Length; i++)
			{
				if (!Main.projectile[leggs[i]].active || Main.projectile[leggs[i]].type != ModContent.ProjectileType<ScorspiderLeg>())
				{
					leggs[i] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, Vector2.Zero, ModContent.ProjectileType<ScorspiderLeg>(), 0, 1, -1, i % 2);
				}
				Leggs[i] = Main.projectile[leggs[i]];
				Leggs[i].timeLeft = 2;
				Leggs[i].velocity = Vector2.Zero;
			}

            //Functions.Chatic(4);

            Vector2 Rotation = Sting.Center - (NPC.position + TailToBody);
			if (NPC.direction == -1)
				Rotation.X -= NPC.width;

			float distance = Rotation.Length();
			Rotation.Normalize();

            //Functions.Chatic(5);

			tails = new List<int>();
            for (int i = 0; i < distance / 30; i++)
			{
				int tail = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.width / 2 * -NPC.direction, -NPC.height / 2) + NPC.Center + TailToBody + Rotation * 30 * i, Vector2.Zero, ModContent.ProjectileType<ScorspiderTail>(), 0, 0);
				Main.projectile[tail].rotation = Functions.AngleFromVector(Rotation);
				tails.Add(tail);
			}

            //Functions.Chatic(6);

            if (!prisoned)
			{
                //Functions.Chatic(7);

                NPC.boss = true;
				NPC.friendly = false;
				Head.friendly = false;
				Sting.friendly = false;
				if (NPC.target == 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
				{
					NPC.TargetClosest();
				}

                //Functions.Chatic(8);

                Player player = Main.player[NPC.target];
				if (movementState != 2)
					NPC.DirectionTo(player.Center + offset - NPC.Center);

                //Functions.Chatic(9);

                if (player.dead || !player.active)
				{
					timeLeft--;
					if (timeLeft == 0)
					{
						NPC.life = 0;
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.Terrapain.BossDespawn.ScorspiderDespawn"), Color.SkyBlue);
					}
				}
				else
				{
					timeLeft = 150;
				}

                //Functions.Chatic(10);

                if (subState == 0)
					timer--;

				if (timer <= 0 && subState == 0)
				{
                    //Functions.Chatic(11);
					if (state != 2)
					{

                        if ((float)NPC.life / NPC.lifeMax < 0.5f)
						{
							if (state == 0)
								attackCount = 0;
							state = 1;
						}
						else
						{
							state = 0;
						}
					}
                    //Functions.Chatic(12);
                    if (secondPhase)
					{
						if (attackCount >= attackLine2.Length)
							attackCount = 0;
						subState = attackLine2[attackCount];
						attackCount++;
						timer = 540;
						if (NPC.life == 1)
							subState = 5;
					}

                    else if (thirdPhase)
					{
						//Functions.Chatic(13);
						if (attackCount >= attackLine3.Length)
							attackCount = 0;
						subState = attackLine3[attackCount];
						attackCount++;
						timer = 540;
					}
					else
					{
                        //Functions.Chatic(14);
                        if (attackCount >= attackLine.Length)
							attackCount = 0;
						subState = attackLine[attackCount];
						attackCount++;
						timer = 540;
					}
				}

                //Functions.Chatic(15);

                switch (state)
				{
					case 0:
						HandleFirstState(player, Head, Sting, Leggs);
						break;
					case 1:
						HandleSecondState(player, Head, Sting, Leggs);
						break;
					case 2:
						player.wingTime = 20;
						HandleThirdState(player, Head, Sting, Leggs);
						break;
				}
                //Functions.Chatic(16);

                Vector2 VectorToTarget = -NPC.Center + player.Center;
				if (/*Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height) ||*/ (VectorToTarget.Length() > 800 && movementState != 1 && shouldJump && movementState != 2) || (VectorToTarget.Length() > 1000 && movementState != 2 && movementState != 1))
				{
                    //Functions.Chatic(17);
                    timer2--;
					if (timer2 <= 0)
					{
						movementState = 1;
						movementTimer = 20;
						timer2 = 60;
					}
				}
				else
					timer2 = 60;
                //Functions.Chatic(18);
                Movment(player, Head, Sting, Leggs);

				foreach (var p in Main.player) 
				{
					if (p.active)
						p.GetModPlayer<PlayerMovement>().ShouldFallThroughtPlatforms = true;
				}
                //Functions.Chatic(19);

                if (NPC.velocity.X * NPC.direction < 0)
				{
					Head.velocity.X += NPC.velocity.X;
				}
			}
			else
			{
                //Functions.Chatic(20);
                NPC.boss = false;
				NPC.friendly = true;
				Head.friendly = true;
				Sting.friendly = true;
				movementState = 2;
			}

            //Functions.Chatic(21);
            if (!ShaderSystem.drawScorspiderBorders && ShaderSystem.ScorspiderTimer < 20)
			{
				ShaderSystem.ScorspiderTimer++;
			}
			if (!ShaderSystem.drawScorspiderAura && ShaderSystem.ScorspiderAuraTimer < 20)
			{
				ShaderSystem.ScorspiderAuraTimer++;
			}
		}
		int direction;
		int count;
		bool CanJump = true;
		bool oldCanJump = true;
		float HorizontalJumpForce;
		int jumpTimer;
		int jumpCount;
		float rotation;
		int WallDirection = 1;
		int oldWallHall = 37;
		Vector2 SpikeVelocity;
		static int secondStageHeadSlot = -1;
		public override void Load()
		{
			// We want to give it a second boss head icon, so we register one
			string texture = BossHeadTexture + "_ThirdStage"; // Our texture is called "ClassName_Head_Boss_SecondStage"
			secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1); // -1 because we already have one registered via the [AutoloadBossHead] attribute, it would overwrite it otherwise
		}
		public override void BossHeadSlot(ref int index)
		{
			int slot = secondStageHeadSlot;
			if (thirdPhase)
			{
				// If the boss is in its second stage, display the other head icon instead
				index = slot;
			}
		}
		int webTimer;
		float buttomOfWall;
		int wallTimer;
		float r;
		Vector2 p;
		UnifiedRandom ur = new UnifiedRandom();
		private void HandleFirstState(Player player, NPC Head, NPC Sting, Projectile[] Leggs)
		{
			switch (subState)
			{
				case 2:
					tailUp = true;
					if (timer > 120)
					{
						if (timer % 80 <= 0)
						{
							float distance = 1;
							int attackStyle = 1;
							int count = (int)Math.Abs(Sting.Center.X - player.Center.X) / 80 + 3 + (WorldDifficultySystem.suicide ? 4 : 0);
							if (attackStyle == 1)
							{
								count += 4;
								distance = 0.8f;
							}
							for (int i = 0; i < count; i++)
							{
								Vector2 projVelocity;
								projVelocity = new Vector2((i + count / 2) * distance * NPC.direction, -15);
								Projectile.NewProjectile(NPC.GetSource_FromAI(), Sting.Center, projVelocity, ModContent.ProjectileType<ScorspiderSpike>(), damage, 2f, -1, i, count, attackStyle);
							}
						}
					}
					else if (timer == 80)
					{
						direction = NPC.direction;
					}
					else if (timer > 1)
					{
						if (Math.Abs(NPC.velocity.X) < 15)
							NPC.velocity.X = 15f * direction;
						if (direction == 1)
						{   
							if (player.Center.X < NPC.Center.X - 100)
							{    
								timer = 0;
							}
						}
						else
						{   
							if (player.Center.X > NPC.Center.X + 100)
							{    
								timer = 0;
							}
						}
					}
					else
					{
						movementState = 1;
						jumpForce = 20;
						movementTimer = 20;
						subState = 0;
						timer = 60;
						tailUp = false;
					}
					timer--;
					break;
				case 3:
					tailUp = true;
					if (WorldDifficultySystem.suicide)
					{
						player.wingTime = 20;
					}
					if (timer == 540)
					{
						timer--;
						Sting.rotation = NPC.direction == 1 ? -0.25f * (float)Math.PI : 1.25f * (float)Math.PI;
					}

					else
					{
						if (NPC.direction > 0)
						{
							if (Sting.rotation > (float)Math.PI)
							{
								angularVelocity += 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity > 2 * (float)Math.PI)
									angularVelocity = 0.0125f * (float)Math.PI;
							}
							else
							{
								angularVelocity -= 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity < 0)
									angularVelocity = -0.0125f * (float)Math.PI;
							}
						}
						else
						{
							if (Sting.rotation > (float)Math.PI)
							{
								angularVelocity -= 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity < (float)Math.PI)
									angularVelocity = -0.0125f * (float)Math.PI;
							}
							else
							{
								angularVelocity += 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity > (float)Math.PI)
									angularVelocity = 0.0125f * (float)Math.PI;
							}
						}
						Sting.rotation += angularVelocity;

						if (NPC.direction != oldDirection)
						{
							angularVelocity *= -1;
							Sting.rotation = (float)Math.PI - Sting.rotation;
						}

						while (Sting.rotation < 0)
							Sting.rotation += (float)Math.PI * 2;
						while (Sting.rotation > Math.PI * 2)
							Sting.rotation -= (float)Math.PI * 2;

						if (timer % 10 == 0)
						{
							Vector2 velocity = new Vector2((float)Math.Cos(Sting.rotation), (float)Math.Sin(Sting.rotation)) * 20 * (WorldDifficultySystem.suicide ? 1.5f : 1);
							Projectile.NewProjectile(NPC.GetSource_FromAI(), Sting.Center, velocity, ModContent.ProjectileType<ScorspiderSpike>(), damage, 2, -1, angularVelocity >= 0 ? 0 : -1);
						}

						if (timer % 4 == 0)
						{
							Random rand = new Random();
							int direction = rand.Next(1);
							if (direction == 0)
								direction--;
							Vector2 velocity = new Vector2(direction * rand.NextSingle(), -20);
							Vector2 position;
							position.X = NPC.position.X + rand.Next(NPC.width);
							position.Y = NPC.position.Y;
							Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<ScorspiderShellShard>(), damage * 2, 2);
						}

						timer--;

						if (timer == 0)
						{
							subState = 0;
							timer = 60;
							tailUp = false;
						}
					}
					break;
				case 4:
					tailUp = true;
					timer--;
					if (timer % 70 == 0)
					{
						if (count < 3 + WorldDifficultySystem.TerrapainDifficulty)
						{
							NPC.NewNPC(NPC.GetSource_FromAI(), (int)Sting.Center.X, (int)Sting.Center.Y, ModContent.NPCType<ScorspiderLittleMinionSpidersCocoon>(), 0, 20 * Sting.DirectionTo(player.Center).X, 20 * Sting.DirectionTo(player.Center).Y);
							count++;
						}
					}
					if (timer <= -60)
					{
						for (int i = 0; i < Main.npc.Length; i++)
						{
							if (Main.npc[i].type == ModContent.NPCType<SmallSpider>())
							{
								Main.npc[i].life = 0;
								Main.npc[i].ModNPC.OnKill();
                            }
						}
						count = 0;
						subState = 0;
						tailUp = false;
					}
					break;
			}
		}
		private void HandleSecondState(Player player, NPC Head, NPC Sting, Projectile[] Leggs)
		{
			switch (subState)
			{
				case 1:
					shouldJump = false;
					if (jumpCount < 3 || !CanJump)
					{
						jumpTimer--;
						if (!oldCanJump && CanJump)
						{
							for (int i = 0; i < 16; i++)
							{
								float angle = 0.2f * (float)Math.PI + ((15f - i) / 15f) * 0.6f * (float)Math.PI;
								Vector2 velocity = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle)) * 15;
								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<ScorspiderShellShard>(), damage, 3f);
							}
							if (WorldDifficultySystem.suicide)
							{
								for (int i = 0; i < 50; i++)
								{
									Vector2 ProjPos = player.Center;
									ProjPos.Y -= 400;
									ProjPos.X += 25 * 100 - i * 100;
									Projectile.NewProjectile(NPC.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4f, -1, 0, 0, 3);
								}
							}
							if (WorldDifficultySystem.TerrapainDifficulty >= 1)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 30), new Vector2(10, 0), ModContent.ProjectileType<ScorspiderRocket>(), damage, 4f);
								Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 30), new Vector2(-10, 0), ModContent.ProjectileType<ScorspiderRocket>(), damage, 4f);
							}
						}
						if (CanJump && jumpTimer <= 0)
						{
							movementState = 1;
							movementTimer = 15;
							CanJump = false;
							HorizontalJumpForce = NPC.direction * 10f;
							jumpTimer = 60;
							jumpCount++;
							if (jumpCount == 3)
								timer = 180;
						}
						else if (player.Center.X > NPC.position.X && player.Center.X < NPC.position.X + NPC.width)
						{
							jumpForce = 15;
							movementState = 0;
							NPC.noTileCollide = false;
							for (int i = 0; i < Leggs.Length; i++)
								Leggs[i].tileCollide = true;
							HorizontalJumpForce = 0;
						}
						oldCanJump = CanJump;
					}
					else if (timer == 180)
					{
						tailUp = true;
						if (WorldDifficultySystem.suicide)
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(player.Center) * 10, ModContent.ProjectileType<ScorspiderRocket>(), damage, 4f);

						direction = NPC.direction;
						timer--;
					}
					else if (timer > 0)
					{
						NPC.DirectionTo(Vector2.UnitX * direction);
						timer--;
						if (NPC.velocity.X < 15)
						{
							NPC.velocity.X += 0.2f * direction;
							if (Math.Abs(NPC.velocity.X) > speed)
							{
								NPC.velocity.X += 0.4f * direction;
							}
						}
						if (timer % (20 - (WorldDifficultySystem.suicide ? 5 : 0)) == 0)
						{
							UnifiedRandom ur = new UnifiedRandom();
							for (int i = 0; i < 3 + WorldDifficultySystem.TerrapainDifficulty; i++)
							{
								float angle = ur.NextFloat() * 0.25f * (float)Math.PI + 0.375f * (float)Math.PI;
								Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * -1) * 25;
								Projectile.NewProjectile(NPC.GetSource_FromAI(), Sting.Center, velocity, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4f, -1, 0, 0, WorldDifficultySystem.suicide ? 0 : 4);
							}
						}
					}
					else if (timer <= 0)
					{
						if (WorldDifficultySystem.TerrapainDifficulty > 0)
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(player.Center) * 10, ModContent.ProjectileType<ScorspiderRocket>(), damage, 4f);

						subState = 0;
						timer = 160;
						shouldJump = true;
						jumpCount = 0;
						tailUp = false;
					}
					break;
				case 5:
				player.wingTime = 20;
					if (movementState == 0)
					{
						movementState = 2;
					}
					if (movementState == 2)
					{
						if (!ShaderSystem.drawScorspiderAura)
						{
							ShaderSystem.ScorspiderAuraTimer = 20;
						}
						ShaderSystem.drawScorspiderAura = true;
						ShaderSystem.AuraRadius = 1000;
						ShaderSystem.ScorspiderPosition = NPC.Center;
						if (ShaderSystem.ScorspiderAuraTimer > 0)
						{
							ShaderSystem.ScorspiderAuraTimer--;
						}

						Functions.AuraHoldPlayer(ShaderSystem.AuraRadius + ShaderSystem.ScorspiderAuraTimer * ShaderSystem.ScorspiderAuraTimer * 2, NPC.Center);
					}
					timer--;
					if (timer % (80 - WorldDifficultySystem.TerrapainDifficulty * 8) == 0)
					{
						int countSpikes = 18 + (WorldDifficultySystem.suicide ? 4 : 0);
						float offset = (rand.NextFloat() * 1000 / countSpikes) / 2 - 1000 / countSpikes;
						for (int i = 0; i < countSpikes; i++)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), offset + player.Center.X - 1000 + i * (2000 / countSpikes), player.Center.Y - 500, 0, 0, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 3);
						}
						float rotate = rand.NextFloat();
						if (timer >= 9 * 60 - (80 - WorldDifficultySystem.TerrapainDifficulty * 8) * 4)
						{
							if (count % 2 == 1)
							{
								for (int i = 0; i < 16; i++)
								{
									Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Functions.UnitVectorFromRotation(i * (float)Math.PI / 8 + rotate * (float)Math.PI) * 300, Functions.UnitVectorFromRotation(i * (float)Math.PI / 8 + rotate * (float)Math.PI) * 8, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 7);
								}
							}
							else
							{
								for (int i = 0; i < 6; i++)
								{
									Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Functions.UnitVectorFromRotation(i * (float)Math.PI / 3 + rotate * (float)Math.PI) * 300, Functions.UnitVectorFromRotation(i * (float)Math.PI / 3 + rotate * (float)Math.PI) * -8, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 7);
								}
							}
							count++;
						}
					}
					if (timer < 9 * 60 - (80 - WorldDifficultySystem.TerrapainDifficulty * 8) * 5 && timer > 0)
					{
						if (timer % (14 - (WorldDifficultySystem.suicide ? 2 : 0)) == 0)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Functions.UnitVectorFromRotation(rotation) * 240, Functions.UnitVectorFromRotation(rotation) * -8, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);
							rotation -= 0.2f * (float)Math.PI;
						}
					}
					if (timer < 300 && timer >= 0)
					{
						Head.ai[2] = 17 - timer / 30;
					}
					if (timer < 0 && timer % 60 == 0)
					{
						Projectile.NewProjectile(Sting.GetSource_FromAI(), player.Center + new Vector2(rand.NextFloat() * 1200 - 600, -400 - rand.NextFloat() * 200), new Vector2(rand.NextFloat() * 3 - 1.5f, 12), ModContent.ProjectileType<ScorspiderFlower>(), damage, 4.5f, -1, 10 + WorldDifficultySystem.TerrapainDifficulty, 0, rand.NextFloat() * 2 * (float)Math.PI);
					}
					if (timer <= -600)
					{
						Head.ai[2] = 18;
						movementState = 0;
						timer = 150;
						subState = 0;
						state = 2;
						NPC.life = (int)(NPC.lifeMax * 0.4);
						NPC.immortal = false;
						Main.npc[head].immortal = false;
						Main.npc[sting].immortal = false;
						attackCount = 0;
						for (int i = 0; i < 5 + WorldDifficultySystem.TerrapainDifficulty; i++)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Functions.UnitVectorFromRotation((float)Math.PI * rand.NextFloat() + (float)Math.PI) * (22 + 8 * rand.NextFloat()), ModContent.ProjectileType<ScorspiderWeb>(), (int)(0.4 * damage), 0);
						}
						count = 0;
						ShaderSystem.drawScorspiderAura = false;
					}
					break;
				case 3:
					tailUp = true;
					if (WorldDifficultySystem.suicide)
					{
						player.wingTime = 20;
					}
					if (timer == 540)
					{
						timer--;
						Sting.rotation = NPC.direction == 1 ? -0.25f * (float)Math.PI : 1.25f * (float)Math.PI;
					}

					else
					{
						if (NPC.direction > 0)
						{
							if (Sting.rotation > (float)Math.PI)
							{
								angularVelocity += 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity > 2 * (float)Math.PI)
									angularVelocity = 0.0125f * (float)Math.PI;
							}
							else
							{
								angularVelocity -= 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity < 0)
									angularVelocity = -0.0125f * (float)Math.PI;
							}
						}
						else
						{
							if (Sting.rotation > (float)Math.PI)
							{
								angularVelocity -= 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity < (float)Math.PI)
									angularVelocity = -0.0125f * (float)Math.PI;
							}
							else
							{
								angularVelocity += 0.0003125f * (float)Math.PI;
								if (Sting.rotation + angularVelocity > (float)Math.PI)
									angularVelocity = 0.0125f * (float)Math.PI;
							}
						}
						Sting.rotation += angularVelocity;

						if (NPC.direction != oldDirection)
						{
							angularVelocity *= -1;
							Sting.rotation = (float)Math.PI - Sting.rotation;
						}

						while (Sting.rotation < 0)
							Sting.rotation += (float)Math.PI * 2;
						while (Sting.rotation > Math.PI * 2)
							Sting.rotation -= (float)Math.PI * 2;

						if (timer % (10 - (WorldDifficultySystem.TerrapainDifficulty > 0 ? 1 : 0)) == 0)
						{
							Vector2 velocity = new Vector2((float)Math.Cos(Sting.rotation), (float)Math.Sin(Sting.rotation)) * 20 * (WorldDifficultySystem.suicide ? 1.5f : 1);
							Projectile.NewProjectile(NPC.GetSource_FromAI(), Sting.Center, velocity, ModContent.ProjectileType<ScorspiderSpike>(), damage, 2, -1, angularVelocity >= 0 ? 0 : -1, 0, 5);
						}

						if (timer % 4 == 0)
						{
							Random rand = new Random();
							int direction = rand.Next(1);
							if (direction == 0)
								direction--;
							Vector2 velocity = new Vector2(direction * rand.NextSingle(), -20);
							Vector2 position;
							position.X = NPC.position.X + rand.Next(NPC.width);
							position.Y = NPC.position.Y;
							Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<ScorspiderShellShard>(), damage * 2, 2);
						}

						timer--;

						if (timer == 0)
						{
							subState = 0;
							timer = 60;
							tailUp = false;
						}
					}
					break;
				case 4:
					tailUp = true;
					timer--;
					if (timer % 70 == 0)
					{
						if (count < 3 + WorldDifficultySystem.TerrapainDifficulty)
						{
							NPC.NewNPC(NPC.GetSource_FromAI(), (int)Sting.Center.X, (int)Sting.Center.Y, ModContent.NPCType<ScorspiderLittleMinionSpidersCocoon>(), 0, 20 * Sting.DirectionTo(player.Center).X, 20 * Sting.DirectionTo(player.Center).Y);
							count++;
						}
						else if (count == 3 + WorldDifficultySystem.TerrapainDifficulty)
						{
							NPC.NewNPC(NPC.GetSource_FromAI(), (int)Sting.Center.X, (int)Sting.Center.Y, ModContent.NPCType<ScorspiderBigMinionSpiderCocoon>(), 0, 20 * Sting.DirectionTo(player.Center).X, 20 * Sting.DirectionTo(player.Center).Y);
							count++;
						}
					}
					if (timer <= 100)
					{    
						for (int i = 0; i < Main.npc.Length; i++)
						{
							if (Main.npc[i].type == ModContent.NPCType<SmallSpider>())
							{
								Main.npc[i].life = 0;
                                Main.npc[i].ModNPC.OnKill();
                            }
						}
					
						count = 0;
						subState = 0;
						tailUp = false;
					}
					break;
			}
		}
		private void HandleThirdState(Player player, NPC Head, NPC Sting, Projectile[] Legs)
		{
			webTimer--;
			if (webTimer <= 0)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Functions.UnitVectorFromRotation((float)Math.PI * rand.NextFloat() + (float)Math.PI) * (22 + 8 * rand.NextFloat()), ModContent.ProjectileType<ScorspiderWeb>(), (int)(0.4 * damage), 0);
				webTimer = 60 - 5 * WorldDifficultySystem.TerrapainDifficulty;
			} 
			switch (subState)
			{
				case 6:
					if (timer > 540 - (60 * 5))
					{
						player.wingTime = 20;
						if (timer % 900 == 0)
						{
							r = Functions.AngleFromVector(player.velocity);
							p = player.Center;
							for (int i = 0; i < 7; i++)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Functions.UnitVectorFromRotation(r) * 350 + Functions.UnitVectorFromRotation(r + 0.5f * (float)Math.PI) * (70 - (i * 140 / 7)), -Functions.UnitVectorFromRotation(r) * 15, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);
							}
						}
						if (timer % 90 <= 50 && timer % 90 > 30)
						{
							if (WorldDifficultySystem.suicide)
							{
								float step = Functions.RightTriangle(new Vector2(Math.Abs(Functions.UnitVectorFromRotation(Math.Abs(r)).X), Functions.UnitVectorFromRotation(Math.Abs(r)).Y), new Vector2(player.width, player.height))[1] * 2;
								Projectile.NewProjectile(NPC.GetSource_FromAI(), p + Functions.UnitVectorFromRotation(r) * 350 + Functions.UnitVectorFromRotation(r + 0.5f * (float)Math.PI) * (step + 105 + Math.Abs(timer % 60 - 50) * (step + 35)), -Functions.UnitVectorFromRotation(r) * 15, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);
								Projectile.NewProjectile(NPC.GetSource_FromAI(), p + Functions.UnitVectorFromRotation(r) * 350 + Functions.UnitVectorFromRotation(r + 0.5f * (float)Math.PI) * (-step - 105 - Math.Abs(timer % 60 - 50) * (step + 35)), -Functions.UnitVectorFromRotation(r) * 15, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);                        
							}
							
						}
						timer--;
					}
					else
					{
						subState = 0;
						timer = 120;
					}
					break;
				case 7:
					tailUp = true;
					if (timer > 300)
					{
						if (timer % (14 - (WorldDifficultySystem.suicide ? 2 : 0)) == 0)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Functions.UnitVectorFromRotation(rotation) * 240, Functions.UnitVectorFromRotation(rotation) * -8, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);
							rotation -= 0.2f * (float)Math.PI;
						}
						if (timer % 30 == 0)
						{
							for (int i = 0; i < 3; i++)
							{
								Projectile.NewProjectile(NPC.GetSource_FromAI(), Sting.Center, Functions.UnitVectorFromRotation(Sting.rotation + (0.15f - i * 0.3f) * (float)Math.PI) * 12, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 1, 0, 0);
							}
						}
						timer--;
					}
					else
					{
						subState = 0;
						timer = 60;
					}
					break;
				case 8:
					if (timer > 540 - 30)
					{
						if (timer % 2 == 0)
						{
							Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + rand.NextFloat() * NPC.width, NPC.position.Y), new Vector2(0, -4), ModContent.ProjectileType<ScorspiderRocket>(), damage, 4.5f);
						}
					}
					else
					{
						subState = 0;
						timer = 90;
					}
					timer--;
					break;
				case 9:
					player.wingTime = 20;
					offset = new Vector2(NPC.direction * 200, 0);
					shouldJump = false;
					tailUp = true;
					if (count > 0 || ShaderSystem.ScorspiderTimer < 20)
					{
						if (!ShaderSystem.drawScorspiderBorders)
						{
							ShaderSystem.ScorspiderTimer = 20;
						}
						if (player.position.Y < buttomOfWall - ((float)49 * 1000 / 74) - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2)
						{
							player.position.Y = buttomOfWall - ((float)49 * 1000 / 74) - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2;
							if (player.velocity.Y < (ShaderSystem.ScorspiderTimer != 0? (ShaderSystem.ScorspiderTimer - 1) * (ShaderSystem.ScorspiderTimer - 1) * 2 - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2 : 0))
							{
								player.velocity.Y = ShaderSystem.ScorspiderTimer != 0? (ShaderSystem.ScorspiderTimer - 1) * (ShaderSystem.ScorspiderTimer - 1) * 2 - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2 : - float.Epsilon;
							}
						}
						if (player.Bottom.Y > buttomOfWall + ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2)
						{
							player.position.Y = buttomOfWall - player.height;
							if (player.velocity.Y > (ShaderSystem.ScorspiderTimer != 0? ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2 - (ShaderSystem.ScorspiderTimer - 1) * (ShaderSystem.ScorspiderTimer - 1) * 2 : 0))
							{
								player.velocity.Y = ShaderSystem.ScorspiderTimer != 0? ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2 - (ShaderSystem.ScorspiderTimer - 1) * (ShaderSystem.ScorspiderTimer - 1) * 2 : 0;
							}
						}
						ShaderSystem.drawScorspiderBorders = true;
						ShaderSystem.BottomOfScorspiderWalls = buttomOfWall;

						if (ShaderSystem.ScorspiderTimer > 0)
						{
							ShaderSystem.ScorspiderTimer--;
						}
					}
					if ((count == 0 && Grounded) || (count != 0 && timer % 120 == 0 && count < 6))
					{
						if (count == 0)
						{
							buttomOfWall = NPC.position.Y - 20;
						}
						int WallHall = oldWallHall + rand.Next(-10, 11);
						if (WallHall < 20)
						{
							WallHall = 20;
						}
						if (WallHall > 45)
						{
							WallHall = 45;
						}
						oldWallHall = WallHall;
						if (count != 0)
						{
							for (int i = 0; i < 50; i++)
							{
								if (i < WallHall - 3 || i > WallHall + 3)
								{
									Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X + 700 * WallDirection, buttomOfWall - ((float)i * 1000 / 74)), new Vector2((10 + (WorldDifficultySystem.suicide? 3 : 0)) * -WallDirection, 0), ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, 6);
								}
							}
						}
						WallDirection *= -1;
						count++;
						timer = 240;
					}
					if (wallTimer % (120 - WorldDifficultySystem.TerrapainDifficulty * 12) == 0)
					{
						Projectile.NewProjectile(Sting.GetSource_FromAI(), Sting.Center, SpikeVelocity, ModContent.ProjectileType<ScorspiderSpike>(), damage, 4.5f, -1, 0, 0, -1);
					}
					if (wallTimer % 250 == 0)
					{
						Projectile.NewProjectile(Sting.GetSource_FromAI(), Sting.Center, new Vector2(0, 12), ModContent.ProjectileType<ScorspiderFlower>(), damage, 4.5f, -1, 10 + WorldDifficultySystem.TerrapainDifficulty, 1, rand.NextFloat() * 2 * (float)Math.PI);
					}
					if (count >= 7)
					{
						if (timer > 60)
						{
							timer = 60;
						}
						Sting.rotation = 0;
					}
					else
					{
						float t;
						Vector2 targetPosition = player.Center + new Vector2(0, -100) - Sting.Center;
						t = (float)Math.Sqrt(Math.Abs((targetPosition.Y * 2) / 0.3f));
						SpikeVelocity = new Vector2 (targetPosition.X / t, targetPosition.Y / t - NPC.gravity * t / 2);
						float angel = Convert.ToSingle(Math.Acos(SpikeVelocity.X / SpikeVelocity.Length()));
						if (SpikeVelocity.Y < 0)
							angel = 2 * Convert.ToSingle(Math.PI) - angel;
						Sting.rotation = angel;
					}
					if (timer <= 0)
					{
						subState = 0;
						shouldJump = true;
						timer = 150;
						count = 0;
						offset = Vector2.Zero;
						tailUp = false;
						ShaderSystem.drawScorspiderBorders = false;
						wallTimer = 0;
					}
					wallTimer++;
					timer--;
					break;
			}
		}
		bool turningBack;
		float leggsFrame;
		float oldHeight;
		bool shouldJump = true;
		bool Grounded;
		private void Movment(Player player, NPC Head, NPC Sting, Projectile[] Leggs)
		{
			Grounded = false;
			Vector2 distance;
			if (movementState == 0)
			{
				leggsFrame += NPC.velocity.X * (turningBack ? -1 : 1);
				if (leggsFrame > 20)
				{
					turningBack = !turningBack;
					leggsFrame = 40 - leggsFrame;
				}
				if (-leggsFrame > 20)
				{
					turningBack = !turningBack;
					leggsFrame = -40 - leggsFrame;
				}

				int LeggsOut = 0;
				float[] LeggsVelocityX = new float[8];
				float[] LeggsGiveVelocity = new float[8];

				distance = new Vector2(-Head.position.X + NPC.Center.X + headIdlePosition.X * NPC.direction - (NPC.direction == -1 ? Head.width : 0), -Head.position.Y + NPC.position.Y + headIdlePosition.Y);
				Head.velocity = distance;
				if (subState != 2)
				{
					distance = new Vector2(-Sting.position.X + NPC.Center.X + stingIdlePosition.X * NPC.direction - Sting.width / 2 + Sting.width * NPC.direction / 2, -Sting.position.Y + NPC.Center.Y + stingIdlePosition.Y);
					Sting.velocity = 0.5f * distance;
				}

				if (NPC.velocity.X * NPC.direction < speed)
					NPC.velocity.X += 0.2f * NPC.direction;
				else
					NPC.velocity.X -= 0.2f * NPC.direction;

				if (player.Center.X > NPC.position.X && player.Center.X < NPC.position.X + NPC.width)
				{
					if (NPC.position.Y + NPC.height < player.position.Y || subState == 1)
						NPC.velocity.Y += 2;
					else if (NPC.position.Y > player.position.Y + player.height && shouldJump)
					{
						movementState = 1;
						jumpForce = 10;
						movementTimer = 10;
					}
				}
				else if (NPC.position.Y > player.position.Y + player.height + 100 && shouldJump)
				{
					movementState = 1;
					jumpForce = 10;
					movementTimer = 10;
				}

				for (int i = 0; i < Leggs.Length; i++)
				{
					if (Leggs[i].rotation != 0)
					{
						if (Leggs[i].rotation < (float)Math.PI)
						{
							if (Leggs[i].rotation - (float)Math.PI / 30 > 0)
								Leggs[i].rotation -= (float)Math.PI / 30;
							else
								Leggs[i].rotation = 0;
						}
						else
						{
							if (Leggs[i].rotation + (float)Math.PI / 30 < 0)
								Leggs[i].rotation += (float)Math.PI / 30;
							else
								Leggs[i].rotation = 0;
						}
					}
				}

				for (int i = 0; i < Leggs.Length; i++)
				{
					if (i % 2 == 0)
					{
						if (NPC.direction > 0)
						{
							LeggsVelocityX[i] = NPC.position.X + NPC.width - firstLeg - distBetwLeggs * (i / 2) - Leggs[i].width - Leggs[i].position.X;
						}
						else
						{
							LeggsVelocityX[i] = NPC.position.X + firstLeg + distBetwLeggs * (i / 2) - Leggs[i].position.X;
						}
					}
					else
					{
						if (NPC.direction > 0)
						{
							LeggsVelocityX[i] = NPC.position.X + NPC.width - firstLeg - Leggs[i].width / 2 - distBetwLeggs * (i / 2) - Leggs[i].width - Leggs[i].position.X;
						}
						else
						{
							LeggsVelocityX[i] = NPC.position.X + firstLeg + Leggs[i].width / 2 + distBetwLeggs * (i / 2) - Leggs[i].position.X;
						}
					}
					Leggs[i].velocity.X = LeggsVelocityX[i];
				}

				for (int i = 0; i < Leggs.Length; i++)
				{
					if (Leggs[i].damage > 0 && ((i % 2 == 0 && !turningBack) || (i % 2 == 1 && turningBack)))
					{
						CanJump = true;
						Grounded = true;
						LeggsGiveVelocity[i] = -(NPC.position.Y + NPC.height + idleLeggsHeight - Leggs[i].position.Y) * 0.03f;
						if (Math.Abs(LeggsVelocityX[i]) > NPC.velocity.X + 0.5f)
						{
							Leggs[i].velocity.Y = -Math.Abs(LeggsVelocityX[i]) / 4;
						}
						if (NPC.position.Y + NPC.height < Leggs[i].position.Y)
							Leggs[i].position.Y = NPC.position.Y + NPC.height;
					}
					else
					{
						float goalHeight = NPC.position.Y + NPC.height + idleLeggsHeight - Leggs[i].position.Y;
						Leggs[i].velocity.Y = goalHeight * 0.5f;
						LeggsGiveVelocity[i] = 0;
					}
					NPC.velocity.Y += LeggsGiveVelocity[i];
					if (Leggs[i].position.Y > NPC.position.Y + NPC.height + NPC.velocity.Y + idleLeggsHeight)
					{
						LeggsOut++;
					}
				}
				if (LeggsOut >= 8)
				{
					NPC.velocity.Y = 0;
				}
				for (int i = 0; i < leggs.Length; i++)
				{
					if (i % 2 == 0)
					{
						Leggs[i].frame = (int)Math.Abs(leggsFrame) / 4;
						Leggs[i].spriteDirection = leggsFrame >= 0 ? 1 : -1;
					}

					//if ((i % 2 == 0 && turningBack) || (i % 2 == 1 && !turningBack))
					//{
					//    Leggs[i].position.Y -= 40 - (float)Math.Abs(leggsFrame) * 2 - oldHeight;
					//    oldHeight = 40 - (float)Math.Abs(leggsFrame) * 2;
					//}
				}
			}
			else if (movementState == 1)
			{
				distance = new Vector2(-Head.position.X + NPC.Center.X + headIdlePosition.X * NPC.direction - (NPC.direction == -1 ? Head.width : 0), -Head.position.Y + NPC.position.Y + headIdlePosition.Y);
				Head.velocity = distance;
				if (movementTimer > 0)
				{
					NPC.velocity = Vector2.Zero;

					distance = new Vector2(-Head.position.X + NPC.Center.X + headIdlePosition.X * NPC.direction - Head.width / 2 + Head.width * NPC.direction / 2, -Head.position.Y + NPC.position.Y + headIdlePosition.Y);
					Head.velocity = 0.5f * distance;

					distance = new Vector2(-Sting.position.X + NPC.Center.X + stingIdlePosition.X * NPC.direction - Sting.width / 2 + Sting.width * NPC.direction / 2, -Sting.position.Y + NPC.Center.Y + stingIdlePosition.Y);
					Sting.velocity = 0.5f * distance;

					NPC.position.Y += 0.25f;
					movementTimer--;
				}
				else if (movementTimer == 0)
				{
					NPC.noTileCollide = true;
					Head.noTileCollide = true;
					Sting.noTileCollide = true;
					for (int i = 0; i < Leggs.Length; i++)
						Leggs[i].tileCollide = false;

					NPC.velocity = (-NPC.Center + player.Center) / 90f;
					NPC.velocity.Y -= jumpForce;
					NPC.velocity.X += HorizontalJumpForce;
					movementTimer--;
				}
				else
				{
					Vector2 NormaizedVelocity = NPC.velocity;
					NormaizedVelocity.Normalize();

					Sting.position.Y = NPC.position.Y + TailToBody.Y + NPC.velocity.Y * -10;
					Sting.position.X = NPC.Center.X + NPC.width * -(float)NPC.direction / 2f + TailToBody.X * (float)NPC.direction + NPC.velocity.X * -10;

					float LeggsGoalRotation;
					if (NPC.velocity.Y < 0)
						LeggsGoalRotation = (float)Math.Asin(NPC.velocity.X / NPC.velocity.Length());
					else
						LeggsGoalRotation = (float)Math.Asin(NPC.velocity.X / NPC.velocity.Length() * -1);
					if (LeggsGoalRotation < 0)
						LeggsGoalRotation += 2 * (float)Math.PI;

					for (int i = 0; i < Leggs.Length; i++)
					{
						while (Leggs[i].rotation < 0)
							Leggs[i].rotation += (float)Math.PI * 2;
						while (Leggs[i].rotation > Math.PI * 2)
							Leggs[i].rotation -= (float)Math.PI * 2;

						float angelBetween = LeggsGoalRotation - Leggs[i].rotation;
						while (angelBetween < 0)
							angelBetween += (float)Math.PI * 2;
						while (angelBetween > Math.PI * 2)
							angelBetween -= (float)Math.PI * 2;

						if (angelBetween >= (float)Math.PI)
						{
							Leggs[i].rotation -= 0.07f;
						}
						else
						{
							Leggs[i].rotation += 0.07f;
						}

						Leggs[i].position.Y = NPC.position.Y + NPC.height + idleLeggsHeight;

						if (i % 2 == 0)
						{
							if (NPC.direction > 0)
							{
								Leggs[i].position.X = NPC.position.X + NPC.width - firstLeg - distBetwLeggs * (i / 2) - Leggs[i].width;
							}
							else
							{
								Leggs[i].position.X = NPC.position.X + firstLeg + distBetwLeggs * (i / 2);
							}
						}
						else
						{
							if (NPC.direction > 0)
							{
								Leggs[i].position.X = NPC.position.X + NPC.width - firstLeg - Leggs[i].width / 2 - distBetwLeggs * (i / 2) - Leggs[i].width;
							}
							else
							{
								Leggs[i].position.X = NPC.position.X + firstLeg + Leggs[i].width / 2 + distBetwLeggs * (i / 2);
							}
						}
					}

					if (NPC.velocity.Y > 0 && player.position.Y - 30f < NPC.position.Y + NPC.height)
					{
						jumpForce = 15;
						movementState = 0;
						NPC.noTileCollide = false;
						for (int i = 0; i < Leggs.Length; i++)
							Leggs[i].tileCollide = true;
						HorizontalJumpForce = 0;
					}
				}
			}
			if (movementState == 2)
			{
				NPC.stairFall = false;
				distance = new Vector2(-Head.position.X + NPC.Center.X + headIdlePosition.X * NPC.direction - (NPC.direction == -1 ? Head.width : 0), -Head.position.Y + NPC.position.Y + headIdlePosition.Y);
				Head.velocity = distance;

				Sting.noGravity = false;
				Sting.noTileCollide = false;
				Sting.position.X = NPC.Center.X + NPC.width * -(float)NPC.direction / 2f + stingIdlePosition.X * (float)NPC.direction + NPC.velocity.X * -10;
				Sting.stairFall = false;

				if (Math.Abs(NPC.velocity.X) < 0.5f)
				{
					NPC.velocity.X -= 0.5f * Math.Sign(NPC.velocity.X);
				}
				else
				{
					NPC.velocity.X = 0;
				}
				for (int i = 0; i < Leggs.Length; i++)
				{
					if (i % 2 == 0)
					{
						if (NPC.direction > 0)
						{
							Leggs[i].velocity.X = NPC.position.X + NPC.width - firstLeg - distBetwLeggs * (i / 2) - Leggs[i].width - Leggs[i].position.X;
						}
						else
						{
							Leggs[i].velocity.X = NPC.position.X + firstLeg + distBetwLeggs * (i / 2) - Leggs[i].position.X;
						}
					}
					else
					{
						if (NPC.direction > 0)
						{
							Leggs[i].velocity.X = NPC.position.X + NPC.width - firstLeg - Leggs[i].width / 2 - distBetwLeggs * (i / 2) - Leggs[i].width - Leggs[i].position.X;
						}
						else
						{
							Leggs[i].velocity.X = NPC.position.X + firstLeg + Leggs[i].width / 2 + distBetwLeggs * (i / 2) - Leggs[i].position.X;
						}
					}
					Leggs[i].velocity.Y = - Leggs[i].position.Y + NPC.position.Y + 64;
				}
			}
			else
			{
				Sting.noGravity = true;
				Sting.noTileCollide = true;
				Sting.stairFall = true;
				NPC.stairFall = true;
			}
			if (tailUp)
			{
				Sting.velocity.X = NPC.Center.X + stingAttackPosition.X * NPC.rotation - Sting.position.X;
				Sting.velocity.Y = NPC.Center.Y + stingAttackPosition.Y - Sting.position.Y;
				Sting.velocity *= 0.7f;
			}
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life - hit.Damage <= 0 && (state == 0 || secondPhase))
			{
				NPC.life = 1;
				NPC.immortal = true;
				Main.npc[head].life = 1;
				Main.npc[head].immortal = true;
				Main.npc[sting].life = 1;
				Main.npc[sting].immortal = true;
			}
			else
			{
				if (NPC.life < hit.Damage)
				{
                    hit.HideCombatText = true;
                    Main.npc[head].StrikeNPC(hit);
                    Main.npc[sting].StrikeNPC(hit);
                }
				Main.npc[head].life -= hit.Damage;
				Main.npc[sting].life -= hit.Damage;
			}
		}
		public override void OnKill()
		{
            int firstGoreType = Mod.Find<ModGore>("ScorspiderBody_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderBody_1").Type;
            int thirdGoreType = Mod.Find<ModGore>("ScorspiderBody_2").Type;

            var entitySource = NPC.GetSource_Death();

	        Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
			Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), thirdGoreType);

            int tailFirstGoreType = Mod.Find<ModGore>("ScorspiderTail_0").Type;
            int tailSecondGoreType = Mod.Find<ModGore>("ScorspiderTail_1").Type;

            for (int i = 0; i < tails.Count; i++)
			{
				var tailSource = Main.projectile[tails[i]].GetSource_FromThis();

                Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailFirstGoreType);
                Gore.NewGore(tailSource, Main.projectile[tails[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), tailSecondGoreType);
            }

            int legFirstGoreType = Mod.Find<ModGore>("ScorspiderLeg_0").Type;
            int legSecondGoreType = Mod.Find<ModGore>("ScorspiderLeg_1").Type;

            for (int i = 0; i < leggs.Length; i++)
            {
                var tailSource = Main.projectile[leggs[i]].GetSource_FromThis();

                Gore.NewGore(tailSource, Main.projectile[leggs[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legFirstGoreType);
                Gore.NewGore(tailSource, Main.projectile[leggs[i]].position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), legSecondGoreType);
            }

            BossDownedSystem.BossDowned(2);
		}
	}
	public class ScorspiderHead : ModNPC
	{
		private int Body
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private int Sting
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}
		private int frame
		{
			get => (int)NPC.ai[2];
			set => NPC.ai[2] = value;
		}
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 36;

			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
			{
				PortraitScale = 0.6f,
				PortraitPositionYOverride = 0f
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
		}
		public override void SetDefaults()
		{
			NPC.width = 78;
			NPC.height = 50;

			NPC.damage = 40;
			NPC.defense = 20;

			NPC.lifeMax = 8000;

			NPC.knockBackResist = 0f;

			NPC.SpawnWithHigherTime(30);
			NPC.npcSlots = 10f;

			NPC.noTileCollide = false;
			NPC.noGravity = true;

			NPC.aiStyle = -1;
			NPC.stairFall = true;

            NPC.HitSound = SoundID.NPCHit4;
        }
		int timer = 5;
		public override void AI()
		{
			timer--;
			if (!Main.npc[Body].active || Main.npc[Body].type != ModContent.NPCType<ScorspiderBody>() && timer <= 0)
			{
				NPC.life = 0;
			}
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderHead_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderHead_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
        }
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life - hit.Damage <= 1 && (Main.npc[Body].ai[0] == 1 || Main.npc[Body].ai[0] == 0))
			{
				NPC.life = 1;
				NPC.immortal = true;
				Main.npc[Body].life = 1;
				Main.npc[Sting].life = 1;
				Main.npc[Body].immortal = true;
				Main.npc[Sting].immortal = true;
			}
			else
			{
				if (Main.npc[Body].life - hit.Damage <= 0)
				{
                    hit.HideCombatText = true;
                    Main.npc[Body].StrikeNPC(hit);
                    Main.npc[Sting].StrikeNPC(hit);
                }
				else
				{
					Main.npc[Body].life -= hit.Damage;
					Main.npc[Sting].life -= hit.Damage;
				}
			}
		}
		public override void FindFrame(int frameHeight)
		{
			NPC.frame = new Rectangle(0, frame * NPC.height, NPC.width, NPC.height);
		}
	}
	public class ScorspiderSting : ModNPC
	{
		private int Body
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private int Head
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;

			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers drawMods = new()
			{
				PortraitScale = 0.6f,
				PortraitPositionYOverride = 0f
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawMods);
		}
		public override void SetDefaults()
		{
			NPC.width = 60;
			NPC.height = 64;

			NPC.damage = 40;
			NPC.defense = 20;

			NPC.lifeMax = 8000;

			NPC.knockBackResist = 0f;

			NPC.SpawnWithHigherTime(30);
			NPC.npcSlots = 10f;

			NPC.noTileCollide = true;
			NPC.noGravity = true;

			NPC.aiStyle = -1;

            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath28;
        }
		public override bool? CanFallThroughPlatforms()
		{
			return true;
		}
		int timer = 5;
		public override void FindFrame(int frameHeight)
		{
			if (Math.Abs(Functions.NormalizeRotation(NPC.rotation)) < Math.PI / 2)
			{
				NPC.frame.Y = 0;
			}
			else
			{
				NPC.frame.Y = NPC.height;
			}
		}
		public override void AI()
		{
			timer--;
			if (!Main.npc[Body].active || Main.npc[Body].type != ModContent.NPCType<ScorspiderBody>() && timer <= 0)
			{
				NPC.life = 0;
			}
		}
		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if(ShaderSystem.drawScorspiderBorders || ShaderSystem.ScorspiderTimer < 20)
			{
				if (ClientConfig.Instance.UseShaders)
				{
					var blackTile = ExtraTextureRegistry.BlackPixel;

					ManagedShader Shade = ShaderManager.GetShader("Terrapain.ScorspiderShader");
					Shade.TrySetParameter("height", 49f * 1000f / 74f);
					Shade.TrySetParameter("startPosY", ShaderSystem.BottomOfScorspiderWalls);   
					Shade.TrySetParameter("playerPos", Main.player[Main.myPlayer].Center);
					Shade.TrySetParameter("screenPosition", Main.screenPosition);
					Shade.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
					Shade.TrySetParameter("timer", ShaderSystem.ScorspiderTimer);

					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
					Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
					spriteBatch.Draw(blackTile.Value, rekt, null, Color.Black, 0f, blackTile.Value.Size() * 0.5f, 0, 1f);
					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
				}
				else
				{
					
					Vector2 center = new Vector2(Main.cameraX, ShaderSystem.BottomOfScorspiderWalls + ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2);
					Vector2 screenSize = Main.ScreenSize.ToVector2();
					Color drawerColor = new Color(0.5f, 0f, 0f, 0.5f);
					Vector2 screenPosition = Main.screenPosition;

					if ((int)(screenPos.Y + screenSize.Y - center.Y) > 0)
					{
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle (0, (int)(center.Y - screenPos.Y), (int)screenSize.X, (int)(screenPos.Y + screenSize.Y - center.Y)), drawerColor);
					}
					
					center = new Vector2(Main.cameraX, ShaderSystem.BottomOfScorspiderWalls - 49f * 1000f / 74f - ShaderSystem.ScorspiderTimer * ShaderSystem.ScorspiderTimer * 2);
					
					if ((int)(center.Y - screenPos.Y) > 0)
					{
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle (0, 0, (int)screenSize.X, (int)(center.Y - screenPos.Y)), drawerColor);
					}
				}
			}

			if(ShaderSystem.drawScorspiderAura || ShaderSystem.ScorspiderAuraTimer < 20)
			{
				if (ClientConfig.Instance.UseShaders)
				{
					var blackTile = ExtraTextureRegistry.BlackPixel;

					ManagedShader Shade = ShaderManager.GetShader("Terrapain.ScorspiderAuraShader");
					Shade.TrySetParameter("radius", ShaderSystem.AuraRadius);
					Shade.TrySetParameter("Scorspider", ShaderSystem.ScorspiderPosition);
					Shade.TrySetParameter("playerPos", Main.player[Main.myPlayer].Center);
					Shade.TrySetParameter("screenPosition", Main.screenPosition);
					Shade.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
					Shade.TrySetParameter("timer", ShaderSystem.ScorspiderAuraTimer);

					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, Shade.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
					Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
					spriteBatch.Draw(blackTile.Value, rekt, null, Color.Black, 0f, blackTile.Value.Size() * 0.5f, 0, 1f);
					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
				}
				else
				{
					Vector2 center = ShaderSystem.ScorspiderPosition;
					Vector2 screenSize = Main.ScreenSize.ToVector2();
					Color drawerColor = new Color(0.5f, 0f, 0f, 0.5f);
					Vector2 screenPosition = Main.screenPosition;
					float radius = ShaderSystem.AuraRadius;

					spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, 0, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);
                    spriteBatch.Draw(ExtraTextureRegistry.Aura.Value, center - screenPos, null, drawerColor, (float)Math.PI / 2 * 3, Vector2.Zero, radius / 2000, SpriteEffects.None, 0);

					if ((int)(center.Y - screenPos.Y - radius) > 0)
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, 0, (int)screenSize.X, (int)(center.Y - screenPos.Y - radius)), drawerColor);
					if ((int)(screenSize.Y - (center.Y + radius - screenPos.Y)) > 0)
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y + radius - screenPos.Y), (int)screenSize.X, (int)(screenSize.Y - (center.Y + radius - screenPos.Y))), drawerColor);
					if ((int)(center.X - radius - screenPosition.X) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle(0, (int)(center.Y - screenPos.Y - radius), (int)(center.X - radius - screenPosition.X) + 1, (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor);
					if ((int)(screenSize.X - (center.X + radius - screenPos.X)) > 0 && (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius)) > 0)
						spriteBatch.Draw(ExtraTextureRegistry.WhitePixel.Value, new Rectangle((int)(center.X + radius - screenPos.X), (int)(center.Y - screenPos.Y - radius), (int)(screenSize.X - (center.X + radius - screenPos.X)), (int)(center.Y + radius - screenPos.Y - (center.Y - screenPos.Y - radius))), drawerColor);
                }
			}
		}
		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 900);
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life - hit.Damage <= 1 && (Main.npc[Body].ai[0] == 1 || Main.npc[Body].ai[0] == 0))
			{
				NPC.life = 1;
				NPC.immortal = true;
				Main.npc[Body].life = 1;
				Main.npc[Head].life = 1;
				Main.npc[Body].immortal = true;
				Main.npc[Head].immortal = true;
			}
			else
			{
				if (Main.npc[Body].life - hit.Damage <= 0)
				{
					hit.HideCombatText = true;
					Main.npc[Body].StrikeNPC(hit);
                    Main.npc[Head].StrikeNPC(hit);
                }
				else
				{
					Main.npc[Body].life -= hit.Damage;
					Main.npc[Head].life -= hit.Damage;
				}
			}
		}
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("ScorspiderSting_0").Type;
            int secondGoreType = Mod.Find<ModGore>("ScorspiderSting_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);
        }
	}

	public class ScorspiderLeg : ModProjectile
	{
		public static bool fallThroughtPlatforms;
		private bool ShouldHide
		{
			get => Projectile.ai[0] == 1;
		}
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5 * 15;
		}
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 120;

			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 60;
		}
		public override void OnSpawn(IEntitySource source)
		{
			Projectile.hide = ShouldHide;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.damage = 2;
			return false;
		}
		public override void AI()
		{
			if (Projectile.damage > 0)
				Projectile.damage--;
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			Main.instance.DrawCacheProjsBehindNPCs.Add(index);
		}
	}
	public class ScorspiderTail : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;

			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 2;
			Projectile.hide = true;
		}
		public override void AI()
		{
			if (Math.Abs(Projectile.rotation % (Math.PI * 2)) < Math.PI / 2)
			{
				Projectile.frame = 0;
			}
			else
			{
				Projectile.frame = 1;
			}
		} 
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			Main.instance.DrawCacheProjsBehindNPCs.Add(index);
		}
	}
	public class ScorspiderSpike : ModProjectile
	{
		private float i
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private float count
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		private int attackStyle
		{
			get => (int)Projectile.ai[2];
			set => Projectile.ai[2] = value;
		}
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;

			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 300;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPostDraw = true;
		}
		NPC Sting;
		Vector2 positionAboutPlayer;
		Vector2 startVlocity;
		float startRotation;
		public override void OnSpawn(IEntitySource source)
		{
			foreach (var npc in Main.npc)
			{
				if (npc.type == ModContent.NPCType<ScorspiderSting>())
				{
					Sting = npc;
					break;
				}
			}

			Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
			positionAboutPlayer = Projectile.position - player.position;

			startVlocity = Projectile.velocity;
			if (Projectile.velocity != Vector2.Zero)
			{
				float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
				if (Projectile.velocity.Y < 0)
					angel = 2 * Convert.ToSingle(Math.PI) - angel;
				startRotation = angel;
			}
		}
		int timer;
		bool gravity = true;
		int[] dusts = new int[100];
		Vector2 memoryVelocity;
		public override void AI()
		{
			if (Projectile.friendly)
			{
				Projectile.tileCollide = true;
			}
			if (Projectile.velocity != Vector2.Zero)
			{
				float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
				if (Projectile.velocity.Y < 0)
					angel = 2 * Convert.ToSingle(Math.PI) - angel;
				Projectile.rotation = angel;
			}

			Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];

			timer++;

			switch (attackStyle)
			{
				case 0:
					if (WorldDifficultySystem.suicide && Projectile.hostile)
					{
						if (Projectile.velocity.X > 0)
						{
							if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
						else
						{
							if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
					}
					break;
				case 1:
					if (timer == 80)
					{
						gravity = false;
						if (i % 2 == 0)
						{
							Projectile.velocity = Functions.UnitVectorFromRotation(0.3f * (float)Math.PI) * 12;
						}
						else
						{
							Projectile.velocity = Functions.UnitVectorFromRotation(0.7f * (float)Math.PI) * 12;
						}
					}
					break;
				case 2:
					if (timer == 80)
					{
						gravity = false;
						Projectile.velocity = Projectile.DirectionTo(player.Center + new Vector2(0, 60)) * 35;
					}
					break;
				case 3:
					if (timer == 1)
					{
						Projectile.alpha = 255;
						gravity = false;
						Projectile.rotation = 0.5f * (float)Math.PI;
					}
					else if (Projectile.alpha > 0)
					{
						if (Projectile.alpha - 8 > 0)
							Projectile.alpha -= 8;
						else
						{
							Projectile.alpha = 0;
							Projectile.velocity = new Vector2(0, 15);
							gravity = true;
						}
					}
					break;
				case 4:
					if (timer < 60)
					{
						if (!WorldDifficultySystem.suicide)
							Projectile.hostile = false;
					}
					else
					{
						Projectile.hostile = true;
					}
					if (WorldDifficultySystem.suicide)
					{
						if (Projectile.velocity.X > 0)
						{
							if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
						else
						{
							if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
					}
					break;
				case 5:
					if (timer < 90 && WorldDifficultySystem.suicide)
					{
						if (Projectile.velocity.X > 0)
						{
							if (Projectile.position.X + 200 - Projectile.position.X % 200 <= Projectile.position.X + Projectile.velocity.X && i != 0)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X + 200 - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
						else
						{
							if (Projectile.position.X - Projectile.position.X % 200 >= Projectile.position.X + Projectile.velocity.X && i != -1)
							{
								Vector2 ProjPos = Projectile.position;
								ProjPos.X = Projectile.position.X - Projectile.position.X % 200;
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), ProjPos, Vector2.Zero, ModContent.ProjectileType<ScorspiderShellShard>(), Projectile.damage, Projectile.knockBack);
							}
						}
					}
					else
					{
						gravity = false;
						Projectile.velocity = Projectile.DirectionTo(Sting.Center) * 20;
					}
					break;
				case 6:
					gravity = false;
					if (timer == 1)
					{
						memoryVelocity = Projectile.velocity;
						Projectile.velocity = Vector2.Zero;
					}
					if (timer == 20)
					{
						Projectile.velocity = memoryVelocity;
					}
					break;
				case 7:
					if (timer < 60)
					{
						Projectile.position = player.position + positionAboutPlayer;
						Projectile.rotation = startRotation;
						Projectile.velocity = Vector2.Zero;
						gravity = false;
					}
					if (timer == 60)
					{
						Projectile.velocity = startVlocity;
						gravity = true;
					}
					break;
			}
			
			if (gravity)
				Projectile.velocity.Y += 0.3f;
			/*if (count != 0 && timer == 80 && WorldDifficultySystem.suicide)
			{
				Vector2 target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)].position;
				target.X += -(float)(count - 1) / 2 * 1.5f * timer + (attackStyle == 1 ? i : count - 1 - i) * 1.5f * timer;
				Projectile.velocity = Projectile.DirectionTo(target) * 15;
			}*/
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override bool CanHitPlayer(Player target)
		{
			if (attackStyle == 3 && Projectile.alpha > 0)
			{
				return false;
			}
			return base.CanHitPlayer(target);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			if ((attackStyle == 1 && timer >= 80) || attackStyle == 3)
			{
				Vector2 center = Projectile.Center;
				float Rotation = Projectile.rotation - MathHelper.PiOver2;
				Color drawColor = new Color(0.7f, 0.1f, 0.12f, 0.6f * (255f - Projectile.alpha) / 255);
			
				Main.EntitySpriteDraw(ExtraTextureRegistry.WhitePixel.Value, center - Main.screenPosition, ExtraTextureRegistry.BlackPixel.Value.Bounds, drawColor, Rotation, new Vector2(0.5f, 0), new Vector2(8, Main.ScreenSize.ToVector2().Length()), SpriteEffects.None);
			}
			return false;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			return true;
		}
	}
	public class ScorspiderShellShard : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;

			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
		}
		float angularVelocity;
		public override void OnSpawn(IEntitySource source)
		{
			UnifiedRandom random = new UnifiedRandom();
			Projectile.rotation = (random.NextFloat() - 0.5f) * 2 * (float)Math.PI;
			angularVelocity = (random.NextFloat() - 0.5f) * 0.2f * (float)Math.PI;
		}

		public override void AI()
		{
			Projectile.rotation += angularVelocity;
			if (Projectile.friendly)
			{
				Projectile.tileCollide = true;
			}
			Projectile.velocity.Y += 0.3f;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			return true;
		}
	}
	public class ScorspiderRocket : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;

			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
		}
		
		float angularVelocity;
		public override void OnSpawn(IEntitySource source)
		{
			float angel = Convert.ToSingle(Math.Acos(Projectile.velocity.X / Projectile.velocity.Length()));
			if (Projectile.velocity.Y < 0)
				angel = 2 * Convert.ToSingle(Math.PI) - angel;
			Projectile.rotation = angel;
		}
		public override void AI()
		{
			if (Projectile.friendly)
			{
				Projectile.tileCollide = true;
			}
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * - 3, Projectile.velocity.Y * -3);
			Entity player = null;
			if (Projectile.hostile)
			{
				player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
			}
			else
			{
			foreach(NPC npc in Main.npc)
				{
					if (npc.active && (player == null || Projectile.Distance(player.Center) > Projectile.Distance(npc.Center)))
					{
						player = npc;
					}
				}
			}

			Projectile.velocity = new Vector2((float)Math.Cos(Projectile.rotation), (float)Math.Sin(Projectile.rotation)) * (Projectile.velocity.Length() + (Projectile.velocity.Length() > 20 ? 0 : 0.5f));
			if (player != null)
			{
				float goalAngle = Projectile.AngleTo(player.Center);
				goalAngle = goalAngle % (2f * (float)Math.PI);
				if (goalAngle < 0)
				{
					goalAngle += (float)Math.PI * 2;
				}
				Projectile.rotation = Projectile.rotation % (2f * (float)Math.PI);
				if (Projectile.rotation < 0)
				{
					Projectile.rotation += (float)Math.PI * 2;
				}

				if (goalAngle < (float)Math.PI)
				{
					if (Projectile.rotation > goalAngle && Projectile.rotation < goalAngle + Math.PI)
					{
						if (angularVelocity > -0.03f)
							angularVelocity -= 0.003f;
					}
					else
					{
						if (angularVelocity < 0.03f)
							angularVelocity += 0.003f;
					}
				}
				else    
				{
					if (Projectile.rotation < goalAngle && Projectile.rotation > goalAngle - Math.PI)
					{
						if (angularVelocity < 0.03f)
							angularVelocity += 0.003f;
					}
					else
					{
						if (angularVelocity > -0.03f)
							angularVelocity -= 0.003f;
					}
				}
				Projectile.rotation += angularVelocity;
			}
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			return true;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
	}
	public class ScorspiderWeb : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 58;

			Projectile.alpha = 60;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.knockBack = 0f;
			Projectile.timeLeft = 500;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true; 
		}
		public override void AI()
		{
			Projectile.velocity *= 0.95f;

			if (Projectile.hostile)
			{
				foreach (Player target in Main.player)
				{
					if (target.active && !target.dead)
					{
						if (Functions.RectangleColision(target, Projectile))
						{
							target.velocity *= 0.8f;
							target.runAcceleration *= 0.2f;
							target.maxRunSpeed *= 0.4f;
							target.maxFallSpeed *= 0.3f;
							target.gravity *= 0.3f;
						}
					}
				}
				foreach (var target in Main.npc)
				{
					if (target.active && target.friendly && target.knockBackResist != 0)
					{
						if (Functions.RectangleColision(target, Projectile))
						{
							target.velocity *= 0.8f * target.knockBackResist;
							target.MaxFallSpeedMultiplier *= 0.3f * target.knockBackResist;
							target.GravityMultiplier *= 0.3f * target.knockBackResist;
						}
					}
				}
			}

			if (Projectile.friendly)
			{
				foreach (var target in Main.npc)
				{
					if (target.active && !target.friendly && target.knockBackResist != 0)
					{
						if (Functions.RectangleColision(target, Projectile))
						{
							target.velocity *= 0.8f * target.knockBackResist;
							target.MaxFallSpeedMultiplier *= 0.3f * target.knockBackResist;
							target.GravityMultiplier *= 0.3f * target.knockBackResist;
						}
					}
				}
			}
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity *= 0;
			return false;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
	}
	public class ScorspiderFlower : ModProjectile
	{
		int spikesCount
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		bool followPlayer
		{
			get => Projectile.ai[1] == 1;
			set => Projectile.ai[1] = value? 1: 0;
		}
		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;

			Projectile.timeLeft = 240;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
		}

		float speed;
		int ShootProjectiles;
		int timer;
		public override void OnSpawn(IEntitySource source)
		{
			speed = Projectile.velocity.Length();
			Projectile.rotation = Projectile.ai[2];
		}
		public override void AI()
		{
			if (followPlayer)
			{
				Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
				Projectile.velocity = Projectile.DirectionTo(player.Center) * speed;
				if (speed > 0.3f)
				{
					speed -= 0.3f;
				}
				else
				{
					speed = 0;
				}
			}
			else
			{
				if (speed != 0)
				{
					Projectile.velocity.Normalize();
				
					Projectile.velocity *= speed;
					if (speed > 0.3f)
					{
						speed -= 0.3f;
					}
					else
					{
						speed = 0;
					}
				}
			}
			if (speed <= 4 && ShootProjectiles < spikesCount && timer <= 0)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Functions.UnitVectorFromRotation(Projectile.rotation + (float)ShootProjectiles / spikesCount * 2 * (float)Math.PI) * 20, ModContent.ProjectileType<ScorspiderSpike>(), Projectile.damage, Projectile.knockBack, -1, 0, 0, -1);
				ShootProjectiles += 1;
				timer = 8;
				Projectile.scale = 1.16f;
			}
			if (ShootProjectiles < spikesCount)
			{
				Projectile.timeLeft = 2;
			}
			if (Projectile.scale > 1)
			{
				Projectile.scale -= 0.02f; 
			}
			timer--;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
	}
	public class ScorspiderLittleMinionSpidersCocoon : ModNPC
	{
		private float VelocityX
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private float VelocityY
		{
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}
		public override void SetDefaults()
		{
			NPC.height = 30;
			NPC.width = 30;

			NPC.lifeMax = 500;
			NPC.friendly = false;
			NPC.damage = 20;
			NPC.knockBackResist = 0.8f;
			NPC.aiStyle = -1;
		}
		public override void OnSpawn(IEntitySource source)
		{
			if (VelocityX != float.NaN && VelocityY != float.NaN && (VelocityX != 0 || VelocityY != 0))
			{
				NPC.velocity.X = VelocityX;
				NPC.velocity.Y = VelocityY;
				NPC.velocity = Functions.Rotate(NPC.velocity, NPC.velocity.X);
			}
		}
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			SpawnMinions();
		}
		public override void AI()
		{
			if (NPC.collideX || NPC.collideY)
			{
				SpawnMinions();
			}
		}
		private void SpawnMinions()
		{
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<ScorspiderWebDust>(), NPC.velocity.X, NPC.velocity.Y);
            }
			for (int i = 0; i < 5; i++)
			{
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SmallSpider>(), 0, NPC.velocity.X * (NPC.collideX ? -1 : 1) - 10 + 5 * i, NPC.velocity.Y * (NPC.collideY ? -1 : 1));
			}
			NPC.life = 0;
		}
	}
	public class ScorspiderBigMinionSpiderCocoon : ModNPC
	{
		private float VelocityX
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private float VelocityY
		{
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}
		public override void SetDefaults()
		{
			NPC.height = 50;
			NPC.width = 50;

			NPC.lifeMax = 500;
			NPC.friendly = false;
			NPC.damage = 20;
			NPC.knockBackResist = 0.8f;
			NPC.aiStyle = -1;
		}
		float angularVelocity;
		UnifiedRandom random = new UnifiedRandom();
		public override void OnSpawn(IEntitySource source)
		{
			if (VelocityX != float.NaN && VelocityY != float.NaN && (VelocityX != 0 || VelocityY != 0))
			{
				NPC.velocity.X = VelocityX;
				NPC.velocity.Y = VelocityY;
				NPC.velocity = Functions.Rotate(NPC.velocity, NPC.velocity.X);
			}
			angularVelocity = random.NextFloat() - 0.5f;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			SpawnMinions();
		}
        public override bool? CanFallThroughPlatforms()
        {
			return true;
        }
		public override void AI()
		{
			NPC.rotation += angularVelocity;
			if (NPC.collideX)
			{
                NPC.velocity.X *= -1;
                SpawnMinions();
			}
            if (NPC.collideY)
            {
				NPC.velocity.Y *= -1;
                SpawnMinions();
            }
        }
		private void SpawnMinions()
		{
			for (int i = 0; i < 30; i ++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Granite, NPC.velocity.X, NPC.velocity.Y);
			}
			NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BigSpider>(), 0, NPC.velocity.X * (NPC.collideX ? -1 : 1), NPC.velocity.Y * (NPC.collideY ? -1 : 1));
			NPC.life = 0;
		}
	}
	public class SmallSpider : ModNPC
	{
		static UnifiedRandom rand = new UnifiedRandom();
		private float VelocityX
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private float VelocityY
		{
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 2;
		}
		public override void SetDefaults()
		{
			NPC.height = 20;
			NPC.width = 28;

			NPC.lifeMax = 20;
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.damage = 50;

            NPC.HitSound = SoundID.NPCHit1;
        }
		public override void OnSpawn(IEntitySource source)
		{
			NPC.velocity.X = VelocityX;
			NPC.velocity.Y = VelocityY;
		}

		float Length;

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.X = (((int)Length / 6) % 2) == 0? 1 : 0; 
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        float jumpForce = 15;
		float speed => (8f + (float)(3 + WorldDifficultySystem.TerrapainDifficulty) / 3) / (WorldDifficultySystem.suicide ? 1.5f : 1.7f);
		int timer = 60;
		public override void AI()
		{
			Length += NPC.velocity.X;
			NPC.TargetClosest();
			Player target = Main.player[NPC.target];
			NPC.spriteDirection = NPC.position.X < target.position.X? 1 : -1;
			timer--;

			if (NPC.collideY)
			{
				if (Math.Abs(NPC.velocity.X) < speed)
					NPC.velocity.X += 0.5f * NPC.direction;
				else if (NPC.velocity.X > speed)
					NPC.velocity.X -= 0.5f;
				else
					NPC.velocity.X += 0.5f;
			}

			if (NPC.collideY && NPC.Distance(target.Center) < 800 && timer <= 0)
			{
				NPC.velocity = (-NPC.Center + target.Center) / 90f;
				NPC.velocity.Y -= jumpForce;
				NPC.velocity.X += rand.Next(-8, 7) + rand.NextFloat();
			}
			timer--;
		}
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("LittleSpider_0").Type;
            int secondGoreType = Mod.Find<ModGore>("LittleSpider_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);

			SoundEngine.PlaySound(SoundID.NPCDeath2, NPC.position);
        }
	}
	public class BigSpider : ModNPC
	{
		int damage = 20;
		static UnifiedRandom rand = new UnifiedRandom();
		private float VelocityX
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private float VelocityY
		{
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 5;
		}

		public override void SetDefaults()
		{
			NPC.height = 170 / 5;
			NPC.width = 40;

			NPC.lifeMax = 50;
			NPC.aiStyle = -1;
			NPC.friendly = false;
			NPC.damage = damage;

            NPC.HitSound = SoundID.NPCHit1;
        }
		public override void OnSpawn(IEntitySource source)
		{
			NPC.velocity.X = VelocityX;
			NPC.velocity.Y = VelocityY;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
		float jumpForce = 15;
		float speed => (8f + (float)(3 + WorldDifficultySystem.TerrapainDifficulty) / 3) / (WorldDifficultySystem.suicide ? 1.5f : 1.7f);
		int timer = 5;
		float oldFallSpeed;
		float length;
		Player target;

		public override void FindFrame(int frameHeight)
		{
			if (NPC.Distance(target.Center) < 600)
			{
				NPC.frame.Y = NPC.height * 4;
			}
			else
			{
				NPC.frame.Y = NPC.height * ((int)length / 2) % 4;
			}
		}
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void AI()
		{
			NPC.TargetClosest();
			target = Main.player[NPC.target];


			if (NPC.Distance(target.Center) < 600)
			{
				length = 0;
				if (NPC.collideY)
				{
					float t;
					Vector2 targetPosition = target.Center + new Vector2(0, -100) - NPC.Center;
					t = (float)Math.Sqrt(Math.Abs((targetPosition.Y * 2) / NPC.gravity));
					NPC.velocity.X = targetPosition.X / t;
					NPC.velocity.Y = targetPosition.Y / t - NPC.gravity * t / 2;
					NPC.rotation = 0;
					NPC.DirectionTo(target.Center - NPC.Center);
					timer = 15;
					NPC.velocity += target.velocity;
				}
				else
				{
					NPC.rotation += (float)Math.PI / 30;
					timer--;
					if (timer == 0)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Functions.UnitVectorFromRotation(NPC.rotation + 0.5f * (float)Math.PI) * 8, ModContent.ProjectileType<BigSpiderSpike>(), damage, 4f);
						timer = 7;
					}
					oldFallSpeed = NPC.velocity.Y;
				}
			}
			else
			{
				if (NPC.collideY)
				{
					length += NPC.velocity.X;
				}
				else
				{
					length = 0;
				}
				NPC.spriteDirection = (NPC.DirectionTo(target.Center).X >= 0)? 1 : -1;
				if (Math.Abs(NPC.velocity.X) < 12)
				{
					NPC.velocity.X += 0.2f * NPC.spriteDirection;
				}
				else
				{
					if (NPC.velocity.X > 0)
					{
						NPC.velocity.X -= 0.2f;
					}
					else
					{
						NPC.velocity.X += 0.2f;
					}
				}
			}
		}
        public override void OnKill()
        {
            int firstGoreType = Mod.Find<ModGore>("BigSpider_0").Type;
            int secondGoreType = Mod.Find<ModGore>("BigSpider_1").Type;

            var entitySource = NPC.GetSource_Death();

            Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), firstGoreType);
			Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), secondGoreType);

            SoundEngine.PlaySound(SoundID.NPCDeath2, NPC.position);
        }
	}
	public class BigSpiderSpike : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;

			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 180;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().useModDrawingInPreDraw = true;
			Projectile.GetGlobalProjectile<TGlobalProjectile>().useVanillaDrawing = false;
		}
		public override void AI()
		{
			if (Projectile.velocity.Y < 5)
				Projectile.velocity.Y += 0.3f;
			float angel = (float)Math.Acos(Projectile.velocity.X / Projectile.velocity.Length());
			if (Projectile.velocity.Y < 0)
				angel = 2 * (float)Math.PI - angel;
			Projectile.rotation = angel;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
		}
	}
}