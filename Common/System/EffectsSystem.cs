using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terrapain.Assets.Extratextures;
using Terrapain.Common.Config;
using Terrapain.Common.DrawTasks;
using Terrapain.Common.Global;
using Terraria;
using Terraria.ModLoader;
using Terrapain.Common.System.Filters;

namespace Terrapain.Common.System
{
    public class EffectsSystem : ModSystem
    {
        public static List<ITerrapainFilter> filters = [];
        public static int MaxUnnecessaryFilters => (int)GraphicsConfig.Instance.filters * 15;
        public override void PostUpdateEverything()
        {
            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                if(filter.Update())
                {
                    filter.OnDispose();
                    filters.RemoveAt(i);
                    i--;
                }
                else
                {
                    filter.Apply();
                }
            }
        }
        public static void AddFilter(ITerrapainFilter filter)
        {
            float totalWeight = 0;
            foreach(var f in filters)
            {
                totalWeight += f.Weight();
            }
            totalWeight += filter.Weight();
            if (!filter.Necessary() && totalWeight < MaxUnnecessaryFilters)
            {
                filters.Add(filter);
            }
            if (filter.Necessary())
            {
                int necessaryFilters = 0;
                while (filters.Count <= necessaryFilters || totalWeight < MaxUnnecessaryFilters)
                {
                    if (filters[necessaryFilters].Necessary())
                    {
                        necessaryFilters++;
                    }
                    else
                    {
                        totalWeight -= filters[necessaryFilters].Weight();
                        filters.RemoveAt(necessaryFilters);
                    }
                }
            }
        }
    }
}