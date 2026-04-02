using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories
{
    public class ClasicDashAccessory : VanillaItemActiveAccessory
    {
        public ClasicDashAccessory()
        {
            DashDuration = 15;
            DashPower = 11;
            DashReloadMax = 90;
        }
    }
}
