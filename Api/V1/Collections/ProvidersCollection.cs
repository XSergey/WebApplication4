using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication4.Api.V1.Collections.Articles;
using WebApplication4.Api.V1.Providers.Database;
using MongoDB.Driver.Builders;

namespace WebApplication4.Api.V1.Collections
{
    public class ProvidersCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1:27017", "uniparts");
        private const string _collectionName = "uniparts_providers";
        public IMongoCollection<Provider> Collection;

        public ProvidersCollection()
        {
            Collection = _repo.Db.GetCollection<Provider>(_collectionName);
        }

        public async Task<IEnumerable<Provider>> FindAll()
        {
            var query = await Collection.Find(new BsonDocument()).ToListAsync();

            return query;
                //query.Result;
        }

        public async Task<IEnumerable<Provider>> Find(string ProviderName)
        {
            var filter = Builders<Provider>.Filter.And(
                 Builders<Provider>.Filter.Where(p => p.ProviderName == ProviderName),
                 Builders<Provider>.Filter.Where(p => p.ProviderIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Provider>> FindById(ObjectId ProviderId)
        {
            var filter = Builders<Provider>.Filter.And(
                 Builders<Provider>.Filter.Where(p => p.ProviderId == ProviderId),
                 Builders<Provider>.Filter.Where(p => p.ProviderIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create(string ProviderName, string MarkUp = "0")
        {
            await Collection.InsertOneAsync(new Provider
            {
                ProviderName = ProviderName,
                ProviderMarkUp = Int32.Parse(MarkUp),
                ProviderStatus = 1,
                ProviderIsImported = 1
            });
        }

        public async Task Destroy(ObjectId ProviderId)
        {
            await Collection.DeleteOneAsync(Builders<Provider>.Filter.Eq("ProviderId", ProviderId));
        }
    }
}
