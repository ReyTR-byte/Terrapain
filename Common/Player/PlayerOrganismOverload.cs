using Terrapain.Common.Global;
using Terrapain.Common.System;
using Terrapain.Content;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Common.Player
{
    public class PlayerOrganismOverload : ModPlayer
    {
        public int load;
        public int overload;
        public int staticOverload = 10;
        public List<int> RemovedBuffs = new List<int>();

        public override void ResetEffects()
        {
            load = 0;
            overload = staticOverload;
        }
        public override void PreUpdateBuffs()
        {
            List<int> dontHasBuff = new List<int>();
            foreach(int i in RemovedBuffs)
            {
                if (Player.HasBuff(i))
                {
                    Player.ClearBuff(i);
                }
                else
                {
                    dontHasBuff.Add(i);
                }
            }
            foreach(int i in dontHasBuff)
            {
                RemovedBuffs.Remove(i);
            }
        }
        public override void PostUpdateBuffs()
        {
            if (load > overload)
            {
                if (Player.lifeRegen > 0 && WorldDifficultySystem.suicide)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegen -= (load - overload) * (load - overload);
            }
        }
    }
}