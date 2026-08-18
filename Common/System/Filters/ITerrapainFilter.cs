namespace Terrapain.Common.System.Filters
{
    public interface ITerrapainFilter
    {
        public abstract bool Update();
        public abstract void Apply();
        public abstract bool Necessary();
        public abstract void OnDispose();
        public abstract float Weight();
    }
}