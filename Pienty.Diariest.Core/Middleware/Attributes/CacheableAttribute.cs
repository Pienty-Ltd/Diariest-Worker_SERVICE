namespace Pienty.Diariest.Core.Middleware.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CacheableAttribute: Attribute
    {
        public int DurationInSeconds { get; set; }

        public CacheableAttribute(int durationInSeconds)
        {
            DurationInSeconds = durationInSeconds;
        }
    }
}