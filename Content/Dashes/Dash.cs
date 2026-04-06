using Terrapain.Common.Player;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Dashes
{
    public abstract class Dash
    {
        public float priority;
        public IDashSource dashSource;
        public bool Diagonal;
        public bool Vertical;
        public bool Horizontal;

        public DamageClass damageType;
        public float DashPower;
        ///how long wil dash effects such as damage by dash would be applyed
        public int DashDuration;
        ///how long will dash apply its velocity
        public int impulse = 1;
        ///how long will imunity after dealing Damage Lust, default to 0
        public int immune;
        ///how many enemies this player can hurt before this player bounse
        public int penetrate = -1;
        public bool hurtfull;

        public bool noGravity;
        public bool straight;
        ///will be replased by item damage if dashSourse.TryGetDashItem() != null;
        public int damage;
        ///will be replased by item knockBack if dashSourse.TryGetDashItem() != null;
        public float knockBack;
        ///will be replased by item crit if dashSourse.TryGetDashItem() != null;
        public float critChanse;

        public virtual bool CanUse(Player player, bool[] Directions)
        {
            return dashSource.CanUse(player, Directions) || (player.Custom().stimulator?.OnUse(player)?? false); 
        }
        public virtual void TryUse(Player player, bool[] Directions)
        {
            if (CanUse(player, Directions))
            {
                OnUse(player, Directions);
            }
        }
        public virtual void OnUse(Player player, bool[] Directions)
        {
            if (Directions[2] && Directions[3])
            {
                return;
            }
            if (Directions[0] && Directions[1])
            {
                return;
            }
            dashSource.OnUse(player, Directions);
            if (Directions[2])
            {
                float angle = 0;
                if (Diagonal)
                {
                    if (Directions[0])
                    {
                        angle = MathF.PI * 0.25f;
                    }
                    if (Directions[1])
                    {
                        angle = -MathF.PI * 0.25f;
                    }
                }
                NPC.HitModifiers? modifiers = null;
                if (hurtfull)
                {
                    modifiers = new NPC.HitModifiers { DamageType = damageType, HitDirection = 1 };
                }
                player.GetModPlayer<PlayerMovement>().Dash(dashSource.TryGetDashItem(), DashPower, angle, DashDuration, impulse, immune, modifiers, penetrate, noGravity, straight, damage, knockBack, critChanse);
                return;
            }
            else if (Directions[3])
            {
                float angle = MathF.PI;
                if (Diagonal)
                {
                    if (Directions[0])
                    {
                        angle = -MathF.PI * 0.25f;
                    }
                    if (Directions[1])
                    {
                        angle = MathF.PI * 0.25f;
                    }
                }
                NPC.HitModifiers? modifiers = null;
                if (hurtfull)
                { 
                    modifiers = new NPC.HitModifiers { DamageType = damageType, HitDirection = -1 }; 
                }
                player.GetModPlayer<PlayerMovement>().Dash(dashSource.TryGetDashItem(), DashPower, angle, DashDuration, impulse, immune, modifiers, penetrate, noGravity, straight, damage, knockBack, critChanse);
                return;
            }
            else if (Vertical)
            {
                if (Directions[0])
                {
                    NPC.HitModifiers? modifiers = null;
                    if (hurtfull)
                    {
                        modifiers = new NPC.HitModifiers { DamageType = damageType, HitDirection = 1 };
                    }
                    player.GetModPlayer<PlayerMovement>().Dash(dashSource.TryGetDashItem(), DashPower, MathF.PI * 0.5f, DashDuration, impulse, immune, modifiers, penetrate, noGravity, straight, damage, knockBack, critChanse);
                    return;
                }
                else if (Directions[1])
                {
                    NPC.HitModifiers? modifiers = null;
                    if (hurtfull)
                    {
                        modifiers = new NPC.HitModifiers { DamageType = damageType, HitDirection = 1 };
                    }
                    player.GetModPlayer<PlayerMovement>().Dash(dashSource.TryGetDashItem(), DashPower, -MathF.PI * 0.5f, DashDuration, impulse, immune, modifiers, penetrate, noGravity, straight, damage, knockBack, critChanse);
                    return;
                } 
            }
        }
    }
}
