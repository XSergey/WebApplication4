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
    public class TecDocArtLookupCollection
    {
        internal MongoDbProvider _repo = new MongoDbProvider("mongodb://127.0.0.1", "td");
        private const string _collectionName = "TOF_ART_LOOKUP";
        public IMongoCollection<ArtLookup> Collection;

        public TecDocArtLookupCollection()
        {
            Collection = _repo.Db.GetCollection<ArtLookup>(_collectionName);
        }

        public async Task<IEnumerable<ArtLookup>> FindAll()
        {
            var filter = Builders<ArtLookup>.Filter.And(
            );

            var query = await Collection.Find(filter).ToListAsync();

            return query;
        }

        public async Task<IEnumerable<ArtLookup>> FindByNumber(string ArlSearchNumber)
        {
            var filter = Builders<ArtLookup>.Filter.And(
                 Builders<ArtLookup>.Filter.Where(p => p.ArlSearchNumber == ArlSearchNumber),
                 Builders<ArtLookup>.Filter.Where(p => p.ArlKind == "4"),
                 Builders<ArtLookup>.Filter.Where(p => p.ArlSearchNumber != null),
                 Builders<ArtLookup>.Filter.Where(p => p.ArlSearchNumber != "")
            );

            var query = await Collection.Find(filter).ToListAsync();
            return query;
        }
    }
}
