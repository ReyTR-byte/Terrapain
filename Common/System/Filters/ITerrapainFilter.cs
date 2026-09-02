namespace Terrapain.Common.System.Filters
{
    public interface ITerrapainFilter
    {
        public abstract bool Update(int i);
        public abstract void Apply(int i);
        public abstract bool Necessary();
        public abstract void OnDispose();
        public abstract float Weight();
    }
}