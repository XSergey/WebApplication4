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
    public class TecDocBrandCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://138.201.142.157", "td");
        private const string _collectionName = "TOF_BRANDS";
        public IMongoCollection<Brands> Collection;

        public TecDocBrandCollection()
        {
            Collection = _repo.Db.GetCollection<Brands>(_collectionName);
        }

        public async Task<IEnumerable<Brands>> FindByBrandId(int BrandId)
        {
            var filter = Builders<Brands>.Filter.And(
                 Builders<Brands>.Filter.Where(p => p.BraId == BrandId)
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }
    }
}
