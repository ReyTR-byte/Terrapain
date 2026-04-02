using Terrapain.Common.System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Terrapain.Content.Items.DropRulls
{
    public class TortureDropRule : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !WorldDifficultySystem.clasic;
        public bool CanShowItemDropInUI() => !WorldDifficultySystem.clasic;

        public string GetConditionDescription() => Language.GetTextValue("Mods.Terrapain.Conditions.Torture");
    }
}