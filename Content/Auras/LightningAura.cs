using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrapain.Content.Buffs;
using Terrapain.Content.DamageClasses;
using Terrapain.Content.Dusts;
using Terraria;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace Terrapain.Content.Auras
{
    public class LightningAura : Aura
    {
        public LightningAura(int player)
        {
            owner = Main.player[player];
            Center = owner.Center;
            checkPlayer = false;
            Radius = 350;
            InternalRadius = 300;
            dustCountMin = 3;
            dustCountMax = 5;
            dustChanse = 1;
            dust = ModContent.DustType<Lightning>();
            AuraColor = Color.White;
            AuraColor.A /= 2;
            AuraColor.R /= 2;
            AuraColor.G /= 2;
            AuraColor.B /= 2;
            internalColor = Color.White;
            internalColor.A /= 10;
            internalColor.R /= 10;
            internalColor.G /= 10;
            internalColor.B /= 10;
            timeLeft = 1;
        }
        Terrapain.LightningDrawInfo? lighting = null;
        public override void PostDraw(SpriteBatch sprite)
        {
            if (lighting.HasValue)
                sprite.DrawLightning(lighting.Value);
        }
        public override void OnNPCInAura(NPC npc)
        {
            npc.AddBuff(ModContent.BuffType<Shocked>(), 10);

            if (owner.RollLuck(100) < 3)
            {
                float rotation = random.NextFloat(0, MathF.PI * 2);
                Vector2 start = Center;//Functions.UnitVectorFromRotation(rotation) * random.NextFloat(0, radius);
                //start += Center;
                lighting = Functions.NewLightning(start, npc.Center, 16, fixedStart: true);
                NPC.HitModifiers modifiers = new NPC.HitModifiers { DamageType = ModContent.GetInstance<Unarmed>(), HitDirection = (npc.position.X - Center.X).NonZeroSign() };
                bool crit = owner.GetCritChance(modifiers.DamageType) > random.NextFloat(0, 100);
                if (npc.knockBackResist == 0)
                {
                    modifiers.DisableKnockback();
                }
                NPCLoader.ModifyIncomingHit(npc, ref modifiers);
                modifiers.Defense.Base += npc.defense;
                NPC.HitInfo hitInfo = modifiers.ToHitInfo((int)owner.GetTotalDamage(modifiers.DamageType).ApplyTo(70), crit, 6 * npc.knockBackResist, true, owner.luck);
                owner.addDPS(hitInfo.Damage);
                PlayerLoader.OnHitAnything(owner, npc.Center.X, npc.Center.Y, npc);
                npc.StrikeNPC(hitInfo);
                PlayerLoader.OnHitNPC(owner, npc, hitInfo, hitInfo.Damage);
                npc.AddBuff(ModContent.BuffType<Shocked>(), 60);
            }
        }
    }
}
