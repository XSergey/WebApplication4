using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.TecDoc
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ArtLookup
    {
        [BsonId]
        public ObjectId ArlId { get; set; }

        [BsonElement("ARL_ART_ID")]
        public int ArlArtId { get; set; }

        [BsonElement("ARL_SEARCH_NUMBER")]
        public string ArlSearchNumber { get; set; }

        [BsonElement("ARL_KIND")]
        public string ArlKind { get; set; }

        //[BsonElement("ARL_BRA_ID")]
        //public int ArlBrandId { get; set; }
    }
}
