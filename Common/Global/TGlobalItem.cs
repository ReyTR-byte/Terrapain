using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Common.System;
using Terrapain.Common.UI;
using Terrapain.Content;
using Terrapain.Content.Items.Abstract;
using Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories;
using Terrapain.Content.NPCs.Bosses.Scorspider;
using Terrapain.Content.Projectiles.Enemies.Bosses.Scorspider;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Common.Global
{
	public class TGlobalItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public bool activeAccessory;
        public bool dashAccessory;
		public bool CanUse = true;
        public float ShootRotation;
		public float ShootSpeedBonus;
		public int dust = -1;
		public Vector3 dustLight;
		public static int SharperUseStyle;
		public static int MassiveSwing;
		public static int NormalSwing;
		public static int LightSwing;
		public static int BatUseStyle;
        public static int LaserUseStyle;
        public static int ShootOverride;
        public static int BowOverride;
        public int drawDir;
		public static List<int> UseModDrawStyles = new List<int>();
		public static Vector2 basicOffset = Vector2.UnitX * 10 + Vector2.UnitY * 2;
		public float? spriteRotation = null;
		public Vector2? DrawOrigin = null;
		public bool[] hitList = new bool[Main.npc.Length];
		float usedShootSpeedBonus;

		public ActiveAccessory ActiveAccesoryModItem;
		public VanillaItemActiveAccessory ActiveAccessoryVanillaItem;

		public int[] MassiveSwords =
		{
			121,
			//ItemID.CopperAxe,
			//ItemID.GoldAxe,
   //         ItemID.CopperPickaxe,
			//ItemID.AcornAxe,
			//ItemID.IronAxe,
			//ItemID.LeadAxe,
   //         ItemID.LucyTheAxe,
			//ItemID.NebulaAxe,
			//ItemID.PickaxeAxe,
			//ItemID.PlatinumAxe,
			//ItemID.SilverAxe,
			//ItemID.SolarFlareAxe,
			//ItemID.StardustAxe,
			//ItemID.TheAxe,
			//ItemID.TinAxe,
			//ItemID.TungstenAxe,
			//ItemID.VortexAxe,
			//ItemID.AdamantitePickaxe,
			//ItemID.BonePickaxe,
			//ItemID.CactusPickaxe,
			//ItemID.ChlorophytePickaxe,
			//ItemID.CnadyCanePickaxe,
			//ItemID.CobaltPickaxe,
			//ItemID.CopperPickaxe,
			//ItemID.DeathbringerPickaxe,
			//ItemID.FossilPickaxe,
			//ItemID.GoldPickaxe,
			//ItemID.IronPickaxe,
			//ItemID.LeadPickaxe,
			//ItemID.MoltenPickaxe,
			//ItemID.MythrilPickaxe,
			//ItemID.NebulaPickaxe,
			//ItemID.NightmarePickaxe,
			//ItemID.OrichalcumPickaxe,
   //         ItemID.PalladiumPickaxe,
			//ItemID.PlatinumPickaxe,
   //         ItemID.SilverPickaxe,
   //         ItemID.SolarFlarePickaxe,
   //         ItemID.SpectrePickaxe,
   //         ItemID.StardustPickaxe,
   //         ItemID.TinPickaxe,
   //         ItemID.TitaniumPickaxe,
			//ItemID.TungstenPickaxe,
   //         ItemID.VortexPickaxe,
        };
        public int[] NormalSwords =
        {
			ItemID.BoneSword,
        };
		public int[] LightSwords =
		{
			ItemID.Muramasa,
			190,
			ItemID.IronBroadsword,
			ItemID.CopperBroadsword,
			ItemID.TinBroadsword,
			ItemID.GoldBroadsword,
            ItemID.LeadBroadsword,
			ItemID.PlatinumBroadsword,
			ItemID.SilverBroadsword,
			ItemID.TungstenBroadsword,
        };
		public int[] Bows =
		{
			ItemID.AshWoodBow,
			ItemID.BeesKnees,
			ItemID.CopperBow,
			ItemID.DemonBow,
			ItemID.EbonwoodBow,
			ItemID.GiantBow,
			ItemID.GoldBow,
			ItemID.HellwingBow,
			ItemID.IceBow,
			ItemID.IronBow,
			ItemID.LeadBow,
			ItemID.Marrow,
			ItemID.MoltenFury,
			ItemID.PalmWoodBow,
			ItemID.PearlwoodBow,
			ItemID.PlatinumBow,
			ItemID.PulseBow,
			ItemID.RichMahoganyBow,
			ItemID.ShadewoodBow,
			ItemID.ShadowFlameBow,
			ItemID.SilverBow,
			ItemID.SkeletonBow,
			ItemID.TendonBow,
			ItemID.TinBow,
			ItemID.Tsunami,
			ItemID.TungstenBow,
			ItemID.WoodenBow,
		};

        public virtual void ModLoad() { }
        public override void Load()
        {
			ModLoad();
        }
		public virtual void ModSetDefaults(Item entity) { }
        public override void SetDefaults(Item entity)
        {
			if (MassiveSwords.Contains(entity.type) || ((entity.pick > 0 || entity.axe > 0 || entity.hammer > 0) && entity.useStyle == ItemUseStyleID.Swing)) {
				entity.useStyle = MassiveSwing;
				entity.useTurn = false;
			}
			else if (NormalSwords.Contains(entity.type)){
				entity.useStyle = NormalSwing;
				entity.useTurn = false;
			}
			else if (LightSwords.Contains(entity.type)){
				entity.useStyle = LightSwing;
				entity.useTurn = false;
			}
            else if (Bows.Contains(entity.type))
            {
                entity.useStyle = BowOverride;
            }
			else if (entity.type == ItemID.DiamondStaff)
			{
                entity.useStyle = LaserUseStyle;
            }
			else if (entity.useStyle == ItemUseStyleID.Shoot)
			{
				entity.useStyle = ShootOverride;
            }
            switch (entity.type)
			{
				case ItemID.SlimeCrown:
				case ItemID.SuspiciousLookingEye:
					entity.maxStack = 1;
					entity.consumable = false;
					break;
			}
			ModSetDefaults(entity);
        }
		public override bool Shoot(Item item, Terraria.Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (item.DamageType == DamageClass.Ranged && player.GetModPlayer<TerrapainPlayer>().RangerGranithShellArmorSet)
			{
				int proj;
				Vector2 Velocity;
				if (item.useTime < 9)
				{
					Velocity = velocity / velocity.Length() * 14;
					proj = Projectile.NewProjectile(source, position, Velocity, ModContent.ProjectileType<ScorspiderShellShard>(), (int)(4 * player.GetDamage(DamageClass.Ranged).Additive), 1, player.whoAmI);
					Main.projectile[proj].friendly = true;
					Main.projectile[proj].hostile = false;
				}
				else if (item.useTime < 25)
				{
					Velocity = velocity / velocity.Length() * 18;
					proj = Projectile.NewProjectile(source, position, Velocity, ModContent.ProjectileType<ScorspiderSpike>(), (int)(8 * player.GetDamage(DamageClass.Ranged).Additive), 1, player.whoAmI);
					Main.projectile[proj].friendly = true;
					Main.projectile[proj].hostile = false;
				}
				else
				{
					Velocity = velocity / velocity.Length();
					proj = Projectile.NewProjectile(source, position, Velocity, ModContent.ProjectileType<ScorspiderRocket>(), (int)(12 * player.GetDamage(DamageClass.Ranged).Additive), 1, player.whoAmI);
					Main.projectile[proj].friendly = true;
					Main.projectile[proj].hostile = false;
				}
				velocity = Vector2.Zero;
			}
			
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}
		public override void ModifyShootStats(Item item, Terraria.Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (velocity != Vector2.Zero)
			{
				velocity.Normalize();
				velocity *= item.shootSpeed + usedShootSpeedBonus;
			}
		}
		public virtual void ModOnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public override void OnHitNPC(Item item, Terraria.Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			hitList[target.whoAmI] = true;
			if (player.magmaStone && dashAccessory)
			{
				UnifiedRandom random = new UnifiedRandom();
				target.AddBuff(BuffID.OnFire, random.Next(8) < 2 ? 360 : (random.Next(8) < 5? 240 : 120));
			}
            ModOnHitNPC(item, player, target, hit, damageDone);
        }
		public virtual void ModUpdateInventory(Item item, Terraria.Player player) { }

        public override void UpdateInventory(Item item, Terraria.Player player)
		{
			usedShootSpeedBonus = ShootSpeedBonus;

			ShootSpeedBonus = 0;

			ModUpdateInventory(item, player);
		}
        public override void UpdateAccessory(Item item, Terraria.Player player, bool hideVisual)
        {
			if (ActiveAccessoryVanillaItem != null)
			{
                ActiveAccessoryVanillaItem.UpdateAccessory(player, item);
                ActiveAccessoryVanillaItem.Countdown(player, item);
            }
			if (dashAccessory || activeAccessory)
			{
				TerrapainUIManager.Open<AccessoriesReloadUI>();
			}
        }
		//offset from mounted center
		public static Vector2 GetHandOffset(Terraria.Player player) => new Vector2(-4 * player.direction, -2);

		int timer;
		public int hitTimer;
		int swingDir;
		bool resetTimer;

        public override void UseAnimation(Item item, Terraria.Player player)
        {
            if (item.useStyle == ItemUseStyleID.Swing)
            {
                player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.MountedCenter.X));
            }
        }
        public override void UseItemFrame(Item item, Terraria.Player player)
        {
            if (item.useStyle == ItemUseStyleID.Swing)
            {
                if (item.DamageType != DamageClass.SummonMeleeSpeed)
                {
                    player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-135 * player.direction));
                }
            }
        }
		public override void UseStyle(Item item, Terraria.Player player, Rectangle heldItemFrame)
		{
			if (item.useStyle == SharperUseStyle)
			{
				bool swing = false;
				if (player.itemAnimation == player.itemAnimationMax && Main.mouseLeft)
				{
					if (resetTimer)
					{
						timer = 0;
						resetTimer = false;
					}
					player.itemRotation = Functions.AngleFromVector(-player.velocity * player.direction);
					if (player.velocity == Vector2.Zero)
						player.itemRotation = Functions.AngleFromVector(Vector2.UnitX * -1);
					if (player.velocity.X != 0)
						player.ChangeDir(player.velocity.X.NonZeroSign());
					player.itemTime = player.itemAnimationMax + 1;
                    player.itemAnimation = player.itemAnimationMax + 1;
					timer++;
                }
				else
				{
					swing = true;
					if (player.itemAnimation == player.itemAnimationMax)
					{ 
						player.HeldItem.GetGlobalItem<TGlobalItem>().hitTimer = timer;
						swingDir = 1;
						if (player.velocity.Length() != 0)
						{
							swingDir = (player.velocity.Y / player.velocity.Length() + 0.8f).NonZeroSign(); 
						}
                        SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    }
                    player.itemRotation += (float)Math.PI / player.itemAnimationMax * player.direction * swingDir;
					resetTimer = true;
                }
				Vector2 offset = item.ModItem.HoldoutOffset().Value.RotatedBy(player.itemRotation * player.direction);
				offset.X *= player.direction;
				player.itemLocation = player.MountedCenter + GetHandOffset(player) + offset;
				player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, player.itemRotation - 0.5f * (float)Math.PI * player.direction);
				player.bodyFrame.Y = player.bodyFrame.Height;

				UnifiedRandom random = new UnifiedRandom();

				if (dust != -1 && random.Next(2) == 0 && timer >= 100)
				{
					bool oldNormalHitbox = normalHitbox;
                    Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
                    bool noHitbox = false;
                    normalHitbox = true;
                    UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
					normalHitbox = oldNormalHitbox;
					Vector2 aditiveVelocity = Vector2.Zero;
					if (swing)
					{
						Vector2 center = miniHitbox.Location.ToVector2() + miniHitbox.Size() / 2;
						Vector2 aboutPlayer = center - player.MountedCenter - GetHandOffset(player);
						aditiveVelocity = aboutPlayer.RotatedBy((float)Math.PI / player.itemAnimationMax * player.direction * swingDir) - aboutPlayer;
                    }
					int d = Dust.NewDust(miniHitbox.Location.ToVector2(), miniHitbox.Width, miniHitbox.Height, dust, player.velocity.X + aditiveVelocity.X, player.velocity.Y + aditiveVelocity.Y);
					TGlobalDust.dustLights[d] = dustLight;
                }
			}
		}
		List<int> dusts = new List<int>();
		int _Dust = -1;
		bool normalHitbox = true;
        public override bool? CanHitNPC(Item item, Terraria.Player player, NPC target)
        {
			if (item.useStyle == SharperUseStyle)
			{
				Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
				bool noHitbox = false;
				normalHitbox = true;
				UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
				normalHitbox = false;
				if (noHitbox)
				{
					return false;
				}
				if (Functions.RectangleColision(target.Hitbox, miniHitbox))
				{
                    return null;
				}
				Vector2 rotatin = Functions.UnitVectorFromRotation(player.itemRotation) * player.direction;
				if (Functions.Collision(player.MountedCenter + GetHandOffset(player), player.MountedCenter + GetHandOffset(player) + rotatin * item.ModItem.HoldoutOffset().Value.X, item.width, target.position, target.width, target.height))
				{
                    return null;
				}
                return false;
			}
			return null;
        }
        public override bool CanHitPvp(Item item, Terraria.Player player, Terraria.Player target)
        {
            if (item.useStyle == SharperUseStyle)
            {
                Rectangle miniHitbox = new Rectangle(0, 0, player.itemWidth, player.itemHeight);
                bool noHitbox = false;
                normalHitbox = true;
                UseItemHitbox(item, player, ref miniHitbox, ref noHitbox);
                normalHitbox = false;
                if (noHitbox)
                {
                    return false;
                }
                if (Functions.RectangleColision(target.Hitbox, miniHitbox))
                {
                    return true;
                }
                Vector2 rotatin = Functions.UnitVectorFromRotation(player.itemRotation) * player.direction;
                if (Functions.Collision(player.MountedCenter + GetHandOffset(player), player.MountedCenter + GetHandOffset(player) + rotatin * item.ModItem.HoldoutOffset().Value.X, item.width, target.position, target.width, target.height))
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public override void ModifyHitNPC(Item item, Terraria.Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item.useStyle == SharperUseStyle)
			{
				if (item.GetGlobalItem<TGlobalItem>().hitTimer < item.useTime * 10)
				{
					//Functions.Chatic(item.GetGlobalItem<TGlobalItem>().hitTimer, item.useTime * 10, item.useTime);
					modifiers.TargetDamageMultiplier *= 0.5f;
					return;
				}
                modifiers.TargetDamageMultiplier *= player.velocity.Length() * 2f + 1;
            }
        }
        public override void ModifyHitPvp(Item item, Terraria.Player player, Terraria.Player target, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (item.useStyle == SharperUseStyle)
            {
                if (item.GetGlobalItem<TGlobalItem>().hitTimer < item.useTime * 10)
                {
                    modifiers.FinalDamage *= 0.5f;
                    return;
                }
                modifiers.FinalDamage *= player.velocity.Length() * 2f + 1;
            }
        }
		public int slot;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (activeAccessory && (!ActiveAccessoryVanillaItem.AbilityUnarmedOnly || Main.LocalPlayer.Custom().unarmed))
			{
				if (ActiveAccessoryVanillaItem.DescriptionLinesCount > 0)
				{
					TooltipLine line = new TooltipLine(Mod, "Ability", NetworkText.FromKey("Mods.Terrapain.Ability") + ": " + NetworkText.FromKey(ActiveAccessoryVanillaItem.AbilityDescription + "_0"));
					tooltips.Add(line);
					for (int i = 1; i < ActiveAccessoryVanillaItem.DescriptionLinesCount; i++)
					{
						line = new TooltipLine(Mod, $"Ability{i}", NetworkText.FromKey(ActiveAccessoryVanillaItem.AbilityDescription + $"_{i}").ToString());
						tooltips.Add(line);
					}
				}
				if (slot > -1 && slot < 7 && !ActiveAccessoryVanillaItem.AutoUse)
				{
                    TooltipLine line = new TooltipLine(Mod, "PressXToActivate", NetworkText.FromKey("Mods.Terrapain.PressXToActivate", KeybindSystem.ActiveAccesories[slot].GetAssignedKeys()[0]).ToString());
					tooltips.Add(line);
				}
			    if (ActiveAccessoryVanillaItem.CanAutoUse)
				{
					if (ActiveAccessoryVanillaItem.AutoUse)
					{
						if (slot > -1 && slot < 7 && !ActiveAccessoryVanillaItem.AutoUse)
						{
							TooltipLine line = new TooltipLine(Mod, "AutoUse", NetworkText.FromKey("Mods.Terrapain.AutoUseIsOn", KeybindSystem.ActiveAccesories[slot].GetAssignedKeys()[0]).ToString());
							tooltips.Add(line);
						}
						else
						{
                            TooltipLine line = new TooltipLine(Mod, "AutoUse", NetworkText.FromKey("Mods.Terrapain.AutoUseIsOnNoKey").ToString());
                            tooltips.Add(line);
                        }	
					}
					else
					{
						TooltipLine line = new TooltipLine(Mod, "AutoUse", NetworkText.FromKey("Mods.Terrapain.AutoUseIsOff").ToString());
						tooltips.Add(line);
					}
				}
            }
        }
        public override bool CanRightClick(Item item)
        {
			return ActiveAccessoryVanillaItem?.CanAutoUse?? false;
        }
        public override void RightClick(Item item, Terraria.Player player)
        {
			if (ActiveAccessoryVanillaItem != null && ActiveAccessoryVanillaItem.CanAutoUse)
				ActiveAccessoryVanillaItem.AutoUse = !ActiveAccessoryVanillaItem.AutoUse;
        }
        public override void UseItemHitbox(Item item, Terraria.Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
			if (item.useStyle == SharperUseStyle)
			{
				if (player.itemAnimation == player.itemAnimationMax + 1 || player.itemAnimation == player.itemAnimationMax)
				{
					noHitbox = true;
				}
				if (normalHitbox)
				{
					Vector2 location = player.itemLocation;
					Vector2 offset = new Vector2(hitbox.Width - hitbox.Height / 2, -hitbox.Height / 2 * player.direction);
					location += offset.RotatedBy(player.itemRotation) * player.direction;
					hitbox.Width = hitbox.Height;
					hitbox.Location = (location - hitbox.Size() / 2).ToPoint();
				}
				else
				{
                    Vector2 location = player.itemLocation;
                    Vector2 offset = new Vector2(hitbox.Width - hitbox.Height / 2, -hitbox.Height / 2 * player.direction);
                    location += offset.RotatedBy(player.itemRotation) * player.direction;
                    hitbox.Width = hitbox.Height;
                    hitbox.Location = (location - hitbox.Size() / 2).ToPoint();
					if (hitbox.X > player.MountedCenter.X + GetHandOffset(player).X)
					{
						hitbox.Width = hitbox.X + hitbox.Width - (int)(player.MountedCenter.X + GetHandOffset(player).X);
						hitbox.X = (int)(player.MountedCenter.X + GetHandOffset(player).X);
                    }
                    if (hitbox.X + hitbox.Width < player.MountedCenter.X)
                    {
                        hitbox.Width = (int)(player.MountedCenter.X + GetHandOffset(player).X) - hitbox.X;
                    }
                    if (hitbox.Y > player.MountedCenter.Y + GetHandOffset(player).Y)
                    {
                        hitbox.Height = hitbox.X + hitbox.Height - (int)(player.MountedCenter.Y + GetHandOffset(player).Y);
                        hitbox.Y = (int)(player.MountedCenter.Y + GetHandOffset(player).Y);
                    }
                    if (hitbox.Y + hitbox.Height < player.MountedCenter.Y)
                    {
                        hitbox.Height = (int)(player.MountedCenter.Y + GetHandOffset(player).X) - hitbox.Y;
                    }
                    normalHitbox = true;
				}
			}

			//for (int i = 0; i < dusts.Count; i++)
			//{
			//	if (!Main.dust[dusts[i]].active || Main.dust[dusts[i]].type != DustID.Torch)
			//	{
			//		dusts.Remove(dusts[i]);
			//		i--;
			//	}
			//}
			//int goalcount = hitbox.Width * 2 + hitbox.Height * 2;
			//while (dusts.Count < goalcount)
			//{
			//	dusts.Add(Dust.NewDust(hitbox.Location.ToVector2(), 0, 0, DustID.Torch));
			//}
			//for (int i = 0; i < dusts.Count; i++)
			//{
			//	if (i < hitbox.Width)
			//		Main.dust[dusts[i]].position = hitbox.Location.ToVector2() + Vector2.UnitX * i;
			//	else if (i < hitbox.Width * 2)
			//		Main.dust[dusts[i]].position = hitbox.Location.ToVector2() + Vector2.UnitY * hitbox.Height + Vector2.UnitX * (i - hitbox.Width);
			//	else if (i < hitbox.Width * 2 + hitbox.Height)
			//		Main.dust[dusts[i]].position = hitbox.Location.ToVector2() + Vector2.UnitY * (i - hitbox.Width * 2);
			//	else
			//		Main.dust[dusts[i]].position = hitbox.Location.ToVector2() + Vector2.UnitY * (i - hitbox.Width * 2 - hitbox.Height) + Vector2.UnitX * hitbox.Width;
			//	Main.dust[dusts[i]].velocity = Vector2.Zero;
			//	Main.dust[dusts[i]].scale = 0.5f;
			//}
			//if (_Dust == -1 || dusts.Contains(_Dust) || !Main.dust[_Dust].active || Main.dust[_Dust].type != DustID.Torch)
			//{
			//	_Dust = Dust.NewDust(hitbox.Location.ToVector2(), 0, 0, DustID.Torch);
			//}
			//Main.dust[_Dust].position = player.itemLocation;
			//Main.dust[_Dust].velocity = Vector2.Zero;
			//Main.dust[_Dust].scale = 2f;

		}

        public override bool CanUseItem(Item item, Terraria.Player player)
        {
			if (player.GetModPlayer<TerrapainPlayer>().unarmed && item.damage > 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0 && !item.accessory && !Main.projHook[item.shoot])
			{
				return false;
			}
			if (!CanUse)
			{
				return false; 
			}
			return base.CanUseItem(item, player);
        }
	}
}