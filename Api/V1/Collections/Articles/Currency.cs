using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication4.Api.V1.Collections.Articles
{
    /// <summary>
    /// Currency collection class provide currency collections in object.
    /// This object in use by mongodb provider class.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Currency
    {
        [BsonId]
        public ObjectId CurrencyId { get; set; }

        [BsonElement("name")]
        public string CurrencyName { get; set; }

        [BsonElement("code")]
        public string CurrencyCode { get; set; }

        [BsonElement("symbol")]
        public string CurrencySymbol { get; set; }

        [BsonElement("value")]
        public string CurrencyValue { get; set; }

        [BsonElement("status")]
        public int CurrencyStatus { get; set; }

        [BsonElement("is_default")]
        public int CurrencyIsDefault { get; set; }
    }
}
