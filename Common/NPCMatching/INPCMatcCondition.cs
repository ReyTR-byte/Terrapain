namespace Terrapain.Common.NPCMatching
{
    public interface INPCMatchCondition
    {
        bool Satisfies(int type);
    }
}
