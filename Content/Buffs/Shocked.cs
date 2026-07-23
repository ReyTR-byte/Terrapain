using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terrapain.Content.Auras;
using Terrapain.Common.System;
using Terrapain.Content.Dusts;

namespace Terrapain.Content.Buffs
{
    public class Shocked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }
        static UnifiedRandom random = new UnifiedRandom();
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 12;
            npc.velocity *= 1 - 0.05f * npc.knockBackResist;
            if (random.Next(3) == 0)
                Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Lightning>());
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (WorldDifficultySystem.suicide && player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }
            player.lifeRegen -= 10;
            player.GetDamage(DamageClass.Generic) *= 1.5f;
            Vector2 start = new Vector2(random.NextFloat(player.position.X - 10, player.Left.X + 10), random.NextFloat(player.position.Y - 10, player.Bottom.Y + 10));
            Vector2 end = new Vector2(random.NextFloat(player.position.X - 10, player.Left.X + 10), random.NextFloat(player.position.Y - 10, player.Bottom.Y + 10));
            if (player.Custom().unarmed && !player.Custom().HasAura<LightningAura>())
            {
                player.Custom().auras.Add(new LightningAura(player.whoAmI));
            }
            if (random.Next(3) == 0)
            {
                Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<Lightning>());
            }
        }
    }
}
