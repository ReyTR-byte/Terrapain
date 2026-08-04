using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Terrapain.Content.Groups
{
    public class Creepers : Group
    {
        public Creepers(int me, int anotherCreeper, float rotationSpeed) 
        {
            AddMember(me);
            AddMember(anotherCreeper);
            anotherCreeper = (int)Main.npc[anotherCreeper].ai[0];
            while (anotherCreeper > 0)
            {
                AddMember(anotherCreeper);
                anotherCreeper = (int)Main.npc[anotherCreeper].ai[0];
            }
            this.rotationSpeed = rotationSpeed;
        }
        float rotationSpeed;
        public override void UpdateGroup()
        {
            CheckMembers();
            if (members.Count == 1)
            {
                Terrapain.group[whoAmI] = null;
                return;
            }
            Vector2 center = Center;
            foreach (var member in members)
            {
                NPC mem = Main.npc[member];
                Vector2 dir = mem.Center - center;
                dir.RotateBy(rotationSpeed);
                mem.velocity += center + dir - mem.Center;
                float distance = mem.Distance(center);
                float force = distance / 40;
                force -= 0.5f;
                mem.velocity += force * dir;
            }
            rotationSpeed = MathF.Max(rotationSpeed - 0.005f, 0);
        }
    }
}
