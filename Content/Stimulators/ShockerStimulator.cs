using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Content.Stimulators
{
    public class ShockerStimulator : Stimulator
    {
        public ShockerStimulator(Item item)
        {
            sourceItem = item;
        }
        public override bool OnUse(Player player)
        {
            bool hasBuff = player.HasBuff<Shocked>();
            bool canuse = sourceItem.GetT().ActiveAccessoryVanillaItem.CanUseAbility(player, sourceItem);
            if (!hasBuff && canuse)
            {
                player.AddBuff(ModContent.BuffType<Shocked>(), player.Custom().Dash.DashDuration);
                sourceItem.GetT().ActiveAccessoryVanillaItem.AbilityReload += 45;
                Player.HurtInfo hurt = new Player.HurtInfo { Damage = 6, Dodgeable = false, Knockback = 0, SourceDamage = 6, CooldownCounter = 0, Cancelled = false, DamageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Terrapain.NetworkText.ShockDeath" + new string(player.Male ? "Male" : "Female"), player.name)), SoundDisabled = true };
                player.Hurt(hurt);
            }
            return !hasBuff && canuse;
        }
    }
}
