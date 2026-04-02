using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Terrapain.Content.Auras
{
    public class SheildAura : Aura
    {
        public int Defense;
        public float DamageReduse;
        public float DamageRedistribution;
        public List<int> WhiteList;
        public SheildAura(float radius, int defense, float damageReduse, float damageRedistribution, Player owner, bool defencePlayer, List<int>whiteList)
        {
            Radius = radius;
            Defense = defense;
            InternalRadius = radius * 0.8f;
            DamageReduse = damageReduse;
            DamageRedistribution = damageRedistribution;
            this.owner = owner;
            checkPlayer = defencePlayer;
            WhiteList = whiteList;
            checkNPC = whiteList.Count != 0;
            AuraColor = Color.Silver * 0.5f;
            internalColor = Color.Silver * 0.1f;
        }
        public SheildAura(float radius, int defense, float damageReduse, float damageRedistribution, NPC owner, bool defencePlayer, List<int> whiteList)
        {
            Radius = radius;
            InternalRadius = radius * 0.8f;
            Defense = defense;
            DamageReduse = damageReduse;
            DamageRedistribution = damageRedistribution;
            npcOwner = owner;
            checkPlayer = defencePlayer;
            WhiteList = whiteList;
            checkNPC = whiteList.Count != 0;
            AuraColor = Color.Silver * 0.5f;
            internalColor = Color.Silver * 0.1f;
        }
        public override void OnNPCInAura(NPC npc)
        {
            if (WhiteList.Contains(npc.type))
            {
                npc.GetT().takenDamageMultiplier *= DamageReduse;
                npc.GetT().bonusDefence += Defense;
                npc.GetT().defender = npcOwner.whoAmI;
                npc.GetT().defenderTakesDamage = DamageRedistribution;
            }
        }
    }
}
