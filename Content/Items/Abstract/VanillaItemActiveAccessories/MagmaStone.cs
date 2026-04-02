using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.DamageClasses;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Items.Abstract.VanillaItemActiveAccessories
{
    public class MagmaStone : VanillaItemActiveAccessory
    {
        public MagmaStone()
        {
            DescriptionLinesCount = 1;
        }
        public override void OnUseAbility(Player player, Item item)
        {
            SoundStyle style = SoundID.Item14;
            style.Volume = 1.5f;
            SoundEngine.PlaySound(style, player.Center);
            UnifiedRandom random = new UnifiedRandom();
            for (int i = 0; i < 150; i++)
            {
                float rotation = random.NextFloat(MathF.PI * 2);
                float distance = random.NextFloat(80);
                Vector2 velocity = Functions.UnitVectorFromRotation(rotation) * distance / 15;
                int dust = Dust.NewDust(player.Center + Functions.UnitVectorFromRotation(rotation) * distance, 0, 0, DustID.Torch, Scale: 2);
                Main.dust[dust].velocity = velocity;
            }
            foreach (var npc in Main.npc)
            {
                if (npc.active && !npc.friendly)
                {
                    if (Functions.CircleColision(player, 120, npc))
                    {
                        NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = (npc.position.X - player.position.X).NonZeroSign()};
                        bool crit = item.crit + player.GetTotalCritChance(modifiers.DamageType) > random.NextFloat(0, 100);
                        if (npc.knockBackResist == 0)
                        {
                            modifiers.DisableKnockback();
                        }
                        NPCLoader.ModifyIncomingHit(npc, ref modifiers);
                        modifiers.Defense.Base += npc.defense;
                        NPC.HitInfo hitInfo = modifiers.ToHitInfo((int)player.GetTotalDamage(modifiers.DamageType).ApplyTo(item.damage), crit, item.knockBack * npc.knockBackResist, true, player.luck);
                        player.addDPS(hitInfo.Damage);
                        PlayerLoader.OnHitAnything(player, npc.Center.X, npc.Center.Y, npc);
                        npc.StrikeNPC(hitInfo);
                        //NPCLoader.HitEffect(npc, hitInfo);
                        ItemLoader.OnHitNPC(item, player, npc, hitInfo, hitInfo.Damage);
                        PlayerLoader.OnHitNPC(player, npc, hitInfo, hitInfo.Damage);

                        npc.AddBuff(BuffID.OnFire, random.Next(8) < 2 ? 360 : (random.Next(8) < 5 ? 240 : 120));
                    }
                }
            }
        }
    }
}
