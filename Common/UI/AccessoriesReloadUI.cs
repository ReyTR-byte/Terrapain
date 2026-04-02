using Microsoft.Xna.Framework.Graphics;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terrapain.Common.UI.Assets.AbilitiesIcons;
using Terrapain.Common.UI.Assets.AbilitiFrames;
using Terrapain.Common.UI.Assets.ChargeStrips;
using Terrapain.Common.UI.Assets.EmptyStrips;
using Terrapain.Common.UI.Assets.ItemFrames;
using Terrapain.Content;
using Terrapain.Content.Dashes;
using Terrapain.Content.Items.Abstract;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Terrapain.Common.UI
{
    public class AccessoriesReloadUI : TerrapainUI
    {
        public struct ItemReloadDrawInfo
        {
            public Item item;
            public ItemFrame itemFrame;
            public AbilityFrame abilityFrame;
            public bool ability;
            public int abilityReload;
            public float abilityCharge;
            public EmptyStrip abilityEmptyStrip;
            public ChargeStrip abilityChargeStrip;
            public AbilityIcon abilityIcon;
            public bool dash;
            public int dashReload;
            public float dashCharge;
            public AbilityIcon dashIcon;
            public EmptyStrip dashEmptyStrip;
            public ChargeStrip dashChargeStrip;
        }
        public List<ItemReloadDrawInfo> itemReloadDrawInfos = new List<ItemReloadDrawInfo>();
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex + 1;
        public override string InterfaceLayerName => "Terrapain: Accesseries reload UI";
        public override void UpdateUI()
        {
            if (Main.gameMenu || Main.playerInventory || !Functions.CheckActiveAccessories(Main.LocalPlayer))
            {
                TerrapainUIManager.Close<AccessoriesReloadUI>();
            }
        }
        public List<UIPanel> ItemPanels = new List<UIPanel>(); 
        public override void Update(GameTime gameTime)
        {
            itemReloadDrawInfos = new List<ItemReloadDrawInfo>();
            List<Item> items = Functions.GetActiveAccessories(Main.LocalPlayer);
            bool dash = true;
            foreach (Item item in items)
            {
                TGlobalItem titem = item.GetGlobalItem<TGlobalItem>();
                VanillaItemActiveAccessory accessory = titem.ActiveAccessoryVanillaItem;
                ItemReloadDrawInfo info = new ItemReloadDrawInfo()
                {
                    item = item,
                    abilityFrame = accessory.abilityFrame,
                    itemFrame = accessory.itemFrame,
                    ability = titem.activeAccessory && (!accessory.AbilityUnarmedOnly || Main.LocalPlayer.GetModPlayer<TerrapainPlayer>().unarmed),
                    abilityReload = accessory.AbilityReloadMax - accessory.AbilityReload,
                    abilityCharge = accessory.AbilityCharge(),
                    abilityIcon = accessory.abilityIcon,
                    abilityEmptyStrip = accessory.abilityEmptyStrip,
                    abilityChargeStrip = accessory.abilityChargeStrip,
                };
                if (Main.LocalPlayer.Custom().Dash?.dashSource.TryGetDrawItem() == item)
                {
                    IDashSource source = Main.LocalPlayer.Custom().Dash.dashSource;
                    info.dash = true;
                    info.dashReload = source.reloadMax - source.reload;
                    info.dashCharge = source.reloadMax == 0 ? 0 : MathF.Max(0, MathF.Min(1, (float)(source.reloadMax - source.reload) / source.reloadMax));
                    info.dashIcon = accessory.dashIcon;
                    info.dashEmptyStrip = source.emptyStrip;
                    info.dashChargeStrip = source.chargeStrip;
                    dash = false;
                }
                itemReloadDrawInfos.Add(info);
            }
            if (dash)
            {
                Item item = Main.LocalPlayer.Custom().Dash?.dashSource.TryGetDrawItem();
                if (item != null)
                {
                    TGlobalItem titem = item.GetGlobalItem<TGlobalItem>();
                    IDashSource source = Main.LocalPlayer.Custom().Dash.dashSource;
                    ItemReloadDrawInfo info = new ItemReloadDrawInfo()
                    {
                        item = item,
                        abilityFrame = source.abilityFrame,
                        itemFrame = source.itemFrame,
                        ability = false,
                        dash = true,
                        dashReload = source.reloadMax - source.reload,
                        dashCharge = source.reloadMax == 0? 0 : MathF.Max(0, MathF.Min(1, (float)(source.reloadMax - source.reload) / source.reloadMax)),
                        dashIcon = source.dashIcon,
                        dashEmptyStrip = source.emptyStrip,
                        dashChargeStrip = source.chargeStrip,
                    };
                    if (titem.dashAccessory)
                    {
                        dash = false;
                    }
                    itemReloadDrawInfos.Add(info);
                }
            }
        }
        public override void OnClose()
        {
            try
            {
                for (int i = 0; i < ItemPanels.Count; i++)
                {
                    RemoveChild(ItemPanels[i]);
                }
            }
            catch { }
            base.OnClose();
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
            Vector2 scale = Vector2.One;
            for(int i = itemReloadDrawInfos.Count - 1; i > -1; i--)
            {
                Vector2 sourcePosition = new Vector2(32/* * Main.UIScale*/, ((itemReloadDrawInfos.Count - 1) / 2f * 100 - i * 100f)/* * Main.UIScale*/ + Main.ScreenSize.Y / 2f);
                itemReloadDrawInfos[i].abilityFrame.Draw(spriteBatch, sourcePosition, scale);
                Vector2 position = sourcePosition + new Vector2 (32, 0)/* * Main.UIScale*/;
                itemReloadDrawInfos[i].itemFrame.Draw(spriteBatch, position, scale);
                //Item drawItem = new Item(itemReloadDrawInfos[i].item.type);
                //drawItem.scale *= Main.UIScale;
                Main.DrawItemIcon(spriteBatch, itemReloadDrawInfos[i].item, position, Color.White, 32f/* * Main.UIScale*/);
                if (itemReloadDrawInfos[i].ability)
                {
                    position = sourcePosition + new Vector2(70, 0) /* * Main.UIScale*/;
                    if (itemReloadDrawInfos[i].dash)
                    {
                        position += Vector2.UnitY * -16/* * Main.UIScale*/;
                    }
                    itemReloadDrawInfos[i].abilityEmptyStrip.Draw(spriteBatch, position, scale);
                    itemReloadDrawInfos[i].abilityChargeStrip.Draw(spriteBatch, position, scale, itemReloadDrawInfos[i].abilityCharge);
                    itemReloadDrawInfos[i].abilityEmptyStrip.DrawOver(spriteBatch, position, scale);

                    if (itemReloadDrawInfos[i].abilityIcon != null)
                    {
                        itemReloadDrawInfos[i].abilityIcon.Draw(spriteBatch, position, scale, itemReloadDrawInfos[i].abilityReload);
                    }
                }
                if (itemReloadDrawInfos[i].dash)
                {
                    position = sourcePosition + new Vector2(70, 0)/* * Main.UIScale*/;
                    if (itemReloadDrawInfos[i].ability)
                    {
                        position += Vector2.UnitY * 16/* * Main.UIScale*/;
                    }
                    itemReloadDrawInfos[i].dashEmptyStrip.Draw(spriteBatch, position, scale);
                    itemReloadDrawInfos[i].dashChargeStrip.Draw(spriteBatch, position, scale, itemReloadDrawInfos[i].dashCharge);
                    itemReloadDrawInfos[i].dashEmptyStrip.DrawOver(spriteBatch, position, scale);
                    if (itemReloadDrawInfos[i].dashIcon != null)
                    {
                        itemReloadDrawInfos[i].dashIcon.Draw(spriteBatch, position, scale, itemReloadDrawInfos[i].dashReload);
                    }
                }
            }
        }
    }
}
