using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Terrapain.Content.Items.Accessories.ActiveAccessories.VanillaItemActiveAccessories
{
    public class MagmaStone : VanillaItemActiveAccessory
    {
        public override VanillaItemRework GetNewInstance(Item item)
        {
            return new MagmaStone();
        }
        public override int[] Items => [ItemID.MagmaStone];
        UnifiedRandom random = new UnifiedRandom();
        public override void ModSetDefaults(Item item)
        {
            item.damage = 40;
            item.DamageType = ModContent.GetInstance<Unarmed>();
            item.knockBack = 3;
            activeAccessory = new ActiveAccessory(this);
            activeAccessory.AbilityReloadMax = 400;
            activeAccessory.AbilityUnarmedOnly = true;
            activeAccessory.abilityIcon = new BlastIcon();
            activeAccessory.HoldAbility = true;
            activeAccessory.HoldConsumption = 2;
            item.GetT().activeAccessory = true;
            item.GetT().ActiveAccessory = activeAccessory;
            DescriptionLinesCount = 1;
        }
        public override void OnUseAbility(Player player, Item item)
        {
            AbilityReloadMax = 400;
            float power = 1 + (1 - activeAccessory.AbilityCharge()) * 5;
            if (power < 2)
            {
                return;
            }
            float powerSqrt = MathF.Sqrt(power);
            SoundStyle style = SoundID.Item14;
            style.Volume = 1.5f * powerSqrt;
            SoundEngine.PlaySound(style, player.Center);
            for (int i = 0; i < 150 * powerSqrt; i++)
            {
                float rotation = random.NextFloat(MathF.PI * 2);
                float distance = random.NextFloat(80 * powerSqrt);
                Vector2 velocity = Functions.UnitVectorFromRotation(rotation) * distance / 15;
                int dust = Dust.NewDust(player.Center + Functions.UnitVectorFromRotation(rotation) * distance, 0, 0, DustID.Torch, Scale: 2);
                Main.dust[dust].velocity = velocity;
            }
            foreach (var npc in Main.npc)
            {
                if (npc.active && !npc.friendly)
                {
                    if (Functions.CircleColision(player, 120 * power, npc))
                    {
                        NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = item.DamageType, HitDirection = (npc.position.X - player.position.X).NonZeroSign()};
                        bool crit = item.crit + player.GetTotalCritChance(modifiers.DamageType) > random.NextFloat(0, 100);
                        if (npc.knockBackResist == 0)
                        {
                            modifiers.DisableKnockback();
                        }
                        NPCLoader.ModifyIncomingHit(npc, ref modifiers);
                        modifiers.Defense.Base += npc.defense;
                        NPC.HitInfo hitInfo = modifiers.ToHitInfo((int)player.GetTotalDamage(modifiers.DamageType).ApplyTo(item.damage * powerSqrt), crit, item.knockBack * npc.knockBackResist, true, player.luck);
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
            AbilityReload = AbilityReloadMax;
        }
        public override void OnHoldAbility(Player player, Item item)
        {
            if (random.NextBool(1 - activeAccessory.AbilityCharge()))
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Torch, Scale: 2f);
                Main.dust[dust].velocity.Y = -5.5f;
                Main.dust[dust].velocity.X *= 2f;
            }
            player.velocity.X *= 0.98f;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach(TooltipLine line in tooltips)
            {
                if ((line.Name == "Damage" || line.Name == "Knockback") && !Main.player[Main.myPlayer].GetModPlayer<TerrapainPlayer>().unarmed)
                {
                    line.Text = "";
                }
            }
        } 
    }
}
