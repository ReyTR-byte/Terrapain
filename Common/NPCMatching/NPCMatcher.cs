using Terrapain.Common.NPCMatching.Conditions;
using System.Collections.Generic;

namespace Terrapain.Common.NPCMatching
{
    public class NPCMatcher
    {
        public List<INPCMatchCondition> Conditions;

        public NPCMatcher()
        {
            Conditions =
            [
                // So that empty matches match everything
                new MatchEverythingCondition()
            ];
        }

        public NPCMatcher MatchType(int type)
        {
            Conditions.Add(new MatchTypeCondition(type));
            return this;
        }

        public NPCMatcher MatchAI(int ai)
        {
            Conditions.Add(new MatchAICondition(ai));
            return this;
        }

        public NPCMatcher MatchTypeRange(params int[] types)
        {
            Conditions.Add(new MatchTypeRangeCondition(types));
            return this;
        }

        public bool Satisfies(int type) => Conditions.TrueForAll(condition => condition.Satisfies(type));
    }
}