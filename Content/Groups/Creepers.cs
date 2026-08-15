using Microsoft.Xna.Framework.Graphics;
using Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses;
using Terrapain.Content.NPCs.Servants.EvilBosses;
using Terrapain.Content.TUtilities.Kinematic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Groups
{
    public class Creepers : Group
    {
        public Creepers(float rotationSpeed, int AIType) 
        {
            this.rotationSpeed = rotationSpeed;
            maxRotationSpeed = rotationSpeed;
            this.AIType = AIType;
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
            autoDisable = false;
            for (int i = 0; i < Count; i++)
            {
                int mem1;
                int mem2;
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
                k.Add(new kishki(new SimulatedChain(AIType == 1? 16 : 8, 16, Main.npc[mem1].Center, 0, 1), mem1, mem2));
            }
        }
        public int attackCount;
        public int AIType;
        float rotation;
        public override int[] NPCType => [NPCID.Creeper];
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
                if (owner1 > -1 && Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
                {
                    chain.Fragments[0].fixedAt = Main.npc[owner1].Center;
                }
                else
                {
                    owner1 = -1;
                    chain.Fragments[0].fixedAt = null;
                    timeLeft--;
                }
                if (owner2 > -1 && Main.npc[owner2].active && Main.npc[owner2].type == NPCID.Creeper)
                {
                    chain.Fragments[chain.Count - 1].fixedAt = Main.npc[owner2].Center;
                }
                else
                {
                    owner2 = -1;
                    chain.Fragments[chain.Count - 1].fixedAt = null;
                    if (owner1 > -1 && Main.npc[owner1].active && Main.npc[owner1].type == NPCID.Creeper)
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
                if (AIType == 0)
                {    
                    Vector2 velocity = AverageVelocity;
                    Vector2 center = Center;
                    float maxCharge = 0;
                    foreach (var member in members)
                    {
                        NPC mem = Main.npc[member];
                        Vector2 dir1 = mem.Center - center;
                        dir1.RotateBy(rotationSpeed);
                        mem.velocity = center + dir1 - mem.Center;
                        Vector2 dir = mem.Center - center;
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
                else if (EaterofWorldsHead.attack == 4)
                {
                    if (Main.npc[members[0]].ai[2] <= 0)
                    {
                        float minCharge = -1;
                        for (int i = 0; i < Count; i++)
                        {
                            NPC mem = Main.npc[members[i]];
                            float rot = rotation + MathF.PI * 2 * i / Count;
                            Vector2 targetPosition = EaterofWorldsHead.savedVector + Vector2.UnitX.RotatedBy(rot) * (500 + mem.GetGlobalNPC<Creeper>().charge * 60);
                            Functions.CommonTerrapainFlyingMovement(mem, targetPosition, 3f, 30, 1f, 75);
                            if(minCharge == -1)
                            {
                                minCharge = mem.GetGlobalNPC<Creeper>().charge;
                            }
                            else
                            {
                                minCharge = MathF.Min(mem.GetGlobalNPC<Creeper>().charge, minCharge);
                            }
                        }
                        if (attackCount == 2)
                        {
                            minCharge = 0;
                        }

                        foreach (var member in members)
                        {
                            NPC mem = Main.npc[member];
                            mem.GetGlobalNPC<Creeper>().charge = minCharge;
                        }
                        rotation += 0.03f * (1 - (minCharge * minCharge * minCharge));
                    }
                    else if (Main.npc[members[0]].ai[2] == 1)
                    {
                        attackCount++;
                        List<int> newMembers = new List<int>(members);
                        for (int i = 0; i < members.Count; i++)
                        {
                            int index = (i + Count / 2) % Count;
                            newMembers[index] = members[i];
                        }
                        members = newMembers;
                    }
                }
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
            Texture2D texture = ModContent.Request<Texture2D>("Terrapain/Content/Groups/kishka").Value;
            for (int i = 0; i < k.Count; i++)
            {
                k[i].chain.DrawSmoothed(spriteBatch, texture, null, Color.White * k[i].progress, true, 1);
            }
        }
        public override void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPosition)
        {
            if (Count == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terrapain/Content/Groups/kishka").Value;
                for (int i = 0; i < k.Count; i++)
                {
                    k[i].chain.DrawSmoothed(spriteBatch, texture, null, Color.White * k[i].progress, true, 1);
                }
            }
        }
    }
}
