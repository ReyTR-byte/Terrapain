using System.ComponentModel.Design;
using Steamworks;
using Terrapain.Common.Global;
using Terrapain.Common.TerrapainModPlayer;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.BarFills;
using Terrapain.Common.UI.Assets.Bars;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Abstract
{
    public abstract class VanillaItemRework : ILoadable
    {
        public void Load(Mod mod)
        {
            foreach(var i in Items)
            {
                TGlobalItem.instances[i] = this;
            }
        }
        public void Unload()
        {
        }
        public abstract VanillaItemRework GetNewInstance(Item item);
        public virtual int[] Items => [];
        public virtual void ModSetDefaults(Item item) { }
        public virtual bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { return true; }
        public virtual void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) { }
        public virtual void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo) { }
        public virtual void UpdateAccessory(Item item, Player player, bool hideVisual) { }
        public virtual void UpdateInventory(Item item, Player player) { }
        public virtual bool CanUseItem(Item item, Player player) { return true; }
        public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips) { }

    }
}