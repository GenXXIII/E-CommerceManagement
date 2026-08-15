
using ECommerce.Application.Interfaces;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Persistence.Services;

public class RedisDistributedLockProvider : IDistributedLockProvider
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisDistributedLockProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<IDistributedLock> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var lockKey = $"distributed-lock:{resource}";
        var lockValue = Guid.NewGuid().ToString();

        var endTime = DateTime.UtcNow.Add(waitTime);
        while (DateTime.UtcNow < endTime)
        {
            if (await db.StringSetAsync(lockKey, lockValue, expiryTime, When.NotExists))
            {
                return new RedisDistributedLock(db, lockKey, lockValue, true);
            }
            await Task.Delay(retryTime, cancellationToken);
        }

        return new RedisDistributedLock(db, lockKey, lockValue, false);
    }

    private class RedisDistributedLock : IDistributedLock
    {
        private readonly IDatabase _db;
        private readonly string _key;
        private readonly string _value;

        public RedisDistributedLock(IDatabase db, string key, string value, bool isAcquired)
        {
            _db = db;
            _key = key;
            _value = value;
            IsAcquired = isAcquired;
        }

        public bool IsAcquired { get; }

        public async ValueTask DisposeAsync()
        {
            if (IsAcquired)
            {
                // Use Lua script to ensure we only delete the lock if the value matches
                var script = @"
                    if redis.call('GET', KEYS[1]) == ARGV[1] then
                        return redis.call('DEL', KEYS[1])
                    else
                        return 0
                    end
                ";
                await _db.ScriptEvaluateAsync(script, new RedisKey[] { _key }, new RedisValue[] { _value });
            }
        }
    }
}

