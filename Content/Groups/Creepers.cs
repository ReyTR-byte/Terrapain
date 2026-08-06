using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.NPCs.Servants.EvilBosses;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.ID;

namespace Terrapain.Content.Groups
{
    public class Creepers : Group
    {
        public Creepers(float rotationSpeed) 
        {
            this.rotationSpeed = rotationSpeed;
            maxRotationSpeed = rotationSpeed;
        }
        public override void OnInitialize()
        {
            int creeper = (int)Main.npc[members[1]].ai[0];
            while (creeper > 0)
            {
                AddMember(creeper);
                creeper = (int)Main.npc[creeper].ai[0];
            }
            maxRotationSpeed = rotationSpeed;
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
            autoDisable = false;
        }
        public override int[] NPCType => [NPCID.Creeper];
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
        public override void PreDrawFirstNPCInGroup(SpriteBatch spriteBatch)
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
