using Terrapain.Content;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Common.Player
{
    public class PlayerReeespawn : ModPlayer
    {
        public override void UpdateDead()
        {
            if (Player.respawnTimer > 300)
            {
                Player.respawnTimer = 300;
            }
            bool bossAlive = false;
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.boss)
                {
                    bossAlive = true;
                }
            } 
            if (!bossAlive)
            {
                if (Player.respawnTimer > 2)
                {
                    Player.respawnTimer = 2;
                }
            }
        }
        public override void OnRespawn()
        {
            Player.statLife = Player.statLifeMax2;
            Player.Custom().ResetAbilities("Dead");
        }
    }
}