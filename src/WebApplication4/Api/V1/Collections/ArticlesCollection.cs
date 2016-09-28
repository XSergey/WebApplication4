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
    [Route("api/[controller]")]
    public class ArticlesCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1:27017", "uniparts");
        private const string _collectionName = "uniparts_articles";
        public IMongoCollection<Article> Collection;

        public ArticlesCollection()
        {
            Collection = _repo.Db.GetCollection<Article>(_collectionName);
        }

        public async Task<IEnumerable<Article>> FindAll()
        {
            var query = await Collection.Find(new BsonDocument()).ToListAsync();

            return query;
                //query.Result;
        }

        public async Task<IEnumerable<Article>> Find(string ArticleNumber, string ArticleBrand)
        {
            var filter = Builders<Article>.Filter.And(
                 Builders<Article>.Filter.Where(p => p.ArticleNumber == ArticleNumber),
                 Builders<Article>.Filter.Where(p => p.ArticleBrand == ArticleBrand),
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create(string ArticleNumber, string ArticleBrand)
        {
            await Collection.InsertOneAsync(new Article
            {
                ArticleNumber = ArticleNumber,
                ArticleBrand = ArticleBrand,
                ArticleStatus = 1,
                ArticleIsImported = 1
            });
        }


    }
}
