using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terrapain.Common.Global;
using Terrapain.Common.Player;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Terrapain.Common.UI
{
    public class DeletedBuffsUI : TerrapainUI
    {
        UIPanel BuffPanel;
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex + 1;
        public override string InterfaceLayerName => "Terrapain: Deleted Buffs";
        List<int> delBuff => Main.player[Main.myPlayer].GetModPlayer<PlayerOrganismOverload>().RemovedBuffs;
        int delBuffCount => delBuff.Count;
        int BuffCount => Main.player[Main.myPlayer].CountBuffs();
        public override void Update(GameTime gameTime)
        {
            BuffPanel = new UIPanel();
            BuffPanel.Width.Set(38 * (delBuffCount < 10? delBuffCount : 10), 0);
            BuffPanel.Height.Set((delBuffCount / 10 + 1) * 50, 0);
            BuffPanel.Top.Set(76 + (BuffCount < 1? 0 : BuffCount / 10 + 1), 0);
            BuffPanel.Top.Set(32, 0);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int top = 76 + (BuffCount < 1? 0 : BuffCount / 10 + 1);
            int left = 32;
            for (int i = 0; i < delBuffCount; i++)
            {
                Asset<Texture2D> buff;
                if (delBuff[i] < BuffID.Count)
                {
                    buff = TextureAssets.Buff[delBuff[i]];
                }
                else
                {
                    buff = ModContent.Request<Texture2D>(ModContent.GetModBuff(delBuff[i]).Texture);
                }
                spriteBatch.Draw(buff.Value, new Rectangle((int)(left * Main.UIScale), (int)(top * Main.UIScale), (int)(i % 10 * 38 * Main.UIScale), (int)(i / 10 * 50 * Main.UIScale)), new Color(0, 0, 0, 127));
            }
        }
    }
}