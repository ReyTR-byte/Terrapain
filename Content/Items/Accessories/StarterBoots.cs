using Terrapain.Common.System;
using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Items.Accessories
{
	[AutoloadEquip(EquipType.Shoes)]
	public class StarterBoots : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.height = 30;
            Item.width = 30;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxRunSpeed += 2.5f;
            if (KeybindSystem.SprintKeybind.Current && MathF.Abs(player.velocity.X) > player.maxRunSpeed && player.accRunSpeed < player.maxRunSpeed + 5)
            {
                player.Custom().UseStamina(0.15f);
                player.accRunSpeed = player.maxRunSpeed + player.Custom().Stamina * 0.05f;
            }
        }
    }
}