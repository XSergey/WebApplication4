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
    public class ArticlesToProvidersCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1:27017", "uniparts");
        private const string _collectionName = "uniparts_articles_to_providers";
        public IMongoCollection<ArticleToProvider> Collection;

        public ArticlesToProvidersCollection()
        {
            Collection = _repo.Db.GetCollection<ArticleToProvider>(_collectionName);
        }

        public async Task<IEnumerable<ArticleToProvider>> FindAll()
        {
            var query = await Collection.Find(new BsonDocument()).ToListAsync();

            return query;
                //query.Result;
        }

        public async Task<IEnumerable<ArticleToProvider>> Find(ObjectId ArticleId, ObjectId ProviderId)
        {
            //new ObjectId()
            var filter = Builders<ArticleToProvider>.Filter.And(
                 Builders<ArticleToProvider>.Filter.Where(p => p.ArticleId == ArticleId),
                 Builders<ArticleToProvider>.Filter.Where(p => p.ProviderId == ProviderId),
                 Builders<ArticleToProvider>.Filter.Where(p => p.ArticleToProviderIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<ArticleToProvider>> FindByArticleId(ObjectId ArticleId)
        {
            //new ObjectId()
            var filter = Builders<ArticleToProvider>.Filter.And(
                 Builders<ArticleToProvider>.Filter.Where(p => p.ArticleId == ArticleId),
                 Builders<ArticleToProvider>.Filter.Where(p => p.ArticleToProviderIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<ArticleToProvider>> FindByProviderId(ObjectId ProviderId)
        {
            //new ObjectId()
            var filter = Builders<ArticleToProvider>.Filter.And(
                 Builders<ArticleToProvider>.Filter.Where(p => p.ProviderId == ProviderId),
                 Builders<ArticleToProvider>.Filter.Where(p => p.ArticleToProviderIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create(ObjectId ArticleId, ObjectId ProviderId, double Price, int Quantity, string Currency, int Period, string Prefix)
        {
            await Collection.InsertOneAsync(new ArticleToProvider
            {
                ArticleId = ArticleId,
                ProviderId = ProviderId,
                ArticleToProviderPrice = Price,
                ArticleToProviderQuantity = Quantity,
                ArticleToProviderCurrency = Currency,
                ArticleToProviderPeriod = Period,
                ArticleToProviderPrefix = Prefix,
                ArticleToProviderStatus = 1,
                ArticleToProviderIsImported = 1
            });
            
        }

        public async Task Update(ObjectId ArticleToProviderId, double Price, int Quantity, string Currency, int Period, string Prefix)
        {
            var filter = Builders<ArticleToProvider>.Filter.Eq(s => s.ArticleToProviderId, ArticleToProviderId);
            var update = Builders<ArticleToProvider>.Update
                .Set("price", Price)
                .Set("quantity", Quantity)
                .Set("currency", Currency)
                .Set("period", Period)
                .Set("prefix", Prefix);
            //var u= Update<ArticleToProvider>
            //.Set(c => c.ArticleToProviderPrice, Price)
            //.Set(c => c.ArticleToProviderQuantity, Quantity)
            //.Set(c => c.ArticleToProviderCurrency, Currency)
            //.Set(c => c.ArticleToProviderPeriod, Period)
            //.Set(c => c.ArticleToProviderPrefix, Prefix);

            await Collection.UpdateOneAsync(filter, update);
        }

        public async Task Destroy(ObjectId ArticleToProviderId)
        {
            await Collection.DeleteOneAsync(Builders<ArticleToProvider>.Filter.Eq("ArticleToProviderId", ArticleToProviderId));
        }
    }
}
