using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Content.Groups;
using Terrapain.Content.NPCs.VanillaNPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.NPCs.Bosses.VanillaBosses.EvilBosses
{
    public class EaterofWorldsTail : NPCBehaviour, ISnakePart
    {
        public override int type => NPCID.EaterofWorldsTail;
        public override void ModSetDefaults(NPC entity)
        {
            t.useVanillaDrawing = false;
            entity.knockBackResist = 0;
            entity.alpha = 0;
            entity.lifeMax = (int)(entity.lifeMax * 2.5f);
            if ((EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1) || Main.getGoodWorld)
            {
                entity.scale = 1.4f;
                entity.width = (int)(entity.width * 1.4f / 1.2f);
                entity.height = entity.width;
            }
            entity.GetT().drawCenter = new Vector2(30, 30);
            entity.GetT().despawnLikeABoss = true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            npc.frame = new Rectangle(0, 120, 72, 60);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            npc.position.Y += npc.height / 2;
            if (BrainOfCthulhu.segmentsLifes.Count > 0)
            {
                npc.life = BrainOfCthulhu.segmentsLifes[0];
                BrainOfCthulhu.segmentsLifes.RemoveAt(0);
            }
            if (EaterofWorldsHead.Phase == 2 && EaterofWorldsHead.attack != -1)
            {   
                NPC head = Main.npc[(int)npc.ai[0]];
                npc.realLife = EaterofWorldsHead.MainHead;
            }
            npc.spriteDirection = 1;
        }
        public override bool ModPreAI(NPC npc)
        {
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            CheckGroup(npc);
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
            NPC head = Main.npc[(int)npc.ai[0]];
            if (!head.active || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody) || head.ai[1] != npc.whoAmI)
            {
                npc.life = 0;
                npc.checkDead();
            }
        }
        public void NextAttack(NPC npc, int oldAttack, int newAttack)
        {
            if (newAttack == 4)
            {
                npc.ai[2] = -1;
            }
        }
        public override bool OverrideTexture(ref Asset<Texture2D> texture)
        {
            texture = ModContent.Request<Texture2D>(this.GetPath());
            return true;
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
                if (npc.ai[3] == -1)
                {
                    return true;
                }
                var g = Terrapain.group[group];
                if (g != null && g is EaterofWorlds)
                {
                    (g as EaterofWorlds).Dying = true;
                }
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
            NPC head = Main.npc[(int)npc.ai[0]];
            if (!head.active || head.ai[1] != npc.whoAmI || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody))
            {
                npc.life = 0;
                npc.checkDead();
            }
        }

        public void UpdateAsBody(NPC npc)
        {
            NPC head = Main.npc[(int)npc.ai[0]];
            if (!head.active || head.ai[1] != npc.whoAmI || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody))
            {
                npc.life = 0;
                npc.checkDead();
            }
        }

        public void UpdateAsTail(NPC npc)
        {
            if (Terrapain.group[group].Count == 1)
            {
                NPC head = Main.npc[(int)npc.ai[0]];
                if (!head.active || head.ai[1] != npc.whoAmI || (head.type != NPCID.EaterofWorldsHead && head.type != NPCID.EaterofWorldsBody))
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
        }
        public int group;
        public void SetGroup(int group, int member)
        {
            this.group = group;
        }
    }
}