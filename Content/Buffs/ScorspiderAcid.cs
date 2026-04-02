using Terrapain.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs
{
    public class ScorspiderAcid : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 20;
            npc.GetGlobalNPC<TGlobalNPC>().ignoreDefence = 2;
            npc.GetGlobalNPC<TGlobalNPC>().damageMultiplier *= 0.95f;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 12;
            player.maxRunSpeed -= 3;
            player.statDefense -= 10;
            player.wingTime -= 0.4f;
            player.GetDamage(DamageClass.Generic) *= 0.9f;
        }
	}
}