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
    public class ArticlesLookupCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1:27017", "uniparts");
        private const string _collectionName = "uniparts_articles_lookup";
        public IMongoCollection<ArticleLookup> Collection;

        public ArticlesLookupCollection()
        {
            Collection = _repo.Db.GetCollection<ArticleLookup>(_collectionName);
        }


        public async Task<IEnumerable<ArticleLookup>> Find(string ArticleNumber, string ArticleBrand)
        {
            var filter = Builders<ArticleLookup>.Filter.And(
                 Builders<ArticleLookup>.Filter.Where(p => p.ArticleLookupArticleNumber == ArticleNumber),
                 //Builders<ArticleLookup>.Filter.Regex("brand", new BsonRegularExpression(ArticleBrand))
                 Builders<ArticleLookup>.Filter.Where(p => p.ArticleLookupArticleBrand == ArticleBrand)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<ArticleLookup>> FindLikeBrand(string ArticleNumber, string ArticleBrand)
        {
            var filter = Builders<ArticleLookup>.Filter.And(
                 Builders<ArticleLookup>.Filter.Where(p => p.ArticleLookupArticleNumber == ArticleNumber)
                 //Builders<ArticleLookup>.Filter.Regex("brand", new BsonRegularExpression(ArticleBrand))
                 //Builders<ArticleLookup>.Filter.Where(p => p.ArticleLookupArticleBrand == ArticleBrand)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create(string ArticleNumber, string ArticleName = "", string ArticleBrand = "")
        {
            await Collection.InsertOneAsync(new ArticleLookup
            {
            });
        }


    }
}
