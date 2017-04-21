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
    public class CurrencyCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1:27017", "uniparts");
        private const string _collectionName = "eshop_currency";
        public IMongoCollection<Currency> Collection;

        public CurrencyCollection()
        {
            Collection = _repo.Db.GetCollection<Currency>(_collectionName);
        }

        public async Task<IEnumerable<Currency>> FindAll()
        {
            var query = await Collection.Find(new BsonDocument()).ToListAsync();

            return query;
                //query.Result;
        }

        public async Task<IEnumerable<Currency>> FindByCurrencyCode(string CurrencyCode, string ArticleBrand)
        {
            var filter = Builders<Currency>.Filter.And(
                 Builders<Currency>.Filter.Where(p => p.CurrencyCode == CurrencyCode),
                 Builders<Currency>.Filter.Where(p => p.CurrencyStatus == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Currency>> FindByCurrencyIsDefault()
        {
            var filter = Builders<Currency>.Filter.And(
                 Builders<Currency>.Filter.Where(p => p.CurrencyIsDefault == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create()
        {
            await Collection.InsertOneAsync(new Currency
            {
                //ArticleNumber = ArticleNumber,
                //ArticleBrand = ArticleBrand,
                //ArticleStatus = 1,
                //ArticleIsImported = 1
            });
        }


    }
}
