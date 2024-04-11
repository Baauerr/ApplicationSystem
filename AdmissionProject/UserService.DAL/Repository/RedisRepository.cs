using StackExchange.Redis;

namespace UserService.DAL.Repository
{
    public class RedisRepository
    {
        private readonly IDatabase _database;

        public RedisRepository(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _database = connection.GetDatabase();
        }

        public async Task AddTokenBlackList(string token)
        {
            await _database.StringSetAsync(token, "blacklisted", TimeSpan.FromMinutes(60));
        }

        public async Task<bool> IsBlacklisted(string token)
        {
            return await _database.KeyExistsAsync(token);
        }
    }
}
