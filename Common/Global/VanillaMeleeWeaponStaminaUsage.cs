using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.Global
{
    public class VanillaMeleeWeaponStaminaUsage : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return base.AppliesToEntity(entity, lateInstantiation);
        }
    }
}
