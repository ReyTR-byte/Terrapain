using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.NPCs.Servants.EvilBosses;
using Terrapain.Content.TUtilities.Kinematic;
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
            maxRotationSpeed = rotationSpeed;
            NPCType = [NPCID.Creeper];
            for (int i = 0; i < Count; i++)
            {
                int mem1 = 0;
                int mem2 = 0;
                if (i == Count - 1)
                {
                    if (Count == 2)
                    {
                        return;
                    }
                    mem1 = members[Count - 1];
                    mem2 = members[0];
                }
                else
                {
                    mem1 = members[i];
                    mem2 = members[i + 1];
                }
                k.Add(new kishki(new SimulatedChain(10, 10, Main.npc[mem1].Center, 0, 1), mem1, mem2));
            }
        }
        bool firstUpdate = true;
        float maxRotationSpeed;
        float rotationSpeed;
        private struct kishki
        {
            public SimulatedChain chain;
            public int owner1;
            public int owner2;
            public int timeLeft = 60;
            public float progress => timeLeft / 60f;
            public kishki(SimulatedChain chain, int owner1, int owner2)
            {
                this.chain = chain;
                this.owner1 = owner1;
                this.owner2 = owner2;
            }
            public void Update()
            {
                if (Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
                {
                    chain.Fragments[0].fixedAt = Main.npc[owner1].Center;
                }
                else
                {
                    chain.Fragments[0].fixedAt = null;
                    timeLeft--;
                }
                if (Main.npc[owner2].active && Main.npc[owner2].type == NPCID.Creeper)
                {
                    chain.Fragments[chain.Count - 1].fixedAt = Main.npc[owner2].Center;
                }
                else
                {
                    chain.Fragments[chain.Count - 1].fixedAt = null;
                    if (Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
                    {
                        timeLeft--;
                    }
                }
                if (timeLeft > 0)
                    chain.Update();
            }
        }
        List<kishki> k = new();
        public override void UpdateGroup()
        {
            if (firstUpdate)
            {
                autoDisable = false;
                CheckContainsGroup();
                firstUpdate = false;
            }
            if (k.Count == 0)
            {
                Disable();
                return;
            }
            if (Count > 0)
            {
                CheckMembers();
                Vector2 velocity = AverageVelocity;
                Vector2 center = Center;
                float maxCharge = 0;
                foreach (var member in members)
                {
                    NPC mem = Main.npc[member];
                    Vector2 dir1 = (mem.Center) - center;
                    dir1.RotateBy(rotationSpeed);
                    mem.velocity = center + dir1 - mem.Center;
                    Vector2 dir = (mem.Center) - center;
                    float distance = mem.Distance(center);
                    float force = distance / 80;
                    force -= 0.5f;
                    force *= 4;
                    mem.velocity -= force * dir.ToUnit();
                    mem.velocity += velocity;
                    maxCharge = MathF.Max(mem.GetGlobalNPC<Creeper>().charge, maxCharge);
                }
                if (maxCharge >= 1)
                {
                    rotationSpeed = maxRotationSpeed;
                }
                foreach (var member in members)
                {
                    NPC mem = Main.npc[member];
                    mem.GetGlobalNPC<Creeper>().charge = maxCharge;
                }
                rotationSpeed = MathF.Max(rotationSpeed - 0.005f, 0);
            }
            for (int i = 0; i < k.Count; i++)
            {
                kishki kishka = k[i];
                kishka.Update();
                k[i] = kishka;
                if (k[i].timeLeft <= 0)
                {
                    k.RemoveAt(i);
                    i--;
                }
            }
        }
        public override void PreDrawFirstGroupNPC(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < k.Count; i++)
            {
                k[i].chain.DrawAsLines(spriteBatch, 6, Color.Red * k[i].progress);
            }
        }
        public override void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            if (Count == 0)
            {
                for (int i = 0; i < k.Count; i++)
                {
                    k[i].chain.DrawAsLines(spriteBatch, 6, Color.Red * k[i].progress);
                }
            }
        }
    }
}
