using Terrapain.Common.NPCMatching;

namespace Terrapain.Common.NPCMatching.Conditions
{
    public class MatchAICondition : INPCMatchCondition
    {
        public int AI;

        public MatchAICondition(int ai)
        {
            AI = ai;
        }

        public bool Satisfies(int type) => type == AI;
    }
}
