namespace Terrapain.Common.NPCMatching.Conditions
{
    public class MatchEverythingCondition : INPCMatchCondition
    {
        public bool Satisfies(int type) => true;
    }
}
