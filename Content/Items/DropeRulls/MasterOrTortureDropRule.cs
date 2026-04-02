using Terrapain.Common.System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Terrapain.Content.Items.DropRulls
{
    public class MasterOrTortureDropRule : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !WorldDifficultySystem.clasic || Main.masterMode;
        public bool CanShowItemDropInUI() => !WorldDifficultySystem.clasic || Main.masterMode;

        public string GetConditionDescription() => Language.GetTextValue("Mods.Terrapain.Conditions.MasterOrTorture");
    }
}