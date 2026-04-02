using Terraria;
using Terraria.ModLoader;

namespace Terrapain.Content.Dusts
{
	public class BloodDrop : ModDust
	{

		public override void OnSpawn(Dust dust) {
            dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
            dust.noLight = true; // Makes the dust emit no light.
        }
	}
}