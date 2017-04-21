using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication4.Api.V1.Collections.TecDoc;
using WebApplication4.Api.V1.Providers.Database;
using MongoDB.Driver.Builders;

namespace WebApplication4.Api.V1.Collections
{
    [Route("api/[controller]")]
    public class TecDocArticlesCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1", "td");
        private const string _collectionName = "TOF_ARTICLES";
        public IMongoCollection<TecDocArticles> Collection;

        public TecDocArticlesCollection()
        {
            Collection = _repo.Db.GetCollection<TecDocArticles>(_collectionName);
        }

        public async Task<IEnumerable<TecDocArticles>> FindAll()
        {
            var filter = Builders<TecDocArticles>.Filter.And(
            );

            var query = await Collection.Find(filter).ToListAsync();

            return query;
        }

        public async Task<IEnumerable<TecDocArticles>> FindByArticleId(int ArticleId)
        {
            var filter = Builders<TecDocArticles>.Filter.And(
                 Builders<TecDocArticles>.Filter.Where(p => p.ArtId == ArticleId)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }

        public static implicit operator TecDocArticlesCollection(ArticlesCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
