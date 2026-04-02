using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Player;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Terrapain.Content;

namespace Terrapain.Common.Global
{
    public class OrganismOverload : GlobalBuff
    {
        List<int> OrganismOverloadingBuffs =
        [
            BuffID.ObsidianSkin,
            BuffID.Regeneration,
            BuffID.Swiftness,
            BuffID.Gills,
            BuffID.Ironskin,
            BuffID.ManaRegeneration,
            BuffID.MagicPower,
            BuffID.Featherfall,
            BuffID.Spelunker,
            BuffID.Invisibility,
            BuffID.Shine,
            BuffID.NightOwl,
            BuffID.Thorns,
            BuffID.WaterWalking,
            BuffID.Archery,
            BuffID.Hunter,
            BuffID.Gravitation,
            BuffID.Poisoned,
            BuffID.PotionSickness,
            BuffID.Darkness,
            BuffID.Cursed,
            BuffID.OnFire,
            BuffID.Tipsy,
            BuffID.Werewolf,
            BuffID.Bleeding,
            BuffID.Confused,
            BuffID.Slow,
            BuffID.Weak,
            BuffID.Merfolk,
            BuffID.Silenced,
            BuffID.CursedInferno,
            BuffID.Frostburn,
            BuffID.Chilled,
            BuffID.Frozen,
            BuffID.RapidHealing,
            BuffID.ShadowDodge,
            BuffID.IceBarrier,
            BuffID.Panic,
            BuffID.Burning,
            BuffID.Suffocation,
            BuffID.Ichor,
            BuffID.Venom,
            BuffID.Midas,
            BuffID.Blackout,
            BuffID.ChaosState,
            BuffID.ManaSickness,
            BuffID.Mining,
            BuffID.Heartreach,
            BuffID.Calm,
            BuffID.Builder,
            BuffID.Titan,
            BuffID.Flipper,
            BuffID.Summoning,
            BuffID.Dangersense,
            BuffID.Lifeforce,
            BuffID.Endurance,
            BuffID.Rage,
            BuffID.Inferno,
            BuffID.Wrath,
            BuffID.Fishing,
            BuffID.Sonar,
            BuffID.Crate,
            BuffID.Warmth,
            BuffID.Electrified,
            BuffID.MoonLeech,
            BuffID.Rabies,
            BuffID.SoulDrain,
            BuffID.ShadowFlame,
            BuffID.Stoned,
            BuffID.Dazed,
            BuffID.Obstructed,
            BuffID.VortexDebuff,
            BuffID.BoneJavelin,
            BuffID.SolarShield1,
            BuffID.SolarShield2,
            BuffID.SolarShield3,
            BuffID.NebulaUpLife1,
            BuffID.NebulaUpLife2,
            BuffID.NebulaUpLife3,
            BuffID.NebulaUpMana1,
            BuffID.NebulaUpMana2,
            BuffID.NebulaUpMana3,
            BuffID.NebulaUpDmg1,
            BuffID.NebulaUpDmg2,
            BuffID.NebulaUpDmg3,
            BuffID.DryadsWardDebuff,
            BuffID.Daybreak,
            BuffID.SugarRush,
            BuffID.OgreSpit,
            BuffID.ParryDamageBuff,
            BuffID.NoBuilding,
            BuffID.BetsysCurse,
            BuffID.OnFire3,
            BuffID.Frostburn2,
            BuffID.NeutralHunger,
            BuffID.Hunger,
            BuffID.Starving,
        ];
        List<bool> BuffIsOverloading = new List<bool>();
        List<int> delBuff => Main.player[Main.myPlayer].GetModPlayer<PlayerOrganismOverload>().RemovedBuffs;
        public override void SetStaticDefaults()
        {
            OrganismOverloadingBuffs.AddRange([ModContent.BuffType<BandageSickness>(),
            ModContent.BuffType<ScorspiderAcid>(),
            ModContent.BuffType<Shocked>()]);
            foreach (var ID in OrganismOverloadingBuffs)
            {
                while (BuffIsOverloading.ToArray().Length <= ID)
                {
                    BuffIsOverloading.Add(false);
                }
                BuffIsOverloading[ID] = true;
            }
        }

        public override bool RightClick(int type, int buffIndex)
        {
            if (!Main.debuff[type])
            {
                bool hasBuff = true;
                foreach (int i in delBuff)
                {
                    if (i == type)
                    {
                        hasBuff = false;
                    }
                }
                delBuff.Remove(type);
                if (hasBuff)   
                {
                    Main.player[Main.myPlayer].GetModPlayer<PlayerOrganismOverload>().RemovedBuffs.Add(type);
                }
            }    
            return false;
        }
        public override void Update(int type, Terraria.Player player, ref int buffIndex)
        {
            if (WorldDifficultySystem.suicide)
            {
                if (type < BuffIsOverloading.ToArray().Length && BuffIsOverloading[type])
                {
                    player.GetModPlayer<PlayerOrganismOverload>().load++;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams)
        {
            foreach (int i in delBuff)
            {
                if (i == type)
                {
                    byte a = drawParams.DrawColor.A;
                    drawParams.DrawColor = Color.Gray * (a / 255f);
                    drawParams.DrawColor.A = a;
                }
            }
            return base.PreDraw(spriteBatch, type, buffIndex, ref drawParams);
        }
    }
}