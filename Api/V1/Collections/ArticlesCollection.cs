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
using System.Text.RegularExpressions;

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

        public async Task<IEnumerable<Article>> FindIsImported(string all = "all")
        {
            var filter = Builders<Article>.Filter.And(
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Article>> Find(string ArticleNumber, string ArticleBrand)
        {
            var filter = Builders<Article>.Filter.And(
                 Builders<Article>.Filter.Where(p => p.ArticleNumber == ArticleNumber),
                 Builders<Article>.Filter.Where(p => p.ArticleBrand == ArticleBrand),
                 //Builders<Article>.Filter.Regex("brand", new BsonRegularExpression(ArticleBrand)),
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Article>> FindByArticleNumber(string ArticleNumber)
        {
            var filter = Builders<Article>.Filter.And(
                 Builders<Article>.Filter.Where(p => p.ArticleNumber == ArticleNumber),
                 //Builders<Article>.Filter.Where(p => p.ArticleBrand == ArticleBrand),
                 //Builders<Article>.Filter.Regex("brand", new BsonRegularExpression(ArticleBrand)),
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Article>> FindById(ObjectId Id)
        {
            var filter = Builders<Article>.Filter.And(
                 Builders<Article>.Filter.Where(p => p.ArticleId == Id),
                 //Builders<Article>.Filter.Where(p => p.ArticleBrand == ArticleBrand),
                 //Builders<Article>.Filter.Regex("brand", new BsonRegularExpression(ArticleBrand)),
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Article>> FindLikeBrand(string ArticleNumber, string ArticleBrand)
        {
            var firstChars = new string(ArticleBrand.Split(new char[] { ' ', '-' }).Select(x => x[0]).ToArray());

            string first = (ArticleBrand.Length > 0) ? ArticleBrand.Substring(0, 1) : "";
            string third = (ArticleBrand.Length > 2) ? ArticleBrand.Substring(2, 1) : "";
            string fourth = (ArticleBrand.Length > 3) ? ArticleBrand.Substring(3, 1) : "";

            string firstAndThirdChars = (ArticleBrand.Length > 2) ? first + third : first;
            string firstAndFourthChars = (ArticleBrand.Length > 3 ) ? first + fourth : first;
            var brandsArray = ArticleBrand.Split(new char[] { ' ', '-' });

            //string regexNumberFilter = "(" + ArticleNumber + "|" + firstChars+ArticleNumber + "|" + firstAndThirdChars+ArticleNumber + "|" + firstAndFourthChars+ArticleNumber + ")"; //( string1 | string2 | string3 ) -> string.Join("|", brands)

            string stringFilter = string.Join("|", brandsArray);
            string regexFilter = "(" + stringFilter + "|" + firstChars + "|" + firstAndThirdChars + "|" + firstAndFourthChars + ")"; //( string1 | string2 | string3 ) -> string.Join("|", brands)

            var filter = Builders<Article>.Filter.And(
                 //Builders<Article>.Filter.Regex("number", new BsonRegularExpression(new Regex(regexNumberFilter, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))),
                 Builders<Article>.Filter.Where(p => p.ArticleNumber == ArticleNumber || p.ArticleNumber == firstAndThirdChars + ArticleNumber || p.ArticleNumber == firstAndFourthChars + ArticleNumber),
                 //Builders<Article>.Filter.Where(p => p.ArticleBrand == ArticleBrand),
                 Builders<Article>.Filter.Regex("brand", new BsonRegularExpression(new Regex(regexFilter, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))),
                 Builders<Article>.Filter.Where(p => p.ArticleIsImported == 1)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public async Task Create(string ArticleNumber, string ArticleName = "", string ArticleBrand = "")
        {
            await Collection.InsertOneAsync(new Article
            {
                ArticleNumber = ArticleNumber,
                ArticleName = ArticleName,
                ArticleBrand = ArticleBrand,
                ArticleStatus = 1,
                ArticleIsImported = 1
            });
        }

        public async Task Destroy(ObjectId ArticleId)
        {
            await Collection.DeleteOneAsync(Builders<Article>.Filter.Eq("ArticleId", ArticleId));
        }
    }
}
