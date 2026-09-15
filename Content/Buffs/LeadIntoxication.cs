using Terrapain.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs
{
    public class LeadIntoxication : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }
        float Power;
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            Power += 0.2f;
            return false;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            Power += 0.2f;
            return false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= (int)Power;
            Power -= 0.00025f;
            npc.buffTime[buffIndex] = (int)(Power / 0.0025f);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= (int)Power;
            Power -= 0.005f;
            player.buffTime[buffIndex] = (int)(Power / 0.005f);
        }
    }
}