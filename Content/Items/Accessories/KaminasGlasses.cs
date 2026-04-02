using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terrapain.Content.Buffs;
using Terraria.ID;

namespace Terrapain.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class KaminasGlasses : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Face.Sets.OverrideHelmet[Item.faceSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 14;
            Item.accessory = true;
        }
    }
}