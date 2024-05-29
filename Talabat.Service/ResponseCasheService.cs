using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Service.Contract;

namespace Talabat.Service
{
    public class ResponseCasheService : IResponseCasheService
    {
        private readonly IDatabase database;
        public ResponseCasheService(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }
        public async Task CasheResponseAsync(string casheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializedResponse = JsonSerializer.Serialize(response, serializeOptions);
            await database.StringSetAsync(casheKey, serializedResponse, timeToLive);
        }

        public async Task<string?> GetCashedResponseAsync(string casheKey)
        {
            var cashedResponse = await database.StringGetAsync(casheKey);

            if (cashedResponse.IsNullOrEmpty) return null;

            return cashedResponse;
        }
    }
}
