using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Common.UI;
using Terrapain.Content;
using Terrapain.Content.Auras;
using Terrapain.Content.Buffs;
using Terrapain.Content.Buffs.Potions;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Weapons.SummonerWeapons;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider;
using Terrapain.Content.Stimulators;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Terrapain.Common.Player
{
	public class TerrapainPlayer : ModPlayer
	{
		public Vector2[] oldCenters = new Vector2[60];
		public Vector2[] oldPositions = new Vector2[60];
		public Vector2[] oldVelocities = new Vector2[60];
		public List<Terrapain.LightningDrawInfo> lightnings = new List<Terrapain.LightningDrawInfo>();
        public List<Aura> auras = new List<Aura>();
        public bool RangerGranithShellArmorSet;
		public bool CactusSet;
		public bool StarPowerSet;
		public bool bonusArcons;
		public bool ExplosiveSkull;
		public bool StarFuryBrasslet;
        public bool AcidCobwebBonus;
		public bool GranithShellChestplateBonus;
		public bool unarmed;
		public string CurentHeart;
		public int ExplosiveSkullDamage;
		public int ExplosiveSkullReload;
		public int StarFuryBrassletReload;
        public int AcidCobwebBonusReload;
        public int ThornsSpikeReload;
		public int StarPowerSetReload;
        public UnifiedRandom playerRandom = new UnifiedRandom();
		public Stimulator stimulator = null;
		private Dash _dash = null;
		public bool bootsActiveAccessory;
		public int oldLifeRegen;

		public int UnarmedMouseActiveTimer;
		public bool UnarmedMouseActive => UnarmedMouseActiveTimer != 0;

		public float Stamina = 100;
        public int StaminaRegenerationTimer;
        public int StaminaRegenerationTimerMax;
		public float StaminaRegenerationTimerCoefficient;
        public float StaminaRegeneration;
		public float MaxStaminaRegenerationSpeed;
		public float StaminaRegenerationAcceleration;
		public float MaxStamina;
		public float staminaUsageMultiplyer;

		public int GetStaminaTimer => (int)(StaminaRegenerationTimerMax * (Stamina / StaminaRegenerationTimerCoefficient + 1));
		public float StaminaDamageBuff => Stamina / 100 * 1.5f + 0.2f;
		public float MaxStaminaDamageBuff => MaxStamina / 100 * 1.5f + 0.2f;
        public float StaminaUseSpeedBuff => Stamina / 100 * 1.1f + 0.5f;
        public float MaxStaminaUseSpeedBuff => MaxStamina / 100 * 1.1f + 0.5f;
        public Dash Dash
		{
			get => _dash;
			set { 
				if (_dash == null || value.priority > _dash.priority)
				{
					_dash = value;
				}
			}
		}

        public ModKeybind[] ActiveAccesoriesKeybinds =
		{
			KeybindSystem.ActiveAccesory1,
			KeybindSystem.ActiveAccesory2,
			KeybindSystem.ActiveAccesory3,
			KeybindSystem.ActiveAccesory4,
			KeybindSystem.ActiveAccesory5,
			KeybindSystem.ActiveAccesory6,
			KeybindSystem.ActiveAccesory7
		};
        public override void Load()
        {
            On_Player.SpawnFastRunParticles += On_Player_SpawnFastRunParticles;
			//On_Player.ApplyItemTime 
        }
        public override void Unload()
        {
			On_Player.SpawnFastRunParticles -= On_Player_SpawnFastRunParticles;
        }
        private void On_Player_SpawnFastRunParticles(On_Player.orig_SpawnFastRunParticles orig, Terraria.Player self)
        {
			if (self.Custom().bootsActiveAccessory)
			{
				int dust = Dust.NewDust(self.position + new Vector2(self.direction == 1 ? 0 : self.width, self.gravDir == 1 ? self.height : 0) - Vector2.One * 3, 6, 6, DustID.Cloud, self.velocity.X * -0.33f, -MathF.Abs(self.velocity.X * 0.33f), Scale: 2);
				Main.dust[dust].velocity.Y = playerRandom.NextFloat(Main.dust[dust].velocity.Y, 0);
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(self.cShoe, self);
            }
			orig(self);
        }
		public override void OnRespawn()
		{
			for (int i = 0; i < oldCenters.Length; i++)
			{
				oldCenters[i] = Player.Center;
				oldPositions[i] = Player.position;
			}
		}
		public override void ResetEffects()
		{
			//Functions.Chatic(Stamina);
            lightnings = new List<Terrapain.LightningDrawInfo>();
			bootsActiveAccessory = false;

			StarFuryBrasslet = false;
			ExplosiveSkull = false;
			RangerGranithShellArmorSet = false;
			StarPowerSet = false;
			CactusSet = false;
			AcidCobwebBonus = false;
			GranithShellChestplateBonus = false;

			if (unarmed && !ClientConfig.Instance.UnarmedMouseAlwaysActive)
			{
				if (UnarmedMouseActiveTimer > 0)
				{
					UnarmedMouseActiveTimer--;
				}

                if (Main.lastMouseX != Main.mouseX || Main.lastMouseY != Main.mouseY)
				{
					UnarmedMouseActiveTimer = (int)(ClientConfig.Instance.UnarmedMouseActiveTime * 60);
				}
			}

			MaxStamina = 100;
			StaminaRegenerationAcceleration = 0.02f;
			MaxStaminaRegenerationSpeed = 0.8f;
			StaminaRegenerationTimerMax = 150;
			staminaUsageMultiplyer = 1;

			if (AcidCobwebBonusReload > 0)
			{ 
				AcidCobwebBonusReload--;
			}
			if (ThornsSpikeReload > 0)
			{
				ThornsSpikeReload--;
			}
			if (StarPowerSetReload > 0)
			{
				StarPowerSetReload--;
			}

			if (StaminaRegenerationTimer > 0)
			{
				if (!Player.ItemAnimationActive)
				{
					StaminaRegenerationTimer--;
				}
			}
			else
			{
				StaminaRegeneration = MathF.Min(StaminaRegeneration + StaminaRegenerationAcceleration, MaxStaminaRegenerationSpeed);
			}
			StaminaRegenerationTimerCoefficient = 60;
			Stamina = MathHelper.Clamp(Stamina + StaminaRegeneration, 0, MaxStamina);
			Player.GetDamage(DamageClass.Melee) *= StaminaDamageBuff;
			if (Stamina < MaxStamina)
			{
                TerrapainUIManager.Open<Stamina>();
            }

			_dash = null;
		}
		public void ResetAbilities(string reason)
		{
			List<Item> accessoties = Functions.GetActiveAccessories(Player);
			foreach (var item in accessoties)
			{
				item.GetT().ActiveAccessoryVanillaItem.ResetAbilities(reason);
			}
		}
		public override void PreUpdate()
		{
			for (int i = oldCenters.Length - 1; i > 0; i--)
			{
				oldCenters[i] = oldCenters[i - 1];
				oldPositions[i] = oldPositions[i - 1];
				oldVelocities[i] = oldVelocities[i - 1];
			}
			oldCenters[0] = Player.Center;
			oldPositions[0] = Player.position;
			oldVelocities[0] = Player.velocity;
			if (Player.statLife <= Player.statLifeMax2 * 0.25f && AcidCobwebBonusReload == 0 && AcidCobwebBonus)
			{
                int proj = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Functions.UnitVectorFromRotation(playerRandom.NextFloat() * (float)Math.PI * 2 - (float)Math.PI) * (playerRandom.NextFloat() + 2.5f), ModContent.ProjectileType<ScorspiderWeb>(), 6, 0, Player.whoAmI);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
                AcidCobwebBonusReload = 600;
            }
			for (int i = 0; i < 7; i++)
			{
				if (Player.armor[i + 3].active && Player.armor[i + 3].GetGlobalItem<TGlobalItem>().activeAccessory)
				{
					Player.armor[i + 3].GetGlobalItem<TGlobalItem>().slot = i;

                    if (ActiveAccesoriesKeybinds[i].Current || ActiveAccesoriesKeybinds[i].JustReleased)
					{
						//if (Player.armor[i + 3].GetGlobalItem<TGlobalItem>().ActiveAccesoryModItem != null)
						//{
						//	Player.armor[i + 3].GetGlobalItem<TGlobalItem>().ActiveAccesoryModItem.TryUseAbilty(Player); 
						//}
						{
                            Player.armor[i + 3].GetGlobalItem<TGlobalItem>().ActiveAccessoryVanillaItem.TryUseAbilty(Player, Player.armor[i + 3], ActiveAccesoriesKeybinds[i].JustReleased);
                        }
					}
				}
			}
			for (int i = 0; i < shootingStarInfo.Count; i++)
			{
				if (shootingStarInfo[i].time > 0)
				{
					ShootingStars(shootingStarInfo[i], i);
				}
				if (shootingStarInfo[i].time == 0)
				{
					shootingStarInfo.RemoveAt(i);
					i--;
				}
			}
		}
		public override void PostUpdate()
		{
			oldLifeRegen = Player.lifeRegen;
			if (ExplosiveSkullReload > 0)
			{
				ExplosiveSkullReload--;
			}
            if (StarFuryBrassletReload > 0)
            {
                StarFuryBrassletReload--;
            }
            if (unarmed)
			{
				Player.maxMinions = 0;
				Player.maxTurrets = 0;
				Player.UpdateMaxTurrets();
			}
            if (brokenHeartLevel > 0)
            {
				PlayerStatsSnapshot snapshot = new(Player);

                float oneHeart = 1f / snapshot.AmountOfLifeHearts;
                int life = (int)(Player.statLifeMax2 * oneHeart);
                if (brokenHeartLevel >= 2)
                {
                    life += (int)(Player.statLifeMax2 * oneHeart) * 2;
                }
                if (brokenHeartLevel == 3)
                {
                    life += (int)(Player.statLifeMax2 * oneHeart) * 3;
                }
                Player.statLife = Math.Min(Player.statLife, Player.statLifeMax2 - life);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (StarPowerSet && StarPowerSetReload == 0)
			{
				ShootingStars(25, 5, (int)Player.GetDamage<Unarmed>().ApplyTo(20), 3, 10, ModContent.GetInstance<Unarmed>(), target.Center + Vector2.UnitY * -800, target.Center, false, false);
				StarPowerSetReload = 90;
			}
        }
        public struct ShootingStarInfo
        {
	        public int time;
			public int shootRate;
		    public int damage;
			public float knockback;
			public float speed;
			public DamageClass damageClass;
			public Vector2 position;
			public Vector2 targetPosition;
			public bool targetClosest;
			public bool folowPlayer;
			public int projectileType;
			public float range;
        }
		public List<ShootingStarInfo> shootingStarInfo = new List<ShootingStarInfo>();
        public void ShootingStars(int time, int shootRate, int damage, float knockback, float speed, DamageClass damageClass, Vector2 position, Vector2 targetPosition, bool targetClosest, bool folowPlayer, int projectileType = ProjectileID.Starfury, float range = 60, int i = -1)
		{
			if (folowPlayer)
			{
				position = Player.Center;
			}
			if (targetClosest)
			{
                NPC npc = Functions.FindClosestNPC(position);
                if (npc != null && npc.Distance(position) < 1000)
				{ 
					targetPosition = Functions.FindClosestNPC(position).Center; 
				}
				else if (folowPlayer)
				{
					if (Player.velocity != Vector2.Zero)
						targetPosition = position + Player.velocity;
					else
						targetPosition = position + Player.direction * Vector2.UnitX;
                }
				else
				{
					targetPosition = position + Vector2.UnitY;
				}
			}
			if (time % shootRate == 0)
			{
				UnifiedRandom random = new UnifiedRandom();
                int proj = Projectile.NewProjectile(Player.GetSource_FromThis(), position + new Vector2(random.NextFloat(-range / 2, range / 2), random.NextFloat(-range / 2, range / 2)), (targetPosition - position).Normalized().RotatedByRandom(0.5f) * speed, projectileType, damage, knockback, ai1: targetPosition.Y);
				Main.projectile[proj].DamageType = damageClass;
            }
			time--;

			ShootingStarInfo newShootingStarInfo = new ShootingStarInfo()
			{
				time = time,
				shootRate = shootRate,
				damage = damage,
				knockback = knockback,
				speed = speed,
				damageClass = damageClass,
				position = position,
				targetPosition = targetPosition,
				targetClosest = targetClosest,
				folowPlayer = folowPlayer,
				projectileType = projectileType,
				range = range,
			};
			if (i != -1)
			{
				shootingStarInfo[i] = newShootingStarInfo;
			}
			else
			{
				shootingStarInfo.Add(newShootingStarInfo);
			}
        }
		public void ShootingStars(ShootingStarInfo shootingStar, int i)
		{
			ShootingStars(shootingStar.time, shootingStar.shootRate, shootingStar.damage, shootingStar.knockback, shootingStar.speed, shootingStar.damageClass, shootingStar.position, shootingStar.targetPosition, shootingStar.targetClosest, shootingStar.folowPlayer, shootingStar.projectileType, shootingStar.range, i);
        }
		public override void ModifyHitByNPC(NPC npc, ref Terraria.Player.HurtModifiers modifiers)
		{
			if (GranithShellChestplateBonus)
			{
				npc.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 300);
			}
			if (AcidCobwebBonus && AcidCobwebBonusReload == 0)
			{
				int proj = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.DirectionTo(npc.Center) * 3.5f, ModContent.ProjectileType<ScorspiderWeb>(), 6, 0, Player.whoAmI);
				Main.projectile[proj].friendly = true;
				Main.projectile[proj].hostile = false;
				AcidCobwebBonusReload = 600;
			}
		}
        public override void ModifyHurt(ref Terraria.Player.HurtModifiers modifiers)
        {
			if (GranithShellChestplateBonus)
				modifiers.FinalDamage *= 0.95f;
        }
		public override void TransformDrawData(ref PlayerDrawSet drawInfo)
		{
			Texture2D texture = null;
			Texture2D textureSample = null;
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<GranithStuff>())
			{
				textureSample = ModContent.Request<Texture2D>(drawInfo.drawPlayer.HeldItem.ModItem.Texture).Value;
				texture = ModContent.Request<Texture2D>(drawInfo.drawPlayer.HeldItem.ModItem.Texture + "_Held").Value;
			}
			for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
			{
				if (drawInfo.DrawDataCache[i].texture == textureSample)
				{
					DrawData data = drawInfo.DrawDataCache[i];
					data.texture = texture;
					data.sourceRect = texture.Bounds;
					drawInfo.DrawDataCache[i] = data;
				}
			}
		}
        public override void SaveData(TagCompound tag)
        {
			tag["Unarmed"] = unarmed;
        }
		public override void LoadData(TagCompound tag)
		{
			unarmed = tag.Get<bool>("Unarmed");
        }
        public override void PostUpdateBuffs()
        {
            for (int i = 0; i < auras.Count; i++)
			{
				Aura aura = auras[i];
				if (aura.timeLeft == 0)
				{
					auras.RemoveAt(i);
					i--;
				}
				else
				{
					aura.Update();
				}
			}
			if (!Player.HasBuff(ModContent.BuffType<BrokenHeart>()))
			{
				brokenHeartLevel = 0;
			}
			if (brokenHeartLevel > 0)
			{
				Player.lifeRegen += 4;
				if (brokenHeartLevel >= 2)
				{
					Player.GetCritChance(DamageClass.Generic) += 4;
				}
				if (brokenHeartLevel == 3)
				{
					Player.GetDamage(DamageClass.Generic) *= 1.1f;
				}
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			if (drawInfo.shadow == 0f)
			{
                foreach (var Aura in auras)
                {
                    Aura.Draw(Main.spriteBatch);
                }
            }
        }
        //int pp;
        //  public override bool? CanHitNPCWithItem(Item item, NPC target)
        //      {
        //	pp++;
        //	Functions.Chatic("pp", pp);
        //	return true;
        //      }
        //int ll;
        //      public override bool CanHitNPC(NPC target)
        //      {
        //	ll++;
        //	Functions.Chatic("ll", ll);
        //          return base.CanHitNPC(target);
        //      }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
			if (TGlobalItem.UseDrawOverride.Contains(drawInfo.drawPlayer.HeldItem.useStyle) && drawInfo.drawPlayer.ItemAnimationActive)
				drawInfo.heldItem.noUseGraphic = true;
        }
		public void PostDrawPlayer(SpriteBatch sprite)
		{
			for (int i = 0; i < lightnings.Count; i++)
			{
				sprite.DrawLightning(lightnings[i]);
            }
			Texture2D texture = ExtraTextureRegistry.WhitePixel.Value;
		}
		public int chaosRegen;
		public float chaosSpeed;
		public float chaosAcceleration;
		public int chaosDefense;
		public float chaosDamage;

		public void NewChaosBuff()
		{
			float[] grades = new float[6];
			int buff = playerRandom.Next(0, 6);
			grades[buff] = 1;
            buff = playerRandom.Next(0, 5);
			for (int i = 0; i < grades.Length; i++)
			{
				if (grades[i] != 0)
				{
					i++;
					buff++;

                }
				if (i == buff)
				{
					grades[i] = -0.75f;
                }
			}
            buff = playerRandom.Next(0, 4);
            for (int i = 0; i < grades.Length; i++)
            {
                if (grades[i] != 0)
                {
                    i++;
					buff++;
                }
                if (i == buff)
                {
                    grades[i] = 2;
                }
            }
            buff = playerRandom.Next(0, 3);
            for (int i = 0; i < grades.Length; i++)
            {
                if (grades[i] != 0)
                {
                    i++;
					buff++;
                }
                if (i == buff)
                {
                    grades[i] = -1.5f;
                }
            }
			chaosRegen = (int)(2 * grades[0]);
			chaosDamage = 0.25f * grades[1];
			chaosDefense = (int)(4 * grades[2]);
			chaosSpeed = 0.25f * grades[3];
			chaosAcceleration = 0.25f * grades[4];
			Player.statLife += (int)(25 * grades[5]);
			if (Player.statLife < 0)
			{
				Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Terrapain.NetworkText.BadPotion")), 666, -Player.direction);
			}
        }
		public void UpdateChaosBuff()
		{
			Player.lifeRegen += chaosRegen;
			Player.maxRunSpeed *= 1 + chaosSpeed;
			Player.runAcceleration *= 1 + chaosAcceleration;
			Player.statDefense += chaosDefense;
			Player.GetDamage(DamageClass.Generic) *= 1 + chaosDamage;
		}
		public int brokenHeartLevel;
    }
}