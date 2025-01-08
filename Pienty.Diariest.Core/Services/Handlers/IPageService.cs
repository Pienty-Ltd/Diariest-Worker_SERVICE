namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IPageService
    {
        Task InitializeAsync();
        Task CacheAppPagesToRedisAsync();
    }
}