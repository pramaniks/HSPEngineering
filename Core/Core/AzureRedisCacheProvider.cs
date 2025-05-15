using Azure.Messaging.EventGrid.SystemEvents;
using Core.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    internal class ConnectionHelper
    {
        static ConnectionHelper()
        {
            var redisendpoint = AppUtility.ConfigurationManager["AzureRedisConnection"];
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return  ConnectionMultiplexer.Connect(redisendpoint);
            });
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
    internal interface IAzureRedisCacheProvider
    {
        Task<T> GetDataAsync<T>(string key);
        Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime);
        Task<bool> RemoveDataAsync(string key);
    }
    internal class AzureRedisCacheProvider : IAzureRedisCacheProvider
    {
        private IDatabase _database;

        public AzureRedisCacheProvider()
        {
            ConfigureRedis();
        }

        private void ConfigureRedis()
        {
            _database = ConnectionHelper.Connection.GetDatabase();
        }
        public async Task<T> GetDataAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime = default)
        {
            bool isSet = false;
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            isSet = await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
        public async Task<bool> RemoveDataAsync(string key)
        {
            bool _isKeyExist = await _database.KeyExistsAsync(key);
            if (_isKeyExist == true)
            {
                return _database.KeyDelete(key);
            }
            return false;
        }
    }
}
