using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public class GroupSystem : ModSystem
    {
        public override void PostUpdateNPCs()
        {
            for (int i = 0; i < Terrapain.group.Length; i++)
            {
                Group g = Terrapain.group[i];
                if (g != null)
                {
                    g.UpdateGroup();
                }
            }
        }
    }
}
