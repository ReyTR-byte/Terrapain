using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class VanillaMeleeWeaponStaminaUsage : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.DamageType == DamageClass.Melee || entity.DamageType == DamageClass.MeleeNoSpeed;
        }
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                case ItemID.FieryGreatsword:
                    entity.GetT().StaminaUsage = 3;
                    break;
            }
        }
    }
}
