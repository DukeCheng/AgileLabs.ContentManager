using AgileLabs.ContentManager.Common.ShareModels;
using AgileLabs.ContentManager.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Repositories
{
    public class MongoDbBaseRepository<TEntity>
        where TEntity : EntityBase
    {
        private IMongoDatabase _mongoDatabase;

        public  IMongoCollection<TEntity> collection;

        public MongoDbBaseRepository(MongoDbContext mongoDatabase)
        {
            _mongoDatabase = mongoDatabase.GetDateBase();
            collection = _mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name, new MongoCollectionSettings() { });
        }

        public void Insert(TEntity entity)
        {
            entity.CreationTime = DateTime.UtcNow;
            entity.ModificationTime = DateTime.UtcNow;
            collection.InsertOne(entity);
        }

        public async Task InsertManyAsync(List<TEntity> items)
        {
            foreach (var item in items)
            {
                item.CreationTime = DateTime.UtcNow;
                item.ModificationTime = DateTime.UtcNow;
            }

            await collection.InsertManyAsync(items);
        }

        public async Task<bool> UpdateAsync(Expression<Func<TEntity, bool>> expression, UpdateDefinition<TEntity> updateDefinition)
        {
            var result = await collection.UpdateManyAsync<TEntity>(expression, updateDefinition);
            return result.ModifiedCount > 0;
        }

        public bool Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            entity.ModificationTime = DateTime.UtcNow;
            return collection.ReplaceOne(filter, entity).ModifiedCount == 1;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            return await SigleOrDefault(filter);
        }

        public async Task<TEntity> SigleOrDefault(FilterDefinition<TEntity> filter)
        {
            return await collection.Find(filter).SingleOrDefaultAsync();
        }

        public bool Delete(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            return collection.DeleteOne(filter).DeletedCount > 0;
        }

        public async Task DeleteAllAsync()
        {
            await collection.DeleteManyAsync(Builders<TEntity>.Filter.Empty);
        }

        public IList<TEntity> Search(Expression<Func<TEntity, bool>> predicate, SortDefinition<TEntity> sort)
        {
            return collection.AsQueryable<TEntity>().Where(predicate.Compile()).ToList();
        }

        public async Task<IList<TEntity>> SearchAsync(FilterDefinition<TEntity> filter)
        {
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IList<TEntity>> SearchAsync(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort, int pageCount)
        {
            return await collection.Find(filter).Sort(sort).Limit(pageCount).ToListAsync();
        }

        public async Task<IList<TEntity>> SearchAsync(FilterDefinition<TEntity> filter, int pageCount)
        {
            return await collection.Find(filter).Limit(pageCount).ToListAsync();
        }

        public async Task<TEntity> SearchOneAsync(FilterDefinition<TEntity> filter)
        {
            return await collection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<Pager<TEntity>> PaginationSearchAsync(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort, int pageIndex = 1, int pageSize = 20, bool ignoreCount = true, int defaultCountNumber = 50000)
        {
            var total = ignoreCount ? defaultCountNumber : collection.Find(filter).CountAsync().Result;
            var records = collection.Find(filter).Sort(sort).Limit(pageSize).Skip((pageIndex - 1) * pageSize).ToListAsync();
            return await Task.FromResult(new Pager<TEntity>()
            {
                Records = records.Result,
                Paging = new Paging() { Total = total, PageIndex = pageIndex, PageSize = pageSize }
            });
        }

        public IList<TEntity> GetAll()
        {
            var filter = Builders<TEntity>.Filter.Empty;
            return collection.Find(filter).ToList();
        }

        public string UploadFile(string fileName, Stream stream)
        {
            var fs = new GridFSBucket(_mongoDatabase);
            var id = fs.UploadFromStream(fileName, stream);
            return id.ToString();
        }
        public GridFSDownloadStream DownloadFile(string fileId)
        {
            var fs = new GridFSBucket(_mongoDatabase);
            var stream = fs.OpenDownloadStream(new ObjectId(fileId));
            return stream;
        }
    }
}
