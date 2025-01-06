using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pienty.Diariest.Core.Middleware.Attributes;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Middleware
{
    public class DBCachingInterceptor : IInterceptor
    {
        private readonly ILogger<DBCachingInterceptor> _logger;
        private readonly IRedisService _redisService;

        public DBCachingInterceptor(ILogger<DBCachingInterceptor> logger, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }
        
        public void Intercept(IInvocation invocation) 
        {
            var methodInfo = invocation.MethodInvocationTarget ?? invocation.Method;
            var cacheableAttribute = methodInfo.GetCustomAttribute<CacheableAttribute>();

            if (cacheableAttribute != null)
            {
                var cacheKey = GenerateCacheKey(methodInfo, invocation.Arguments);
                try
                {
                    var cachedValue = _redisService.Get<string>(cacheKey);
                    if (!string.IsNullOrEmpty(cachedValue))
                    {
                        var returnType = methodInfo.ReturnType;
                        var deserializedValue = JsonConvert.DeserializeObject(cachedValue, returnType);
                        invocation.ReturnValue = deserializedValue;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving data from cache.");
                }
                
                invocation.Proceed();
                
                var result = invocation.ReturnValue;
                if (result != null)
                {
                    try
                    {
                        var serializedResult = JsonConvert.SerializeObject(result);
                        _redisService.Set(cacheKey, serializedResult, TimeSpan.FromSeconds(cacheableAttribute.DurationInSeconds));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error setting data to cache.");
                    }
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        private string GenerateCacheKey(MethodInfo methodInfo, object[] arguments)
        {
            var methodName = methodInfo.Name;
            var argumentsString = string.Join(",", arguments.Select(a => a?.ToString() ?? "null"));
            return $"{methodName}({argumentsString})";
        }
    }
}