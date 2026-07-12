using Microsoft.Xna.Framework;
using Terrapain.Common.Global;
using Terrapain.Content;
using Terrapain.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Weapons.MeleeWeapons
{
	public class Sharper : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.width = 72;
			Item.height = 40;
			Item.useStyle = TGlobalItem.SharperUseStyle;
			Item.GetGlobalItem<TGlobalItem>().dust = DustID.Granite;
            Item.GetGlobalItem<TGlobalItem>().dustLight = new Vector3 (0.1f, 0.8f, 0.1f);
            Item.value = Item.buyPrice(gold: 8);
			Item.GetT().StaminaUsage = 2;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(ModContent.BuffType<ScorspiderAcid>(), 60);
        }
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(10, 23);
		}
	}
}
