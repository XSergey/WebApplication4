using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.TecDoc
{
    /// <summary>
    /// Article collection class provide Article collection object.
    /// This object use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Brands
    {
        [BsonId]
        public ObjectId ArlId { get; set; }

        [BsonElement("BRA_ID")]
        public int BraId { get; set; }

        [BsonElement("BRA_BRAND")]
        public string BraBrand { get; set; }
    }
}
