using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Content;
using Terrapain.Content.DamageClasses;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Terrapain.Common.Player
{
    public class PlayerMovement : ModPlayer
    {
        public bool ShouldFallThroughtPlatforms;

        public Item DashItem;
        public float DashPower;
        public float DashDirection;
        public int DashDuration;
        public int DashImpulse;
        public int DashImmune;
        public NPC.HitModifiers? DashHitModifiers;
        public int DashPenetrate;
        public bool DashNoGravity;
        public bool DashStraight;
        public List<int> HittenNPCByDash;
        public bool[] DashDirections 
        {
            get => [Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] < 15, 
                Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[1] < 15, 
                Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15, 
                Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15] ;
        }

        Vector2 preUpdatePos;
        Vector2 preUpdateVelosity;

        public override void ResetEffects()
        {
            if (DashDuration <= 0)
            {
                DashImmune--;
            }
        }
        public override void PreUpdateMovement()
        {
            if (Player.controlDown && Player.mount.Type == MountID.None)
            {
                Player.gravity += 0.15f * Player.gravity.NonZeroSign();
                Player.maxFallSpeed += 1;
            }
            preUpdatePos = Player.position;
            preUpdateVelosity = Player.velocity;
            if (DashDuration > 0) 
            { 
                if (!DashStraight)
                {
                    Dash(DashItem, DashPower, DashDirection, DashDuration, DashImpulse, DashImmune, DashHitModifiers, DashPenetrate, DashNoGravity, DashStraight); 
                }
                else
                {
                    Player.velocity = Vector2.Zero;
                }
                if (DashNoGravity)
                {
                    Player.gravity = 0;
                }
            }
            if ((DashDirections[0] || DashDirections[1] || DashDirections[2] || DashDirections[3]) && DashDuration <= 0)
            {
                HittenNPCByDash = new List<int>();
                Player.Custom().Dash?.TryUse(Player, DashDirections);
            }
        }
        public override void PostUpdateEquips()
        {
            Player.dashType = 0;
        }
        public override void PostUpdate()
        {
            if (DashDuration > 0)
            {
                if (DashStraight)
                {
                    Dash(DashItem, DashPower, DashDirection, DashDuration, DashImpulse, DashImmune, DashHitModifiers, DashPenetrate, DashNoGravity, DashStraight);
                }
            }
            Player.Custom().stimulator = null;
            if (ShouldFallThroughtPlatforms)
            {
                if (Player.velocity.Y >= 0)
                {
                    List<Vector2> tiles;
                    if (!Functions.HitTiles(Player) && Functions.StairsColision(Player.Bottom, Player.width, 0, out tiles))
                    {
                        Player.position.Y = preUpdatePos.Y + preUpdateVelosity.Y/* + Player.gravity < Player.maxFallSpeed? preUpdateVelosity.Y + Player.gravity : (preUpdateVelosity.Y < Player.maxFallSpeed? Player.maxFallSpeed : preUpdateVelosity.Y))*/;
                        Player.velocity.Y = preUpdateVelosity.Y/* + Player.gravity < Player.maxFallSpeed? preUpdateVelosity.Y + Player.gravity : (preUpdateVelosity.Y < Player.maxFallSpeed? Player.maxFallSpeed : preUpdateVelosity.Y)*/;
                    }
                }
            }
            ShouldFallThroughtPlatforms = false;
        }
        UnifiedRandom random = new UnifiedRandom();
        /// <summary>
        /// makes new dash
        /// </summary>
        /// <param name="DashAccesory">Needs to get damage, knock back and crit chanse. 
        /// If hurt someone ItemLoader.OnHitNPC(DashAccesory, Player, npc, hitInfo, hitInfo.Damage); would be called.
        /// Could be null</param>
        /// <param name="power"></param>
        /// <param name="direction"></param>
        /// <param name="duration">how long will dash effects such as damage by dash would be applyed</param>
        /// <param name="impulse">how long will dash apply its velocity</param>
        /// <param name="immune">how long will imunity after dealing Damage Lust</param>
        /// <param name="hitModifiers">if null dash wouldnt hurt anybody</param>
        /// <param name="penetrate">how many enemies this player can hurt before this player bounse</param>
        /// <param name="noGravity"></param>
        /// <param name="straight">if true players velocity would be replased by dash velocity</param>
        /// <param name="damage">will be replased by item damage if DashAccessory != null</param>
        /// <param name="knockBack">will be replased by item knockBack if DashAccessory != null</param>
        /// <param name="critChanse">will be replased by item crit if DashAccessory != null</param>
        public void Dash(Item DashAccesory, float power, float direction, int duration, int impulse = 1, int immune = 0, NPC.HitModifiers? hitModifiers = null, int penetrate = 1, bool noGravity = false, bool straight = false, int damage = 0, float knockBack = 0, float critChanse = 0)
        {
            if (impulse != 0)
            {
                Vector2 velocity = Player.velocity.RotatedBy(-direction);
                velocity.X = power;
                if (straight)
                {
                    velocity.Y = 0;
                }
                Player.velocity = velocity.RotatedBy(direction);
            }
            Player.eocDash = duration;
            Player.armorEffectDrawShadowEOCShield = true;

            DashItem = DashAccesory;
            DashPower = power;
            DashDirection = direction;
            DashDuration = duration;
            DashImmune = immune;
            DashHitModifiers = hitModifiers;
            DashPenetrate = penetrate;
            DashNoGravity = noGravity;
            DashStraight = straight;
            if (hitModifiers != null)
            {
                if (Player.magmaStone)
                {
                    if (random.Next(2) == 0)
                    {
                        int dust = Dust.NewDust(Player.position, Player.height, Player.width, DustID.Torch, Scale: 2.5f);
                    }
                }
                foreach (var npc in Main.npc)
                {
                    if (penetrate == 0)
                        break;
                    if (npc.active && !npc.friendly)
                    {
                        if (!HittenNPCByDash.Contains(npc.whoAmI))
                        {
                            if (Functions.RectangleColision(Player, npc))
                            {
                                HittenNPCByDash.Add(npc.whoAmI);
                                NPC.HitModifiers modifiers = hitModifiers.Value;
                                UnifiedRandom random = new UnifiedRandom();
                                bool crit = (DashAccesory?.crit ?? critChanse) + Player.GetTotalCritChance(modifiers.DamageType) > random.NextFloat(0, 100);
                                if (npc.knockBackResist == 0)
                                {
                                    modifiers.DisableKnockback();
                                }
                                NPCLoader.ModifyIncomingHit(npc, ref modifiers);
                                modifiers.Defense.Base += npc.defense;
                                NPC.HitInfo hitInfo = modifiers.ToHitInfo((int)Player.GetTotalDamage(modifiers.DamageType).ApplyTo(DashAccesory?.damage ?? damage), crit, (DashAccesory?.knockBack ?? knockBack) * npc.knockBackResist, true, Player.luck);
                                Player.addDPS(hitInfo.Damage);
                                PlayerLoader.OnHitAnything(Player, npc.Center.X, npc.Center.Y, npc);
                                npc.StrikeNPC(hitInfo);
                                //NPCLoader.HitEffect(npc, hitInfo);
                                if (DashAccesory != null)
                                {
                                    ItemLoader.OnHitNPC(DashAccesory, Player, npc, hitInfo, hitInfo.Damage);
                                    //DashAccesory.ModItem.OnHitNPC(Player, npc, hitInfo, hitInfo.Damage); 
                                }
                                PlayerLoader.OnHitNPC(Player, npc, hitInfo, hitInfo.Damage);
                                penetrate--;
                            }
                        }
                    }
                }
            }
            duration--;
            if (impulse > 0)
            {
                impulse--;
            }
            DashItem = DashAccesory;
            DashPower = power;
            DashDirection = direction;
            DashDuration = duration;
            DashImmune = immune;
            DashHitModifiers = hitModifiers;
            DashPenetrate = penetrate;
            DashNoGravity = noGravity;
            DashStraight = straight;
            if (penetrate == 0 && duration != -1)
            {   
                if(impulse == 0)
                {
                    power = Player.velocity.RotatedBy(-direction).X;
                }
                Dash(null, -power, direction, 0, 1, immune, noGravity: false, straight: false);
            }
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (DashImmune > 0)
            {
                return false; 
            }
            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }
    }
}