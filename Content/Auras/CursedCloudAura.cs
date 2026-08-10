using Luminance.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.System;
using Terrapain.Content.Buffs;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terrapain.Content.Auras
{
    public class CursedCloudAura : Aura
    {
        public CursedCloudAura()
        {
            checkNPC = false;
            checkPlayer = true;
            Radius = 550;
            InternalRadius = 300;
            dustCountMin = 0;
            dustCountMax = 2;
            dustChanse = 1;
            dust = ModContent.DustType<Sparcle>();
            AuraColor = Color.White * 0.5f;
            internalColor = Color.White * 0.1f;
            timeLeft = 2;
        }
        public bool recharge;
        public float Charge;
        public bool CatchPlayer;
        int soundTimer;
        public override void PostUpdate()
        {
            checkPlayer = true;
            if (soundTimer <= 0)
            {
                soundTimer = random.Next(35, 55);
                SoundEngine.PlaySound(new SoundStyle("Terrapain/Assets/SoundEffects/ElectricField") { Volume = 1.2f, MaxInstances = 0 }, Center);
            }
            soundTimer--;
            if (recharge)
            {
                Charge -= 0.2f;
                if (Charge < 0)
                {
                    Charge = 0;
                    recharge = false;
                }
            }
        }
        public override void OnPlayerInAura(Player player)
        {
            CatchPlayer = true;
            if (!recharge)
            {
                player.AddBuff(ModContent.BuffType<Shocked>(), 10);
                Charge += 0.012f;
                if (Charge > 1)
                {
                    recharge = true;
                    float rotation = random.NextFloat(0, MathF.PI * 2);
                    Vector2 start = Center;//Functions.UnitVectorFromRotation(rotation) * random.NextFloat(0, radius);
                    //start += Center;
                    var lightning = Functions.NewLightning(start, player.Center, 60, volume: 0.6f);
                    TunderSystem.NewLightning(lightning);
                    int reason = random.Next(3);
                    string Gender = player.Male ? "Male" : "Female";
                    PlayerDeathReason deathReason = PlayerDeathReason.ByCustomReason(NetworkText.FromKey($"Mods.Terrapain.DeathReasons.Lightning_{reason}_{Gender}", player.name));
                    player.Hurt(deathReason, 75, (player.Center.X - Center.X).NonZeroSign());
                }
            }
        }
    }
}
