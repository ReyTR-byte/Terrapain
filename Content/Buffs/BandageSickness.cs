using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Buffs
{
	public class BandageSickness : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
	}
}