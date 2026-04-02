using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs
{
    internal class DaybloomInfection : ModBuff
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
            npc.lifeRegen -= 1;
            npc.GetGlobalNPC<TGlobalNPC>().damageMultiplier *= 0.9f;
        }
    }
}
