using Terraria;

namespace Terrapain.Content.Dashes
{
    internal class ActiveAccessoryDash : Dash
    {
        public ActiveAccessoryDash(Item sourceItem)
        {
            dashSource = new ActiveAccessoryDashSource(sourceItem);
        }
    }
}
