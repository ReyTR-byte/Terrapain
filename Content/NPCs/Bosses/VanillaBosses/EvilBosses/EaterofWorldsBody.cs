using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Content.Groups;
using Terrapain.Content.NPCs.VanillaNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    internal class EaterofWorldsBody : NPCBehaviour, ISnakePart
    {
        public override int type => NPCID.EaterofWorldsBody;
        NPC tail
        {
            get
            {
                NPC me = Main.npc[t.npcid];
                if (me.ai[1] >= 0)
                {
                    NPC _tail = Main.npc[(int)me.ai[1]];
                    if (_tail.active && (_tail.type == NPCID.EaterofWorldsTail || _tail.type == NPCID.EaterofWorldsBody) && _tail.ai[0] == me.whoAmI)
                    {
                        return _tail;
                    }
                }
                return null;
            }
        }    
        NPC head
        {
            get
            {
                NPC _head = Main.npc[(int)Main.npc[t.npcid].ai[0]];
                if (_head.active && (_head.type == NPCID.EaterofWorldsHead || _head.type == NPCID.EaterofWorldsBody) && _head.ai[1] == t.npcid)
                {
                    return _head;
                }
                return null;
            }
        }
        public override void ModSetDefaults(NPC entity)
        {
            t.useVanillaDrawing = false;
            entity.alpha = 0;
            entity.knockBackResist = 0;
            entity.lifeMax = (int)(entity.lifeMax * 2.5f);
            if ((EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1) || Main.getGoodWorld)
            {
                entity.scale = 1.4f;
                entity.width = (int)(entity.width * 1.4f / 1.2f);
                entity.height = entity.width;
            }
            entity.GetT().drawCenter = new Vector2(30, 30);
            entity.dontTakeDamage = true;
            entity.GetT().despawnLikeABoss = true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            npc.frame = new Rectangle(0, 120, 60, 60);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.ai[1] < 0)
            {
                npc.alpha = 255;
            }
            npc.position.Y += npc.height / 2;
            if (BrainOfCthulhu.segmentsLifes.Count > 0)
            {
                npc.life = BrainOfCthulhu.segmentsLifes[0];
                BrainOfCthulhu.segmentsLifes.RemoveAt(0);
            }
            base.OnSpawn(npc, source);
            if (EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1)
            {   
                NPC head = Main.npc[EaterofWorldsHead.MainHead];
                npc.realLife = head.whoAmI;
                npc.lifeMax = head.lifeMax;
                npc.life = head.lifeMax;
            }
        }
        public override bool OverrideTexture(ref Asset<Texture2D> texture)
        {
            texture = ModContent.Request<Texture2D>(this.GetPath());
            return true;
        }
        public override bool ModPreAI(NPC npc)
        {
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            if (npc.dontTakeDamage && NPC.AnyNPCs(NPCID.EaterofWorldsTail))
            {
                npc.dontTakeDamage = false;
            }
            CheckGroup(npc);
            NPC brain = Main.npc[EaterofWorldsHead.brainofCthulhu];
            if (npc.ai[1] < 0 && npc.Distance(brain.Center) > npc.width)
            {
                npc.alpha = 0;
                if (npc.ai[1] < -1)
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), brain.Center, NPCID.EaterofWorldsBody, npc.whoAmI, npc.whoAmI, npc.ai[1] + 1);
                }
                else
                {
                    npc.ai[1] = NewNPC(npc.GetSource_FromThis(), brain.Center, NPCID.EaterofWorldsTail, npc.whoAmI, npc.whoAmI);
                }
                for (int i = 0; i < t.MyGroups.Count; i++)
                {
                    int g = t.MyGroups[i];
                    if (Terrapain.group[g] is EaterofWorlds)
                    {
                        (Terrapain.group[g] as EaterofWorlds).RebuidSnake();
                    }
                }
            }
            
            return false;
        }
        public void CheckGroup(NPC npc)
        {
            foreach (int group in npc.GetT().MyGroups)
            {
                if (Terrapain.group[group] != null && Terrapain.group[group] is EaterofWorlds)
                {
                    return;
                }
            }
            NPC head = this.head;
            NPC tail = this.tail;
            if (head == null)
            {
                if (tail != null)
                {
                    npc.active = false;
                    Vector2 velocity = npc.oldPosition == Vector2.Zero? Vector2.Zero : npc.position - npc.oldPosition;
                    tail.ai[0] = NewNPC(npc.GetSource_FromThis(), npc.Center, velocity, NPCID.EaterofWorldsHead, ai0: 1, ai1: tail.whoAmI);
                    Main.npc[(int)tail.ai[0]].rotation = npc.rotation;
                }
                else
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
        }
        public void NextAttack(NPC npc, int oldAttack, int newAttack)
        {
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            return npc.alpha == 0 && (EaterofWorldsHead.Phase == 1 || EaterofWorldsHead.attack == -1);
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return npc.alpha == 0? null : false;
        }
        public override bool CanBeHitByNPC(NPC npc, NPC attacker)
        {
            return npc.alpha == 0;
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return npc.alpha == 0? null : false;
        }
        public override bool CheckActive(NPC npc)
        {
            return false;
        }
        public override bool CheckDead(NPC npc)
        {
            if (EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1)
            {
                return false;
            }
            return true;
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1)
            {
                npc.GetLifeStats(out int life, out _);
                if (life - hit.Damage <= 0)
                {
                    npc.immortal = true;
                    npc.life = 1;
                    var g = Terrapain.group[group];
                    if (g != null && g is EaterofWorlds)
                    {
                        (g as EaterofWorlds).Dying = true;
                    }
                }
            }
        }

        public void UpdateAsHead(NPC npc)
        {
            var g = Terrapain.group[group];
            if (head == null)
            {
                NPC tail = this.tail;
                if (tail != null)
                {
                    npc.active = false;
                    Vector2 velocity = npc.oldPos[1] == Vector2.Zero? Vector2.Zero : npc.position - npc.oldPos[1];
                    g.members[member] = NewNPC(npc.GetSource_FromThis(), npc.Center, velocity, NPCID.EaterofWorldsHead, ai0: 1, ai1: tail.whoAmI);
                    tail.ai[0] = g.members[member];
                    Main.npc[g.members[member]].rotation = npc.rotation;
                }
                else
                {
                    _ = this.tail;
                    npc.life = 0;
                    npc.checkDead();        

                    _ = this.tail;
                }
            }
        }

        public void UpdateAsBody(NPC npc) { }

        public void UpdateAsTail(NPC npc)
        {
            if (npc.ai[1] < 0)
            {
                return;
            }
            var g = Terrapain.group[group];
            if (tail == null)
            {
                NPC head = this.head;
                if (head != null)
                {
                    npc.active = false;
                    g.members[member] = NewNPC(npc.GetSource_FromThis(), npc.Center, NPCID.EaterofWorldsTail, ai0: head.whoAmI);
                    head.ai[1] = g.members[member];
                    Main.npc[g.members[member]].rotation = npc.rotation;
                }
                else
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
        }

        public int group;
        public int member;
        public void SetGroup(int group, int member)
        {
            this.member = member;
            this.group = group;
        }
    }
}
