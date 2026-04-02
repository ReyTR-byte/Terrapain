using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.DamageClasses
{
    public class Unarmed : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );
        }

        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<Unarmed>() += 4;
        }
    }
}