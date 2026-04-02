using Terrapain.Common.System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Terrapain.Content.Items.DropRulls
{
    public class NotSuicideDropRule : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !WorldDifficultySystem.suicide;
        public bool CanShowItemDropInUI() => false;
        public string GetConditionDescription() => Language.GetTextValue("Mods.Terrapain.Conditions.NotSuicide");
    }
}