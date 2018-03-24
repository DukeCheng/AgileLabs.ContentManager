using AgileLabs.ContentManager.Controllers;
using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Services
{
    public class UrlRecordServcie
    {
        private IMemoryCache _memoryCache;
        private MongoDbBaseRepository<UrlRecord> _urlRecordRepository;
        private MongoDbBaseRepository<Page> _pageRepository;
        private MongoDbBaseRepository<Template> _templateRepository;
        private MongoDbBaseRepository<Settings> _settingsRepository;

        public UrlRecordServcie(IMemoryCache memoryCache,
            MongoDbBaseRepository<UrlRecord> urlRecordRepository,
            MongoDbBaseRepository<Page> pageRepository,
            MongoDbBaseRepository<Template> templateRepository,
            MongoDbBaseRepository<Settings> settingsRepository)
        {
            _memoryCache = memoryCache;
            _urlRecordRepository = urlRecordRepository;
            _pageRepository = pageRepository;
            _templateRepository = templateRepository;
            _settingsRepository = settingsRepository;
        }

        public async Task<UrlRecord> GetUrlRecord(Guid urlRecordId)
        {
            return await _urlRecordRepository.GetByIdAsync(urlRecordId);
        }

        public UrlRecord GetUrlRecord(string slug)
        {
            return LocalCache(string.Format(CacheKeys.UrlRecordBySlug, slug), () =>
            {
                slug = NormalizeSlug(slug);
                var filterBuilder = Builders<UrlRecord>.Filter;
                var filter = filterBuilder.Eq(x => x.Slug, slug);//slug will be null or other string

                return _urlRecordRepository.SearchOneAsync(filter).Result;
            }, TimeSpan.FromSeconds(60));
        }

        private TCacheEntry LocalCache<TCacheEntry>(string key, Func<TCacheEntry> cacheEntryAction, TimeSpan slidingTime)
        {
            TCacheEntry cacheEntry;
            // Look for cache key.
            if (!_memoryCache.TryGetValue<TCacheEntry>(key, out cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = cacheEntryAction();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(slidingTime);

                // Save data in cache.
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        private async Task<TCacheEntry> DistributedCacheAsync<TCacheEntry>(IDistributedCache cache, string key, Func<TCacheEntry> cacheEntryAction, TimeSpan slidingTime)
        {
            TCacheEntry cacheEntry;
            // Look for cache key.
            var value = await cache.GetAsync(key);
            if (value == null)
            {
                // Key not in cache, so get data.
                cacheEntry = cacheEntryAction();

                // Set cache options.
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(slidingTime);

                // Save data in cache.
                await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cacheEntry)), cacheEntryOptions);
            }
            else
            {
                cacheEntry = JsonConvert.DeserializeObject<TCacheEntry>(Encoding.UTF8.GetString(value));
            }
            return cacheEntry;
        }

        private static string NormalizeSlug(string slug)
        {
            if (slug != null)
            {
                slug = slug.ToLower();
            }
            else if (string.IsNullOrWhiteSpace(slug))
            {
                slug = null;
            }

            return slug;
        }

        private string FormatUrl(string refValue)
        {
            return refValue;
        }
    }
}
