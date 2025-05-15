namespace Core.Cache
{
    public interface IApplicationCache
    {
        Task<T> GetDataAsync<T>(string Key);
        Task<bool> SetDataAsync<T>(string key, T Value, DateTimeOffset expirationTime = default);
        Task<bool> RemoveDataAsync(string Key);
    }

    public class ApplicationCache : IApplicationCache
    {
        private IAzureRedisCacheProvider _AzureRedisCacheProvider;
        public ApplicationCache()
        {
            _AzureRedisCacheProvider = new AzureRedisCacheProvider();
        }

        public async Task<T> GetDataAsync<T>(string Key)
        {
            return await _AzureRedisCacheProvider.GetDataAsync<T>(Key);
        }

        public async Task<bool> SetDataAsync<T>(string key, T Value, DateTimeOffset expirationTime = default)
        {
            return await _AzureRedisCacheProvider.SetDataAsync(key, Value, expirationTime);
        }

        public async Task<bool> RemoveDataAsync(string Key)
        {
            return await _AzureRedisCacheProvider.RemoveDataAsync(Key);
        }
    }
}
